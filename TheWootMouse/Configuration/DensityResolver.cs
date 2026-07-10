using TheWootMouse.Infrastructure.Windows;

namespace TheWootMouse.Configuration;

/// <summary>
/// Resolves the speed scale factor (effective px/s ÷ configured px/s) for the density profile that
/// currently applies. When auto-switching is on, it picks the profile bound to the monitor under the
/// cursor; otherwise it uses the named active profile. Monitor lookups are cached so the hot loop
/// only re-scans profiles when the cursor crosses to a different screen.
/// </summary>
public sealed class DensityResolver(WootMouseEngineSettings settings)
{
    private string? _lastDeviceName;
    private double _cachedScale = 1.0;
    private bool _hasCached;

    /// <summary>Multiplier to apply to configured pixel speeds for the currently active profile.</summary>
    public double CurrentScale()
    {
        double referencePpi = settings.ReferencePpi > 0 ? settings.ReferencePpi : 96.0;

        if (settings.AutoSwitchProfileByMonitor)
        {
            var monitor = DisplayInfo.GetMonitorUnderCursor();
            string? device = monitor?.DeviceName;

            if (_hasCached && device == _lastDeviceName)
                return _cachedScale;

            var profile = FindProfileForMonitor(device);
            _cachedScale = ScaleFor(profile, referencePpi);
            _lastDeviceName = device;
            _hasCached = true;
            return _cachedScale;
        }

        return ScaleFor(ActiveProfile(), referencePpi);
    }

    private DensityProfile? ActiveProfile()
    {
        if (string.IsNullOrEmpty(settings.ActiveProfileName))
            return null;
        return settings.DensityProfiles.FirstOrDefault(
            p => string.Equals(p.Name, settings.ActiveProfileName, StringComparison.OrdinalIgnoreCase));
    }

    private DensityProfile? FindProfileForMonitor(string? deviceName)
    {
        if (!string.IsNullOrEmpty(deviceName))
        {
            var match = settings.DensityProfiles.FirstOrDefault(
                p => string.Equals(p.MonitorDeviceName, deviceName, StringComparison.OrdinalIgnoreCase));
            if (match != null)
                return match;
        }

        // No monitor-bound profile — fall back to the named active profile, if any.
        return ActiveProfile();
    }

    private static double ScaleFor(DensityProfile? profile, double referencePpi)
    {
        if (profile is null || profile.Ppi <= 0)
            return 1.0;
        return profile.Ppi / referencePpi;
    }
}
