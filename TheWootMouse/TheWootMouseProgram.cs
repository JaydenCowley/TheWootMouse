using System.Diagnostics;
using System.Runtime.InteropServices;
using TheWootMouse.Configuration;
using TheWootMouse.Controllers;
using TheWootMouse.Infrastructure.Wooting;

namespace TheWootMouse;

internal abstract partial class TheWootMouseProgram
{
    // Per-monitor DPI aware v2, so monitor DPI queries report each screen's real scaling.
    private static readonly IntPtr DpiAwarePerMonitorV2 = new(-4);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetProcessDpiAwarenessContext(IntPtr value);

    private static void Main()
    {
        try { SetProcessDpiAwarenessContext(DpiAwarePerMonitorV2); }
        catch { /* older Windows without this API — density auto-switch simply falls back. */ }

        if (!WootingSdk.wooting_analog_initialise())
        {
            Console.WriteLine("Failed to initialize Wooting Analog SDK");
            return;
        }

        var settings = SettingsManager.Load();
        var engine = new WootMouseEngine(settings);

        var stopwatch = Stopwatch.StartNew();
        long lastTicks = stopwatch.ElapsedTicks;
        double tickFreq = Stopwatch.Frequency;

        while (true)
        {
            long now = stopwatch.ElapsedTicks;
            float deltaSeconds = (float)((now - lastTicks) / tickFreq);
            lastTicks = now;

            engine.Update(deltaSeconds);

            Thread.Sleep(8);
        }
    }
}
