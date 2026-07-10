using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace WootMouseSettingsWpf.Controls;

/// <summary>
/// Renders an analog-stick style preview. (0,0) is centered; X: -1 left..1 right, Y: -1 up..1 down.
/// Bind <see cref="StickX"/>, <see cref="StickY"/>, and <see cref="Deadzone"/> to live analog values.
/// </summary>
public sealed class AnalogStickView : FrameworkElement
{
    public static readonly DependencyProperty StickXProperty = DependencyProperty.Register(
        nameof(StickX), typeof(double), typeof(AnalogStickView),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty StickYProperty = DependencyProperty.Register(
        nameof(StickY), typeof(double), typeof(AnalogStickView),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public static readonly DependencyProperty DeadzoneProperty = DependencyProperty.Register(
        nameof(Deadzone), typeof(double), typeof(AnalogStickView),
        new FrameworkPropertyMetadata(0.1, FrameworkPropertyMetadataOptions.AffectsRender));

    public double StickX { get => (double)GetValue(StickXProperty); set => SetValue(StickXProperty, value); }
    public double StickY { get => (double)GetValue(StickYProperty); set => SetValue(StickYProperty, value); }
    public double Deadzone { get => (double)GetValue(DeadzoneProperty); set => SetValue(DeadzoneProperty, value); }

    private static readonly Brush Background = new SolidColorBrush(Color.FromRgb(24, 24, 30));
    private static readonly Pen BorderPen = new(new SolidColorBrush(Color.FromRgb(70, 70, 84)), 2);
    private static readonly Pen AxisPen = new(new SolidColorBrush(Color.FromRgb(52, 52, 64)), 1);
    private static readonly Brush DeadzoneFill = new SolidColorBrush(Color.FromArgb(40, 220, 90, 90));
    private static readonly Pen DeadzonePen = new(new SolidColorBrush(Color.FromArgb(180, 200, 90, 90)), 1)
        { DashStyle = DashStyles.Dash };
    private static readonly Pen TrailPen = new(new SolidColorBrush(Color.FromArgb(150, 90, 200, 255)), 2);
    private static readonly Brush DotFill = new SolidColorBrush(Color.FromRgb(90, 200, 255));
    private static readonly Pen DotEdge = new(Brushes.White, 1.5);
    private static readonly Brush TextBrush = new SolidColorBrush(Color.FromRgb(200, 205, 215));

    static AnalogStickView()
    {
        Background.Freeze(); BorderPen.Freeze(); AxisPen.Freeze();
        DeadzoneFill.Freeze(); DeadzonePen.Freeze(); TrailPen.Freeze();
        DotFill.Freeze(); DotEdge.Freeze(); TextBrush.Freeze();
    }

    protected override void OnRender(DrawingContext dc)
    {
        double w = ActualWidth, h = ActualHeight;
        dc.DrawRectangle(Background, null, new Rect(0, 0, w, h));

        double size = Math.Min(w, h) - 24;
        if (size <= 20) return;

        double cx = w / 2, cy = h / 2, r = size / 2;
        double x = Math.Clamp(StickX, -1, 1);
        double y = Math.Clamp(StickY, -1, 1);
        double dz = Math.Clamp(Deadzone, 0, 1);

        // Outer boundary
        dc.DrawEllipse(null, BorderPen, new Point(cx, cy), r, r);

        // Crosshair
        dc.DrawLine(AxisPen, new Point(cx - r, cy), new Point(cx + r, cy));
        dc.DrawLine(AxisPen, new Point(cx, cy - r), new Point(cx, cy + r));

        // Deadzone ring
        double dzr = r * dz;
        if (dzr > 1)
            dc.DrawEllipse(DeadzoneFill, DeadzonePen, new Point(cx, cy), dzr, dzr);

        // Trail + dot
        double sx = cx + x * r, sy = cy + y * r;
        dc.DrawLine(TrailPen, new Point(cx, cy), new Point(sx, sy));
        dc.DrawEllipse(DotFill, DotEdge, new Point(sx, sy), 10, 10);

        // Readout
        var text = new FormattedText(
            $"X: {x,6:0.00}   Y: {y,6:0.00}",
            CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
            new Typeface(new FontFamily("Consolas"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
            13, TextBrush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
        dc.DrawText(text, new Point(cx - text.Width / 2, cy + r + 6));
    }
}
