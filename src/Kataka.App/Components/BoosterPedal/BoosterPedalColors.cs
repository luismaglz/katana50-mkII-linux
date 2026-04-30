using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Components.BoosterPedal;

/// <summary>
///     Background color palette for the booster pedal card, keyed by effect category.
///     White   = Boost/Tuner (MID BOOST, CLEAN BOOST, TREBLE BOOST)
///     Green   = Overdrive / Tube-Screamer style
///     Blue    = Blues / Transparent OD
///     Orange  = Distortion
///     Charcoal = Fuzz / Heavy
/// </summary>
public static class BoosterPedalColors
{
    public static readonly IBrush Boost = Gradient("#302e2a", "#1e1c18");
    public static readonly IBrush Overdrive = Gradient("#1e3222", "#111e14");
    public static readonly IBrush Distortion = Gradient("#342216", "#20140c");
    public static readonly IBrush Fuzz = Gradient("#201e2c", "#141218");

    public static readonly IBrush BluesDriver = Solid("#11578c");
    public static readonly IBrush CentaOD = Solid("#e3b25c");

    public static IBrush GetBackgroundBrush(string? typeName) => typeName switch
    {
        "MID BOOST" or "CLEAN BOOST" or "TREBLE BOOST" => Boost,
        "CRUNCH OD" or "NATURAL OD" or "WARM OD" or "OVERDRIVE" or "T-SCREAM" or "TURBO OD" => Overdrive,
        "BLUES DRIVE" => BluesDriver,
        "CENTA OD" => CentaOD,
        "FAT DS" or "DISTORTION" or "RAT" or "GUV DS" or "DST+" => Distortion,
        "METAL DS" or "OCT FUZZ" or "METAL ZONE" or "'60S FUZZ" or "MUFF FUZZ" or "HM-2" or "METAL CORE" => Fuzz,
        _ => Overdrive
    };

    private static IBrush Gradient(string top, string bottom) =>
        new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops = { new GradientStop(Color.Parse(top), 0), new GradientStop(Color.Parse(bottom), 1) }
        };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
