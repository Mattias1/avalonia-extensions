using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Declarative;
using System;
using System.Collections.Generic;

namespace AvaloniaExtensions;

public sealed class ExtendedWindow : Window {
  // Unfortunately CanvasComponentBase.Parent is null during initialization, so we have to find another way
  private static ExtendedWindow? _windowObject;
  public static ExtendedWindow Get => _windowObject ?? throw new InvalidOperationException("No window created yet.");

  private readonly Dictionary<Type, ViewBase> _components;
  private readonly Dictionary<Type, Func<ViewBase>> _lazyComponents;

  public SettingsFiles SettingsFiles => SettingsFiles.Get;

  private ExtendedWindow() {
    _windowObject = this;
    _components = new Dictionary<Type, ViewBase>();
    _lazyComponents = new Dictionary<Type, Func<ViewBase>>();
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
  public T SwitchToComponent<T>() where T : ViewBase {
    var component = FindComponent<T>();
    if (component is CanvasComponentBase canvasComponent) {
      canvasComponent.ActivateOnSwitchingToComponent();
    }
    Content = component;
    return component;
  }

  private T FindComponent<T>() where T : ViewBase {
    if (_components.TryGetValue(typeof(T), out ViewBase? component)) {
      return component as T ?? throw new InvalidOperationException("Component is not of the expected type");
    }
    if (_lazyComponents.TryGetValue(typeof(T), out Func<ViewBase>? componentFunc)) {
      var newComponent = componentFunc() as T ?? throw new InvalidOperationException("Component is not of the expected type");
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
    SettingsFiles.SaveSettings();
    base.OnClosing(e);
  }

  /// <summary>
  /// Add a settings file. It'll save the settings file when closing the app.
  /// </summary>
  /// <param name="path">The location of the settings file. You can use './filename.json' to save it in the apps dir.</param>
  /// <typeparam name="T">The type of the settings class.</typeparam>
  /// <returns></returns>
  public ExtendedWindow WithSettingsFile<T>(string path) where T : class, new() {
    SettingsFiles.AddSettingsFile(path, () => new T());
    return this;
  }

  /// <summary>
  /// Add a settings file. It'll save the settings file when closing the app.
  /// </summary>
  /// <param name="path">The location of the settings file. You can use './filename.json' to save it in the apps dir.</param>
  /// <param name="constructorLambda">A lambda function to construct the settings object if it can't be loaded.</param>
  /// <typeparam name="T">The type of the settings class.</typeparam>
  /// <returns></returns>
  public ExtendedWindow WithSettingsFile<T>(string path, Func<T> constructorLambda) where T : class {
    SettingsFiles.AddSettingsFile(path, constructorLambda);
    return this;
  }

  // --- Initialisation ---
  public static ExtendedWindow Init<T>(string windowTitle) where T : ViewBase, new() {
    return Init(windowTitle, () => new T());
  }
  public static ExtendedWindow Init<T>(string windowTitle, T initialComponent) where T : ViewBase {
    return Init(windowTitle, () => initialComponent);
  }
  public static ExtendedWindow Init<T>(string windowTitle, Func<T> initialComponentConstructor) where T : ViewBase {
    var window = new ExtendedWindow().AddInitialComponent(initialComponentConstructor());
    window.Title = windowTitle;
    return window;
  }
}
