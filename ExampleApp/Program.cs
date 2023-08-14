using Avalonia;
using AvaloniaExtensions;
using ExampleApp;

AppBuilderExtensions.Init().StartDesktopApp(() => ExtendedWindow.Init<MainComponent>("Example app")
    .AddLazyComponent<SettingsComponent>()
    .WithSize(size: new Size(800, 500), minSize: new Size(600, 350)));
