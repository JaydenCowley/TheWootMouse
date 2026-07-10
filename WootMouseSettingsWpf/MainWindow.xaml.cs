using System.Windows;
using System.Windows.Threading;
using WootMouseSettingsWpf.ViewModels;

namespace WootMouseSettingsWpf;

public partial class MainWindow : Window
{
    private readonly MainViewModel _vm = new();
    private readonly DispatcherTimer _pollTimer;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;

        _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) }; // ~60 Hz
        _pollTimer.Tick += (_, _) => _vm.PollPreview();
        _pollTimer.Start();

        Closed += (_, _) => _pollTimer.Stop();
    }
}
