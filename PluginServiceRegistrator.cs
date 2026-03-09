using Jellyfin.Plugin.EpisodeLimit.Services;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.EpisodeLimit;

public class PluginServiceRegistrator : IPluginServiceRegistrator
{
    public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
    {
        serviceCollection.AddSingleton<PlaybackLimitService>();
        serviceCollection.AddHostedService(sp => sp.GetRequiredService<PlaybackLimitService>());
        serviceCollection.AddHostedService<ScriptInjectorService>();
    }
}
