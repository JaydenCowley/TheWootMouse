using System.Runtime.InteropServices;

namespace TheWootMouse.Infrastructure.Windows;

public static partial class KeyboardState
{
    [LibraryImport("user32.dll")]
    private static partial short GetAsyncKeyState(int virtualKey);

    public static bool IsVirtualKeyDown(int virtualKey)
    {
        Console.WriteLine(GetAsyncKeyState(virtualKey));
        return (GetAsyncKeyState(virtualKey) & 0x8000) != 0;
    }
}
