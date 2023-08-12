using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using AvaloniaExtensions;

namespace ExampleApp;

public class MainComponent : CanvasComponentBase {
  private TextBox _tb = null!;

  protected override void InitializeControls() {
    // Some realistic-ish example controls
    var btnHelloWorld = AddButton("Hello world", AddText("Whazzup folks, watcha doing?")).TopLeftInPanel();
    _tb = AddMultilineTextBox().Below(btnHelloWorld);
    InsertLabelLeftOf("Say:", btnHelloWorld);
    AddComboBox(new [] { "item1", "item2", "item3" }, AddSelectedText).TopRightInPanel();
    AddLabelLeftOf("Select:");

    AddImage(AppBuilderExtensions.StartupPath + "/assets/smiley.png").TopCenterInPanel().YCenter(btnHelloWorld);

    AddButton("Cancel", ClearText()).BottomRightInPanel();
    AddButton("Ok", AddText("kk")).LeftOf();

    AddRadio("r-group-1", "Left 2", AddTextIfChecked("Left radio two")).BottomLeftInPanel();
    var radioLeft1 = AddRadio("r-group-1", "Left 1", AddTextIfChecked("Left radio one")).Above();
    AddRadio("r-group-2", "Right 1", AddTextIfChecked("Right radio one")).RightOf();
    AddRadio("r-group-2", "Right 2", AddTextIfChecked("Right radio two")).Below();
    AddCheckBox("Check 2", AddTextIfChecked("Checkbox two")).RightOf();
    AddCheckBox("Check 1", AddTextIfChecked("Checkbox one")).Above();

    var separator = AddSeparator().Above(radioLeft1).StretchRightInPanel();
    _tb.StretchRightInPanel().StretchDownTo(separator);

    // Some extra buttons to show off
    AddButton("CL").CenterLeftInPanel();
    var cr = AddButton("CR").CenterRightInPanel();
    AddButton("BC").BottomCenterInPanel();

    var cc = AddButton("CC").CenterInPanel();
    AddButton("CC T").Above(cc);
    AddButton("CC L ==").LeftOf(cc).StretchDownInPanel();
    AddButton("CC R ==").RightOf(cc).StretchRightTo(cr);
    AddButton("CC B").Below(cc);
  }

  private Action<RoutedEventArgs> AddText(string text) => _ => _tb.Text += $"{text}\n";

  private void AddSelectedText(SelectedItemChangedEventArgs<string> e) => _tb.Text += $"Selected: {e.SelectedItem}\n";

  private Action<RoutedEventArgs> AddTextIfChecked(string text) {
    return e => {
      var sender = e.Source as ToggleButton;
      if (sender?.IsChecked == true) {
        _tb.Text += text + '\n';
      }
    };
  }

  private Action<RoutedEventArgs> ClearText() => _ => _tb.Text = "";
}
