namespace TheWootMouse.Configuration;

/// <summary>
/// A pixel-density profile for one screen. Cursor speed is configured in physical terms and
/// converted to pixels/second using <see cref="Ppi"/>, so perceived speed stays constant across
/// screens of differing density. Windows' per-monitor DPI reports only the scaling setting, not the
/// true physical pixel density, so <see cref="Ppi"/> is stored explicitly (entered directly, or
/// computed from resolution + diagonal via <see cref="FromResolution"/>).
/// </summary>
public sealed class DensityProfile
{
    /// <summary>Friendly name shown in the UI, e.g. "4K 27\"".</summary>
    public string Name { get; set; } = "";

    /// <summary>True physical pixels-per-inch of the screen.</summary>
    public double Ppi { get; set; } = 96.0;

    /// <summary>
    /// Windows device name of the monitor this profile applies to (e.g. "\\.\DISPLAY1"). When set,
    /// the engine can auto-select this profile whenever the cursor is on that monitor. Null means the
    /// profile is not bound to a specific monitor.
    /// </summary>
    public string? MonitorDeviceName { get; set; }

    /// <summary>Computes PPI from a pixel resolution and physical diagonal in inches.</summary>
    public static double FromResolution(int widthPx, int heightPx, double diagonalInches)
    {
        if (diagonalInches <= 0) return 96.0;
        double diagonalPx = Math.Sqrt((double)widthPx * widthPx + (double)heightPx * heightPx);
        return diagonalPx / diagonalInches;
    }
}
