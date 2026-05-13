using Avalonia.Media;

using Kataka.App.ViewModels;

namespace Kataka.App.Components.ReverbPedal;

public static class ReverbPedalColors
{
    public static readonly IBrush Room = Solid("#1e1e1e");
    public static readonly IBrush Hall = Solid("#141828");
    public static readonly IBrush Plate = Solid("#1a1c20");
    public static readonly IBrush Spring = Solid("#0e2020");
    public static readonly IBrush Modulate = Solid("#1c1428");

    public static PedalColorScheme GetColorScheme(string? typeName) => typeName switch
    {
        "ROOM" => new(Room),
        "HALL" => new(Hall),
        "PLATE" => new(Plate),
        "SPRING" => new(Spring),
        "MODULATE" => new(Modulate),
        _ => new(Hall)
    };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
