using System.Collections.ObjectModel;
using System.Windows.Input;
using TheWootMouse.Configuration;
using TheWootMouse.Infrastructure.Windows;
using TheWootMouse.Infrastructure.Wooting;
using TheWootMouse.infrastructure;
using WootMouseSettingsWpf.Mvvm;

namespace WootMouseSettingsWpf.ViewModels;

/// <summary>A connected monitor offered as a binding target for a density profile.</summary>
public sealed record MonitorOption(string DeviceName, string Label)
{
    public override string ToString() => Label;
}

public sealed class MainViewModel : ObservableObject
{
    private readonly bool _sdkReady;

    public MainViewModel()
    {
        try { _sdkReady = WootingSdk.wooting_analog_initialise(); }
        catch { _sdkReady = false; }

        SaveCommand = new RelayCommand(Save);
        ReloadCommand = new RelayCommand(Reload);
        AddProfileCommand = new RelayCommand(AddProfile);
        DeleteProfileCommand = new RelayCommand(DeleteProfile, () => SelectedProfile is not null);
        DetectMonitorsCommand = new RelayCommand(DetectMonitors);
        ComputePpiCommand = new RelayCommand(ComputePpiFromResolution, () => SelectedProfile is not null);

        RefreshMonitors();
        Reload();
    }

    // ---- SDK status ----
    public bool SdkConnected => _sdkReady;
    public string SdkStatusText => _sdkReady ? "Wooting SDK connected" : "Wooting SDK not connected — analog preview disabled";

    // ---- Available options ----
    public IReadOnlyList<KeyCodes> AvailableKeys { get; } = Enum.GetValues<KeyCodes>();
    public ObservableCollection<MonitorOption> AvailableMonitors { get; } = new();

    // ---- Key bindings ----
    private KeyCodes _keyUp, _keyDown, _keyLeft, _keyRight, _keyScrollUp, _keyScrollDown, _keyTurbo, _mouseLayerKey;
    public KeyCodes KeyUp        { get => _keyUp;        set { if (SetProperty(ref _keyUp, value))        UpdatePreviewKeys(); } }
    public KeyCodes KeyDown      { get => _keyDown;      set { if (SetProperty(ref _keyDown, value))      UpdatePreviewKeys(); } }
    public KeyCodes KeyLeft      { get => _keyLeft;      set { if (SetProperty(ref _keyLeft, value))      UpdatePreviewKeys(); } }
    public KeyCodes KeyRight     { get => _keyRight;     set { if (SetProperty(ref _keyRight, value))     UpdatePreviewKeys(); } }
    public KeyCodes KeyScrollUp  { get => _keyScrollUp;  set => SetProperty(ref _keyScrollUp, value); }
    public KeyCodes KeyScrollDown{ get => _keyScrollDown;set => SetProperty(ref _keyScrollDown, value); }
    public KeyCodes KeyTurbo     { get => _keyTurbo;     set => SetProperty(ref _keyTurbo, value); }
    public KeyCodes MouseLayerKey{ get => _mouseLayerKey;set => SetProperty(ref _mouseLayerKey, value); }

    private bool _mouseLayerEnabled;
    public bool MouseLayerEnabled { get => _mouseLayerEnabled; set => SetProperty(ref _mouseLayerEnabled, value); }

    // ---- Mouse behaviour ----
    private double _deadzone;
    private int _maxSpeed, _scrollSpeed, _turboMultiplier, _curvePower;
    public double Deadzone { get => _deadzone; set { if (SetProperty(ref _deadzone, value)) OnPropertyChanged(nameof(DeadzoneText)); } }
    public string DeadzoneText => $"{Deadzone:0.00}";
    public int MaxSpeed { get => _maxSpeed; set => SetProperty(ref _maxSpeed, value); }
    public int ScrollSpeed { get => _scrollSpeed; set => SetProperty(ref _scrollSpeed, value); }
    public int TurboMultiplier { get => _turboMultiplier; set => SetProperty(ref _turboMultiplier, value); }
    public int CurvePower { get => _curvePower; set { if (SetProperty(ref _curvePower, value)) OnPropertyChanged(nameof(CurvePower)); } }

    // ---- Density profiles ----
    public ObservableCollection<DensityProfileViewModel> Profiles { get; } = new();

    private DensityProfileViewModel? _selectedProfile;
    public DensityProfileViewModel? SelectedProfile
    {
        get => _selectedProfile;
        set
        {
            if (SetProperty(ref _selectedProfile, value))
            {
                OnPropertyChanged(nameof(HasSelectedProfile));
                CommandManagerInvalidate();
            }
        }
    }

