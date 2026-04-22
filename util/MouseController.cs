namespace TheWootMouse.util;

using System.Runtime.InteropServices;

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

    private const uint MouseWheelScroll = 0;
    private const uint MouseWheelEvent = 0x0800;

    [LibraryImport("user32.dll")]
    private static partial uint SendInput(uint nInputs, MouseScrollData[] pInputs, int cbSize);

    public static void Scroll(int delta)
    {
        var input = new MouseScrollData
        {
            type = MouseWheelScroll,
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
    private static partial bool SetCursorPos(int x, int y);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetCursorPos(out CursorPosition lpCursorPosition);

    [StructLayout(LayoutKind.Sequential)]
    private struct CursorPosition
    {
        public int X;
        public int Y;
    }

    public static void MoveBy(int dx, int dy)
    {
        GetCursorPos(out var p);
        SetCursorPos(p.X + dx, p.Y + dy);
    }

}