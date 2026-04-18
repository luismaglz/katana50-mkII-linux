using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>Large rotary knob — 174 × 234 px, no Scale.</summary>
public sealed class LargeImageRotaryKnob : RotaryKnobBase
{
    private const double ImgSize = 100;
    private const double PadX = 2;
    private const double TopPad = 15;
    private const double BottomPad = 8;
    private const double ValueSize = 21;

    protected override Size MeasureOverride(Size availableSize) => new(150, 200);

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        var bounds = Bounds.Deflate(PadX);
        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize, FontWeight.Bold, LabelBrush);

        var cx = bounds.Width / 2;
        var cy = labelText.Height + TopPad + (ImgSize / 2);

        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));
        context.DrawEllipse(KatanaPalette.KnobBgBrush, null, new Point(cx, cy), ImgSize / 1.9, ImgSize / 1.9);

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