    public bool HasSelectedProfile => SelectedProfile is not null;

    private double _referencePpi = 96;
    public double ReferencePpi { get => _referencePpi; set => SetProperty(ref _referencePpi, value); }

    private bool _autoSwitchByMonitor;
    public bool AutoSwitchByMonitor { get => _autoSwitchByMonitor; set => SetProperty(ref _autoSwitchByMonitor, value); }

    // Resolution → PPI helper (edit fields).
    private int _resWidth = 3840, _resHeight = 2160;
    private double _diagonalInches = 27;
    public int ResWidth { get => _resWidth; set => SetProperty(ref _resWidth, value); }
    public int ResHeight { get => _resHeight; set => SetProperty(ref _resHeight, value); }
    public double DiagonalInches { get => _diagonalInches; set => SetProperty(ref _diagonalInches, value); }

    // ---- Analog preview ----
    private double _previewX, _previewY, _previewDeadzone;
    public double PreviewX { get => _previewX; private set => SetProperty(ref _previewX, value); }
    public double PreviewY { get => _previewY; private set => SetProperty(ref _previewY, value); }
    public double PreviewDeadzone { get => _previewDeadzone; private set => SetProperty(ref _previewDeadzone, value); }

    // ---- Status ----
    private string _statusText = "";
    public string StatusText { get => _statusText; private set => SetProperty(ref _statusText, value); }

    // ---- Commands ----
    public ICommand SaveCommand { get; }
    public ICommand ReloadCommand { get; }
    public ICommand AddProfileCommand { get; }
    public ICommand DeleteProfileCommand { get; }
    public ICommand DetectMonitorsCommand { get; }
    public ICommand ComputePpiCommand { get; }

    // ---- Load / Save ----
    private void Reload()
    {
        var s = SettingsManager.Load();

        KeyUp         = ToKey(s.KeyUp,        InputConfig.KeyUp);
        KeyDown       = ToKey(s.KeyDown,      InputConfig.KeyDown);
        KeyLeft       = ToKey(s.KeyLeft,      InputConfig.KeyLeft);
        KeyRight      = ToKey(s.KeyRight,     InputConfig.KeyRight);
        KeyScrollUp   = ToKey(s.KeyScrollUp,  InputConfig.KeyScrollUp);
        KeyScrollDown = ToKey(s.KeyScrollDown, InputConfig.KeyScrollDown);
        KeyTurbo      = ToKey(s.KeyTurbo,     InputConfig.TurboKey);
        MouseLayerKey = ToKey(s.MouseLayerKey, InputConfig.KeyEnable);
        MouseLayerEnabled = s.MouseLayerKeyEnabled;

        Deadzone        = s.DeadZone                  > 0 ? s.DeadZone                  : Settings.Deadzone;
        MaxSpeed        = s.MaxMouseSpeed             > 0 ? s.MaxMouseSpeed             : (int)Settings.MaxSpeed;
        ScrollSpeed     = s.MaxMouseScrollSpeed       > 0 ? s.MaxMouseScrollSpeed       : (int)Settings.ScrollSpeed;
        TurboMultiplier = s.MouseTurboSpeedMultiplier > 0 ? s.MouseTurboSpeedMultiplier : (int)Settings.TurboMultiplier;
        CurvePower      = s.MouseCurvePower           > 0 ? s.MouseCurvePower           : (int)Settings.Exponent;

        ReferencePpi = s.ReferencePpi > 0 ? s.ReferencePpi : 96;
        AutoSwitchByMonitor = s.AutoSwitchProfileByMonitor;

        Profiles.Clear();
        foreach (var p in s.DensityProfiles)
            Profiles.Add(new DensityProfileViewModel(p));

        SelectedProfile = Profiles.FirstOrDefault(
            p => string.Equals(p.Name, s.ActiveProfileName, StringComparison.OrdinalIgnoreCase))
            ?? Profiles.FirstOrDefault();

        UpdatePreviewKeys();
        StatusText = $"Loaded at {DateTime.Now:HH:mm:ss}";
    }

