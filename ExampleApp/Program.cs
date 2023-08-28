﻿using Avalonia;
using AvaloniaExtensions;
using Avalonia.Markup.Declarative;
using ExampleApp;

AppBuilderExtensions.Init().StartDesktopApp(() => ExtendedWindow.Init<MainComponent>("Example app")
    .AddLazyComponent<SettingsComponent>()
    .WithSettingsFile<SettingsComponent.ExampleSettings>("./example-settings.json")
    .WithSize(size: new Size(800, 500), minSize: new Size(600, 350))
    .Icon(AssetExtensions.LoadWindowIcon("assets/smiley.png")));
