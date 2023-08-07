using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Declarative;

namespace AvaloniaExtensions;

public class ExtendedComboBox<T> : ComboBox where T : class {
  private readonly Dictionary<ComboBoxItem, T> _originalItems = new();

  protected override Type StyleKeyOverride => typeof(ComboBox);

  public ExtendedComboBox<T> WithItems(IEnumerable<T> items, Func<T, string> contentFunc) {
    foreach (var item in items) {
      var comboBoxItem = new ComboBoxItem().Content(contentFunc(item));
      _originalItems.Add(comboBoxItem, item);
      Items.Add(comboBoxItem);
    }
    return this;
  }

  public ExtendedComboBox<T> OnSelectedItemChanged(Action<SelectedItemChangedEventArgs<T>> action) {
    this.OnSelectionChanged(e => {
      var to = (e.AddedItems.Count == 1 ? e.AddedItems[0] as ComboBoxItem : null)
          ?? throw new InvalidOperationException("Can't find the selected item for this ComboBox");
      var from = e.RemovedItems.Count == 1 ? e.RemovedItems[0] as ComboBoxItem : null;
      var deselectedItem = from is null ? null : _originalItems[from];
      action(new SelectedItemChangedEventArgs<T>(_originalItems[to], deselectedItem, e.RoutedEvent, this));
    });
    return this;
  }
}

public class SelectedItemChangedEventArgs<T> : RoutedEventArgs where T : class {
  public T SelectedItem { get; }
  public T? DeselectedItem { get; }
  public ExtendedComboBox<T> SourceTyped { get; }

  public SelectedItemChangedEventArgs(T selectedItem, T? deselectedItem, RoutedEvent? routedEvent,
      ExtendedComboBox<T> source) : base(routedEvent, source) {
    SelectedItem = selectedItem;
    DeselectedItem = deselectedItem;
    SourceTyped = source;
  }
}
