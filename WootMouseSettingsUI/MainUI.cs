using TheWootMouse.Configuration;
using TheWootMouse.Infrastructure.Wooting;
using TheWootMouse.infrastructure;

namespace WootMouseSettingsUI;

public partial class MainUI : Form
{
    private readonly System.Windows.Forms.Timer _pollTimer;
    private readonly bool _sdkReady;

    public MainUI()
    {
        InitializeComponent();

        // Try to initialize the Wooting SDK so the analog preview works.
        try
        {
            _sdkReady = WootingSdk.wooting_analog_initialise();
        }
        catch
        {
            _sdkReady = false;
        }

        if (!_sdkReady)
        {
            sdkStatusLabel.Text = "Wooting SDK: not connected (analog preview disabled)";
            sdkStatusLabel.ForeColor = Color.Firebrick;
        }
        else
        {
            sdkStatusLabel.Text = "Wooting SDK: connected";
            sdkStatusLabel.ForeColor = Color.SeaGreen;
        }

        PopulateKeyCombos();
        LoadFromDisk();

        _pollTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 Hz
        _pollTimer.Tick += PollTimer_Tick;
        _pollTimer.Start();
    }

    private void PopulateKeyCombos()
    {
        var keys = Enum.GetValues<KeyCodes>();
        foreach (var combo in new[]
                 {
                     keyUpCombo, keyDownCombo, keyLeftCombo, keyRightCombo,
                     keyScrollUpCombo, keyScrollDownCombo, keyTurboCombo, mouseLayerKeyCombo
                 })
        {
            combo.Items.Clear();
            foreach (var k in keys) combo.Items.Add(k);
        }
    }

    private void LoadFromDisk()
    {
        var s = SettingsManager.Load() ?? new WootMouseEngineSettings();
        ApplySettingsToUi(s);
    }

    private void ApplySettingsToUi(WootMouseEngineSettings s)
    {
        // Keys (fall back to InputConfig defaults if value is 0)
        keyUpCombo.SelectedItem        = (KeyCodes)(s.KeyUp        != 0 ? s.KeyUp        : InputConfig.KeyUp);
        keyDownCombo.SelectedItem      = (KeyCodes)(s.KeyDown      != 0 ? s.KeyDown      : InputConfig.KeyDown);
        keyLeftCombo.SelectedItem      = (KeyCodes)(s.KeyLeft      != 0 ? s.KeyLeft      : InputConfig.KeyLeft);
        keyRightCombo.SelectedItem     = (KeyCodes)(s.KeyRight     != 0 ? s.KeyRight     : InputConfig.KeyRight);
        keyScrollUpCombo.SelectedItem  = (KeyCodes)(s.KeyScrollUp  != 0 ? s.KeyScrollUp  : InputConfig.KeyScrollUp);
        keyScrollDownCombo.SelectedItem= (KeyCodes)(s.KeyScrollDown!= 0 ? s.KeyScrollDown: InputConfig.KeyScrollDown);
        keyTurboCombo.SelectedItem     = (KeyCodes)(s.KeyTurbo     != 0 ? s.KeyTurbo     : InputConfig.TurboKey);
        mouseLayerKeyCombo.SelectedItem= (KeyCodes)(s.MouseLayerKey!= 0 ? s.MouseLayerKey: InputConfig.KeyEnable);

        // Toggles
        mouseLayerEnabledCheck.Checked = s.MouseLayerKeyEnabled;

        // Numeric values (fall back to Settings.* defaults if 0)
        deadzoneTrack.Value     = ClampTrack(deadzoneTrack,    (int)Math.Round((s.DeadZone     > 0 ? s.DeadZone : Settings.Deadzone) * 100));
        maxSpeedNumeric.Value   = ClampNumeric(maxSpeedNumeric, s.MaxMouseSpeed       > 0 ? s.MaxMouseSpeed       : (int)Settings.MaxSpeed);
        scrollSpeedNumeric.Value= ClampNumeric(scrollSpeedNumeric, s.MaxMouseScrollSpeed > 0 ? s.MaxMouseScrollSpeed : (int)Settings.ScrollSpeed);
        turboMultiplierNumeric.Value = ClampNumeric(turboMultiplierNumeric, s.MouseTurboSpeedMultiplier > 0 ? s.MouseTurboSpeedMultiplier : (int)Settings.TurboMultiplier);
        curvePowerNumeric.Value = ClampNumeric(curvePowerNumeric, s.MouseCurvePower > 0 ? s.MouseCurvePower : (int)Settings.Exponent);

        UpdateDeadzoneLabel();
    }

