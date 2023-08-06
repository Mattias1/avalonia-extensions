using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Declarative;
using AvaloniaExtensions;

namespace ExampleApp;

public class MainComponent : CanvasComponentBase {
  private TextBox _tb = null!;

  protected override void InitializeControls() {
    // Some realistic-ish example controls
    AddButton("Hello world", AddText("Whazzup folks, watcha doing?")).TopLeftInPanel();
    InsertLabelLeftOf("Say:");
    AddMultilineTextBox().Ref(out _tb).Below();

    AddButton("Cancel", ClearText()).BottomRightInPanel();
    AddButton("Ok", AddText("kk")).LeftOf();

    AddRadio("r-group-1", "Left 2", AddTextIfChecked("Left radio two")).BottomLeftInPanel();
    AddRadio("r-group-1", "Left 1", AddTextIfChecked("Left radio one")).Ref(out var radioLeft1).Above();
    AddRadio("r-group-2", "Right 1", AddTextIfChecked("Right radio one")).RightOf();
    AddRadio("r-group-2", "Right 2", AddTextIfChecked("Right radio two")).Below();
    AddCheckBox("Check 2", AddTextIfChecked("Checkbox two")).RightOf();
    AddCheckBox("Check 1", AddTextIfChecked("Checkbox one")).Above();

    _tb.StretchRightInPanel().StretchDownTo(radioLeft1);

    // Some extra buttons to show off
    AddButton("CC").Ref(out var cc).CenterInPanel();
    AddButton("CC T").Above(cc);
    AddButton("CC L ==").Ref(out var ccl).LeftOf(cc).StretchDownInPanel();
    AddButton("CC R").RightOf(cc);
    AddButton("CC B").Below(cc);

    AddButton("TC").TopCenterInPanel();
    AddButton("CL").CenterLeftInPanel();
    AddButton("CR").CenterRightInPanel();
    AddButton("BC").BottomCenterInPanel();
  }

  private Action<RoutedEventArgs> AddText(string text) => _ => _tb.Text += text + '\n';

  protected Action<RoutedEventArgs> AddTextIfChecked(string text) {
    return e => {
      var sender = e.Source as ToggleButton;
      if (sender?.IsChecked == true) {
        _tb.Text += text + '\n';
      }
    };
  }

  private Action<RoutedEventArgs> ClearText() => _ => _tb.Text = "";
}
