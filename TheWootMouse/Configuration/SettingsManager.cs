using System.Text.Json;

namespace TheWootMouse.Configuration;

public static class SettingsManager
{
    private const string FullSettingsFilePath = "settings.json";

    public static WootMouseEngineSettings? Load()
    {
        return File.Exists(FullSettingsFilePath) ? JsonSerializer.Deserialize<WootMouseEngineSettings>(File.ReadAllText(FullSettingsFilePath)) : new WootMouseEngineSettings();
    }

    public static void Save(WootMouseEngineSettings settings)
    {
        File.WriteAllText(FullSettingsFilePath, JsonSerializer.Serialize(settings));
    }
}