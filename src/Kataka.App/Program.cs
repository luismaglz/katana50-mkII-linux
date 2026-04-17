using System;

using Avalonia;
using Avalonia.ReactiveUI;

using CommunityToolkit.Mvvm.DependencyInjection;

using Kataka.App.KatanaState;
using Kataka.App.Logging;
using Kataka.App.Services;
using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Application.Midi;
using Kataka.Infrastructure.Midi;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kataka.App;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        Ioc.Default.ConfigureServices(services.BuildServiceProvider());

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var loggerProvider = new ObservableLoggerProvider();
        services.AddSingleton(loggerProvider);
        services.AddLogging(b => b.AddProvider(loggerProvider).SetMinimumLevel(LogLevel.Debug));

        services.AddSingleton<IMidiTransport>(_ => DefaultMidiTransport.Create());
        services.AddSingleton<IKatanaSession, KatanaSession>();
        services.AddSingleton<IKatanaState, KatanaState.KatanaState>();
        services.AddSingleton<IAmpSyncService, AmpSyncService>();
        services.AddSingleton<MainWindowViewModel>();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .UseReactiveUI()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
