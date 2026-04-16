using System;

using Avalonia;

using CommunityToolkit.Mvvm.DependencyInjection;

using Kataka.App.Services;
using Kataka.App.ViewModels;
using Kataka.Application.Katana;
using Kataka.Application.Midi;
using Kataka.Domain.KatanaState;
using Kataka.Infrastructure.Midi;

using Microsoft.Extensions.DependencyInjection;

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
        services.AddSingleton<IMidiTransport>(_ => DefaultMidiTransport.Create());
        services.AddSingleton<IKatanaSession, KatanaSession>();
        services.AddSingleton<IKatanaState, KatanaState>();
        services.AddSingleton<IAmpSyncService, AmpSyncService>();
        services.AddSingleton<MainWindowViewModel>();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if DEBUG
            .WithDeveloperTools()
#endif
            .WithInterFont()
            .LogToTrace();
}
