using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Kataka.App.Controls;

/// <summary>Shared bitmap loader and sweep-angle constants for image-based knobs.</summary>
internal static class KnobImageAsset
{
    private const double MinAngleDeg = -135.0;
    private const double MaxAngleDeg = 135.0;
    internal static readonly double AngleSweep = MaxAngleDeg - MinAngleDeg;
    internal static readonly double MinAngleRad = MinAngleDeg * Math.PI / 180.0;

    internal static readonly Lazy<Bitmap?> Bitmap = new(() =>
    {
        try
        {
            var uri = new Uri("avares://Kataka.App/Assets/knob.png");
            using var stream = AssetLoader.Open(uri);
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    });
}
