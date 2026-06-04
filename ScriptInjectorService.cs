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
    
    private const string ScriptMarker = "Episode Limit Plugin";
    private const string ManualInjectionInstruction = 
        "/* Episode Limit Plugin: Manual Injection Required for Jellyfin 10.11+ */\n" +
        "/* Due to security changes in Jellyfin 10.11, automatic script injection is disabled. */\n" +
        "/* To enable the moon button: */\n" +
        "/* 1. Open your browser's Developer Tools (F12 or Cmd+Option+I) */\n" +
        "/* 2. Go to the Console tab */\n" +
        "/* 3. Paste this code and press Enter: */\n" +
        "/* var s = document.createElement('script'); s.src = '/EpisodeLimit/episodelimit.js'; document.head.appendChild(s); */\n" +
        "/* 4. Refresh the page. The moon button should now appear! */\n";

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
            _logger.LogInformation("Episode Limit Plugin: Starting...");
            _logger.LogInformation("Episode Limit Plugin: IMPORTANT: Jellyfin 10.11+ blocks automatic script injection.");
            _logger.LogInformation("Episode Limit Plugin: Please follow the manual injection steps in the Custom CSS field.");

            // Write instructions to CustomCss (which still works in 10.11)
            var config = _configManager.GetConfiguration<BrandingOptions>("branding");
            var currentCss = config.CustomCss ?? string.Empty;

            if (!currentCss.Contains(ScriptMarker))
            {
                config.CustomCss = string.IsNullOrWhiteSpace(currentCss) 
                    ? ManualInjectionInstruction 
                    : currentCss + "\n" + ManualInjectionInstruction;

                _configManager.SaveConfiguration("branding", config);
                _logger.LogInformation("Episode Limit Plugin: Instructions written to Custom CSS. Check Dashboard > General > Custom CSS.");
            }
            else
            {
                _logger.LogInformation("Episode Limit Plugin: Instructions already present in Custom CSS.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Episode Limit Plugin: Failed to write instructions");
        }
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
