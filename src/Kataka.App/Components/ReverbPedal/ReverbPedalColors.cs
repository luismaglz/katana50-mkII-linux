using Avalonia.Media;

namespace Kataka.App.Components.ReverbPedal;

public static class ReverbPedalColors
{
    public static readonly IBrush Room     = Solid("#1e1e1e");
    public static readonly IBrush Hall     = Solid("#141828");
    public static readonly IBrush Plate    = Solid("#1a1c20");
    public static readonly IBrush Spring   = Solid("#0e2020");
    public static readonly IBrush Modulate = Solid("#1c1428");

    public static readonly IBrush RoomLabel     = Solid("#aaaaaa");
    public static readonly IBrush HallLabel     = Solid("#8090c0");
    public static readonly IBrush PlateLabel    = Solid("#a0a8b8");
    public static readonly IBrush SpringLabel   = Solid("#70b8b0");
    public static readonly IBrush ModulateLabel = Solid("#a080c8");

    public static IBrush GetBackgroundBrush(string? typeName) => typeName switch
    {
        "ROOM"     => Room,
        "HALL"     => Hall,
        "PLATE"    => Plate,
        "SPRING"   => Spring,
        "MODULATE" => Modulate,
        _          => Hall,
    };

    public static IBrush GetLabelBrush(string? typeName) => typeName switch
    {
        "ROOM"     => RoomLabel,
        "HALL"     => HallLabel,
        "PLATE"    => PlateLabel,
        "SPRING"   => SpringLabel,
        "MODULATE" => ModulateLabel,
        _          => HallLabel,
    };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
