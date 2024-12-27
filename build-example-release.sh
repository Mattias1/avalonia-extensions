#!/bin/sh
cd ExampleApp/

# Clean up old builds if present
rm -rf publish-win-x64
rm -rf publish-linux-x64

# Build application (note that you might want to disable trimming, because it could cause problems)
dotnet publish -c Release -o ./publish-win-x64 -f net8.0 -r win-x64 --self-contained /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=true
dotnet publish -c Release -o ./publish-linux-x64 -f net8.0 -r linux-x64 --self-contained /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:PublishTrimmed=true
