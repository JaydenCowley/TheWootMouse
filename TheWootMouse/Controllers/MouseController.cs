using System.Runtime.InteropServices;

namespace TheWootMouse.Controllers;

public static partial class MouseController
{
    // Scroll Logic
    [StructLayout(LayoutKind.Sequential)]
    private struct MouseScrollData
    {
        public uint type;
        public MouseInput mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    // SendInput type + mouse-event flags (see MOUSEINPUT / INPUT in the Win32 API).
    private const uint InputMouse = 0;
    private const uint MouseWheelEvent = 0x0800; // MOUSEEVENTF_WHEEL
    private const uint MouseEventMove = 0x0001;      // MOUSEEVENTF_MOVE
    private const uint MouseEventAbsolute = 0x8000;  // MOUSEEVENTF_ABSOLUTE
    private const uint MouseEventVirtualDesk = 0x4000; // MOUSEEVENTF_VIRTUALDESK

    // GetSystemMetrics indices for the bounding virtual desktop across all monitors.
    private const int SmXVirtualScreen = 76;
    private const int SmYVirtualScreen = 77;
    private const int SmCxVirtualScreen = 78;
    private const int SmCyVirtualScreen = 79;

    [LibraryImport("user32.dll")]
    private static partial uint SendInput(uint nInputs, MouseScrollData[] pInputs, int cbSize);

    [LibraryImport("user32.dll")]
    private static partial int GetSystemMetrics(int nIndex);

    public static void Scroll(int delta)
    {
        var input = new MouseScrollData
        {
            type = InputMouse,
            mi = new MouseInput
            {
                mouseData = (uint)delta,
                dwFlags = MouseWheelEvent
            }
        };

        SendInput(1, [input], Marshal.SizeOf<MouseScrollData>());
    }

    // Cursor logic
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(out CursorPosition lpCursorPosition);

    [StructLayout(LayoutKind.Sequential)]
    private struct CursorPosition
    {
        public int X;
        public int Y;
    }

    /// <summary>
    /// Moves the cursor by a pixel delta.
    ///
    /// Uses SendInput with an absolute move rather than SetCursorPos. SetCursorPos repositions
    /// the cursor but injects no input event, so Windows never re-shows a cursor it has hidden
    /// (e.g. the "hide pointer while typing" setting in text fields, or before any mouse input at
    /// startup) — the cursor moves invisibly. An injected move event un-hides it. Absolute
    /// coordinates also bypass pointer acceleration, keeping movement 1:1 with our pixel math, and
    /// MOUSEEVENTF_VIRTUALDESK spans all monitors.
    /// </summary>
    public static void MoveCursorBy(int dx, int dy)
    {
        if (dx == 0 && dy == 0) return;

        GetCursorPos(out var p);
        MoveCursorTo(p.X + dx, p.Y + dy);
    }

    private static void MoveCursorTo(int x, int y)
    {
        int vx = GetSystemMetrics(SmXVirtualScreen);
        int vy = GetSystemMetrics(SmYVirtualScreen);
        int vw = GetSystemMetrics(SmCxVirtualScreen);
        int vh = GetSystemMetrics(SmCyVirtualScreen);
        if (vw <= 1 || vh <= 1) return;

        // Normalize target pixel to the 0..65535 absolute coordinate space of the virtual desktop.
        int nx = (int)Math.Round((x - vx) * 65535.0 / (vw - 1));
        int ny = (int)Math.Round((y - vy) * 65535.0 / (vh - 1));

        var input = new MouseScrollData
        {
            type = InputMouse,
            mi = new MouseInput
            {
                dx = nx,
                dy = ny,
                dwFlags = MouseEventMove | MouseEventAbsolute | MouseEventVirtualDesk
            }
        };

        SendInput(1, [input], Marshal.SizeOf<MouseScrollData>());
    }
}