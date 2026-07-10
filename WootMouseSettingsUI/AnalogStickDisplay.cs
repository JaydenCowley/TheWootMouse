using System.ComponentModel;

namespace WootMouseSettingsUI;

/// <summary>
/// Renders an analog-stick style display. (0,0) is centered; (1,1) is bottom-right.
/// </summary>
public sealed class AnalogStickDisplay : Control
{
    private float _x;
    private float _y;
    private float _deadzone = 0.1f;

    public AnalogStickDisplay()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint
                                                    | ControlStyles.OptimizedDoubleBuffer
                                                    | ControlStyles.ResizeRedraw, true);
        BackColor = Color.FromArgb(30, 30, 34);
        ForeColor = Color.Gainsboro;
        MinimumSize = new Size(180, 180);
    }

    [Browsable(false)]
    public float StickX => _x;

    [Browsable(false)]
    public float StickY => _y;

    /// <summary>
    /// Set the stick position in normalized space. X: -1 (left) .. 1 (right), Y: -1 (up) .. 1 (down).
    /// </summary>
    public void SetStick(float x, float y)
    {
        x = Math.Clamp(x, -1f, 1f);
        y = Math.Clamp(y, -1f, 1f);
        if (Math.Abs(x - _x) < 0.001f && Math.Abs(y - _y) < 0.001f) return;
        _x = x;
        _y = y;
        Invalidate();
    }

    public void SetDeadzone(float deadzone)
    {
        deadzone = Math.Clamp(deadzone, 0f, 1f);
        if (Math.Abs(deadzone - _deadzone) < 0.001f) return;
        _deadzone = deadzone;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.Clear(BackColor);

        // Fit a square inside the control.
        int size = Math.Min(Width, Height) - 16;
        if (size <= 20) return;
        int cx = Width / 2;
        int cy = Height / 2;
        int r  = size / 2;

        var outerRect    = new Rectangle(cx - r, cy - r, size, size);
        int deadzoneR    = (int)(r * _deadzone);
        var deadzoneRect = new Rectangle(cx - deadzoneR, cy - deadzoneR, deadzoneR * 2, deadzoneR * 2);

        // Outer circle (boundary)
        using (var border = new Pen(Color.FromArgb(80, 80, 90), 2f))
            g.DrawEllipse(border, outerRect);

        // Crosshair
        using (var axis = new Pen(Color.FromArgb(60, 60, 70), 1f))
        {
            g.DrawLine(axis, cx - r, cy, cx + r, cy);
            g.DrawLine(axis, cx, cy - r, cx, cy + r);
        }

        // Deadzone ring
        if (deadzoneR > 1)
        {
            using var dzBrush = new SolidBrush(Color.FromArgb(40, 200, 90, 90));
            using var dzPen   = new Pen(Color.FromArgb(180, 90, 90), 1f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            g.FillEllipse(dzBrush, deadzoneRect);
            g.DrawEllipse(dzPen, deadzoneRect);
        }

        // Stick position
        int stickX = cx + (int)(_x * r);
        int stickY = cy + (int)(_y * r);
        const int dotR = 10;

        // Trail line from center to stick
        using (var trail = new Pen(Color.FromArgb(120, 90, 200, 255), 2f))
            g.DrawLine(trail, cx, cy, stickX, stickY);

        // Dot
        var dotRect = new Rectangle(stickX - dotR, stickY - dotR, dotR * 2, dotR * 2);
        using (var dotBrush = new SolidBrush(Color.FromArgb(255, 90, 200, 255)))
            g.FillEllipse(dotBrush, dotRect);
        using (var dotEdge = new Pen(Color.White, 1.5f))
            g.DrawEllipse(dotEdge, dotRect);

        // Numeric readout
        string text = $"X: {_x,+5:0.00}    Y: {_y,+5:0.00}";
        using var font = new Font(FontFamily.GenericMonospace, 9f, FontStyle.Bold);
        var textSize = g.MeasureString(text, font);
        g.DrawString(text, font, Brushes.Gainsboro,
            cx - textSize.Width / 2f,
            cy + r + 4);
    }
}