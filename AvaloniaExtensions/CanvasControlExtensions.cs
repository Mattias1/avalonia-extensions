using Avalonia.Controls;
using Avalonia.Markup.Declarative;

namespace AvaloniaExtensions;

public static class CanvasControlExtensions {
  // --- Reference to static variable ---
  private const string REF_ERROR = "You can only use this method without parameters if you've previously referenced an "
      + "object. Did you call `.Ref()` on the previous element?";
  public static Control? LastReferenced = null;
  public static Control? CurrentReferenced = null;
  public static Control LastReferencedOrThrow => LastReferenced ?? throw new InvalidOperationException(REF_ERROR);
  public static Control CurrentReferencedOrThrow => CurrentReferenced ?? throw new InvalidOperationException(REF_ERROR);

  public static T Ref<T>(this T control) where T : Control {
    LastReferenced = CurrentReferenced;
    CurrentReferenced = control;
    return control;
  }

  // --- Basic position and size ---
  public static int Margin = 10;

  public static double GetX(this Control control) => Canvas.GetLeft(control);
  public static double GetY(this Control control) => Canvas.GetTop(control);

  // --- Position relative to other controls ---
  public static T RightOf<T>(this T control) where T : Control => control.RightOf(LastReferencedOrThrow);
  public static T LeftOf<T>(this T control) where T : Control => control.LeftOf(LastReferencedOrThrow);
  public static T Below<T>(this T control) where T : Control => control.Below(LastReferencedOrThrow);
  public static T Above<T>(this T control) where T : Control => control.Above(LastReferencedOrThrow);

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
  public static T XLeftOf<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() - control.Bounds.Width - Margin));
  }
  public static T XAlignLeft<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Left(other.GetX()));
  }
  public static T XCenter<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() + (other.Bounds.Width - control.Bounds.Width) * .5));
  }
  public static T XAlignRight<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() + other.Bounds.Width - control.Bounds.Width));
  }
  public static T XRightOf<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Left(other.GetX() + other.Bounds.Width + Margin));
  }

  public static T XLeftInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Left(Margin));
  }
  public static T XCenterInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Left((canvas.Bounds.Width - control.Bounds.Width) * .5);
    });
  }
  public static T XRightInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Left(canvas.Bounds.Width - control.Bounds.Width - Margin);
    });
  }

  public static T YAbove<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      control.Top(other.GetY() - control.Bounds.Height - Margin);
    });
  }
  public static T YAlignTop<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Top(other.GetY()));
  }
  public static T YCenter<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Top(other.GetY() + (other.Bounds.Height - control.Bounds.Height) * .5));
  }
  public static T YAlignBottom<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Top(other.GetY() + other.Bounds.Height - control.Bounds.Height));
  }
  public static T YBelow<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Top(other.GetY() + other.Bounds.Height + Margin));
  }

  public static T YTopInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => control.Top(Margin));
  }
  public static T YCenterInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Top((canvas.Bounds.Height - control.Bounds.Height) * .5);
    });
  }
  public static T YBottomInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Top(canvas.Bounds.Height - control.Bounds.Height - Margin);
    });
  }

  // --- Size and stretching ---
  public static T StretchRightTo<T>(this T control) where T : Control => control.StretchRightTo(LastReferencedOrThrow);
  public static T StretchRightTo<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Width(other.GetX() - control.GetX() - Margin));
  }

  public static T StretchDownTo<T>(this T control) where T : Control => control.StretchDownTo(LastReferencedOrThrow);
  public static T StretchDownTo<T>(this T control, Control other) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control,
        () => control.Height(other.GetY() - control.GetY() - Margin));
  }

  public static T StretchRightInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Width(canvas.Bounds.Width - control.GetX() - Margin);
    });
  }
  public static T StretchDownInPanel<T>(this T control) where T : Control {
    return CanvasComponentBase.RegisterOnResizeAction(control, () => {
      var canvas = CanvasComponentBase.FindCanvas(control);
      control.Height(canvas.Bounds.Height - control.GetY() - Margin);
    });
  }
}
