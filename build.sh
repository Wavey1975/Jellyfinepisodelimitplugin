#!/bin/bash
# Build script for Episode Limit Plugin

echo "Building Episode Limit Plugin for Jellyfin..."

# Clean previous builds
rm -rf bin/ obj/

# Restore dependencies
dotnet restore

# Build in Release mode
dotnet build --configuration Release

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Build successful!"
    echo ""
    echo "Output files are in: bin/Release/net8.0/"
    echo ""
    echo "To install:"
    echo "1. Create folder: /var/lib/jellyfin/plugins/EpisodeLimit/"
    echo "2. Copy these files to that folder:"
    echo "   - bin/Release/net8.0/Jellyfin.Plugin.EpisodeLimit.dll"
    echo "   - meta.json"
    echo "3. Restart Jellyfin"
    echo ""
else
    echo ""
    echo "❌ Build failed. Check the errors above."
    exit 1
fi
