using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media.Imaging;

namespace AvaloniaExtensions;

// ReSharper disable MemberCanBePrivate.Global
public abstract class CanvasComponentBase : ComponentBase {
  private static readonly Dictionary<Canvas, CanvasComponentBase> CANVAS_COMPONENT_DICTIONARY = new();

  private readonly List<Action> _resizeActions = new List<Action>();
  protected Canvas Canvas { get; private set; } = null!; // This will be initialised before it's being used (hopefully :P)

  public virtual CustomStyle CustomStyle => new CustomStyle(new Thickness(10), 80,
      ThemedBrushes.FromHex("FAFAFA", "363636"));

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

  protected override void OnLoaded(RoutedEventArgs e) {
    SetupThemeColours();
    base.OnLoaded(e);
  }

  public void SetupThemeColours() {
    if (CustomStyle.Background is not null) {
      Canvas.Background = CustomStyle.Background.ForTheme(ActualThemeVariant);
    }
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
  protected Button AddButton(string text) {
    return Add(new Button())
        .Content(text)
        .MinWidth(CustomStyle.MinWidth)
        .HorizontalContentAlignment(HorizontalAlignment.Center);
  }

  protected TextBox AddMultilineTextBox(string text) => AddMultilineTextBox().Text(text);
  protected TextBox AddMultilineTextBox() => AddTextBox().AcceptsReturn(true).AcceptsTab(true);
  protected TextBox AddTextBox(string text) => AddTextBox().Text(text);
  protected TextBox AddTextBox() => Add(new TextBox()).MinWidth(CustomStyle.MinWidth);

  protected CheckBox AddCheckBox(string text, Action<RoutedEventArgs> onIsCheckedChanged) {
    var checkBox = AddCheckBox(text);
    checkBox.OnIsCheckedChanged(onIsCheckedChanged);
    return checkBox;
  }
  protected CheckBox AddCheckBox(string text) => AddCheckBox().Content(text);
  protected CheckBox AddCheckBox() => Add(new CheckBox());

  protected RadioButton AddRadio(string groupName, string text, Action<RoutedEventArgs> onIsCheckedChanged) {
    var radio = AddRadio(groupName, text);
    radio.OnIsCheckedChanged(onIsCheckedChanged);
    return radio;
  }
  protected RadioButton AddRadio(string groupName, string text) => AddRadio(groupName).Content(text);
  protected RadioButton AddRadio(string groupName) => Add(new RadioButton()).GroupName(groupName);

  protected ExtendedComboBox<T> AddComboBox<T>(IEnumerable<T> items,
      Action<SelectedItemChangedEventArgs<T>> onSelectedItemChanged) where T : class, IControlContentItem {
    return AddComboBox(items).OnSelectedItemChanged(onSelectedItemChanged);
  }
  protected ExtendedComboBox<T> AddComboBox<T>(IEnumerable<T> items) where T : class, IControlContentItem {
    return AddComboBox(items, i => i.ControlContent());
  }
  protected ExtendedComboBox<string> AddComboBox(IEnumerable<string> items,
      Action<SelectedItemChangedEventArgs<string>> onSelectedItemChanged) {
    return AddComboBox(items).OnSelectedItemChanged(onSelectedItemChanged);
  }
  protected ExtendedComboBox<string> AddComboBox(IEnumerable<string> items) => AddComboBox(items, i => i);
  protected ExtendedComboBox<T> AddComboBox<T>(IEnumerable<T> items, Func<T, string> contentFunc) where T : class {
    var comboBox = Add(new ExtendedComboBox<T>()).WithItems(items, contentFunc).MinWidth(CustomStyle.MinWidth);
    if (comboBox.Items.Count > 0) {
      comboBox.SelectedIndex = 0;
    }
    return comboBox;
  }

  protected Label AddLabelRightOf(string text) => AddLabelRightOf(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  protected Label AddLabelRightOf(string text, Control target) => AddLabel(text, target).XRightOf(target).YCenter(target);
  protected Label AddLabelLeftOf(string text) => AddLabelLeftOf(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  protected Label AddLabelLeftOf(string text, Control target) => AddLabel(text, target).XLeftOf(target).YCenter(target);
  protected Label InsertLabelLeftOf(string text) {
    return InsertLabelLeftOf(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  }
  protected Label InsertLabelLeftOf(string text, Control target) {
    var label = AddLabel(text, target).XAlignLeft(target).YCenter(target);
    target.XRightOf(label);
    return label;
  }
  protected Label AddLabel(string text, Control target) => Add(new Label()).Content(text).Target(target);

  protected TextBlock AddTextBlock(string text) => Add(new TextBlock()).Text(text);

  protected Image AddImage(int width, int height) => AddImage().Width(width).Height(height);
  protected Image AddImage(string fileName) => AddImage(new Bitmap(fileName));
  protected Image AddImage(Bitmap bitmap) => AddImage().Source(bitmap);
  protected Image AddImage() => Add(new Image());

  protected Separator AddSeparator() {
    var separator = Add(new Separator());
    var originalMargin = separator.Margin;
    separator.Margin(originalMargin.Left * 2, originalMargin.Top, originalMargin.Right * 2, originalMargin.Bottom);
    return separator;
  }

  protected T Add<T>(T control) where T : Control {
    Canvas.Children.Add(control);
    return control.Ref().Margin(CustomStyle.Margin);
  }
}

public interface IControlContentItem {
  public string ControlContent();
}
