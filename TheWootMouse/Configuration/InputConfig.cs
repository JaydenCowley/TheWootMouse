using TheWootMouse.infrastructure;

namespace TheWootMouse.Configuration;

public static class InputConfig
{
    public const int KeyEnable = (int)KeyCodes.F20;
    // Physical keys providing analog input (HID codes)
    public const int KeyUp    = (int)KeyCodes.F13;
    public const int KeyRight = (int)KeyCodes.F14;
    public const int KeyDown  = (int)KeyCodes.F15;
    public const int KeyLeft  = (int)KeyCodes.F16;

    public const int KeyScrollUp = (int)KeyCodes.F17;
    public const int KeyScrollDown = (int)KeyCodes.F18;

    // Virtual key for "mouse layer" (OS-level key)l
    // Example: F18 (you can remap this in Wootility)
    public const int TurboKey = (int)KeyCodes.LeftShift;
    // public const int TurboKey = (int)KeyCodes.LeftShift;
    // modifier setting for speed control preference variable slow/fast modifier key(turbo)
}