#!/bin/bash

# Episode Limit Plugin - Uninstaller (Docker & Native Compatible)
# https://github.com/Wavey1975/Jellyfinepisodelimitplugin

echo "🌙 Uninstalling Episode Limit Plugin..."

# Function to remove script from index.html
remove_script() {
    local file="$1"
    local container="$2"
    
    if [ -n "$container" ]; then
        # Docker mode: remove inside container
        docker exec "$container" sed -i '/Episode Limit/d' "$file" 2>/dev/null
        docker exec "$container" sed -i 's|<script src="/EpisodeLimit/episodelimit.js"></script>||' "$file" 2>/dev/null
        echo "✅ Script removed from container"
    else
        # Native mode: remove directly
        sed -i '/Episode Limit/d' "$file" 2>/dev/null
        sed -i 's|<script src="/EpisodeLimit/episodelimit.js"></script>||' "$file" 2>/dev/null
        echo "✅ Script removed"
    fi
}

# Detect installation type
if docker ps --format '{{.Names}}' | grep -qi jellyfin; then
    echo "📦 Docker installation detected"
    
    CONTAINER=$(docker ps --format '{{.Names}}' | grep -i jellyfin | head -1)
    echo "📦 Using container: $CONTAINER"
    
    INDEX_PATH=$(docker exec "$CONTAINER" find / -name "index.html" -path "*/jellyfin-web/*" 2>/dev/null | head -1)
    
    if [ -z "$INDEX_PATH" ]; then
        INDEX_PATH=$(docker exec "$CONTAINER" find / -name "index.html" -path "*/web/*" 2>/dev/null | head -1)
    fi
    
    if [ -n "$INDEX_PATH" ]; then
        remove_script "$INDEX_PATH" "$CONTAINER"
    else
        echo "⚠️ Could not find index.html inside container. Manual cleanup may be needed."
    fi
    
    echo "🔄 Restart the container: docker restart $CONTAINER"

elif [ -f /usr/share/jellyfin/web/index.html ]; then
    echo "🖥️ Native installation detected"
    
    remove_script "/usr/share/jellyfin/web/index.html" ""
    
    echo "🔄 Restart Jellyfin: sudo systemctl restart jellyfin"

else
    echo "⚠️ Could not detect Jellyfin installation."
fi

echo ""
echo "🌙 Also remove the plugin from: Dashboard → Plugins → My Plugins"
echo ""
echo "Done!"
