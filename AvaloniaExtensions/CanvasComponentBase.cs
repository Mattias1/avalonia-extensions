using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Declarative;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;

namespace AvaloniaExtensions;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
public abstract class CanvasComponentBase : ComponentBase {
  private static readonly Dictionary<Canvas, CanvasComponentBase> CANVAS_COMPONENT_DICTIONARY = new();

  private readonly List<Action> _resizeActions = new List<Action>();
  public Canvas Canvas { get; private set; } = null!; // This will be initialized before it's being used (hopefully :P)

  public CustomStyle CustomStyle { get; set; } = new CustomStyle(new Thickness(10), 80,
      ThemedBrushes.FromHex("FAFAFA", "363636"));

  public Control? InitialControlToFocus { get; set; }

  protected override object Build() {
    Canvas = new Canvas();
    CANVAS_COMPONENT_DICTIONARY.Add(Canvas, this);
    InitializeControls();
    this.OnActualThemeVariantChanged(SetupThemeColours);
    return Canvas;
  }

  protected abstract void InitializeControls();

  protected override void OnSizeChanged(SizeChangedEventArgs e) {
    RepositionControls();
    base.OnSizeChanged(e);
  }

  public void RepositionControls() => _resizeActions.ForEach(action => action());

  protected override void OnLoaded(RoutedEventArgs e) {
    SetupThemeColours();
    InitialControlToFocus?.Focus();
    if (InitialControlToFocus is TextBox textBox) {
      textBox.CaretIndex = textBox.Text?.Length ?? 0;
    }
    base.OnLoaded(e);
  }

  public void SetupThemeColours() {
    if (CustomStyle.Background is not null) {
      Canvas.Background = CustomStyle.Background.ForTheme(ActualThemeVariant);
    }
  }

  internal T SetInitialControlToFocus<T>(T control) where T : Control {
    InitialControlToFocus = control;
    return control;
  }

  public static T RegisterOnResizeAction<T>(T control, Action resizeAction) where T : Control {
    var canvasComponent = FindCanvasComponent(control);
    canvasComponent.RegisterOnResizeAction(resizeAction);
    return control;
  }
  public void RegisterOnResizeAction(Action resizeAction) => _resizeActions.Add(resizeAction);

  public void SwitchToComponent<T>() => FindWindow().SwitchToComponent<T>();

  public T GetSettings<T>() where T : class => SettingsFiles.Get.GetSettings<T>();

