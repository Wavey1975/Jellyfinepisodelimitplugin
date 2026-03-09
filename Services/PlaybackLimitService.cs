using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using MediaBrowser.Model.Session;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.EpisodeLimit.Services;

/// <summary>
/// Service that monitors playback and stops after the configured number of episodes.
/// Supports per-session limits set via the player UI.
/// </summary>
public class PlaybackLimitService : IHostedService, IDisposable
{
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<PlaybackLimitService> _logger;
    
    // Track limits and counts per device (deviceId -> tracker)
    private readonly ConcurrentDictionary<string, SessionTracker> _deviceTrackers = new();
    
    // Map Jellyfin session IDs to device IDs
    private readonly ConcurrentDictionary<string, string> _sessionToDevice = new();

    public PlaybackLimitService(
        ISessionManager sessionManager,
        ILogger<PlaybackLimitService> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _sessionManager.PlaybackStart += OnPlaybackStart;
        _sessionManager.PlaybackStopped += OnPlaybackStopped;
        _sessionManager.SessionEnded += OnSessionEnded;
        
        _logger.LogInformation("Episode Limit Plugin: Playback monitoring service started");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _sessionManager.PlaybackStart -= OnPlaybackStart;
        _sessionManager.PlaybackStopped -= OnPlaybackStopped;
        _sessionManager.SessionEnded -= OnSessionEnded;
        
        _logger.LogInformation("Episode Limit Plugin: Service stopped");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Set the episode limit for a device/session.
    /// Called from the API when user sets limit via player UI.
    /// </summary>
    public SessionStatus SetSessionLimit(string deviceId, int limit)
    {
        var tracker = _deviceTrackers.GetOrAdd(deviceId, _ => new SessionTracker());
        
        tracker.Limit = limit;
        tracker.IsActive = limit > 0;
        
        // Reset count when setting a new limit
        if (limit > 0)
        {
            tracker.EpisodesCompleted = 0;
        }
        
        _logger.LogInformation(
            "Episode Limit: Set limit to {Limit} for device {DeviceId}",
            limit, deviceId);
        
        return new SessionStatus
        {
            Limit = tracker.Limit,
            EpisodesPlayed = tracker.EpisodesCompleted,
            IsActive = tracker.IsActive
        };
    }

    /// <summary>
    /// Get the current status for a device.
    /// </summary>
    public SessionStatus GetSessionStatus(string deviceId)
    {
        if (_deviceTrackers.TryGetValue(deviceId, out var tracker))
        {
            return new SessionStatus
            {
                Limit = tracker.Limit,
                EpisodesPlayed = tracker.EpisodesCompleted,
                IsActive = tracker.IsActive
            };
        }
        
        return new SessionStatus { Limit = 0, EpisodesPlayed = 0, IsActive = false };
    }

    /// <summary>
    /// Clear the limit for a device.
    /// </summary>
    public void ClearSessionLimit(string deviceId)
    {
        if (_deviceTrackers.TryGetValue(deviceId, out var tracker))
        {
            tracker.Limit = 0;
            tracker.IsActive = false;
            tracker.EpisodesCompleted = 0;
            
            _logger.LogInformation("Episode Limit: Cleared limit for device {DeviceId}", deviceId);
        }
    }

    private void OnPlaybackStart(object? sender, PlaybackProgressEventArgs e)
    {
        var deviceId = e.Session.DeviceId;
        var sessionId = e.Session.Id;
        var itemName = e.Item?.Name ?? "Unknown";

        // Map session to device
        _sessionToDevice[sessionId] = deviceId;

        if (!_deviceTrackers.TryGetValue(deviceId, out var tracker) || !tracker.IsActive)
        {
            return;
        }

        tracker.CurrentItemId = e.Item?.Id ?? Guid.Empty;
        tracker.CurrentItemName = itemName;
        
        _logger.LogInformation(
            "Episode Limit: Playing episode {Current}/{Limit} - {ItemName}",
            tracker.EpisodesCompleted + 1, tracker.Limit, itemName);
    }

    private async void OnPlaybackStopped(object? sender, PlaybackStopEventArgs e)
    {
        var deviceId = e.Session.DeviceId;
        var sessionId = e.Session.Id;
        
        if (!_deviceTrackers.TryGetValue(deviceId, out var tracker) || !tracker.IsActive)
        {
            return;
        }

        // Check if episode finished naturally
        if (e.PlayedToCompletion)
        {
            tracker.EpisodesCompleted++;
            
            _logger.LogInformation(
                "Episode Limit: Completed {Completed}/{Limit} - {ItemName}",
                tracker.EpisodesCompleted, tracker.Limit, tracker.CurrentItemName);

            // Check if we've hit the limit
            if (tracker.EpisodesCompleted >= tracker.Limit)
            {
                _logger.LogInformation(
                    "Episode Limit: Reached limit of {Limit}! Stopping playback.",
                    tracker.Limit);

                // Deactivate the limit (user needs to set it again next time)
                tracker.IsActive = false;
                tracker.EpisodesCompleted = 0;
                
                // Wait for next episode to start queuing
                await Task.Delay(2000).ConfigureAwait(false);
                
                try
                {
                    await _sessionManager.SendPlaystateCommand(
                        null,
                        sessionId,
                        new PlaystateRequest
                        {
                            Command = PlaystateCommand.Stop,
                            ControllingUserId = e.Session.UserId.ToString()
                        },
                        default).ConfigureAwait(false);

                    _logger.LogInformation("Episode Limit: Stop command sent. Sweet dreams!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Episode Limit: Failed to stop playback");
                }
            }
        }
    }

    private void OnSessionEnded(object? sender, SessionEventArgs e)
    {
        var sessionId = e.SessionInfo.Id;
        _sessionToDevice.TryRemove(sessionId, out _);
        
        // Note: We keep the device tracker so the limit persists if they reconnect
        _logger.LogDebug("Episode Limit: Session ended {SessionId}", sessionId);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _sessionToDevice.Clear();
        _deviceTrackers.Clear();
    }

    /// <summary>
    /// Tracks state for a device.
    /// </summary>
    private class SessionTracker
    {
        public int Limit { get; set; }
        public bool IsActive { get; set; }
        public int EpisodesCompleted { get; set; }
        public Guid CurrentItemId { get; set; }
        public string CurrentItemName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Status returned to API callers.
    /// </summary>
    public class SessionStatus
    {
        public int Limit { get; set; }
        public int EpisodesPlayed { get; set; }
        public bool IsActive { get; set; }
    }
}
