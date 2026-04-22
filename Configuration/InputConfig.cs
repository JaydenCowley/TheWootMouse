using TheWootMouse.inputs;

namespace TheWootMouse.Configuration;

public static class InputConfig
{
    // Physical keys providing analog input (HID codes)
    public const int KeyUp    = (int)KeyCodes.F13;
    public const int KeyRight = (int)KeyCodes.F14;
    public const int KeyDown  = (int)KeyCodes.F15;
    public const int KeyLeft  = (int)KeyCodes.F16;
    
    public const int KeyScrollUp = (int)KeyCodes.F17;
    public const int KeyScrollDown = (int)KeyCodes.F18;

    // Virtual key for "mouse layer" (OS-level key)
    // Example: F18 (you can remap this in Wootility)
    public const int MouseLayer = (int)KeyCodes.F24;
}