#!/bin/sh
cd ExampleApp/

# Clean up old builds if present
rm -rf publish-win-x64
rm -rf publish-linux-x64

# Build application
dotnet publish -c Release -o ./publish-win-x64 -f net7.0 -r win-x64 --self-contained /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true
dotnet publish -c Release -o ./publish-linux-x64 -f net7.0 -r linux-x64 --self-contained /p:PublishSingleFile=true /p:PublishTrimmed=true /p:IncludeNativeLibrariesForSelfExtract=true
