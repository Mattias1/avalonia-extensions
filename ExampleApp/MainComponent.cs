using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Declarative;
using AvaloniaExtensions;

namespace ExampleApp;

public class MainComponent : CanvasComponentBase {
  private TextBox _tb = null!;

  protected override void InitializeControls() {
    // Some realistic-ish example controls
    AddButton("Hello world", AddText("Whazzup folks, watcha doing?")).TopLeftInPanel();
    AddMultilineTextBox().Ref(out _tb).Below();
    AddButton("Cancel", ClearText()).Ref(out var btnCancel).BottomRightInPanel();
    AddButton("Ok", AddText("kk")).LeftOf();

    _tb.StretchRightInPanel().StretchDownTo(btnCancel);

    // Some extra buttons to show off
    AddButton("CC").Ref(out var cc).CenterInPanel();
    AddButton("CC T").Above(cc);
    AddButton("CC L ==").Ref(out var ccl).LeftOf(cc).StretchDownInPanel();
    AddButton("CC R").RightOf(cc);
    AddButton("CC B").Below(cc);

    AddButton("TC").TopCenterInPanel();
    AddButton("TR").TopRightInPanel();
    AddButton("CL").CenterLeftInPanel();
    AddButton("CR").CenterRightInPanel();
    AddButton("BL ==").BottomLeftInPanel().StretchRightTo(ccl);
    AddButton("BC").BottomCenterInPanel();
  }

  private Action<RoutedEventArgs> AddText(string text) => _ => _tb.Text += text + '\n';

  private Action<RoutedEventArgs> ClearText() => _ => _tb.Text = "";
}
