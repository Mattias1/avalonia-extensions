﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Declarative;
using Avalonia.Styling;
using System;
using System.Linq;

#if SIMPLE_THEME
using Avalonia.Themes.Simple;
#else
using Avalonia.Themes.Fluent;
#endif

namespace AvaloniaExtensions;

public static class AvaloniaExtensionsApp {
  public static string StartupPath => AssetExtensions.StartupPath;

  public static AppBuilder Init() => AppBuilder.Configure<Application>().UsePlatformDetect();
}

public static class AppBuilderExtensions {
  /// <summary>
  /// Add a settings file. It'll save the settings file when closing the app.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="path">The location of the settings file. You can use './filename.json' to save it in the apps dir.</param>
  /// <typeparam name="T">The type of the settings class.</typeparam>
  /// <returns></returns>
  public static AppBuilder WithSettingsFile<T>(this AppBuilder builder, string path) where T : class, new() {
    SettingsFiles.Get.AddSettingsFile(path, () => new T());
    return builder;
  }

  /// <summary>
  /// Add a settings file. It'll save the settings file when closing the app.
  /// </summary>
  /// <param name="builder"></param>
  /// <param name="path">The location of the settings file. You can use './filename.json' to save it in the apps dir.</param>
  /// <param name="constructorLambda">A lambda function to construct the settings object if it can't be loaded.</param>
  /// <typeparam name="T">The type of the settings class.</typeparam>
  /// <returns></returns>
  public static AppBuilder WithSettingsFile<T>(this AppBuilder builder, string path, Func<T> constructorLambda) where T : class {
    SettingsFiles.Get.AddSettingsFile(path, constructorLambda);
    return builder;
  }

  public static AppBuilder WithTheme(this AppBuilder builder, IStyle style) {
    if (builder.Instance is null) {
      throw new InvalidOperationException("No builder instance found for some reason.");
    }
    builder.Instance.Styles.Add(style);
    return builder;
  }

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
      // builder.WithTheme(new StyleInclude(new Uri("avares://Semi.Avalonia/Themes/")) {
      //     Source = new Uri("avares://Semi.Avalonia/Themes/Index.axaml")
      // });
#if SIMPLE_THEME
      builder.WithTheme(new SimpleTheme());
#else
      builder.WithTheme(new FluentTheme());
#endif
    }

    lifetime.MainWindow = windowFunc();
    lifetime.Start(Array.Empty<string>());

    if (builder.Instance is null) {
      throw new InvalidOperationException("Hmmph, cannot build the builder. That's weird.");
    }
    return builder.Instance;
  }
}
