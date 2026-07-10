using System.Runtime.InteropServices;

namespace TheWootMouse.Infrastructure.Windows;

/// <summary>
/// A physical display and the pixel region it occupies on the virtual desktop.
/// </summary>
public sealed record MonitorInfo(
    string DeviceName,
    int Left,
    int Top,
    int Width,
    int Height,
    uint EffectiveDpi)
{
    public bool IsPrimary => Left == 0 && Top == 0;

    /// <summary>Windows scale factor for this monitor (e.g. 1.0 at 96 DPI, 1.5 at 144 DPI).</summary>
    public double ScaleFactor => EffectiveDpi / 96.0;
}

/// <summary>
/// Thin wrapper over the Win32 monitor / DPI APIs. Used to figure out which physical screen the
/// cursor is currently on so the engine can apply that screen's pixel-density profile.
/// </summary>
public static partial class DisplayInfo
{
    private const uint MonitorDefaultToNearest = 0x00000002;
    private const uint MdtEffectiveDpi = 0;

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private struct MonitorInfoEx
    {
        public int cbSize;
        public Rect rcMonitor;
        public Rect rcWork;
        public uint dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string szDevice;
    }

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(out Point lpPoint);

    [LibraryImport("user32.dll")]
    private static partial IntPtr MonitorFromPoint(Point pt, uint dwFlags);

    // MonitorInfoEx contains an inline fixed-length string, which the LibraryImport source generator
    // does not support, so this one uses classic DllImport marshalling.
    [DllImport("user32.dll", EntryPoint = "GetMonitorInfoW", CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfoEx lpmi);

    [LibraryImport("Shcore.dll")]
    private static partial int GetDpiForMonitor(IntPtr hMonitor, uint dpiType, out uint dpiX, out uint dpiY);

    private delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdc, IntPtr lprcMonitor, IntPtr dwData);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

    /// <summary>
    /// Returns the monitor the cursor is currently over, or null if it can't be determined.
    /// </summary>
    public static MonitorInfo? GetMonitorUnderCursor()
    {
        if (!GetCursorPos(out var pt))
            return null;

        IntPtr hMonitor = MonitorFromPoint(pt, MonitorDefaultToNearest);
        return hMonitor == IntPtr.Zero ? null : BuildMonitorInfo(hMonitor);
    }

    /// <summary>Enumerates every connected monitor (used by the UI to bind profiles to screens).</summary>
    public static IReadOnlyList<MonitorInfo> GetAllMonitors()
    {
        var result = new List<MonitorInfo>();
        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (hMonitor, _, _, _) =>
        {
            var info = BuildMonitorInfo(hMonitor);
            if (info != null) result.Add(info);
            return true;
        }, IntPtr.Zero);
        return result;
    }

    private static MonitorInfo? BuildMonitorInfo(IntPtr hMonitor)
    {
        var mi = new MonitorInfoEx { cbSize = Marshal.SizeOf<MonitorInfoEx>() };
        if (!GetMonitorInfo(hMonitor, ref mi))
            return null;

        uint dpi = 96;
        if (GetDpiForMonitor(hMonitor, MdtEffectiveDpi, out uint dpiX, out _) == 0)
            dpi = dpiX;

        return new MonitorInfo(
            DeviceName: mi.szDevice,
            Left: mi.rcMonitor.Left,
            Top: mi.rcMonitor.Top,
            Width: mi.rcMonitor.Right - mi.rcMonitor.Left,
            Height: mi.rcMonitor.Bottom - mi.rcMonitor.Top,
            EffectiveDpi: dpi);
    }
}
