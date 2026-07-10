using System.Text.Json;

namespace TheWootMouse.Configuration;

public static class SettingsManager
{
    private const string FullSettingsFilePath = "settings.json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    public static WootMouseEngineSettings Load()
    {
        if (!File.Exists(FullSettingsFilePath))
            return new WootMouseEngineSettings();

        return JsonSerializer.Deserialize<WootMouseEngineSettings>(File.ReadAllText(FullSettingsFilePath), Options)
               ?? new WootMouseEngineSettings();
    }

    public static void Save(WootMouseEngineSettings settings)
    {
        File.WriteAllText(FullSettingsFilePath, JsonSerializer.Serialize(settings, Options));
    }
}