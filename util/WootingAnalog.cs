using System.Runtime.InteropServices;

namespace TheWootMouse.util;

public static class WootingAnalog
{
    private const string DllName = "wooting_analog_sdk.dll";
    private const string DllNameUser = "user32.dll";

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool wooting_analog_initialise();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void wooting_analog_reset();

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern float wooting_analog_read_analog(int keycode);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern int wooting_analog_read_full_buffer();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern float wooting_analog_read_analog_device();
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern float wooting_analog_read(int keycode);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern float wooting_analog_read_full(int keycode);

    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool wooting_analog_set_keycode_mode(int keycodeMode);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool wooting_analog_get_connected_devices_info(int keycodeMode);
    
    [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool read_analog(int keycodeMode);
    
}