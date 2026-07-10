using TheWootMouse.Configuration;
using WootMouseSettingsWpf.Mvvm;

namespace WootMouseSettingsWpf.ViewModels;

/// <summary>Editable, bindable wrapper around a <see cref="DensityProfile"/>.</summary>
public sealed class DensityProfileViewModel : ObservableObject
{
    private string _name;
    private double _ppi;
    private string? _monitorDeviceName;

    public DensityProfileViewModel(DensityProfile model)
    {
        _name = model.Name;
        _ppi = model.Ppi;
        _monitorDeviceName = model.MonitorDeviceName;
    }

    public DensityProfileViewModel() : this(new DensityProfile { Name = "New Profile", Ppi = 96 }) { }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public double Ppi
    {
        get => _ppi;
        set { if (SetProperty(ref _ppi, value)) OnPropertyChanged(nameof(Summary)); }
    }

    /// <summary>Windows device name of the bound monitor, or null/empty for "not bound".</summary>
    public string? MonitorDeviceName
    {
        get => _monitorDeviceName;
        set { if (SetProperty(ref _monitorDeviceName, value)) OnPropertyChanged(nameof(Summary)); }
    }

    public string Summary =>
        string.IsNullOrEmpty(MonitorDeviceName)
            ? $"{Ppi:0} PPI"
            : $"{Ppi:0} PPI · {MonitorDeviceName}";

    public DensityProfile ToModel() => new()
    {
        Name = Name,
        Ppi = Ppi,
        MonitorDeviceName = string.IsNullOrWhiteSpace(MonitorDeviceName) ? null : MonitorDeviceName,
    };
}
