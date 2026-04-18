using Avalonia.Media;
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
            var source = SvgSource.Load("avares://Kataka.App/Assets/knob.svg", null);
            return new SvgImage { Source = source };
        }
        catch
        {
            return null;
        }
    });
}
