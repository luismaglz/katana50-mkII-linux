using Avalonia.Controls;

namespace Kataka.App.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closing += (_, _) => (DataContext as ViewModels.MainWindowViewModel)?.Shutdown();
    }
}