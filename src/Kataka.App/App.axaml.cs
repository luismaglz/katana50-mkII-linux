using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

using CommunityToolkit.Mvvm.DependencyInjection;

using Kataka.App.ViewModels;
using Kataka.App.Views;

namespace Kataka.App;

public partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var vm = Ioc.Default.GetRequiredService<MainWindowViewModel>();
            var window = new MainWindow { DataContext = vm };
            window.Closing += (_, _) => vm.Shutdown();
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
