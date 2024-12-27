using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Declarative;
using System;

namespace AvaloniaExtensions;

// This class helps with positioning and sizing of controls
// An annoying detail: The margin is included in the coordinates (x, y / left, top), but not in the size (width, height)
public static class CanvasControlExtensions {
  // --- Reference to static variable ---
  private const string REF_ERROR = "You can only use this method without parameters if you've previously referenced an "
      + "object. Did you call `.Ref()` on the previous element?";
  public static Control? LastReferenced = null;
  public static Control? CurrentReferenced = null;
  public static Control LastReferencedOrThrow => LastReferenced ?? throw new InvalidOperationException(REF_ERROR);
  public static Control CurrentReferencedOrThrow => CurrentReferenced ?? throw new InvalidOperationException(REF_ERROR);

  public static Control PreviousControlFor(Control control) {
    return control == CurrentReferenced ? LastReferencedOrThrow : CurrentReferencedOrThrow;
  }

  public static T Ref<T>(this T control) where T : Control {
    LastReferenced = CurrentReferenced;
    CurrentReferenced = control;
    return control;
  }

  // --- Misc ---
  public static T WithInitialFocus<T>(this T control) where T : Control {
    return CanvasComponentBase.FindCanvasComponent(control).SetInitialControlToFocus(control);
  }

  public static T HotKeyCtrl<T>(this T control, Key key) where T : Button => control.HotKey(key, KeyModifiers.Control);
  public static T HotKeyAlt<T>(this T control, Key key) where T : Button => control.HotKey(key, KeyModifiers.Alt);
  public static T HotKey<T>(this T control, Key key, KeyModifiers modifiers) where T : Button {
    control.HotKey(new KeyGesture(key, modifiers));
    return control;
  }

  public static T HotKey<T>(this T control, string hotkey) where T : Button {
    control.HotKey(KeyGesture.Parse(hotkey));
    return control;
  }

  // --- Basic position and size ---
  public static double GetX(this Control control) => Canvas.GetLeft(control);
  public static double GetY(this Control control) => Canvas.GetTop(control);

  public static double GetWidth(this Control ctrl) => double.IsNaN(ctrl.Width) ? ctrl.Bounds.Width : ctrl.Width;
  public static double GetHeight(this Control ctrl) => double.IsNaN(ctrl.Height) ? ctrl.Bounds.Height : ctrl.Height;
  public static T SetWidth<T>(this T ctrl, double width) where T : Control => ctrl.Width(Math.Max(width, 2));
  public static T SetHeight<T>(this T ctrl, double height) where T : Control => ctrl.Height(Math.Max(height, 2));

  // --- Position relative to other controls ---
  public static T RightOf<T>(this T control) where T : Control => control.RightOf(PreviousControlFor(control));
  public static T LeftOf<T>(this T control) where T : Control => control.LeftOf(PreviousControlFor(control));
  public static T Below<T>(this T control) where T : Control => control.Below(PreviousControlFor(control));
  public static T Above<T>(this T control) where T : Control => control.Above(PreviousControlFor(control));

  public static T RightOf<T>(this T control, Control other) where T : Control {
    return control.YAlignTop(other).XRightOf(other);
  }
  public static T LeftOf<T>(this T control, Control other) where T : Control {
    return control.YAlignTop(other).XLeftOf(other);
  }
  public static T Below<T>(this T control, Control other) where T : Control {
    return control.YBelow(other).XAlignLeft(other);
  }
  public static T Above<T>(this T control, Control other) where T : Control {
    return control.YAbove(other).XAlignLeft(other);
  }

  // --- Position inside a relative panel ---
  public static T TopLeftInPanel<T>(this T control) where T : Control {
    return control.YTopInPanel().XLeftInPanel();
  }
  public static T TopCenterInPanel<T>(this T control) where T : Control {
    return control.YTopInPanel().XCenterInPanel();
  }
  public static T TopRightInPanel<T>(this T control) where T : Control {
    return control.YTopInPanel().XRightInPanel();
  }

  public static T CenterRightInPanel<T>(this T control) where T : Control {
    return control.YCenterInPanel().XRightInPanel();
  }

  public static T BottomRightInPanel<T>(this T control) where T : Control {
    return control.YBottomInPanel().XRightInPanel();
  }
  public static T BottomCenterInPanel<T>(this T control) where T : Control {
    return control.YBottomInPanel().XCenterInPanel();
  }
  public static T BottomLeftInPanel<T>(this T control) where T : Control {
    return control.YBottomInPanel().XLeftInPanel();
  }

  public static T CenterLeftInPanel<T>(this T control) where T : Control {
    return control.YCenterInPanel().XLeftInPanel();
  }

  public static T CenterInPanel<T>(this T control) where T : Control {
    return control.YCenterInPanel().XCenterInPanel();
  }

