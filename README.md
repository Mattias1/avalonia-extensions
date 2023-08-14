Avalonia Extensions
====================
Some extensions to quickly build Avalonia UI desktop applications without needing XAML. Built on top
of [Avalonia.Markup.Declarative](https://github.com/AvaloniaUI/Avalonia.Markup.Declarative).

Note that while using these extensions allow you to very quickly setup an application, they do
somewhat push you into using Avalonia UI my way, rather than extending Avalonia UI generically.


NuGet packages
---------------
You can install the avalonia extensions via the NuGet package
[Mattias1.AvaloniaExtensions](https://www.nuget.org/packages/Mattias1.AvaloniaExtensions).


Example
--------
This is a simple example application:
``` csharp
using AvaloniaExtensions;

AppBuilderExtensions.Init().StartDesktopApp("Example app", () => new ReadmeComponent());

public class ReadmeComponent : CanvasComponentBase {
  protected override void InitializeControls() {
    AddTextBlock("A simple example application").TopLeftInPanel();
    AddButton("Cancel").BottomRightInPanel();
    AddButton("Ok").LeftOf();
  }
}
```

For a more elaborate example, you can take a look at the
[Example App](https://github.com/Mattias1/avalonia-extensions/tree/master/ExampleApp) source.


Setup development environment
------------------------------
Clone the [Avalonia.Markup.Declarative](https://github.com/AvaloniaUI/Avalonia.Markup.Declarative)
repo in the `lib/` folder, then run `cd ExampleApp/ && dotnet run`.


Publish release
----------------
Create a github release with a tag named 'vx.y.z'.
