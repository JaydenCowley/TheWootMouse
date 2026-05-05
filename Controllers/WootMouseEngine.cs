using TheWootMouse.Configuration;
using TheWootMouse.util;

namespace TheWootMouse.Controllers;

public class WootMouseEngine(
    float deadzone = Settings.Deadzone,
    float maxSpeed = Settings.MaxSpeed, // pixels per second
    float exponent = Settings.Exponent,
    float scrollSpeed = Settings.ScrollSpeed,
    float turboMultiplier = Settings.TurboMultiplier)
{
    // Smooth Scroll
    private float _scrollAccumulator = 0f;
    
    private void UpdateScroll(float deltaSeconds)
    {
        float up = WootingSDK.wooting_analog_read_analog(InputConfig.KeyScrollUp);
        float down = WootingSDK.wooting_analog_read_analog(InputConfig.KeyScrollDown);

        float scroll = ApplyAxis(up, down); // -1..1

        if (Math.Abs(scroll) < 0.0001f)
            return;

        // Pixels per second → convert to wheel ticks
        float ticksPerSecond = scroll * scrollSpeed;
        _scrollAccumulator += ticksPerSecond * deltaSeconds * 120f;
        
        int wholeTicks = (int)_scrollAccumulator;

        if (wholeTicks == 0) return;
        MouseController.Scroll(wholeTicks);
        _scrollAccumulator -= wholeTicks;
    }
    private void UpdateMouseCursor(float deltaSeconds)
    {
        // Read analog values from physical keys
        float up    = WootingSDK.wooting_analog_read_analog(InputConfig.KeyUp);
        float down  = WootingSDK.wooting_analog_read_analog(InputConfig.KeyDown);
        float left  = WootingSDK.wooting_analog_read_analog(InputConfig.KeyLeft);
        float right = WootingSDK.wooting_analog_read_analog(InputConfig.KeyRight);
        float turbo = WootingSDK.wooting_analog_read_analog(InputConfig.TurboKey);
        
        float vertical   = ApplyAxis(down, up);    // -1..1
        float horizontal = ApplyAxis(right, left); // -1..1
        
        // KeyLogger.LogHidAnalog();
        
        if (Math.Abs(vertical) < 0.0001f && Math.Abs(horizontal) < 0.0001f)
            return;
        // Convert to pixels this frame
        float pixelsPerFrame = maxSpeed * deltaSeconds;
        if (turbo > 0.0001f)
        {
            pixelsPerFrame *= turboMultiplier * turbo;
        }
        int dx = (int)(horizontal * pixelsPerFrame);
        int dy = (int)(vertical   * pixelsPerFrame);

        MouseController.MoveBy(dx, dy);
    }

    public void Update(float deltaSeconds)
    {
        // Only active when layer key is held (needs to be mapped to an actual key not Fn Layer key since not tracked in Wooting SDK)
        // if (!KeyboardState.IsKeyDown(InputConfig.VkMouseLayer))
        //     return;


        UpdateMouseCursor(deltaSeconds);
        UpdateScroll(deltaSeconds);
    }

    private float ApplyAxis(float positive, float negative)
    {
        float value = positive - negative; // Cancels out opposite directions

        if (Math.Abs(value) < deadzone)
            return 0f;

        float sign = Math.Sign(value);
        float mag = (Math.Abs(value) - deadzone) / (1f - deadzone);
        mag = Math.Clamp(mag, 0f, 1f);
        mag = (float)Math.Pow(mag, exponent);

        return sign * mag;
    }
}
