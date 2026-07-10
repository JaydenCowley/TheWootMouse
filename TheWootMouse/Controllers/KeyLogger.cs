using System.Runtime.InteropServices;
using TheWootMouse.Infrastructure.Wooting;

namespace TheWootMouse.Controllers;

public static partial class KeyLogger
{
    // --- Virtual Key Logger (Windows-level keys) ---
    [LibraryImport("user32.dll")]
    private static partial short GetAsyncKeyState(int vKey);

    public static void LogVirtualKeys()
    {
        for (int vk = 1; vk < 256; vk++)
        {
            if ((GetAsyncKeyState(vk) & 0x8000) != 0)
                Console.WriteLine($"VK {vk:X2} pressed");
        }
    }

    // --- HID Logger (Wooting physical switches) ---
    public static void LogHidAnalog()
    {
        for (int hid = 0; hid < 256; hid++)
        {
            float v = WootingSdk.wooting_analog_read_analog(hid);
            if (v > 0.01f)
                Console.WriteLine($"HID {hid:X2} = {v}");
        }
    }
}