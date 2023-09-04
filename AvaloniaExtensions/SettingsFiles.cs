using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace AvaloniaExtensions;

public sealed class SettingsFiles {
  public static SettingsFiles Get { get; } = new SettingsFiles();

  private readonly Dictionary<Type, SettingsFile> _settingsFiles = new();

  /// <summary>
  /// Add a settings file. It'll save the settings file when closing the app.
  /// </summary>
  /// <param name="path">The location of the settings file. You can use './filename.json' to save it in the apps dir.</param>
  /// <typeparam name="T">The type of the settings class.</typeparam>
  /// <returns></returns>
  public void AddSettingsFile<T>(string path) where T : class, new() => AddSettingsFile(path, () => new T());

  /// <summary>
  /// Add a settings file. It'll save the settings file when closing the app.
  /// </summary>
  /// <param name="path">The location of the settings file. You can use './filename.json' to save it in the apps dir.</param>
  /// <param name="constructorLambda">A lambda function to construct the settings object if it can't be loaded.</param>
  /// <typeparam name="T">The type of the settings class.</typeparam>
  /// <returns></returns>
  public void AddSettingsFile<T>(string path, Func<T> constructorLambda) where T : class {
    var settings = LoadOrCreateSettings(path, constructorLambda);
    _settingsFiles.Add(typeof(T), new SettingsFile(path, settings));
  }

  private T LoadOrCreateSettings<T>(string path, Func<T> constructorLambda) where T : class {
    try {
      using var stream = File.OpenRead(CompletePath(path));
      var result = JsonSerializer.Deserialize<T>(stream);
      return result ?? constructorLambda();
    } catch (Exception e) {
      Console.Error.WriteLine("An avalonia extensions app encountered an error while loading a settings file." +
          $"Settings type: '{typeof(T)}', error: '{e.Message}'.");
      return constructorLambda();
    }
  }

  public bool SaveSettings() {
    bool allSavedSuccessfully = true;
    foreach (var (type, settingsFile) in _settingsFiles) {
      try {
        using var stream = File.Create(CompletePath(settingsFile.Path));
        JsonSerializer.Serialize(stream, settingsFile.Settings);
      } catch (Exception e) {
        allSavedSuccessfully = false;
        Console.Error.WriteLine("An avalonia extensions app encountered an error while saving a settings file." +
            $"Settings type: '{type}', error: '{e.Message}'.");
      }
    }
    return allSavedSuccessfully;
  }

  private string CompletePath(string path) {
    if (string.IsNullOrWhiteSpace(path)) {
      path = "./settings.json";
    }
    if (path.Substring(0, 2) == "./") {
      path = AssetExtensions.StartupPath + path.Substring(1);
    }
    return path;
  }

  public void ResetSettings<T>() where T : class, new() => OverwriteSettings(new T());
  public void ResetSettings<T>(Func<T> constructorLambda) where T : class => OverwriteSettings(constructorLambda());

  public void OverwriteSettings<T>(T settings) where T : class {
    if (!_settingsFiles.TryGetValue(typeof(T), out SettingsFile? originalSettingsFile)) {
      throw new InvalidOperationException($"Cannot find settings with type {typeof(T)}. "
          + "You can add it with the 'WithSettingsFile' or 'AddSettingsFile' method.");
    }
    _settingsFiles[typeof(T)] = originalSettingsFile with { Settings = settings };
  }

  public T GetSettings<T>() where T : class {
    if (!_settingsFiles.TryGetValue(typeof(T), out SettingsFile? settingsFile)) {
      throw new InvalidOperationException($"Cannot find settings with type {typeof(T)}. "
          + "You can add it with the 'WithSettingsFile' or 'AddSettingsFile' method.");
    }
    if (settingsFile.Settings is not T settings) {
      throw new InvalidOperationException($"Cannot cast settings with type {typeof(T)} to its type, weird.");
    }
    return settings;
  }

  private record SettingsFile(string Path, object Settings);
}
