namespace TheWootMouse.Configuration;

public class WootMouseEngineSettings
{
    // Toggles
    public bool MouseLayerKeyEnabled { get; init;}
    // Key Assignments
    public int KeyUp{ get; init;}
    public int KeyRight{ get; init;}
    public int KeyDown{ get; init;}
    public int KeyLeft{ get; init;}
    public int KeyScrollUp{ get; init;}
    public int KeyScrollDown{ get; init;}
    public int KeyTurbo{ get; init;}
    public int MouseLayerKey { get; init;}
    // Mouse Control Settings
    public float DeadZone{ get; init;}
    public int MaxMouseSpeed{ get; init;}
    public int MaxMouseScrollSpeed{ get; init;}
    public int MouseTurboSpeedMultiplier{ get; init;}
    public int MouseCurvePower{ get; init;}

    // Pixel-density profiles.
    // MaxMouseSpeed is interpreted as px/s at ReferencePpi; the active profile scales it by
    // (profile.Ppi / ReferencePpi) so perceived cursor speed stays constant across screens.
    public double ReferencePpi { get; init; } = 96.0;
    public bool AutoSwitchProfileByMonitor { get; init; }
    public string? ActiveProfileName { get; init; }
    public List<DensityProfile> DensityProfiles { get; init; } = new();
}