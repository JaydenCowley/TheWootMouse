namespace TheWootMouse.util;

using System.Runtime.InteropServices;

public static class MouseController
{
    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public MOUSEINPUT mi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    private const uint INPUT_MOUSE = 0;
    private const uint MOUSEEVENTF_WHEEL = 0x0800;

    [DllImport("user32.dll")]
    private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

    public static void Scroll(int delta)
    {
        var input = new INPUT
        {
            type = INPUT_MOUSE,
            mi = new MOUSEINPUT
            {
                mouseData = (uint)delta,
                dwFlags = MOUSEEVENTF_WHEEL
            }
        };

        SendInput(1, new[] { input }, Marshal.SizeOf(typeof(INPUT)));
    }

    // Cursor logic
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int X, int Y);

    [DllImport("user32.dll")]
    private static extern bool GetCursorPos(out POINT lpPoint);

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
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