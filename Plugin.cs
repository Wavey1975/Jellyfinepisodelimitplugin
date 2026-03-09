using System;
using System.Collections.Generic;
using Jellyfin.Plugin.EpisodeLimit.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.EpisodeLimit;

/// <summary>
/// Episode Limit Plugin - Stops playback after a specified number of episodes.
/// Perfect for a sleep timer based on episode count rather than time.
/// </summary>
public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
    public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
        : base(applicationPaths, xmlSerializer)
    {
        Instance = this;
    }

    /// <inheritdoc />
    public override string Name => "Episode Limit";

    /// <inheritdoc />
    public override Guid Id => Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");

    /// <inheritdoc />
    public override string Description => "Automatically stops playback after a specified number of episodes. Great for falling asleep to your favourite shows!";

    /// <summary>
    /// Gets the current plugin instance.
    /// </summary>
    public static Plugin? Instance { get; private set; }

    /// <inheritdoc />
    public IEnumerable<PluginPageInfo> GetPages()
    {
        return new[]
        {
            new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = $"{GetType().Namespace}.Configuration.configPage.html"
            },
            new PluginPageInfo
            {
                Name = "episodelimit.js",
                EmbeddedResourcePath = $"{GetType().Namespace}.Web.episodelimit.js"
            }
        };
    }
}
