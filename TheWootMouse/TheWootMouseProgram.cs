using System.Diagnostics;
using TheWootMouse.Controllers;
using TheWootMouse.util;

namespace TheWootMouse;

internal abstract class TheWootMouseProgram
{
    private static void Main()
    {
        if (!WootingSDK.wooting_analog_initialise())
        {
            Console.WriteLine("Failed to initialize Wooting Analog SDK");
            return;
        }

        // Console.WriteLine("Wooting SDK initialized");
        // Console.WriteLine("Keys Assigned to mouse movement: ");
        // Console.WriteLine($"Up: {KeyCodes.F13}, Right: {KeyCodes.F14}, Down: {KeyCodes.F15}, Left: {KeyCodes.F16}");

        var engine = new WootMouseEngine(
            deadzone: 0.05f,
            maxSpeed: 1400f,  // tune this
            exponent: 1.6f
        );

        var stopwatch = Stopwatch.StartNew();
        long lastTicks = stopwatch.ElapsedTicks;
        double tickFreq = Stopwatch.Frequency;

        while (true)
        {
            // KeyLogger.LogVirtualKeys();
            // KeyLogger.LogHidAnalog();

            const float deltaSeconds = 0.01f;

            engine.Update(deltaSeconds);

            Thread.Sleep(8);
        }
    }
}