    private WootMouseEngineSettings BuildSettingsFromUi() => new()
    {
        MouseLayerKeyEnabled        = mouseLayerEnabledCheck.Checked,
        KeyUp                       = (int)(KeyCodes)keyUpCombo.SelectedItem!,
        KeyDown                     = (int)(KeyCodes)keyDownCombo.SelectedItem!,
        KeyLeft                     = (int)(KeyCodes)keyLeftCombo.SelectedItem!,
        KeyRight                    = (int)(KeyCodes)keyRightCombo.SelectedItem!,
        KeyScrollUp                 = (int)(KeyCodes)keyScrollUpCombo.SelectedItem!,
        KeyScrollDown               = (int)(KeyCodes)keyScrollDownCombo.SelectedItem!,
        KeyTurbo                    = (int)(KeyCodes)keyTurboCombo.SelectedItem!,
        MouseLayerKey               = (int)(KeyCodes)mouseLayerKeyCombo.SelectedItem!,
        DeadZone                    = deadzoneTrack.Value / 100f,
        MaxMouseSpeed               = (int)maxSpeedNumeric.Value,
        MaxMouseScrollSpeed         = (int)scrollSpeedNumeric.Value,
        MouseTurboSpeedMultiplier   = (int)turboMultiplierNumeric.Value,
        MouseCurvePower             = (int)curvePowerNumeric.Value,
    };

    private static int ClampTrack(TrackBar t, int v) => Math.Clamp(v, t.Minimum, t.Maximum);
    private static decimal ClampNumeric(NumericUpDown n, int v) => Math.Clamp(v, (int)n.Minimum, (int)n.Maximum);

    private void UpdateDeadzoneLabel() =>
        deadzoneValueLabel.Text = $"{deadzoneTrack.Value / 100f:0.00}";

    private void deadzoneTrack_Scroll(object? sender, EventArgs e) => UpdateDeadzoneLabel();

    private void saveButton_Click(object? sender, EventArgs e)
    {
        try
        {
            SettingsManager.Save(BuildSettingsFromUi());
            statusLabel.Text = $"Saved at {DateTime.Now:HH:mm:ss}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Failed to save settings",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void reloadButton_Click(object? sender, EventArgs e)
    {
        LoadFromDisk();
        statusLabel.Text = $"Reloaded at {DateTime.Now:HH:mm:ss}";
    }

    private void PollTimer_Tick(object? sender, EventArgs e)
    {
        if (!_sdkReady) return;

        // Read raw values for the currently-selected movement keys.
        float up    = SafeRead((KeyCodes)keyUpCombo.SelectedItem!);
        float down  = SafeRead((KeyCodes)keyDownCombo.SelectedItem!);
        float left  = SafeRead((KeyCodes)keyLeftCombo.SelectedItem!);
        float right = SafeRead((KeyCodes)keyRightCombo.SelectedItem!);

        float deadzone = deadzoneTrack.Value / 100f;
        float exponent = (float)curvePowerNumeric.Value;

        float vertical   = ShapeAxis(down, up,    deadzone, exponent);
        float horizontal = ShapeAxis(right, left, deadzone, exponent);

        analogDisplay.SetStick(horizontal, vertical);
        analogDisplay.SetDeadzone(deadzone);
    }

    private static float SafeRead(KeyCodes k)
    {
        try { return WootingSdk.wooting_analog_read_analog((int)k); }
        catch { return 0f; }
    }

    private static float ShapeAxis(float positive, float negative, float deadzone, float exponent)
    {
        float value = positive - negative;
        if (Math.Abs(value) < deadzone) return 0f;

        float sign = Math.Sign(value);
        float mag  = (Math.Abs(value) - deadzone) / (1f - deadzone);
        mag = Math.Clamp(mag, 0f, 1f);
        mag = (float)Math.Pow(mag, exponent);
        return sign * mag;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _pollTimer.Stop();
        base.OnFormClosing(e);
    }
}