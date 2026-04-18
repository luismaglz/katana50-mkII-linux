using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Svg.Skia;

namespace Kataka.App.Controls;

internal static class KnobImageAsset
{
    private const double MinAngleDeg = -135.0;
    private const double MaxAngleDeg = 135.0;
    internal static readonly double AngleSweep = MaxAngleDeg - MinAngleDeg;
    internal static readonly double MinAngleRad = MinAngleDeg * Math.PI / 180.0;

    internal static readonly Lazy<IImage?> Bitmap = new(() =>
    {
        try
        {
            var primary = ToHex(KatanaPalette.Primary);
            var indicator = ToHex(KatanaPalette.PrimaryLit);

            using var stream = AssetLoader.Open(new Uri("avares://Kataka.App/Assets/knob.svg"));
            using var reader = new System.IO.StreamReader(stream);
            var svg = reader.ReadToEnd()
                .Replace("{{COLOR_PRIMARY}}", primary)
                .Replace("{{COLOR_INDICATOR}}", indicator);

            var source = SvgSource.LoadFromSvg(svg);
            return new SvgImage { Source = source };
        }
        catch
        {
            return null;
        }
    });

    private static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
