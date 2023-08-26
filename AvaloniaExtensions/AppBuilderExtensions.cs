using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.LogicalTree;
using Avalonia.Markup.Declarative;
using Avalonia.Themes.Fluent;
using System;
using System.Linq;

namespace AvaloniaExtensions;

public static class AppBuilderExtensions {
  public static string? StartupPath => AssetExtensions.StartupPath;

  public static AppBuilder Init() => AppBuilder.Configure<Application>().UsePlatformDetect();

  public static Application StartDesktopApp(this AppBuilder builder, string windowTitle, Func<ViewBase> contentFunc) {
    return builder.StartDesktopApp(() => ExtendedWindow.Init(windowTitle, contentFunc()));
  }
  public static Application StartDesktopApp(this AppBuilder builder, string windowTitle, Func<ViewBase> contentFunc,
      Size size) {
    return builder.StartDesktopApp(() => ExtendedWindow.Init(windowTitle, contentFunc()).WithSize(size));
  }

  public static Application StartDesktopApp(this AppBuilder builder, string windowTitle, Func<ViewBase> contentFunc,
      Size size, Size minSize) {
    return builder.StartDesktopApp(() => ExtendedWindow.Init(windowTitle, contentFunc()).WithSize(size, minSize));
  }

  public static Application StartDesktopApp(this AppBuilder builder, Func<Window> windowFunc) {
    // Note that despite it looks like this uses a builder pattern, the order of method- and constructor-calls matter
    var lifetime = new ClassicDesktopStyleApplicationLifetime() {
        Args = Array.Empty<string>(),
        ShutdownMode = ShutdownMode.OnLastWindowClose
    };
    builder.SetupWithLifetime(lifetime);

    if (!builder.Instance?.Styles.Any() ?? false) {
      // builder.Instance.Styles.Add(new StyleInclude(new Uri("avares://Semi.Avalonia/Themes/")) {
      //     Source = new Uri("avares://Semi.Avalonia/Themes/Index.axaml")
      // });
      builder.Instance.Styles.Add(new FluentTheme());
    }

    lifetime.MainWindow = windowFunc();
    lifetime.MainWindow.OnActualThemeVariantChanged(() => {
      foreach (var child in lifetime.MainWindow.GetLogicalChildren()) {
        if (child is CanvasComponentBase canvasComponent) {
          canvasComponent.SetupThemeColours();
        }
      }
    });
    lifetime.Start(Array.Empty<string>());

    if (builder.Instance is null) {
      throw new InvalidOperationException("Hmmph, cannot build the builder. That's weird.");
    }
    return builder.Instance;
  }
}
