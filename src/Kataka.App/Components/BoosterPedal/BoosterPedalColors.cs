using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Components.BoosterPedal;

/// <summary>
/// Background color palette for the booster pedal card, keyed by effect category.
///   White   = Boost/Tuner (MID BOOST, CLEAN BOOST, TREBLE BOOST)
///   Green   = Overdrive / Tube-Screamer style
///   Blue    = Blues / Transparent OD
///   Orange  = Distortion
///   Charcoal = Fuzz / Heavy
/// </summary>
public static class BoosterPedalColors
{
    public static readonly IBrush Boost      = Gradient("#302e2a", "#1e1c18");
    public static readonly IBrush Overdrive  = Gradient("#1e3222", "#111e14");
    public static readonly IBrush Blues      = Gradient("#1a2c3e", "#111c28");
    public static readonly IBrush Distortion = Gradient("#342216", "#20140c");
    public static readonly IBrush Fuzz       = Gradient("#201e2c", "#141218");

    public static readonly IBrush BoostLabel      = new SolidColorBrush(Color.Parse("#c8c4bc"));
    public static readonly IBrush OverdriveLabel  = new SolidColorBrush(Color.Parse("#7ab88a"));
    public static readonly IBrush BluesLabel      = new SolidColorBrush(Color.Parse("#7aa8c4"));
    public static readonly IBrush DistortionLabel = new SolidColorBrush(Color.Parse("#c49070"));
    public static readonly IBrush FuzzLabel       = new SolidColorBrush(Color.Parse("#a09ab4"));

    public static IBrush GetBackgroundBrush(string? typeName) => typeName switch
    {
        "MID BOOST" or "CLEAN BOOST" or "TREBLE BOOST"                                                    => Boost,
        "CRUNCH OD" or "NATURAL OD" or "WARM OD" or "OVERDRIVE" or "T-SCREAM" or "TURBO OD"              => Overdrive,
        "BLUES DRIVE" or "CENTA OD"                                                                        => Blues,
        "FAT DS" or "DISTORTION" or "RAT" or "GUV DS" or "DST+"                                          => Distortion,
        "METAL DS" or "OCT FUZZ" or "METAL ZONE" or "'60S FUZZ" or "MUFF FUZZ" or "HM-2" or "METAL CORE" => Fuzz,
        _ => Overdrive,
    };

    public static IBrush GetLabelBrush(string? typeName) => typeName switch
    {
        "MID BOOST" or "CLEAN BOOST" or "TREBLE BOOST"                                                    => BoostLabel,
        "CRUNCH OD" or "NATURAL OD" or "WARM OD" or "OVERDRIVE" or "T-SCREAM" or "TURBO OD"              => OverdriveLabel,
        "BLUES DRIVE" or "CENTA OD"                                                                        => BluesLabel,
        "FAT DS" or "DISTORTION" or "RAT" or "GUV DS" or "DST+"                                          => DistortionLabel,
        "METAL DS" or "OCT FUZZ" or "METAL ZONE" or "'60S FUZZ" or "MUFF FUZZ" or "HM-2" or "METAL CORE" => FuzzLabel,
        _ => OverdriveLabel,
    };

    private static IBrush Gradient(string top, string bottom) =>
        new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(Color.Parse(top), 0),
                new GradientStop(Color.Parse(bottom), 1),
            }
        };
}
