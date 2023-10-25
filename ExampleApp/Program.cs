using Avalonia;
using AvaloniaExtensions;
using Avalonia.Markup.Declarative;
using ExampleApp;

AvaloniaExtensionsApp.Init()
    .WithSettingsFile<SettingsComponent.ExampleSettings>("./example-settings.json")
    .StartDesktopApp(() => ExtendedWindow.Init<MainComponent>("Example app")
        .AddLazyComponent<SettingsComponent>()
        .WithSize(size: new Size(800, 500), minSize: new Size(600, 350))
        .Icon(AssetExtensions.LoadWindowIcon("assets/smiley.png")));
