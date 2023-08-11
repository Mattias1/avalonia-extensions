using Avalonia;
using Avalonia.Media;
using Avalonia.Styling;
using System.Globalization;

namespace AvaloniaExtensions;

// Not the prettiest of solutions, but avalonia themes aren't made for XAML-less in-place editing,
// and creating my own custom theme feels a bit overkill
public class CustomStyle {
  public Thickness Margin { get; set; }
  public int MinWidth { get; set; }
  public ThemedBrushes? Background { get; set; }

  public CustomStyle(Thickness margin, int minWidth, ThemedBrushes? background) {
    Margin = margin;
    MinWidth = minWidth;
    Background = background;
  }

  public static CustomStyle Default => new CustomStyle(new Thickness(0), 0, null);
}

public record ThemedBrushes(IBrush LightThemeVariant, IBrush DarkThemeVariant) {
  public IBrush ForTheme(ThemeVariant theme) => theme == ThemeVariant.Light ? LightThemeVariant : DarkThemeVariant;

  public static ThemedBrushes FromHex(string lightThemeVariant, string darkThemeVariant) {
    return new ThemedBrushes(ParseHexBrush(lightThemeVariant), ParseHexBrush(darkThemeVariant));
  }

  private static SolidColorBrush ParseHexBrush(string argbHex) {
    if (argbHex.StartsWith("#")) {
      argbHex = argbHex.Substring(1);
    }
    if (argbHex.StartsWith("0x")) {
      argbHex = argbHex.Substring(2);
    }
    if (argbHex.Length == 6) {
      argbHex = "FF" + argbHex;
    }
    var number = uint.Parse(argbHex, NumberStyles.HexNumber);
    return new SolidColorBrush(number);
  }
}
