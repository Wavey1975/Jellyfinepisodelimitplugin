# Episode Limit Plugin for Jellyfin

🌙 **Stop playback automatically after X episodes** — with a button right in the player!

Perfect for falling asleep to your favourite shows without them playing all night.

---

## Features

- ✅ **In-player control** — Bedtime button appears right in the video player
- ✅ Quick presets: Off, 1, 2, 3, 4, 5 files
- ✅ Custom limit up to 100 files
- ✅ Visual indicator (green icon) when active
- ✅ Per-device tracking — set different limits on tablet vs TV
- ✅ Works with episodes, movies, music videos — anything!

---

## Installation

### From Jellyfin Plugin Repository (Recommended)

1. In Jellyfin, go to **Dashboard → Plugins → Repositories**
2. Add repository: `https://raw.githubusercontent.com/Wavey1975/jellyfin-plugin-episodelimit/main/manifest.json`
3. Go to **Catalog** and find "Episode Limit"
4. Click **Install**
5. Restart Jellyfin
6. **Enable the player button** (see below)

### Manual Installation

1. Download the latest release from [Releases](https://github.com/Wavey1975/jellyfin-plugin-episodelimit/releases)
2. Extract to your Jellyfin plugins folder:
   - **Linux**: `/var/lib/jellyfin/plugins/EpisodeLimit/`
   - **Docker/LXC**: `/config/plugins/EpisodeLimit/`
   - **Windows**: `C:\ProgramData\Jellyfin\Server\plugins\EpisodeLimit\`
3. Restart Jellyfin
4. **Enable the player button** (see below)

### Enable the Player Button (Required)

After installing, add this line to **Dashboard → General → Custom JavaScript**:

```javascript
/* Episode Limit Plugin */
(function(){var s=document.createElement('script');s.src='/EpisodeLimit/episodelimit.js';s.async=true;document.head.appendChild(s);})();
```

Click **Save**, then refresh your browser. The 🌙 button will appear in the video player!

---

## How to Use

1. **Start watching** any TV episode or movie
2. **Tap the screen** to show player controls
3. **Tap the 🌙 bedtime icon** in the control bar
4. **Choose your limit** — quick buttons (1-5) or custom number
5. **Sleep easy!** — Playback stops after your episodes finish

The icon turns **green** when a limit is active.

### Works with Any Media

The plugin counts **completed files**, not time. So it works with:
- 🎬 Short episodes (sitcoms, cartoons)
- 📺 Long episodes (dramas, documentaries)
- 🎥 Movies (set to 1 for one movie, 3 for a trilogy, etc.)
- 🎵 Music videos, concerts, anything!

**Set it to 3** = playback stops after 3 files finish, regardless of length.

---

## Building from Source

Requirements: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
git clone https://github.com/Wavey1975/jellyfin-plugin-episodelimit.git
cd jellyfin-plugin-episodelimit
dotnet build --configuration Release
```

Output: `bin/Release/net8.0/Jellyfin.Plugin.EpisodeLimit.dll`

---

## For Developers: Submitting to Jellyfin Plugin Repository

Want to make this available to everyone? Here's how to submit to the official (or community) Jellyfin plugin repositories.

### Option A: Create Your Own Plugin Repository (Easiest)

This lets users add your repo and install directly from Jellyfin's UI.

1. **Create a GitHub repository** for your plugin

2. **Set up GitHub Actions** — the `.github/workflows/build.yml` file is already included

3. **Create a release**:
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
   GitHub Actions will build and create the release automatically.

4. **Create `manifest.json`** in your repo root:
   ```json
   [
     {
       "guid": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
       "name": "Episode Limit",
       "overview": "Stop playback after X episodes",
       "description": "Adds a bedtime button to the player...",
       "owner": "Wavey1975",
       "category": "General",
       "versions": [
         {
           "version": "1.0.0.0",
           "changelog": "Initial release",
           "targetAbi": "10.9.0.0",
           "sourceUrl": "https://github.com/Wavey1975/jellyfin-plugin-episodelimit/releases/download/v1.0.0/episode-limit-1.0.0.zip",
           "checksum": "MD5_HASH_HERE",
           "timestamp": "2025-03-09T00:00:00Z"
         }
       ]
     }
   ]
   ```

5. **Get the MD5 checksum** of your release zip:
   ```bash
   md5sum episode-limit-1.0.0.zip
   ```

6. **Users can now add your repository**:
   ```
   https://raw.githubusercontent.com/Wavey1975/jellyfin-plugin-episodelimit/main/manifest.json
   ```

### Option B: Submit to Official Jellyfin Repository

The official repository has stricter requirements but reaches more users.

1. **Review the guidelines**: https://jellyfin.org/docs/general/server/plugins/

2. **Ensure your plugin meets requirements**:
   - Clean, well-documented code
   - Follows Jellyfin coding standards
   - Has a clear purpose and doesn't duplicate existing functionality
   - Includes proper error handling
   - Has a LICENSE file (MIT, GPL, etc.)

3. **Fork the official repository**:
   ```bash
   git clone https://github.com/jellyfin/jellyfin-plugin-repository.git
   ```

4. **Add your plugin to `manifest.json`** — add your plugin entry to the array

5. **Submit a Pull Request** to https://github.com/jellyfin/jellyfin-plugin-repository

6. **Wait for review** — maintainers will review your plugin

### Option C: Submit to Community/Third-Party Repositories

There are community-maintained repositories with easier submission processes:

- **Jellyfin Plugin Repository (Community)**: Search GitHub for community plugin collections
- Create an issue or PR on their repository following their guidelines

---

## File Structure

```
JellyfinEpisodeLimitPlugin/
├── Plugin.cs                      # Main plugin entry
├── PluginServiceRegistrator.cs    # DI registration
├── Api/
│   ├── EpisodeLimitController.cs  # API for set/get/clear
│   └── ClientScriptController.cs  # Serves player JavaScript
├── Configuration/
│   ├── PluginConfiguration.cs     # Settings storage
│   └── configPage.html            # Dashboard help page
├── Services/
│   ├── PlaybackLimitService.cs    # Monitors & stops playback
│   └── ScriptInjectorService.cs   # Auto-registers JavaScript
├── Web/
│   └── episodelimit.js            # Player button & dialog
├── .github/
│   └── workflows/build.yml        # GitHub Actions CI/CD
├── meta.json                      # Plugin metadata
└── repository-manifest.json       # Template for repo submission
```

---

## Troubleshooting

### Button not appearing

1. Go to **Dashboard → Plugins** and confirm Episode Limit is installed
2. Check **Dashboard → General → Custom JavaScript** — you should see the Episode Limit script
3. Clear browser cache and refresh (Ctrl+Shift+R)
4. Check browser console (F12) for errors

### Playback not stopping

- Only **completed** episodes count (not skipped or manually stopped)
- Check server logs for "Episode Limit:" messages
- The limit resets after triggering — set it again for your next session

### Manual script removal

If you uninstall and want to remove the auto-injected script:
1. Go to **Dashboard → General → Custom JavaScript**
2. Delete the block between `/* Episode Limit Plugin */` comments
3. Save

---

## Compatibility

- **Jellyfin**: 10.9.x and later
- **Clients**: Web player and any client using Jellyfin's web interface
- **Note**: Native mobile apps may not show the button unless they use the web player

---

## License

MIT License — Feel free to use, modify, and share!

---

## Contributing

Pull requests welcome! Please:
- Follow existing code style
- Test on Jellyfin 10.9+
- Update documentation if needed

---

*Made for neurodivergent folks and anyone who needs predictable wind-down routines. Sweet dreams! 😴*
