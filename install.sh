#!/bin/bash

# Episode Limit Plugin - Auto Installer
# https://github.com/Wavey1975/Jellyfinepisodelimitplugin

echo "🌙 Installing Episode Limit Plugin..."

# Create the patch script
cat > /usr/local/bin/episodelimit-patch.sh << 'SCRIPT'
#!/bin/bash
sleep 5
if ! grep -q "Episode Limit" /usr/share/jellyfin/web/index.html; then
    sed -i 's|</body></html>|<script>/* Episode Limit Plugin */(function(){var s=document.createElement("script");s.src="/EpisodeLimit/episodelimit.js";s.async=true;document.head.appendChild(s);})();</script></body></html>|' /usr/share/jellyfin/web/index.html
fi
SCRIPT
chmod +x /usr/local/bin/episodelimit-patch.sh

# Create the systemd service
cat > /etc/systemd/system/episodelimit-patch.service << 'EOF'
[Unit]
Description=Patch Jellyfin for Episode Limit Plugin
After=jellyfin.service

[Service]
Type=oneshot
ExecStart=/usr/local/bin/episodelimit-patch.sh

[Install]
WantedBy=multi-user.target
EOF

# Enable and run
systemctl daemon-reload
systemctl enable episodelimit-patch.service
systemctl start episodelimit-patch.service

echo ""
echo "✅ Installation complete!"
echo ""
echo "Now:"
echo "1. Add this repository in Jellyfin (Dashboard → Plugins → Manage Repositories):"
echo "   https://raw.githubusercontent.com/Wavey1975/Jellyfinepisodelimitplugin/main/manifest.json"
echo ""
echo "2. Install 'Episode Limit' from Plugins → Available → General"
echo ""
echo "3. Restart Jellyfin and refresh your browser"
echo ""
echo "🌙 Enjoy your sleep!"
