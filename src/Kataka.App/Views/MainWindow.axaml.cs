using Avalonia.Controls;

namespace Kataka.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) =>
        {
            if (DataContext is ViewModels.MainWindowViewModel vm)
                vm.StorageProvider = StorageProvider;
        };
        Closing += (_, _) => (DataContext as ViewModels.MainWindowViewModel)?.Shutdown();
    }
}
