using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Declarative;

namespace AvaloniaExtensions;

public sealed class ExtendedWindow : Window {
  private readonly Dictionary<Type, ViewBase> _components;
  private readonly Dictionary<Type, Func<ViewBase>> _lazyComponents;

  private ExtendedWindow() {
    _components = new Dictionary<Type, ViewBase>();
    _lazyComponents = new Dictionary<Type, Func<ViewBase>>();
  }

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

  public static ExtendedWindow Init<T>(string windowTitle) where T : ViewBase, new() => Init(windowTitle, new T());
  public static ExtendedWindow Init<T>(string windowTitle, T initialComponent) where T : ViewBase {
    var window = new ExtendedWindow().AddInitialComponent(initialComponent);
    window.Title = windowTitle;
    return window;
  }
}
