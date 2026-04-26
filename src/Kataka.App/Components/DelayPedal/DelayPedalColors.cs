using Avalonia.Media;

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

    public static readonly IBrush DigitalLabel = Solid("#70a8d0");
    public static readonly IBrush PanLabel = Solid("#70c0cc");
    public static readonly IBrush ReverseLabel = Solid("#a080c0");
    public static readonly IBrush AnalogLabel = Solid("#c09858");
    public static readonly IBrush TapeEchoLabel = Solid("#c08850");
    public static readonly IBrush ModulateLabel = Solid("#70c088");
    public static readonly IBrush Sde3000Label = Solid("#8898c0");

    public static IBrush GetBackgroundBrush(string? typeName) => typeName switch
    {
        "DIGITAL" or "STEREO" => Digital,
        "PAN" => Pan,
        "REVERSE" => Reverse,
        "ANALOG" => Analog,
        "TAPE ECHO" => TapeEcho,
        "MODULATE" => Modulate,
        "SDE-3000" => Sde3000,
        _ => Digital,
    };

    public static IBrush GetLabelBrush(string? typeName) => typeName switch
    {
        "DIGITAL" or "STEREO" => DigitalLabel,
        "PAN" => PanLabel,
        "REVERSE" => ReverseLabel,
        "ANALOG" => AnalogLabel,
        "TAPE ECHO" => TapeEchoLabel,
        "MODULATE" => ModulateLabel,
        "SDE-3000" => Sde3000Label,
        _ => DigitalLabel,
    };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