  // --- Positioning of individual coordinates ---
  public static T XLeftOf<T>(this T control) where T : Control => control.XLeftOf(PreviousControlFor(control));
  public static T XLeftOf<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() - control.GetWidth() - control.Margin.Right));
  }

  public static T XAlignLeft<T>(this T control) where T : Control => control.XAlignLeft(PreviousControlFor(control));
  public static T XAlignLeft<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Left(other.GetX()));
  }

  public static T XCenter<T>(this T control) where T : Control => control.XCenter(PreviousControlFor(control));
  public static T XCenter<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() + (other.GetWidth() - control.GetWidth()) * .5));
  }

  public static T XAlignRight<T>(this T control) where T : Control => control.XAlignRight(PreviousControlFor(control));
  public static T XAlignRight<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() + other.GetWidth() - control.GetWidth()));
  }

  public static T XRightOf<T>(this T control) where T : Control => control.XRightOf(PreviousControlFor(control));
  public static T XRightOf<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() + other.GetWidth() + other.Margin.Left));
  }

  public static T XLeftInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Left(0));
  }
  public static T XCenterInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Left((canvas.GetWidth() - control.GetWidth() - control.Margin.Left - control.Margin.Right) * .5);
    });
  }
  public static T XRightInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Left(canvas.GetWidth() - control.GetWidth() - control.Margin.Left - control.Margin.Right);
    });
  }

  public static T YAbove<T>(this T control) where T : Control => control.YAbove(PreviousControlFor(control));
  public static T YAbove<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      control.Top(other.GetY() - control.GetHeight() - control.Margin.Bottom);
    });
  }

  public static T YAlignTop<T>(this T control) where T : Control => control.YAlignTop(PreviousControlFor(control));
  public static T YAlignTop<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Top(other.GetY()));
  }

  public static T YCenter<T>(this T control) where T : Control => control.YCenter(PreviousControlFor(control));
  public static T YCenter<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Top(other.GetY() + (other.GetHeight() - control.GetHeight()) * .5));
  }

  public static T YAlignBottom<T>(this T control) where T : Control => control.YAlignBottom(PreviousControlFor(control));
  public static T YAlignBottom<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Top(other.GetY() + other.GetHeight() - control.GetHeight()));
  }

  public static T YBelow<T>(this T control) where T : Control => control.YBelow(PreviousControlFor(control));
  public static T YBelow<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Top(other.GetY() + other.GetHeight() + other.Margin.Top));
  }

  public static T YTopInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Top(0));
  }
  public static T YCenterInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Top((canvas.GetHeight() - control.GetHeight() - control.Margin.Top - control.Margin.Bottom) * .5);
    });
  }
  public static T YBottomInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Top(canvas.GetHeight() - control.GetHeight() - control.Margin.Top - control.Margin.Bottom);
    });
  }

  // --- Size and stretching ---
  public static T StretchRightTo<T>(this T control) where T : Control => control.StretchRightTo(PreviousControlFor(control));
  public static T StretchRightTo<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.SetWidth(other.GetX() - control.GetX() - Math.Max(control.Margin.Right, other.Margin.Left)));
  }

  public static T StretchDownTo<T>(this T control) where T : Control => control.StretchDownTo(PreviousControlFor(control));
  public static T StretchDownTo<T>(this T control, Control other) where T : Control {
    ThrowIfPositioningRelativeToYourself(control, other);
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.SetHeight(other.GetY() - control.GetY() - Math.Max(control.Margin.Bottom, other.Margin.Top)));
  }

  public static T StretchRightInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.SetWidth(canvas.GetWidth() - control.GetX() - control.Margin.Left - control.Margin.Right);
    });
  }
  public static T StretchDownInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.SetHeight(canvas.GetHeight() - control.GetY() - control.Margin.Top - control.Margin.Bottom);
    });
  }

  public static T StretchFractionRightInPanel<T>(this T control, int numerator, int denominator) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      var fullWidth = canvas.GetWidth() - control.GetX() - control.Margin.Left - control.Margin.Right;
      control.SetWidth(fullWidth * numerator / denominator - control.Margin.Right * .5f);
    });
  }
  public static T StretchFractionDownInPanel<T>(this T control, int numerator, int denominator) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      var fullHeight = canvas.GetHeight() - control.GetY() - control.Margin.Top - control.Margin.Bottom;
      control.SetHeight(fullHeight * numerator / denominator - control.Margin.Bottom * .5f);
    });
  }

  private static void ThrowIfPositioningRelativeToYourself(Control control, Control other) {
    if (control == other) {
      throw new InvalidOperationException("You cannot position (or stretch) an object relative to itself.");
    }
  }
}
