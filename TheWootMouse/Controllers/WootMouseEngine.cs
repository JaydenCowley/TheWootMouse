using TheWootMouse.Configuration;
using TheWootMouse.Infrastructure.Wooting;

namespace TheWootMouse.Controllers;

public class WootMouseEngine
{
    // Resolved key bindings (HID codes).
    private readonly int _keyUp;
    private readonly int _keyDown;
    private readonly int _keyLeft;
    private readonly int _keyRight;
    private readonly int _keyScrollUp;
    private readonly int _keyScrollDown;
    private readonly int _turboKey;
    private readonly int _enableKey;
    private readonly bool _requireEnableKey;

    // Resolved mouse behaviour.
    private readonly float _deadzone;
    private readonly float _baseMaxSpeed; // px/s at ReferencePpi
    private readonly float _exponent;
    private readonly float _scrollSpeed;
    private readonly float _turboMultiplier;

    private readonly DensityResolver _density;

    // Sub-pixel / sub-tick accumulators so slow movement isn't lost to integer truncation.
    private float _scrollAccumulator;
    private float _moveAccumX;
    private float _moveAccumY;

    public WootMouseEngine(WootMouseEngineSettings settings)
    {
        _keyUp         = Resolve(settings.KeyUp,        InputConfig.KeyUp);
        _keyDown       = Resolve(settings.KeyDown,      InputConfig.KeyDown);
        _keyLeft       = Resolve(settings.KeyLeft,      InputConfig.KeyLeft);
        _keyRight      = Resolve(settings.KeyRight,     InputConfig.KeyRight);
        _keyScrollUp   = Resolve(settings.KeyScrollUp,  InputConfig.KeyScrollUp);
        _keyScrollDown = Resolve(settings.KeyScrollDown, InputConfig.KeyScrollDown);
        _turboKey      = Resolve(settings.KeyTurbo,     InputConfig.TurboKey);
        _enableKey     = Resolve(settings.MouseLayerKey, InputConfig.KeyEnable);
        _requireEnableKey = settings.MouseLayerKeyEnabled;

        _deadzone        = settings.DeadZone                  > 0 ? settings.DeadZone                       : Settings.Deadzone;
        _baseMaxSpeed    = settings.MaxMouseSpeed             > 0 ? settings.MaxMouseSpeed                  : Settings.MaxSpeed;
        _scrollSpeed     = settings.MaxMouseScrollSpeed       > 0 ? settings.MaxMouseScrollSpeed           : Settings.ScrollSpeed;
        _turboMultiplier = settings.MouseTurboSpeedMultiplier > 0 ? settings.MouseTurboSpeedMultiplier      : Settings.TurboMultiplier;
        _exponent        = settings.MouseCurvePower           > 0 ? settings.MouseCurvePower               : Settings.Exponent;

        _density = new DensityResolver(settings);
    }

    private static int Resolve(int configured, int fallback) => configured != 0 ? configured : fallback;

    public void Update(float deltaSeconds)
    {
        if (_requireEnableKey)
        {
            float enable = WootingSdk.wooting_analog_read_analog(_enableKey);
            if (enable < 0.0001f) return;
        }

        UpdateMouseCursor(deltaSeconds);
        UpdateScroll(deltaSeconds);
    }

    private void UpdateMouseCursor(float deltaSeconds)
    {
        float up    = WootingSdk.wooting_analog_read_analog(_keyUp);
        float down  = WootingSdk.wooting_analog_read_analog(_keyDown);
        float left  = WootingSdk.wooting_analog_read_analog(_keyLeft);
        float right = WootingSdk.wooting_analog_read_analog(_keyRight);
        float turbo = WootingSdk.wooting_analog_read_analog(_turboKey);

        float vertical   = ApplyAxis(down, up);    // -1..1
        float horizontal = ApplyAxis(right, left); // -1..1

        if (Math.Abs(vertical) < 0.0001f && Math.Abs(horizontal) < 0.0001f)
        {
            // No input — drop any residual sub-pixel so movement doesn't "creep" on release.
            _moveAccumX = 0f;
            _moveAccumY = 0f;
            return;
        }

        // Scale configured speed by the active density profile so perceived speed is density-independent.
        float pixelsPerFrame = _baseMaxSpeed * (float)_density.CurrentScale() * deltaSeconds;
        if (turbo > 0.0001f)
            pixelsPerFrame *= _turboMultiplier * turbo;

        _moveAccumX += horizontal * pixelsPerFrame;
        _moveAccumY += vertical   * pixelsPerFrame;

        int dx = (int)_moveAccumX;
        int dy = (int)_moveAccumY;
        if (dx == 0 && dy == 0) return;

        _moveAccumX -= dx;
        _moveAccumY -= dy;
        MouseController.MoveCursorBy(dx, dy);
    }

    private void UpdateScroll(float deltaSeconds)
    {
        float up    = WootingSdk.wooting_analog_read_analog(_keyScrollUp);
        float down  = WootingSdk.wooting_analog_read_analog(_keyScrollDown);
        float turbo = WootingSdk.wooting_analog_read_analog(_turboKey);
        float scroll = ApplyAxis(up, down); // -1..1

        if (Math.Abs(scroll) < 0.0001f)
        {
            _scrollAccumulator = 0f;
            return;
        }

        // Wheel ticks are density-independent, so scroll speed is not scaled by the profile.
        float ticksPerSecond = scroll * _scrollSpeed;
        if (turbo > 0.0001f)
            ticksPerSecond *= _turboMultiplier;

        _scrollAccumulator += ticksPerSecond * deltaSeconds * 120f;

        int wholeTicks = (int)_scrollAccumulator;
        if (wholeTicks == 0) return;

        MouseController.Scroll(wholeTicks);
        _scrollAccumulator -= wholeTicks;
    }

    private float ApplyAxis(float positive, float negative)
    {
        float value = positive - negative; // Cancels out opposite directions

        if (Math.Abs(value) < _deadzone)
            return 0f;

        float sign = Math.Sign(value);
        float mag = (Math.Abs(value) - _deadzone) / (1f - _deadzone);
        mag = Math.Clamp(mag, 0f, 1f);
        mag = (float)Math.Pow(mag, _exponent);

        return sign * mag;
    }
}
