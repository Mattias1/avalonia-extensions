using AvaloniaExtensions;

namespace ExampleApp;

public class SettingsComponent : CanvasComponentBase {
  protected override void InitializeControls() {
    AddTextBlock("This is an imaginary settings component").TopLeftInPanel();
    AddButton("Cancel", _ => SwitchToComponent<MainComponent>()).BottomRightInPanel();
    AddButton("Ok", _ => SwitchToComponent<MainComponent>()).LeftOf();

    AddButton("Quit", _ => Quit()).BottomLeftInPanel();
  }
}
