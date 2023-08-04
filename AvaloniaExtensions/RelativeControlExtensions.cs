using Avalonia.Controls;

namespace AvaloniaExtensions;

public static class RelativeControlExtensions {
  // --- Reference to static variable ---
  public static Control? LastReferenced = null;
  private static Control LastReferencedOrThrow => LastReferenced ?? throw new InvalidOperationException("You can only "
      + "use this method without parameters if you've previously referenced an object. "
      + "Did you call `.Ref()` on the previous element?");

  public static T Ref<T>(this T control) where T : Control {
    LastReferenced = control;
    return control;
  }

  // --- Position relative to other controls ---
  public static T RightOf<T>(this T control) where T : Control => control.RightOf(LastReferencedOrThrow);
  public static T LeftOf<T>(this T control) where T : Control => control.LeftOf(LastReferencedOrThrow);
  public static T Below<T>(this T control) where T : Control => control.Below(LastReferencedOrThrow);
  public static T Above<T>(this T control) where T : Control => control.Above(LastReferencedOrThrow);

  public static T RightOf<T>(this T control, Control other) where T : Control {
    RelativePanel.SetAlignTopWith(control, other);
    RelativePanel.SetRightOf(control, other);
    return control;
  }
  public static T LeftOf<T>(this T control, Control other) where T : Control {
    RelativePanel.SetAlignTopWith(control, other);
    RelativePanel.SetLeftOf(control, other);
    return control;
  }
  public static T Below<T>(this T control, Control other) where T : Control {
    RelativePanel.SetBelow(control, other);
    RelativePanel.SetAlignLeftWith(control, other);
    return control;
  }
  public static T Above<T>(this T control, Control other) where T : Control {
    RelativePanel.SetAbove(control, other);
    RelativePanel.SetAlignLeftWith(control, other);
    return control;
  }

  // --- Position inside a relative panel ---
  public static T TopLeftInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignTopWithPanel(control, true);
    RelativePanel.SetAlignLeftWithPanel(control, true);
    return control;
  }
  public static T TopCenterInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignTopWithPanel(control, true);
    RelativePanel.SetAlignHorizontalCenterWithPanel(control, true);
    return control;
  }
  public static T TopRightInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignTopWithPanel(control, true);
    RelativePanel.SetAlignRightWithPanel(control, true);
    return control;
  }

  public static T CenterRightInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignVerticalCenterWithPanel(control, true);
    RelativePanel.SetAlignRightWithPanel(control, true);
    return control;
  }

  public static T BottomRightInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignBottomWithPanel(control, true);
    RelativePanel.SetAlignRightWithPanel(control, true);
    return control;
  }
  public static T BottomCenterInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignBottomWithPanel(control, true);
    RelativePanel.SetAlignHorizontalCenterWithPanel(control, true);
    return control;
  }
  public static T BottomLeftInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignBottomWithPanel(control, true);
    RelativePanel.SetAlignLeftWithPanel(control, true);
    return control;
  }

  public static T CenterLeftInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignVerticalCenterWithPanel(control, true);
    RelativePanel.SetAlignLeftWithPanel(control, true);
    return control;
  }

  public static T CenterInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignVerticalCenterWithPanel(control, true);
    RelativePanel.SetAlignHorizontalCenterWithPanel(control, true);
    return control;
  }

  // --- Size and stretching ---
  public static T Size<T>(this T control, int width, int height) where T : Control {
    return control.SetWidth(width).SetHeight(height);
  }
  public static T SetWidth<T>(this T control, int width) where T : Control {
    control.Width = width;
    return control;
  }
  public static T SetHeight<T>(this T control, int height) where T : Control {
    control.Height = height;
    return control;
  }

  public static T StretchRightTo<T>(this T control, Control other) where T : Control {
    RelativePanel.SetAlignRightWith(control, other);
    return control;
  }
  public static T StretchDownTo<T>(this T control, Control other) where T : Control {
    RelativePanel.SetAlignBottomWith(control, other);
    return control;
  }

  public static T StretchRightInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignRightWithPanel(control, true);
    return control;
  }
  public static T StretchDownInPanel<T>(this T control) where T : Control {
    RelativePanel.SetAlignBottomWithPanel(control, true);
    return control;
  }
}
