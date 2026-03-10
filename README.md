# 🌙 Episode Limit Plugin for Jellyfin

Stop playback after X episodes — perfect for falling asleep!

Adds a 🌙 moon button to the video player. Set how many episodes to play before automatically stopping.

---

## Installation

### Step 1: Run the Installer

SSH into your Jellyfin server and run:

```bash
curl -sSL https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/install.sh | bash
```

**For LXC containers (Proxmox):** First run `pct enter <container_id>` then run the command above.

### Step 2: Add the Plugin in Jellyfin

1. Go to **Dashboard** → **Plugins** → **Manage Repositories**
2. Click **+ New Repository**
3. Add:
   - **Name:** `Episode Limit`
   - **URL:** `https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/manifest.json`

### Step 3: Install and Restart

1. Go to **Plugins** → **Available** → **General**
2. Click **Install** on Episode Limit
3. Restart Jellyfin
4. Hard refresh your browser (Ctrl+Shift+R or Cmd+Shift+R)

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

Then remove the plugin from Jellyfin Dashboard.

---

Made with 🌙 for better sleep
