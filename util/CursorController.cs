namespace TheWootMouse.util;

using System.Runtime.InteropServices;

public static class CursorController
{
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