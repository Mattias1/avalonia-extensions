using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Declarative;

namespace AvaloniaExtensions;

public abstract class CanvasComponentBase : ComponentBase {
  private static readonly Dictionary<Canvas, CanvasComponentBase> CANVAS_COMPONENT_DICTIONARY = new();

  private readonly List<Action> _resizeActions = new List<Action>();
  protected Canvas Canvas { get; private set; } = null!; // This will be initialised before it's being used (hopefully :P)

  protected override object Build() {
    Canvas = new Canvas();
    CANVAS_COMPONENT_DICTIONARY.Add(Canvas, this);
    InitializeControls();
    return Canvas;
  }

  protected abstract void InitializeControls();

  protected override void OnSizeChanged(SizeChangedEventArgs e) {
    foreach (var action in _resizeActions) {
      action();
    }
    base.OnSizeChanged(e);
  }

  public static T RegisterOnResizeAction<T>(T control, Action resizeAction) where T : Control {
    var canvasComponent = FindComponent(control);
    canvasComponent.RegisterOnResizeAction(resizeAction);
    return control;
  }
  public void RegisterOnResizeAction(Action resizeAction) => _resizeActions.Add(resizeAction);

  public static CanvasComponentBase FindComponent(StyledElement? element) {
    // Unfortunately Canvas.Parent is null during initialization, so we have to find another way
    var canvas = FindCanvas(element);
    return CANVAS_COMPONENT_DICTIONARY[canvas];
  }
  public static Canvas FindCanvas(StyledElement? element) {
    while ((element = element?.Parent) is not null) {
      if (element is Canvas result) {
        return result;
      }
    }
    throw new InvalidOperationException("Cannot find parent of type Canvas");
  }

  // --- Control initializers ---
  protected Button AddButton(string text, Action<RoutedEventArgs> onClick) => AddButton(text).OnClick(onClick);
  protected Button AddButton(string text) => Add(new Button()).Content(text);

  protected TextBox AddMultilineTextBox(string text) => AddMultilineTextBox().Text(text);
  protected TextBox AddMultilineTextBox() => AddTextBox().AcceptsReturn(true).AcceptsTab(true);
  protected TextBox AddTextBox(string text) => AddTextBox().Text(text);
  protected TextBox AddTextBox() => Add(new TextBox());

  protected T Add<T>(T control) where T : Control {
    Canvas.Children.Add(control);
    return control.Ref();
  }
}
