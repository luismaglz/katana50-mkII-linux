using Avalonia;
using System;
using Kataka.App.Services;
using Microsoft.Extensions.DependencyInjection;
using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Application.Midi;
using Kataka.Infrastructure.Midi;

namespace Kataka.App;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        BuildAvaloniaApp(provider).StartWithClassicDesktopLifetime(args);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IMidiTransport>(_ => DefaultMidiTransport.Create());
        services.AddSingleton<IKatanaSession, KatanaSession>();
        services.AddSingleton<IAmpStateService, AmpStateService>();
        services.AddSingleton<MainWindowViewModel>();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp(IServiceProvider? services = null)
        => AppBuilder.Configure(() => new App(services))
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
