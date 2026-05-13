using Avalonia.Media;

using Kataka.App.ViewModels;

namespace Kataka.App.Components.DelayPedal;

public static class DelayPedalColors
{
    public static readonly IBrush Digital = Solid("#162030");
    public static readonly IBrush Pan = Solid("#0e2028");
    public static readonly IBrush Reverse = Solid("#1e1428");
    public static readonly IBrush Analog = Solid("#2a1e0e");
    public static readonly IBrush TapeEcho = Solid("#281808");
    public static readonly IBrush Modulate = Solid("#0e2418");
    public static readonly IBrush Sde3000 = Solid("#181e2a");

    public static PedalColorScheme GetColorScheme(string? typeName) => typeName switch
    {
        "DIGITAL" or "STEREO" => new(Digital),
        "PAN" => new(Pan),
        "REVERSE" => new(Reverse),
        "ANALOG" => new(Analog),
        "TAPE ECHO" => new(TapeEcho),
        "MODULATE" => new(Modulate),
        "SDE-3000" => new(Sde3000),
        _ => new(Digital)
    };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
