using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Kataka.App.Controls;

/// <summary>
/// Rotary knob that renders a PNG asset (<c>Assets/knob.png</c>) rotated to represent
/// the current value. Shares all interaction and property logic via <see cref="RotaryKnobBase"/>.
/// </summary>
public class ImageRotaryKnob : RotaryKnobBase
{
    // 270° sweep: −135° (min) → +135° (max), matching the drawn RotaryKnob.
    private const double MinAngleDeg = -135.0;
    private const double MaxAngleDeg =  135.0;

    private static readonly Lazy<Bitmap?> KnobImage = new(() =>
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

    // Override the default Scale so image knobs start at 1.5×.
    static ImageRotaryKnob()
    {
        ScaleProperty.OverrideDefaultValue<ImageRotaryKnob>(1.5);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var s = Scale;
        return new Size(116 * s, 156 * s);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var s      = Scale;
        var bounds = Bounds.Deflate(4 * s);

        var labelText   = CreateText(Label.ToUpperInvariant(), LabelFontSize * s, FontWeight.Bold, LabelBrush);
        var labelOrigin = new Point((bounds.Width - labelText.Width) / 2, bounds.Top);
        context.DrawText(labelText, labelOrigin);

        var imgSize = Math.Min(bounds.Width - 12 * s, bounds.Height - labelText.Height - 56 * s);
        var center  = new Point(bounds.Width / 2, labelText.Height + 20 * s + imgSize / 2);
        var imgRect = new Rect(center.X - imgSize / 2, center.Y - imgSize / 2, imgSize, imgSize);

        var angleDeg = MinAngleDeg + NormalizedValue * (MaxAngleDeg - MinAngleDeg);
        var angleRad = angleDeg * Math.PI / 180.0;

        using (context.PushTransform(
            Matrix.CreateTranslation(-center.X, -center.Y) *
            Matrix.CreateRotation(angleRad) *
            Matrix.CreateTranslation(center.X, center.Y)))
        {
            var bitmap = KnobImage.Value;
            if (bitmap is not null)
                context.DrawImage(bitmap, imgRect);
            else
                context.DrawEllipse(
                    new SolidColorBrush(Color.Parse("#2c2f35")),
                    new Pen(new SolidColorBrush(Color.Parse("#565b64")), 1.5 * s),
                    center, imgSize / 2, imgSize / 2);
        }

        var valueText   = CreateText(DisplayValueText, 14 * s, FontWeight.SemiBold, ValueBrush);
        var valueOrigin = new Point((bounds.Width - valueText.Width) / 2, center.Y + imgSize / 2 + 6 * s);
        context.DrawText(valueText, valueOrigin);
    }
}
