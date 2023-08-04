using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Declarative;
using AvaloniaExtensions;

namespace ExampleApp;

public class MainComponent : ComponentBase {
  private TextBox _tb = null!;

  protected override object Build() {
    return new RelativePanel() // Maybe CanvasPanel anyway :( StretchTo isn't going to work without manual math it seems
        .Children(
            new Button().Ref(out var btnHelloWorld).Content("Hello world").OnClick(SetText("Hi")).TopLeftInPanel(),
            new Button().Ref(out var btnCancel).Content("Cancel").OnClick(SetText("Aww")).BottomRightInPanel(),
            new Button().Content("Ok").OnClick(SetText("kk")).LeftOf(btnCancel),
            new TextBox().Ref(out _tb).AcceptsReturn(true).AcceptsTab(true).Below(btnHelloWorld)
                .StretchRightInPanel().SetHeight(65) // .StretchDownTo(btnCancel) // This covers the button
        );
  }

  private Action<RoutedEventArgs> SetText(string text) {
    return e => _tb.Text = text;
  }
}