    private void Save()
    {
        var settings = new WootMouseEngineSettings
        {
            MouseLayerKeyEnabled = MouseLayerEnabled,
            KeyUp = (int)KeyUp,
            KeyDown = (int)KeyDown,
            KeyLeft = (int)KeyLeft,
            KeyRight = (int)KeyRight,
            KeyScrollUp = (int)KeyScrollUp,
            KeyScrollDown = (int)KeyScrollDown,
            KeyTurbo = (int)KeyTurbo,
            MouseLayerKey = (int)MouseLayerKey,
            DeadZone = (float)Deadzone,
            MaxMouseSpeed = MaxSpeed,
            MaxMouseScrollSpeed = ScrollSpeed,
            MouseTurboSpeedMultiplier = TurboMultiplier,
            MouseCurvePower = CurvePower,
            ReferencePpi = ReferencePpi,
            AutoSwitchProfileByMonitor = AutoSwitchByMonitor,
            ActiveProfileName = SelectedProfile?.Name,
            DensityProfiles = Profiles.Select(p => p.ToModel()).ToList(),
        };

        SettingsManager.Save(settings);
        StatusText = $"Saved at {DateTime.Now:HH:mm:ss}";
    }

    // ---- Profile management ----
    private void AddProfile()
    {
        var vm = new DensityProfileViewModel();
        Profiles.Add(vm);
        SelectedProfile = vm;
    }

    private void DeleteProfile()
    {
        if (SelectedProfile is null) return;
        int idx = Profiles.IndexOf(SelectedProfile);
        Profiles.Remove(SelectedProfile);
        SelectedProfile = Profiles.Count == 0 ? null : Profiles[Math.Min(idx, Profiles.Count - 1)];
    }

    private void ComputePpiFromResolution()
    {
        if (SelectedProfile is null) return;
        SelectedProfile.Ppi = Math.Round(DensityProfile.FromResolution(ResWidth, ResHeight, DiagonalInches), 1);
        StatusText = $"Computed {SelectedProfile.Ppi:0.0} PPI for {ResWidth}×{ResHeight} @ {DiagonalInches}\"";
    }

    private void DetectMonitors()
    {
        RefreshMonitors();

        // Create a starter profile for any monitor that doesn't have one yet.
        foreach (var m in DisplayInfo.GetAllMonitors())
        {
            bool exists = Profiles.Any(p => string.Equals(p.MonitorDeviceName, m.DeviceName, StringComparison.OrdinalIgnoreCase));
            if (exists) continue;

            Profiles.Add(new DensityProfileViewModel(new DensityProfile
            {
                Name = $"{m.Width}×{m.Height} ({m.DeviceName.TrimStart('\\', '.')})",
                Ppi = m.EffectiveDpi, // starting estimate — refine with the resolution helper
                MonitorDeviceName = m.DeviceName,
            }));
        }

        StatusText = $"Detected {AvailableMonitors.Count} monitor(s)";
    }

    private void RefreshMonitors()
    {
        AvailableMonitors.Clear();
        AvailableMonitors.Add(new MonitorOption("", "(not bound)"));
        foreach (var m in DisplayInfo.GetAllMonitors())
        {
            string primary = m.IsPrimary ? " · primary" : "";
            AvailableMonitors.Add(new MonitorOption(m.DeviceName, $"{m.DeviceName} — {m.Width}×{m.Height}{primary}"));
        }
    }

    // ---- Analog preview ----
    private int _pvUp, _pvDown, _pvLeft, _pvRight;
    private void UpdatePreviewKeys()
    {
        _pvUp = (int)KeyUp; _pvDown = (int)KeyDown; _pvLeft = (int)KeyLeft; _pvRight = (int)KeyRight;
    }

    public void PollPreview()
    {
        if (!_sdkReady) return;

        float up = SafeRead(_pvUp), down = SafeRead(_pvDown), left = SafeRead(_pvLeft), right = SafeRead(_pvRight);
        float dz = (float)Deadzone, exp = CurvePower;

        PreviewX = ShapeAxis(right, left, dz, exp);
        PreviewY = ShapeAxis(down, up, dz, exp);
        PreviewDeadzone = Deadzone;
    }

    private static float SafeRead(int hidCode)
    {
        try { return WootingSdk.wooting_analog_read_analog(hidCode); }
        catch { return 0f; }
    }

    private static float ShapeAxis(float positive, float negative, float deadzone, float exponent)
    {
        float value = positive - negative;
        if (Math.Abs(value) < deadzone) return 0f;
        float sign = Math.Sign(value);
        float mag = (Math.Abs(value) - deadzone) / (1f - deadzone);
        mag = Math.Clamp(mag, 0f, 1f);
        mag = (float)Math.Pow(mag, exponent);
        return sign * mag;
    }

    private static KeyCodes ToKey(int configured, int fallback) => (KeyCodes)(configured != 0 ? configured : fallback);

    private static void CommandManagerInvalidate() => System.Windows.Input.CommandManager.InvalidateRequerySuggested();
}
