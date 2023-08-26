using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;
using System.Reflection;

namespace AvaloniaExtensions;

public static class AssetExtensions {
  public static string? StartupPath => Path.GetDirectoryName(GetAssembly().Location);

  public static WindowIcon LoadWindowIcon(string relativePath) => new WindowIcon(LoadResource(relativePath));
  public static Bitmap LoadBitmap(string relativePath) => new Bitmap(LoadResource(relativePath));
  public static Stream LoadResource(string relativePath) {
    var uri = new Uri($"avares://{GetAssembly().GetName().Name}/{relativePath}");
    return AssetLoader.Open(uri);
  }

  private static Assembly GetAssembly() => Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
}