  public void Quit() {
    if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktopApp) {
      throw new InvalidOperationException("This method only works for desktop applications");
    }
    desktopApp.Shutdown();
  }

  public ExtendedWindow FindWindow() => FindComponent<ExtendedWindow>(this);
  public static CanvasComponentBase FindCanvasComponent(StyledElement? element) {
    // Unfortunately Canvas.Parent is null during initialization, so we have to find another way
    var canvas = FindCanvas(element);
    return CANVAS_COMPONENT_DICTIONARY[canvas];
  }
  public static Canvas FindCanvas(StyledElement? element) => FindComponent<Canvas>(element);
  public static T FindComponent<T>(StyledElement? element) {
    while ((element = element?.Parent) is not null) {
      if (element is T result) {
        return result;
      }
    }
    throw new InvalidOperationException($"Cannot find parent of type {typeof(T)}");
  }

  // --- Control initializers ---
  public Button AddButton(string text, Action<RoutedEventArgs> onClick) => AddButton(text).OnClick(onClick);
  public Button AddButton(string text) {
    return Add(new Button())
        .Content(text)
        .MinWidth(CustomStyle.MinWidth)
        .HorizontalContentAlignment(HorizontalAlignment.Center);
  }

  public TextBox AddMultilineTextBox(string text) => AddMultilineTextBox().Text(text);
  public TextBox AddMultilineTextBox() => AddTextBox().AcceptsReturn(true).AcceptsTab(true);
  public TextBox AddTextBox(string text) => AddTextBox().Text(text);
  public TextBox AddTextBox() => Add(new TextBox()).MinWidth(CustomStyle.MinWidth * 2);

  public CheckBox AddCheckBox(string text, Action<RoutedEventArgs> onIsCheckedChanged) {
    var checkBox = AddCheckBox(text);
    checkBox.OnIsCheckedChanged(onIsCheckedChanged);
    return checkBox;
  }
  public CheckBox AddCheckBox(string text) => AddCheckBox().Content(text);
  public CheckBox AddCheckBox() => Add(new CheckBox());

  public RadioButton AddRadio(string groupName, string text, Action<RoutedEventArgs> onIsCheckedChanged) {
    var radio = AddRadio(groupName, text);
    radio.OnIsCheckedChanged(onIsCheckedChanged);
    return radio;
  }
  public RadioButton AddRadio(string groupName, string text) => AddRadio(groupName).Content(text);
  public RadioButton AddRadio(string groupName) => Add(new RadioButton()).GroupName(groupName);

  public ExtendedComboBox<T> AddComboBox<T>(IEnumerable<T> items,
      Action<SelectedItemChangedEventArgs<T>> onSelectedItemChanged) where T : class, IControlContentItem {
    return AddComboBox(items).OnSelectedItemChanged(onSelectedItemChanged);
  }
  public ExtendedComboBox<T> AddComboBox<T>(IEnumerable<T> items) where T : class, IControlContentItem {
    return AddComboBox(items, i => i.ControlContent());
  }
  public ExtendedComboBox<string> AddComboBox(IEnumerable<string> items,
      Action<SelectedItemChangedEventArgs<string>> onSelectedItemChanged) {
    return AddComboBox(items).OnSelectedItemChanged(onSelectedItemChanged);
  }
  public ExtendedComboBox<string> AddComboBox(IEnumerable<string> items) => AddComboBox(items, i => i);
  public ExtendedComboBox<T> AddComboBox<T>(IEnumerable<T> items, Func<T, string> contentFunc) where T : class {
    var comboBox = Add(new ExtendedComboBox<T>()).WithItems(items, contentFunc).MinWidth(CustomStyle.MinWidth);
    if (comboBox.Items.Count > 0) {
      comboBox.SelectedIndex = 0;
    }
    return comboBox;
  }

  public Label AddLabelRightOf(string text) => AddLabelRightOf(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  public Label AddLabelRightOf(string text, Control target) => AddLabel(text, target).XRightOf(target).YCenter(target);
  public Label AddLabelLeftOf(string text) => AddLabelLeftOf(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  public Label AddLabelLeftOf(string text, Control target) => AddLabel(text, target).XLeftOf(target).YCenter(target);
  public Label AddLabelAbove(string text) => AddLabelAbove(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  public Label AddLabelAbove(string text, Control target) => AddLabel(text, target).Above(target);
  public Label AddLabelBelow(string text) => AddLabelBelow(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  public Label AddLabelBelow(string text, Control target) => AddLabel(text, target).Below(target);
  public Label InsertLabelAbove(string text) {
    return InsertLabelAbove(text, CanvasControlExtensions.CurrentReferencedOrThrow);
  }
  public Label InsertLabelAbove(string text, Control target) {
    var label = AddLabel(text, target).XAlignLeft(target).YAlignTop(target);
    target.YBelow(label);
    return label;
  }
  public Label InsertLabelLeftOf(string text, double width = double.NaN) {
    return InsertLabelLeftOf(text, CanvasControlExtensions.CurrentReferencedOrThrow, width);
  }
  public Label InsertLabelLeftOf(string text, Control target, double width = double.NaN) {
    var label = AddLabel(text, target).XAlignLeft(target).YCenter(target).Width(width);
    target.XRightOf(label);
    return label;
  }
  public Label AddLabel(string text, Control target) => Add(new Label()).Content(text).Target(target);

  public TextBlock AddTextBlockHeader(string text) => AddTextBlock(text).FontSize(20).FontWeight(FontWeight.Bold);
  public TextBlock AddTextBlock(string text) => AddTextBlock().Text(text);
  public TextBlock AddTextBlock() => Add(new TextBlock());

  public Image AddImage(int width, int height) => AddImage().Width(width).Height(height);
  public Image AddImage(string fileName) => AddImage(new Bitmap(fileName));
  public Image AddImage(Bitmap bitmap) => AddImage().Source(bitmap);
  public Image AddImage() => Add(new Image());

  public Separator AddSeparator() {
    var control = Add(new Separator());
    control.Margin(control.Margin.Left * 2, control.Margin.Top, control.Margin.Right * 2, control.Margin.Bottom);
    return control;
  }

  public T Add<T>(T control) where T : Control {
    Canvas.Children.Add(control);
    return control.Ref().Margin(CustomStyle.Margin);
  }
}

public interface IControlContentItem {
  public string ControlContent();
}
