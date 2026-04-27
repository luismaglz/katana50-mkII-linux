using Avalonia.Media;

namespace Kataka.App.Components.ReverbPedal;

public static class ReverbPedalColors
{
    public static readonly IBrush Room = Solid("#1e1e1e");
    public static readonly IBrush Hall = Solid("#141828");
    public static readonly IBrush Plate = Solid("#1a1c20");
    public static readonly IBrush Spring = Solid("#0e2020");
    public static readonly IBrush Modulate = Solid("#1c1428");

    public static IBrush GetBackgroundBrush(string? typeName) => typeName switch
    {
        "ROOM" => Room,
        "HALL" => Hall,
        "PLATE" => Plate,
        "SPRING" => Spring,
        "MODULATE" => Modulate,
        _ => Hall
    };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
