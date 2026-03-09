using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.EpisodeLimit.Configuration;

/// <summary>
/// Plugin configuration.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// </summary>
    public PluginConfiguration()
    {
        // Default to 3 episodes (like your 1hr 9min timer for 23-min episodes!)
        EpisodeLimit = 3;
        IsEnabled = false;
    }

    /// <summary>
    /// Gets or sets the number of episodes to play before stopping (1-100).
    /// </summary>
    public int EpisodeLimit { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the episode limit is currently active.
    /// </summary>
    public bool IsEnabled { get; set; }
}
