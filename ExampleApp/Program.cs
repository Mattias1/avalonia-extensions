using Avalonia;
using AvaloniaExtensions;
using ExampleApp;

var size = new Size(700, 400);
var minSize = new Size(400, 260);
AppBuilderExtensions.Init().StartDesktopApp("Example app", () => new MainComponent(), size, minSize);
