using System;

using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// Rotary knob where grip notches are part of the rotating face.
/// 12 evenly-spaced notches are carved around the rim; one notch at the 12 o'clock
/// position is a different color — it acts as the position indicator and sweeps with
/// the knob. Static reference ticks on the outside give a scale to read against.
/// Scale-aware, same footprint as RotaryKnob.
/// </summary>
public sealed class RotaryKnobV3 : RotaryKnobBase
{
    private const int GripCount = 12;
    private const double StartDeg = 135.0;
    private const double SweepDeg = 270.0;

    private static readonly SolidColorBrush FaceBrush = KatanaPalette.KnobBgBrush;
    private static readonly SolidColorBrush BezelBrush = KatanaPalette.BgBaseBrush;
    private static readonly SolidColorBrush RefTickBrush = new(KatanaPalette.BorderLight);
    private static readonly SolidColorBrush RefTickCenterBrush = KatanaPalette.PrimaryLitBrush;
    private static readonly SolidColorBrush GripBrush = new(Color.FromArgb(100, 0, 0, 0));
    private static readonly SolidColorBrush IndicatorBrush = KatanaPalette.PrimaryLitBrush;

    protected override Size MeasureOverride(Size availableSize)
    {
        var s = Scale;
        return new Size(116 * s, 156 * s);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var s = Scale;
        var bounds = Bounds.Deflate(4 * s);
        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize > 0 ? LabelFontSize : 12 * s, FontWeight.Bold, LabelBrush);
        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        var diameter = Math.Min(bounds.Width - 12 * s, bounds.Height - labelText.Height - 56 * s);
        var r = diameter / 2;
        var center = new Point(bounds.Width / 2, labelText.Height + 20 * s + r);

        /// <summary> Static reference ticks (outside the knob) ────────────────────────── </summary>
        DrawRefTicks(context, center, r, s);

        /// <summary> Static bezel ring ───────────────────────────────────────────────── </summary>
        context.DrawEllipse(BezelBrush,
            new Pen(new SolidColorBrush(KatanaPalette.BgBase), 2 * s),
            center, r + 8 * s, r + 8 * s);

        /// <summary> Rotating knob face + grip notches ───────────────────────────────── </summary>
        var rotationRad = DegreesToRadians(-135.0 + NormalizedValue * SweepDeg);
        using (context.PushTransform(
            Matrix.CreateTranslation(-center.X, -center.Y) *
            Matrix.CreateRotation(rotationRad) *
            Matrix.CreateTranslation(center.X, center.Y)))
        {
            // Face
            context.DrawEllipse(FaceBrush,
                new Pen(new SolidColorBrush(KatanaPalette.BorderLight), 1.5 * s),
                center, r, r);

            // Grip notches evenly around the rim
            for (var i = 0; i < GripCount; i++)
            {
                var angleDeg = i * 360.0 / GripCount;
                var angleRad = DegreesToRadians(angleDeg);
                var sin = Math.Sin(angleRad);
                var cos = Math.Cos(angleRad);

                // Indicator notch: at 270° (top = 12 o'clock in unrotated space)
                var isIndicator = angleDeg == 270.0;

                var outer = r - 1.5 * s;
                var inner = isIndicator ? r - 14 * s : r - 8 * s;
                var thickness = isIndicator ? 2.5 * s : 1.5 * s;
                var brush = isIndicator ? IndicatorBrush : GripBrush;

                context.DrawLine(
                    new Pen(brush, thickness, lineCap: PenLineCap.Round),
                    new Point(center.X + cos * inner, center.Y + sin * inner),
                    new Point(center.X + cos * outer, center.Y + sin * outer));
            }
        }

        /// <summary> Value text (static) ──────────────────────────────────────────────── </summary>
        var valueText = CreateText(DisplayValueText, 14 * s, FontWeight.SemiBold, ValueBrush);
        context.DrawText(valueText, new Point(
            (bounds.Width - valueText.Width) / 2,
            center.Y + r + 14 * s));
    }

    private void DrawRefTicks(DrawingContext context, Point center, double r, double s)
    {
        for (var i = 0; i <= 10; i++)
        {
            var isCenterTick = IsEffectivelyBipolar && i == 5;
            var angleDeg = StartDeg + i * (SweepDeg / 10);
            var angleRad = DegreesToRadians(angleDeg);
            var cos = Math.Cos(angleRad);
            var sin = Math.Sin(angleRad);

            var outer = r + 12 * s;
            var inner = r + 5 * s;
            var pen = isCenterTick
                ? new Pen(RefTickCenterBrush, 2.5 * s, lineCap: PenLineCap.Round)
                : new Pen(RefTickBrush, 1.5 * s, lineCap: PenLineCap.Round);

            context.DrawLine(pen,
                new Point(center.X + cos * inner, center.Y + sin * inner),
                new Point(center.X + cos * outer, center.Y + sin * outer));
        }
    }
}
