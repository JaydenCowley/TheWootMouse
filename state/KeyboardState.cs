namespace TheWootMouse.state;

using System.Runtime.InteropServices;

public static class KeyboardState
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int virtualKey);

    public static bool IsKeyDown(int virtualKey)
    {
        return (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
    }
}
