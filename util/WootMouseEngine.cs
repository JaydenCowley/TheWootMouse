using System.IO.Compression;
using TheWootMouse.state;

namespace TheWootMouse.util;

using static KeyCodes;

public class WootMouseEngine
{
    private readonly float _deadzone;
    private readonly float _maxSpeed;   // pixels per second at full press
    private readonly float _exponent;   // response curve
    private readonly float _scrollSpeed;
    
    public WootMouseEngine(
        float deadzone = 0.05f,
        float maxSpeed = 1200f,   // pixels per second
        float exponent = 1.6f,
        float scrollSpeed = 20f)
    {
        _deadzone = deadzone;
        _maxSpeed = maxSpeed;
        _exponent = exponent;
        _scrollSpeed = scrollSpeed;
    }
    private void UpdateScroll(float deltaSeconds)
    {
        float up = WootingSDK.wooting_analog_read_analog(InputConfig.KeyScrollUp);
        float down = WootingSDK.wooting_analog_read_analog(InputConfig.KeyScrollDown);

        float scroll = ApplyAxis(up, down); // -1..1

        if (Math.Abs(scroll) < 0.0001f)
            return;

        // Pixels per second → convert to wheel ticks
        float ticksPerSecond = scroll * _scrollSpeed;
        float ticksThisFrame = ticksPerSecond * deltaSeconds;

        int wheelDelta = (int)(ticksThisFrame * 120); // 120 = 1 wheel notch

        if (wheelDelta != 0)
            MouseController.Scroll(wheelDelta);
    }
    private void UpdateMouseCursor(float deltaSeconds)
    {
        // Read analog values from physical keys
        float up    = WootingSDK.wooting_analog_read_analog(InputConfig.KeyUp);
        float down  = WootingSDK.wooting_analog_read_analog(InputConfig.KeyDown);
        float left  = WootingSDK.wooting_analog_read_analog(InputConfig.KeyLeft);
        float right = WootingSDK.wooting_analog_read_analog(InputConfig.KeyRight);
        float scrollUp = WootingSDK.wooting_analog_read_analog(InputConfig.KeyScrollUp);
        float scrollDown = WootingSDK.wooting_analog_read_analog(InputConfig.KeyScrollDown);

        float vertical   = ApplyAxis(down, up);    // -1..1
        float horizontal = ApplyAxis(right, left); // -1..1

        if (Math.Abs(vertical) < 0.0001f && Math.Abs(horizontal) < 0.0001f)
            return;
        // Convert to pixels this frame
        float pixelsPerFrame = _maxSpeed * deltaSeconds;
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
        float value = positive - negative; // -1..1

        if (Math.Abs(value) < _deadzone)
            return 0f;

        float sign = Math.Sign(value);
        float mag = (Math.Abs(value) - _deadzone) / (1f - _deadzone);
        mag = Math.Clamp(mag, 0f, 1f);
        mag = (float)Math.Pow(mag, _exponent);

        return sign * mag;
    }
}
