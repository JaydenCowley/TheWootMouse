namespace TheWootMouse.state;

using System.Runtime.InteropServices;

public static partial class KeyboardState
{
    [LibraryImport("user32.dll")]
    private static partial short GetAsyncKeyState(int virtualKey);

    public static bool IsKeyDown(int virtualKey)
    {
        return (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
    }
}
