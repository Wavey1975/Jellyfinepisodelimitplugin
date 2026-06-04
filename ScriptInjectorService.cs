using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting; // <-- THIS LINE WAS MISSING!

namespace Jellyfin.Plugin.EpisodeLimit.Services;

public class ScriptInjectorService : IHostedService
{
    private readonly ILogger<ScriptInjectorService> _logger;
    
    public ScriptInjectorService(ILogger<ScriptInjectorService> logger)
    {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Episode Limit Plugin: Starting...");
            _logger.LogWarning("Episode Limit Plugin: Jellyfin 10.11+ removed the 'CustomJs' branding option.");
            _logger.LogWarning("Episode Limit Plugin: Automatic script injection is disabled for security reasons.");
            _logger.LogInformation("Episode Limit Plugin: To enable the moon button, please run the 'install.sh' script or use the manual console method.");
            _logger.LogInformation("Episode Limit Plugin: Manual method: Open Browser Console (F12) and paste:");
            _logger.LogInformation("Episode Limit Plugin: var s = document.createElement('script'); s.src = '/EpisodeLimit/episodelimit.js'; document.head.appendChild(s);");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Episode Limit Plugin: Failed to start service");
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
