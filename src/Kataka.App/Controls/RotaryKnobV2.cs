using System;

using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// Arc-fill rotary knob (Scale-aware). A dim full-sweep track ring surrounds the knob
/// face; a bright amber arc fills from minimum up to the current value. This gives an
/// at-a-glance level readout without needing to read the pointer angle.
/// </summary>
public sealed class RotaryKnobV2 : RotaryKnobBase
{
    private const double BaseWidth = 116.0;
    private const double BaseHeight = 156.0;
    private const double BaseKnobR = 34.0;
    private const double BaseArcR = 46.0;
    private const double BaseArcThick = 4.0;
    private const double BasePad = 4.0;
    private const double BaseTopPad = 20.0;
    private const double BaseBottomPad = 8.0;
    private const double BaseValueFontSize = 14.0;
    private const double StartDeg = 135.0;
    private const double SweepDeg = 270.0;

    private static readonly SolidColorBrush FaceBrush = KatanaPalette.KnobBgBrush;
    private static readonly SolidColorBrush BezelBrush = KatanaPalette.BgBaseBrush;
    private static readonly SolidColorBrush ArcTrackBrush = new(Color.FromArgb(55, 255, 255, 255));

    protected override Size MeasureOverride(Size availableSize)
    {
        var s = Scale;
        return new Size(BaseWidth * s, BaseHeight * s);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var s = Scale;
        var pad = BasePad * s;
        var knobR = BaseKnobR * s;
        var arcR = BaseArcR * s;
        var arcThick = BaseArcThick * s;
        var topPad = BaseTopPad * s;
        var bottomPad = BaseBottomPad * s;
        var valueFontSize = BaseValueFontSize * s;
        var bounds = Bounds.Deflate(pad);

        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize * s, FontWeight.Bold, LabelBrush);
        var cx = bounds.Width / 2;
        var cy = labelText.Height + topPad + arcR;

        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        // Full-sweep dim arc track
        DrawArcStroke(context, cx, cy, arcR, StartDeg, StartDeg + SweepDeg,
            new Pen(ArcTrackBrush, arcThick, lineCap: PenLineCap.Round));

        // Filled arc up to current value
        if (NormalizedValue > 0.005)
        {
            var endDeg = StartDeg + NormalizedValue * SweepDeg;
            var c = KatanaPalette.PrimaryLit;
            DrawArcStroke(context, cx, cy, arcR, StartDeg, endDeg,
                new Pen(new SolidColorBrush(Color.FromArgb(45, c.R, c.G, c.B)),
                    arcThick + 4 * s, lineCap: PenLineCap.Round));
            DrawArcStroke(context, cx, cy, arcR, StartDeg, endDeg,
                new Pen(KatanaPalette.PrimaryLitBrush, arcThick, lineCap: PenLineCap.Round));
        }

        // Bezel + face
        context.DrawEllipse(BezelBrush, null, new Point(cx, cy), knobR + 4 * s, knobR + 4 * s);
        context.DrawEllipse(FaceBrush,
            new Pen(new SolidColorBrush(KatanaPalette.BorderLight), 1.5 * s),
            new Point(cx, cy), knobR, knobR);

        // Pointer
        var angleRad = DegreesToRadians(StartDeg + NormalizedValue * SweepDeg);
        var pointerLen = knobR * 0.68;
        var tip = new Point(cx + Math.Cos(angleRad) * pointerLen, cy + Math.Sin(angleRad) * pointerLen);
        context.DrawLine(new Pen(ValueBrush, 3.0 * s, lineCap: PenLineCap.Round), new Point(cx, cy), tip);
        context.DrawEllipse(ValueBrush, null, new Point(cx, cy), 3.5 * s, 3.5 * s);

        var valueText = CreateText(DisplayValueText, valueFontSize, FontWeight.SemiBold, ValueBrush);
        context.DrawText(valueText, new Point(
            (bounds.Width - valueText.Width) / 2,
            cy + arcR + bottomPad));
    }

    private static void DrawArcStroke(DrawingContext ctx, double cx, double cy, double r,
        double startDeg, double endDeg, IPen pen)
    {
        var sweep = Math.Min(Math.Abs(endDeg - startDeg), 359.9);
        if (sweep < 0.5) return;

        var startRad = DegreesToRadians(startDeg);
        var endRad = DegreesToRadians(startDeg + sweep);
        var startPt = new Point(cx + r * Math.Cos(startRad), cy + r * Math.Sin(startRad));
        var endPt = new Point(cx + r * Math.Cos(endRad), cy + r * Math.Sin(endRad));

        var geo = new StreamGeometry();
        using (var gctx = geo.Open())
        {
            gctx.BeginFigure(startPt, false);
            gctx.ArcTo(endPt, new Size(r, r), 0, sweep > 180, SweepDirection.Clockwise);
        }
        ctx.DrawGeometry(null, pen, geo);
    }
}
