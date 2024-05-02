using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaloniaExtensions;

namespace ExampleApp;

public class SettingsComponent : CanvasComponentBase {
  private const int LABEL_WIDTH = 100;

  private ExampleSettings? _settings;
  private ExampleSettings Settings => _settings ??= GetSettings<ExampleSettings>();

  private CheckBox _cbExampleToggle = null!;
  private TextBox _tbExampleString = null!;

  private Control _last = null!;

  protected override void InitializeControls() {
    AddTextBlockHeader("Some settings").TopLeftInPanel();
    _cbExampleToggle = AddCheckBox("Something to toggle").Below();
    InsertLabelAbove("A label above, to show it off");
    _tbExampleString = AddTextBox().Below(_cbExampleToggle).WithInitialFocus();
    InsertLabelLeftOf("Checkbox:", _cbExampleToggle, LABEL_WIDTH);
    InsertLabelLeftOf("Text:", _tbExampleString, LABEL_WIDTH);

    AddButton("Reset defaults", OnResetSettingsClick).HotKeyAlt(Key.R).BottomLeftInPanel();
    var btnOk = AddButton("Ok", OnSaveClick).HotKeyCtrl(Key.Enter);
    AddButton("Cancel", OnCancelClick).HotKey("Ctrl+Escape").BottomRightInPanel();
    btnOk.LeftOf();

    _last = _tbExampleString;
  }

  protected override void OnInitialized() {
    base.OnInitialized();
    LoadSettings();
  }

  private void OnCancelClick(RoutedEventArgs e) {
    LoadSettings();
    SwitchToComponent<MainComponent>();
  }

  private void OnSaveClick(RoutedEventArgs e) {
    SaveSettings();
    SwitchToComponent<MainComponent>();
  }

  private void OnResetSettingsClick(RoutedEventArgs e) {
    SettingsFiles.Get.ResetSettings<ExampleSettings>();
    _settings = null;
    LoadSettings();
  }

  private void LoadSettings() {
    _cbExampleToggle.IsChecked = Settings.ExampleToggle;
    _tbExampleString.Text = Settings.ExampleString;
  }

  private void SaveSettings() {
    Settings.ExampleToggle = _cbExampleToggle.IsChecked ?? false;
    Settings.ExampleString = _tbExampleString.Text;
  }

  protected override void OnSwitchingToComponent() {
    _last = AddTextBlock("You switched to the settings component.").Below(_last);
    RepositionControls();
  }

  public class ExampleSettings {
    public bool ExampleToggle { get; set; }
    public string? ExampleString { get; set; }
  }
}
