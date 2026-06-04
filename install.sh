#!/bin/bash

# Episode Limit Plugin - Auto Installer (Docker & Native Compatible)
# https://github.com/Wavey1975/Jellyfinepisodelimitplugin

echo "🌙 Installing Episode Limit Plugin..."

# Function to patch index.html
patch_index_html() {
    local file="$1"
    local container="$2"
    
    if [ -n "$container" ]; then
        # Docker mode: patch inside container
        if docker exec "$container" grep -q "Episode Limit" "$file" 2>/dev/null; then
            echo "✅ Script already injected in container."
            return 0
        fi
        
        docker exec "$container" sed -i 's|</head>|<script src="/EpisodeLimit/episodelimit.js"></script></head>|' "$file"
        echo "✅ Script injected into $file inside container"
    else
        # Native mode: patch directly
        if grep -q "Episode Limit" "$file"; then
            echo "✅ Script already injected."
            return 0
        fi
        
        sed -i 's|</head>|<script src="/EpisodeLimit/episodelimit.js"></script></head>|' "$file"
        echo "✅ Script injected into $file"
    fi
}

# Detect installation type
if docker ps --format '{{.Names}}' | grep -qi jellyfin; then
    echo "📦 Docker installation detected"
    
    # Find the Jellyfin container
    CONTAINER=$(docker ps --format '{{.Names}}' | grep -i jellyfin | head -1)
    echo "📦 Using container: $CONTAINER"
    
    # Find index.html inside the container
    INDEX_PATH=$(docker exec "$CONTAINER" find / -name "index.html" -path "*/jellyfin-web/*" 2>/dev/null | head -1)
    
    if [ -z "$INDEX_PATH" ]; then
        # Try alternative path
        INDEX_PATH=$(docker exec "$CONTAINER" find / -name "index.html" -path "*/web/*" 2>/dev/null | head -1)
    fi
    
    if [ -z "$INDEX_PATH" ]; then
        echo "❌ Could not find index.html inside the container."
        echo "💡 Manual fix: Open Browser Console (F12) and paste:"
        echo "   var s = document.createElement('script'); s.src = '/EpisodeLimit/episodelimit.js'; document.head.appendChild(s);"
        exit 1
    fi
    
    echo "📝 Found index.html at: $INDEX_PATH"
    patch_index_html "$INDEX_PATH" "$CONTAINER"
    
    echo ""
    echo "🔄 Restart the container: docker restart $CONTAINER"
    echo "🔄 Then hard-refresh your browser (Ctrl+Shift+R or Cmd+Shift+R)"
    echo ""
    echo "⚠️ Note: You will need to re-run this script after container updates."

elif [ -f /usr/share/jellyfin/web/index.html ]; then
    echo "🖥️ Native installation detected"
    
    patch_index_html "/usr/share/jellyfin/web/index.html" ""
    
    echo ""
    echo "🔄 Restart Jellyfin: sudo systemctl restart jellyfin"
    echo "🔄 Then hard-refresh your browser (Ctrl+Shift+R or Cmd+Shift+R)"

else
    echo "❌ Could not detect Jellyfin installation (Docker or Native)."
    echo "💡 Manual fix: Open Browser Console (F12) and paste:"
    echo "   var s = document.createElement('script'); s.src = '/EpisodeLimit/episodelimit.js'; document.head.appendChild(s);"
    exit 1
fi

echo ""
echo "🌙 Done! The moon button should now appear in your video player."
echo ""
echo "Next steps:"
echo "1. Add this repository in Jellyfin (Dashboard → Plugins → Manage Repositories):"
echo "   https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/manifest.json"
echo ""
echo "2. Install 'Episode Limit' from Plugins → Available → General"
echo ""
echo "3. Restart Jellyfin and refresh your browser"
echo ""
echo "🌙 Enjoy your sleep!"
