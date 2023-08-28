using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaExtensions;

namespace ExampleApp;

public class SettingsComponent : CanvasComponentBase {
  private ExampleSettings? _settings;
  private ExampleSettings Settings => _settings ??= GetSettings<ExampleSettings>();

  private CheckBox _cbExampleToggle = null!;

  protected override void InitializeControls() {
    AddTextBlock("This is a simple settings component").TopLeftInPanel();
    _cbExampleToggle = AddCheckBox("Something to toggle").Below();

    AddButton("Cancel", OnCancelClick).BottomRightInPanel();
    AddButton("Ok", OnSaveClick).LeftOf();
  }

  protected override void OnInitialized() {
    base.OnInitialized();
    LoadSettings();
  }

  private void OnSaveClick(RoutedEventArgs e) {
    Settings.ExampleToggle = _cbExampleToggle.IsChecked ?? false;
    SwitchToComponent<MainComponent>();
  }

  private void OnCancelClick(RoutedEventArgs e) {
    LoadSettings();
    SwitchToComponent<MainComponent>();
  }

  private void LoadSettings() {
    _cbExampleToggle.IsChecked = Settings.ExampleToggle;
  }

  public class ExampleSettings {
    public bool ExampleToggle { get; set; }
  }
}
