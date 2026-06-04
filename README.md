# 🌙 Episode Limit Plugin for Jellyfin

Stop playback after X episodes — perfect for falling asleep!

Adds a 🌙 moon button to the video player. Set how many episodes to play before automatically stopping.

## Installation

### Step 1: Add the Plugin Repository

1. Go to **Dashboard → Plugins → Manage Repositories**
2. Click **+ New Repository**
3. Add:
   - **Name:** `Episode Limit`
   - **URL:** `https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/manifest.json`
4. Click **Save**

### Step 2: Install the Plugin

1. Go to **Plugins → Available → General**
2. Click **Install** on Episode Limit
3. Restart Jellyfin

### Step 3: Enable the Player Button

**For Docker installations:**

SSH into your server and run:
```bash
curl -sSL https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/install.sh | bash
