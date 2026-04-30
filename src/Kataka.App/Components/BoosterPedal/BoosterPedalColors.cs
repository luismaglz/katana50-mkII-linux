using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

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
    public static readonly IBrush Overdrive = Gradient("#1e3222", "#111e14");
    public static readonly IBrush Distortion = Gradient("#342216", "#20140c");
    public static readonly IBrush Fuzz = Gradient("#201e2c", "#141218");


    public static readonly IBrush Boost = Solid("#cdcfce");
    public static readonly IBrush BluesDriver = Solid("#11578c");
    public static readonly IBrush CentaOD = Solid("#e3b25c");
    public static readonly IBrush DSTP = Solid("#fddc43");
    public static readonly IBrush FatDS = Solid("#2a2a2a");
    public static readonly IBrush GuvDS = Solid("#0d0d0d");
    public static readonly IBrush HM2 = Solid("#181c1f");

    public static readonly IBrush MetalCore = CreateOrganicNoise(500, 800);

    public static IBrush GetBackgroundBrush(string? typeName)
    {
        return typeName switch
        {
            "MID BOOST" or "CLEAN BOOST" or "TREBLE BOOST" => Boost,
            "CRUNCH OD" or "NATURAL OD" or "WARM OD" or "OVERDRIVE" or "T-SCREAM" or "TURBO OD" => Overdrive,
            "BLUES DRIVE" => BluesDriver,
            "CENTA OD" => CentaOD,
            "FAT DS" => FatDS,
            "DISTORTION" or "RAT" or "GUV DS" => GuvDS,
            "DST+" => DSTP,
            "METAL DS" or "OCT FUZZ" or "METAL ZONE" or "'60S FUZZ" or "MUFF FUZZ" or "HM-2" => HM2,
            "METAL CORE" => MetalCore,
            _ => Overdrive
        };
    }

    private static IBrush Gradient(string top, string bottom)
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
            GradientStops = { new GradientStop(Color.Parse(top), 0), new GradientStop(Color.Parse(bottom), 1) }
        };
    }

    private static IBrush Solid(string hex)
    {
        return new SolidColorBrush(Color.Parse(hex));
    }

    public static IBrush CreateOrganicNoise(int width, int height)
    {
        var bitmap = new WriteableBitmap(
            new PixelSize(width, height),
            new Vector(96, 96),
            PixelFormat.Rgba8888,
            AlphaFormat.Premul);

        using (var frame = bitmap.Lock())
        {
            unsafe
            {
                var ptr = (uint*)frame.Address;
                Random random = new();

                // Background color components (#181b20)
                // Note: WriteableBitmap usually expects BGRA or RGBA depending on platform
                // This hex is 0xAARRGGBB
                var baseColor = 0xFF181B20;

                for (var i = 0; i < width * height; i++)
                {
                    var roll = random.NextDouble();

                    if (roll > 0.98) // 2% chance for a speckle
                    {
                        // Generate a range for the silver speckle (160 to 220 for variation)
                        var intensity = (byte)random.Next(160, 221);

                        // Construct the pixel (Alpha: FF, R: intensity, G: intensity, B: intensity)
                        ptr[i] = (uint)((0xFF << 24) | (intensity << 16) | (intensity << 8) | intensity);
                    }
                    else
                    {
                        ptr[i] = baseColor;
                    }
                }
            }
        }

        return new ImageBrush(bitmap)
        {
            TileMode = TileMode.None,
            Stretch = Stretch.Fill
        };
    }
}
