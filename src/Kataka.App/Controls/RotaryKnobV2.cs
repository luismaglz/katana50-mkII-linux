using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
///     Arc-fill rotary knob (Scale-aware). A dim full-sweep track ring surrounds the knob
///     face; a bright amber arc fills from minimum up to the current value. This gives an
///     at-a-glance level readout without needing to read the pointer angle.
/// </summary>
public sealed class RotaryKnobV2 : RotaryKnobBase
{
    private const double BaseWidth = 116.0;
    private const double BaseHeight = 156.0;
    private const double BaseKnobR = 34.0;
    private const double BaseArcR = 46.0;
    private const double BaseArcThick = 4.0;
    private const double BasePad = 4.0;
    private const double BaseTopPad = 16.0;
    private const double BaseBottomPad = 6.0;
    private const double StartDeg = 135.0;
    private const double SweepDeg = 270.0;

    public static readonly StyledProperty<double> ValueFontSizeProperty =
        AvaloniaProperty.Register<RotaryKnobV2, double>(nameof(ValueFontSize), 14.0);

    static RotaryKnobV2()
    {
        AffectsRender<RotaryKnobV2>(ValueFontSizeProperty);
    }

    public double ValueFontSize
    {
        get => GetValue(ValueFontSizeProperty);
        set => SetValue(ValueFontSizeProperty, Math.Max(1.0, value));
    }

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
        var bounds = Bounds.Deflate(pad);

        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize * s, FontWeight.Bold, LabelBrush);
        var cx = bounds.Width / 2;
        var cy = labelText.Height + topPad + arcR;

        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        var accentColor = IsSet(AccentColorProperty) ? AccentColor : RuntimePaletteService.Accent;
        var trackColor = IsSet(TrackColorProperty)
            ? TrackColor
            : Color.FromArgb(55, accentColor.R, accentColor.G, accentColor.B);
        var faceColor = IsSet(FaceColorProperty) ? FaceColor : RuntimePaletteService.KnobFace;
        var bezelColor = IsSet(BezelColorProperty) ? BezelColor : RuntimePaletteService.BgBase;
        var accentBrush = new SolidColorBrush(accentColor);
        var valueBrush = IsSet(ValueBrushProperty) ? ValueBrush : accentBrush;

        // Full-sweep dim arc track
        DrawArcStroke(context, cx, cy, arcR, StartDeg, StartDeg + SweepDeg,
            new Pen(new SolidColorBrush(trackColor), arcThick, lineCap: PenLineCap.Round));

        // Filled arc up to current value
        if (NormalizedValue > 0.005)
        {
            var endDeg = StartDeg + (NormalizedValue * SweepDeg);
            DrawArcStroke(context, cx, cy, arcR, StartDeg, endDeg,
                new Pen(new SolidColorBrush(Color.FromArgb(45, accentColor.R, accentColor.G, accentColor.B)),
                    arcThick + (4 * s), lineCap: PenLineCap.Round));
            DrawArcStroke(context, cx, cy, arcR, StartDeg, endDeg,
                new Pen(accentBrush, arcThick, lineCap: PenLineCap.Round));
        }

        // Bezel + face TOP and TOP COLOR
        context.DrawEllipse(new SolidColorBrush(bezelColor), null, new Point(cx, cy), knobR + (4 * s), knobR + (4 * s));
        context.DrawEllipse(new SolidColorBrush(faceColor),
            new Pen(new SolidColorBrush(KatanaPalette.BorderLight), 1.5 * s),
            new Point(cx, cy), knobR, knobR);

        // Pointer
        var angleRad = DegreesToRadians(StartDeg + (NormalizedValue * SweepDeg));
        var indicatorEnd = knobR * 0.8;
        var indicatorStart = knobR * 0.4;
        var tipCx = cx + (Math.Cos(angleRad) * indicatorEnd);
        var tipCy = cy + (Math.Sin(angleRad) * indicatorEnd);
        var tip = new Point(tipCx, tipCy);
        var endTipCx = cx + (Math.Cos(angleRad) * indicatorStart);
        var endTipCy = cy + (Math.Sin(angleRad) * indicatorStart);
        var endTip = new Point(endTipCx, endTipCy);
        context.DrawLine(new Pen(accentBrush, 5.0 * s, lineCap: PenLineCap.Round), tip, endTip);

        var valueText = CreateText(DisplayValueText, ValueFontSize * s, FontWeight.Bold, valueBrush);
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
        var startPt = new Point(cx + (r * Math.Cos(startRad)), cy + (r * Math.Sin(startRad)));
        var endPt = new Point(cx + (r * Math.Cos(endRad)), cy + (r * Math.Sin(endRad)));

        var geo = new StreamGeometry();
        using (var gctx = geo.Open())
        {
            gctx.BeginFigure(startPt, false);
            gctx.ArcTo(endPt, new Size(r, r), 0, sweep > 180, SweepDirection.Clockwise);
        }

        ctx.DrawGeometry(null, pen, geo);
    }
}
