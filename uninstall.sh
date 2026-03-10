#!/bin/bash

# Episode Limit Plugin - Uninstaller
# https://github.com/Wavey1975/Jellyfinepisodelimitplugin

echo "🌙 Uninstalling Episode Limit Plugin..."

systemctl disable episodelimit-patch.service 2>/dev/null
rm -f /etc/systemd/system/episodelimit-patch.service
rm -f /usr/local/bin/episodelimit-patch.sh
systemctl daemon-reload

echo ""
echo "✅ Uninstalled!"
echo ""
echo "Now remove the plugin from Jellyfin:"
echo "Dashboard → Plugins → My Plugins → Episode Limit → Uninstall"
