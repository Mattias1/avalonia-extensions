using Avalonia;
using AvaloniaExtensions;
using ExampleApp;

var size = new Size(800, 500);
var minSize = new Size(600, 350);
AppBuilderExtensions.Init().StartDesktopApp("Example app", () => new MainComponent(), size, minSize);
