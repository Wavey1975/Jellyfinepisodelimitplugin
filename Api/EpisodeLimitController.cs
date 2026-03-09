using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Jellyfin.Plugin.EpisodeLimit.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.EpisodeLimit.Api;

/// <summary>
/// API controller for Episode Limit plugin.
/// Allows clients to set/get episode limits per session.
/// </summary>
[ApiController]
[Route("EpisodeLimit")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
public class EpisodeLimitController : ControllerBase
{
    private readonly PlaybackLimitService _playbackLimitService;

    public EpisodeLimitController(PlaybackLimitService playbackLimitService)
    {
        _playbackLimitService = playbackLimitService;
    }

    /// <summary>
    /// Set the episode limit for the current session.
    /// </summary>
    /// <param name="request">The limit settings.</param>
    /// <returns>The updated settings.</returns>
    [HttpPost("SetLimit")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<EpisodeLimitResponse> SetLimit([FromBody] SetLimitRequest request)
    {
        if (request.Limit < 0 || request.Limit > 100)
        {
            return BadRequest("Limit must be between 0 and 100 (0 = disabled)");
        }

        var deviceId = Request.Headers["X-Emby-Device-Id"].ToString();
        if (string.IsNullOrEmpty(deviceId))
        {
            // Fallback to using a session identifier from auth
            deviceId = User.FindFirst("Jellyfin-DeviceId")?.Value ?? "unknown";
        }

        var result = _playbackLimitService.SetSessionLimit(deviceId, request.Limit);
        
        return Ok(new EpisodeLimitResponse
        {
            Limit = result.Limit,
            EpisodesPlayed = result.EpisodesPlayed,
            IsActive = result.IsActive
        });
    }

    /// <summary>
    /// Get the current episode limit status for this session.
    /// </summary>
    /// <returns>Current limit and progress.</returns>
    [HttpGet("Status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<EpisodeLimitResponse> GetStatus()
    {
        var deviceId = Request.Headers["X-Emby-Device-Id"].ToString();
        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = User.FindFirst("Jellyfin-DeviceId")?.Value ?? "unknown";
        }

        var result = _playbackLimitService.GetSessionStatus(deviceId);
        
        return Ok(new EpisodeLimitResponse
        {
            Limit = result.Limit,
            EpisodesPlayed = result.EpisodesPlayed,
            IsActive = result.IsActive
        });
    }

    /// <summary>
    /// Clear/disable the episode limit for this session.
    /// </summary>
    /// <returns>Confirmation.</returns>
    [HttpPost("Clear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult<EpisodeLimitResponse> ClearLimit()
    {
        var deviceId = Request.Headers["X-Emby-Device-Id"].ToString();
        if (string.IsNullOrEmpty(deviceId))
        {
            deviceId = User.FindFirst("Jellyfin-DeviceId")?.Value ?? "unknown";
        }

        _playbackLimitService.ClearSessionLimit(deviceId);
        
        return Ok(new EpisodeLimitResponse
        {
            Limit = 0,
            EpisodesPlayed = 0,
            IsActive = false
        });
    }
}

/// <summary>
/// Request to set episode limit.
/// </summary>
public class SetLimitRequest
{
    /// <summary>
    /// Number of episodes before stopping (0 = disabled, 1-100 = active).
    /// </summary>
    [Range(0, 100)]
    public int Limit { get; set; }
}

/// <summary>
/// Response with current episode limit status.
/// </summary>
public class EpisodeLimitResponse
{
    /// <summary>
    /// The configured limit (0 = disabled).
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// Number of episodes played so far in this session.
    /// </summary>
    public int EpisodesPlayed { get; set; }

    /// <summary>
    /// Whether the limit is currently active.
    /// </summary>
    public bool IsActive { get; set; }
}
