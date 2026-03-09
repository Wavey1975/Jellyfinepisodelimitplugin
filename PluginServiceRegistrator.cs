using Jellyfin.Plugin.EpisodeLimit.Services;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.EpisodeLimit;

/// <summary>
/// Register plugin services with the DI container.
/// </summary>
public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    /// <inheritdoc />
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        // Register PlaybackLimitService as singleton so API controller and hosted service share the same instance
        serviceCollection.AddSingleton<PlaybackLimitService>();
        serviceCollection.AddHostedService(sp => sp.GetRequiredService<PlaybackLimitService>());
    }
}
