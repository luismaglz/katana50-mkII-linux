using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Kataka.App.Controls;

/// <summary>Shared bitmap loader and sweep-angle constants for image-based knobs.</summary>
internal static class KnobImageAsset
{
    private const double MinAngleDeg = -135.0;
    private const double MaxAngleDeg =  135.0;
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
        catch { return null; }
    });
}

/// <summary>
/// Large image rotary knob — 114 px image, 174 × 234 px control. No scaling.
/// </summary>
public sealed class LargeImageRotaryKnob : RotaryKnobBase
{
    private const double ImgSize    = 114;
    private const double PadX       = 6;
    private const double TopPad     = 30;
    private const double BottomPad  = 8;
    private const double ValueSize  = 21;

    protected override Size MeasureOverride(Size availableSize) => new(174, 234);

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var bounds = Bounds.Deflate(PadX);

        var labelText   = CreateText(Label.ToUpperInvariant(), LabelFontSize, FontWeight.Bold, LabelBrush);
        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        var cx     = bounds.Width  / 2;
        var cy     = labelText.Height + TopPad + ImgSize / 2;
        var imgRect = new Rect(cx - ImgSize / 2, cy - ImgSize / 2, ImgSize, ImgSize);

        var angleRad = KnobImageAsset.MinAngleRad + NormalizedValue * KnobImageAsset.AngleSweep * Math.PI / 180.0;

        using (context.PushTransform(
            Matrix.CreateTranslation(-cx, -cy) *
            Matrix.CreateRotation(angleRad) *
            Matrix.CreateTranslation(cx, cy)))
        {
            var bmp = KnobImageAsset.Bitmap.Value;
            if (bmp is not null)
                context.DrawImage(bmp, imgRect);
            else
                context.DrawEllipse(
                    new SolidColorBrush(Color.Parse("#2c2f35")),
                    new Pen(new SolidColorBrush(Color.Parse("#565b64")), 1.5),
                    new Point(cx, cy), ImgSize / 2, ImgSize / 2);
        }

        var valueText = CreateText(DisplayValueText, ValueSize, FontWeight.SemiBold, ValueBrush);
        context.DrawText(valueText, new Point((bounds.Width - valueText.Width) / 2, cy + ImgSize / 2 + BottomPad));
    }
}

/// <summary>
/// Small image rotary knob — 76 px image, 116 × 156 px control. No scaling.
/// </summary>
public sealed class SmallImageRotaryKnob : RotaryKnobBase
{
    private const double ImgSize    = 76;
    private const double PadX       = 4;
    private const double TopPad     = 20;
    private const double BottomPad  = 6;
    private const double ValueSize  = 14;

    protected override Size MeasureOverride(Size availableSize) => new(116, 156);

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var bounds = Bounds.Deflate(PadX);

        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize, FontWeight.Bold, LabelBrush);
        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        var cx      = bounds.Width  / 2;
        var cy      = labelText.Height + TopPad + ImgSize / 2;
        var imgRect = new Rect(cx - ImgSize / 2, cy - ImgSize / 2, ImgSize, ImgSize);

        var angleRad = KnobImageAsset.MinAngleRad + NormalizedValue * KnobImageAsset.AngleSweep * Math.PI / 180.0;

        using (context.PushTransform(
            Matrix.CreateTranslation(-cx, -cy) *
            Matrix.CreateRotation(angleRad) *
            Matrix.CreateTranslation(cx, cy)))
        {
            var bmp = KnobImageAsset.Bitmap.Value;
            if (bmp is not null)
                context.DrawImage(bmp, imgRect);
            else
                context.DrawEllipse(
                    new SolidColorBrush(Color.Parse("#2c2f35")),
                    new Pen(new SolidColorBrush(Color.Parse("#565b64")), 1.5),
                    new Point(cx, cy), ImgSize / 2, ImgSize / 2);
        }

        var valueText = CreateText(DisplayValueText, ValueSize, FontWeight.SemiBold, ValueBrush);
        context.DrawText(valueText, new Point((bounds.Width - valueText.Width) / 2, cy + ImgSize / 2 + BottomPad));
    }
}
