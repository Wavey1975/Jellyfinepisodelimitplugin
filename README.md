# 🌙 Episode Limit Plugin for Jellyfin

Stop playback after X episodes — perfect for falling asleep!

Adds a 🌙 moon button to the video player. Set how many episodes to play before automatically stopping.

---

## Installation

### Step 1: Add the Plugin Repository

1. Go to **Dashboard** → **Plugins** → **Manage Repositories**
2. Click **+ New Repository**
3. Add:
   - **Name:** `Episode Limit`
   - **URL:** `https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/manifest.json`
4. Click **Save**

### Step 2: Install the Plugin

1. Go to **Plugins** → **Available** → **General**
2. Click **Install** on Episode Limit
3. Restart Jellyfin

### Step 3: Enable the Player Button

SSH into your Jellyfin server and run:

```bash
curl -sSL https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/install.sh | bash
```

**For LXC containers (Proxmox):** First run `pct enter <container_id>` then run the command above.

### Step 4: Refresh Your Browser

Hard refresh your browser (Ctrl+Shift+R or Cmd+Shift+R) and you're done!

---

## Usage

1. Play any video
2. Click the 🌙 moon icon (bottom right of player)
3. Pick how many episodes (1-5 or custom)
4. Fall asleep! Playback stops automatically.

---

## Uninstall

```bash
curl -sSL https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/uninstall.sh | bash
```

Then remove the plugin from Jellyfin Dashboard → Plugins → My Plugins.

---

Made with 🌙 for better sleep

A collaboration between Wavey1975 and Claude (Opus 4.5 Ext), created Tuesday 10th March 2026.
