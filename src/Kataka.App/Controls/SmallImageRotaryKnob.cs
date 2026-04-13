using System;
using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// Small image rotary knob — 76 px image, 116 × 156 px control. No scaling.
/// </summary>
public sealed class SmallImageRotaryKnob : RotaryKnobBase
{
    private const double ImgSize = 64;
    private const double PadX = 4;
    private const double TopPad = 10;
    private const double BottomPad = 6;
    private const double ValueSize = 14;

    public static readonly StyledProperty<IBrush> KnobBackgroundBrushProperty =
        AvaloniaProperty.Register<SmallImageRotaryKnob, IBrush>(
            nameof(KnobBackgroundBrush),
            new SolidColorBrush(Color.Parse("#D9D3C1")));

    private static readonly SolidColorBrush KnobShadowBrush = new(Color.Parse("#28000000"));

    static SmallImageRotaryKnob()
    {
        AffectsRender<SmallImageRotaryKnob>(KnobBackgroundBrushProperty);
    }

    public IBrush KnobBackgroundBrush
    {
        get => GetValue(KnobBackgroundBrushProperty);
        set => SetValue(KnobBackgroundBrushProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize) => new(80, 115);

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var bounds = Bounds.Deflate(PadX);

        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize, FontWeight.Bold, LabelBrush);
        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        var cx = bounds.Width / 2;
        var cy = labelText.Height + TopPad + ImgSize / 2;
        var imgRect = new Rect(cx - ImgSize / 2, cy - ImgSize / 2, ImgSize, ImgSize);

        // Subtle drop shadow behind the knob background.
        context.DrawEllipse(KnobShadowBrush, null, new Point(cx + 1.5, cy + 2), ImgSize / 2, ImgSize / 2);

        // Fill behind transparent PNG pixels.
        context.DrawEllipse(KnobBackgroundBrush, null, new Point(cx, cy), ImgSize / 2, ImgSize / 2);

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