using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>Small rotary knob — 80 × 115 px, no Scale.</summary>
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
            KatanaPalette.KnobBgBrush);

    private static readonly SolidColorBrush KnobShadowBrush = KatanaPalette.KnobShadowBrush;

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
        var cy = labelText.Height + TopPad + (ImgSize / 2);
        var imgRect = new Rect(cx - (ImgSize / 2), cy - (ImgSize / 2), ImgSize, ImgSize);

        var angleRad = KnobImageAsset.MinAngleRad + (NormalizedValue * KnobImageAsset.AngleSweep * Math.PI / 180.0);

        using (context.PushTransform(
                   Matrix.CreateTranslation(-cx, -cy) *
                   Matrix.CreateRotation(angleRad) *
                   Matrix.CreateTranslation(cx, cy)))
        {
            var bmp = KnobImageAsset.Bitmap.Value;
            if (bmp is not null)
                context.DrawImage(bmp, imgRect);
            else
                context.DrawEllipse(KatanaPalette.KnobFaceBrush, KatanaPalette.KnobRimPen,
                    new Point(cx, cy), ImgSize / 2, ImgSize / 2);
        }

        var valueText = CreateText(DisplayValueText, ValueSize, FontWeight.SemiBold, ValueBrush);
        context.DrawText(valueText, new Point((bounds.Width - valueText.Width) / 2, cy + (ImgSize / 2) + BottomPad));
    }
}
