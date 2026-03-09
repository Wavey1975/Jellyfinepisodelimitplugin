using System;
using System.Threading;
using System.Threading.Tasks;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Branding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.EpisodeLimit.Services;

public class ScriptInjectorService : IHostedService
{
    private readonly IServerConfigurationManager _configManager;
    private readonly ILogger<ScriptInjectorService> _logger;
    
    private const string ScriptTag = @"/* Episode Limit Plugin */(function(){var s=document.createElement('script');s.src='/EpisodeLimit/episodelimit.js';s.async=true;document.head.appendChild(s);})();";
    private const string ScriptMarker = "Episode Limit Plugin";

    public ScriptInjectorService(
        IServerConfigurationManager configManager,
        ILogger<ScriptInjectorService> logger)
    {
        _configManager = configManager;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var config = _configManager.GetConfiguration<BrandingOptions>("branding");
            var currentJs = config.CustomJs ?? string.Empty;

            if (currentJs.Contains(ScriptMarker))
            {
                _logger.LogInformation("Episode Limit: Client script already registered");
                return Task.CompletedTask;
            }

            config.CustomJs = string.IsNullOrWhiteSpace(currentJs) 
                ? ScriptTag 
                : currentJs + "\n" + ScriptTag;

            _configManager.SaveConfiguration("branding", config);
            _logger.LogInformation("Episode Limit: Client script auto-registered! Refresh your browser.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Episode Limit: Failed to auto-register script");
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
