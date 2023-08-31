using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaExtensions;

namespace ExampleApp;

public class SettingsComponent : CanvasComponentBase {
  private const int LABEL_WIDTH = 100;

  private ExampleSettings? _settings;
  private ExampleSettings Settings => _settings ??= GetSettings<ExampleSettings>();

  private CheckBox _cbExampleToggle = null!;
  private TextBox _tbExampleString = null!;

  protected override void InitializeControls() {
    AddTextBlockHeader("Some settings").TopLeftInPanel();
    _cbExampleToggle = AddCheckBox("Something to toggle").Below();
    InsertLabelAbove("A label above, to show it off");
    _tbExampleString = AddTextBox().Below(_cbExampleToggle);
    InsertLabelLeftOf("Checkbox:", _cbExampleToggle, LABEL_WIDTH);
    InsertLabelLeftOf("Text:", _tbExampleString, LABEL_WIDTH);

    AddButton("Reset defaults", OnResetSettingsClick).BottomLeftInPanel();
    AddButton("Cancel", OnCancelClick).BottomRightInPanel();
    AddButton("Ok", OnSaveClick).LeftOf();
  }

  protected override void OnInitialized() {
    base.OnInitialized();
    LoadSettings();
  }

  private void OnSaveClick(RoutedEventArgs e) {
    Settings.ExampleToggle = _cbExampleToggle.IsChecked ?? false;
    Settings.ExampleString = _tbExampleString.Text;
    SwitchToComponent<MainComponent>();
  }

  private void OnCancelClick(RoutedEventArgs e) {
    LoadSettings();
    SwitchToComponent<MainComponent>();
  }

  private void OnResetSettingsClick(RoutedEventArgs e) {
    FindWindow().ResetSettings<ExampleSettings>();
    _settings = null;
    LoadSettings();
  }

  private void LoadSettings() {
    _cbExampleToggle.IsChecked = Settings.ExampleToggle;
    _tbExampleString.Text = Settings.ExampleString;
  }

  public class ExampleSettings {
    public bool ExampleToggle { get; set; }
    public string? ExampleString { get; set; }
  }
}
