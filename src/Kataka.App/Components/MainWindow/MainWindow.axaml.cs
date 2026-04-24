using Avalonia.Controls;

using Kataka.App.ViewModels;

namespace Kataka.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Opened += (_, _) =>
        {
            if (DataContext is MainWindowViewModel vm)
                vm.StorageProvider = StorageProvider;
        };
        Closing += (_, _) => (DataContext as MainWindowViewModel)?.Shutdown();
    }
}
