using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AvaloniaExtensions;

public sealed class ExtendedWindow : Window {
  private readonly Dictionary<Type, ViewBase> _components;
  private readonly Dictionary<Type, Func<ViewBase>> _lazyComponents;
  private readonly Dictionary<Type, SettingsFile> _settingsFiles;

  private ExtendedWindow() {
    _components = new Dictionary<Type, ViewBase>();
    _lazyComponents = new Dictionary<Type, Func<ViewBase>>();
    _settingsFiles = new Dictionary<Type, SettingsFile>();
  }

  // --- Miscellaneous functions ---
  public ExtendedWindow WithSize(Size size, Size minSize) {
    MinWidth = minSize.Width;
    MinHeight = minSize.Height;
    return WithSize(size);
  }
  public ExtendedWindow WithSize(Size size) => WithSize(size.Width, size.Height);
  public ExtendedWindow WithSize(double width, double height) {
    Width = width;
    Height = height;
    return this;
  }

  // --- Components (that make up the 'screens' or 'views' of the application) ---
  public void SwitchToComponent<T>() {
    Content = FindComponent<T>();
  }

  private ViewBase FindComponent<T>() {
    if (_components.TryGetValue(typeof(T), out ViewBase? component)) {
      return component;
    }
    if (_lazyComponents.TryGetValue(typeof(T), out Func<ViewBase>? componentFunc)) {
      var newComponent = componentFunc();
      _lazyComponents.Remove(typeof(T));
      _components.Add(typeof(T), newComponent);
      return newComponent;
    }
    throw new InvalidOperationException($"Cannot find component with type {typeof(T)}.");
  }

  private ExtendedWindow AddInitialComponent<T>(T component) where T : ViewBase {
    Content = component;
    return AddComponent(component);
  }

  public ExtendedWindow AddComponent<T>() where T : ViewBase, new() => AddComponent(new T());
  public ExtendedWindow AddComponent<T>(T component) where T : ViewBase {
    _components.Add(typeof(T), component);
    return this;
  }

  public ExtendedWindow AddLazyComponent<T>() where T : ViewBase, new() => AddLazyComponent(() => new T());
  public ExtendedWindow AddLazyComponent<T>(Func<T> componentFunc) where T : ViewBase {
    _lazyComponents.Add(typeof(T), componentFunc);
    return this;
  }

  // --- Settings files ---
  protected override void OnClosing(WindowClosingEventArgs e) {
    SaveSettings();
    base.OnClosing(e);
  }

  /// <summary>
  /// Add a settings file. It'll save the settings file when closing the app.
  /// </summary>
  /// <param name="path">The location of the settings file. You can use './filename.json' to save it in the apps dir.</param>
  /// <typeparam name="T">The type of the settings class</typeparam>
  /// <returns></returns>
  public ExtendedWindow WithSettingsFile<T>(string path) where T : class, new() {
    var settings = LoadOrCreateSettings<T>(path);
    _settingsFiles.Add(typeof(T), new SettingsFile(path, settings));
    return this;
  }

  private T LoadOrCreateSettings<T>(string path) where T : class, new() {
    try {
      using var stream = File.OpenRead(CompletePath(path));
      var result = JsonSerializer.Deserialize<T>(stream);
      return result ?? new T();
    } catch (Exception e) {
      Console.Error.WriteLine("An avalonia extensions app encountered an error while loading a settings file." +
          $"Settings type: '{typeof(T)}', error: '{e.Message}'.");
      return new T();
    }
  }

  private void SaveSettings() {
    foreach (var (type, settingsFile) in _settingsFiles) {
      try {
        using var stream = File.Create(CompletePath(settingsFile.Path));
        JsonSerializer.Serialize(stream, settingsFile.Settings);
      } catch (Exception e) {
        Console.Error.WriteLine("An avalonia extensions app encountered an error while saving a settings file." +
            $"Settings type: '{type}', error: '{e.Message}'.");
      }
    }
  }

  private string CompletePath(string path) {
    if (string.IsNullOrWhiteSpace(path)) {
      path = "./settings.json";
    }
    if (path.Substring(0, 2) == "./") {
      path = (AssetExtensions.StartupPath ?? "") + path.Substring(1);
    }
    return path;
  }

  public T GetSettings<T>() where T : class {
    if (!_settingsFiles.TryGetValue(typeof(T), out SettingsFile? settingsFile)) {
      throw new InvalidOperationException($"Cannot find settings with type {typeof(T)}. " +
          $"You can add it with the '{nameof(WithSettingsFile)}' method.");
    }
    if (settingsFile.Settings is not T settings) {
      throw new InvalidOperationException($"Cannot cast settings with type {typeof(T)} to its type, weird.");
    }
    return settings;
  }

  // --- Initialisation ---
  public static ExtendedWindow Init<T>(string windowTitle) where T : ViewBase, new() => Init(windowTitle, new T());
  public static ExtendedWindow Init<T>(string windowTitle, T initialComponent) where T : ViewBase {
    var window = new ExtendedWindow().AddInitialComponent(initialComponent);
    window.Title = windowTitle;
    return window;
  }

  private record SettingsFile(string Path, object Settings);
}
