using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
///     Code-drawn stepped rotary knob (Scale-aware). No SVG dependency — ticks and face
///     are all Avalonia primitives using KatanaPalette colors. The active tick is lit in
///     amber; inactive ticks are dimmed. Minimum/Maximum are auto-set from Steps.Count.
/// </summary>
public sealed class SteppedKnobV2 : RotaryKnobBase
{
    private const double BaseWidth = 116.0;
    private const double BaseHeight = 156.0;
    private const double BaseKnobR = 34.0;
    private const double BaseArcR = 46.0;
    private const double BaseTickDotR = 2.5;
    private const double BasePad = 4.0;
    private const double BaseTopPad = 16.0;
    private const double BaseBottomPad = 6.0;
    private const double StartDeg = 135.0;
    private const double SweepDeg = 270.0;


    public static readonly StyledProperty<string[]?> StepsProperty =
        AvaloniaProperty.Register<SteppedKnobV2, string[]?>(nameof(Steps));

    public static readonly StyledProperty<double> ValueFontSizeProperty =
        AvaloniaProperty.Register<SteppedKnobV2, double>(nameof(ValueFontSize), 14.0);

    static SteppedKnobV2()
    {
        AffectsRender<SteppedKnobV2>(StepsProperty, ValueFontSizeProperty);
        AffectsMeasure<SteppedKnobV2>(StepsProperty);
        StepsProperty.Changed.AddClassHandler<SteppedKnobV2>((k, _) => k.SyncRangeToSteps());
    }

    public string[]? Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
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
        var s = Scale;
        var pad = BasePad * s;
        var knobR = BaseKnobR * s;
        var arcR = BaseArcR * s;
        var dotR = BaseTickDotR * s;
        var topPad = BaseTopPad * s;
        var bottomPad = BaseBottomPad * s;
        var bounds = Bounds.Deflate(pad);
        var steps = Steps;

        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize * s, FontWeight.Bold, LabelBrush);
        var cx = bounds.Width / 2;
        var cy = labelText.Height + topPad + arcR;

        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        var accentColor = AccentColor;
        var trackColor = IsSet(TrackColorProperty)
            ? TrackColor
            : Color.FromArgb(70, accentColor.R, accentColor.G, accentColor.B);
        var faceColor = IsSet(FaceColorProperty) ? FaceColor : RuntimePaletteService.KnobFace;
        var bezelColor = IsSet(BezelColorProperty) ? BezelColor : RuntimePaletteService.BgBase;
        var accentBrush = new SolidColorBrush(accentColor);
        var valueBrush = ValueBrush;
        var trackBrush = new SolidColorBrush(trackColor);

        // Dots at each step position on the arc ring
        if (steps is { Length: >= 2 })
            for (var i = 0; i < steps.Length; i++)
            {
                var angleDeg = StartDeg + ((double)i / (steps.Length - 1) * SweepDeg);
                var angleRad = DegreesToRadians(angleDeg);
                var isActive = i == Value;
                var r = isActive ? dotR * 1.5 : dotR;
                var dotCenter = new Point(cx + (Math.Cos(angleRad) * arcR), cy + (Math.Sin(angleRad) * arcR));
                context.DrawEllipse(isActive ? accentBrush : trackBrush, null, dotCenter, r, r);
            }

        // Bezel + face
        context.DrawEllipse(new SolidColorBrush(bezelColor), null, new Point(cx, cy), knobR + (4 * s), knobR + (4 * s));
        context.DrawEllipse(new SolidColorBrush(faceColor),
            new Pen(new SolidColorBrush(KatanaPalette.BorderLight), 1.5 * s),
            new Point(cx, cy), knobR, knobR);

        // Pointer snapped to step
        var pointerAngle = DegreesToRadians(StartDeg + (NormalizedValue * SweepDeg));
        var pointerLen = knobR * 0.68;
        var tip = new Point(cx + (Math.Cos(pointerAngle) * pointerLen), cy + (Math.Sin(pointerAngle) * pointerLen));
        context.DrawLine(new Pen(accentBrush, 3.0 * s, lineCap: PenLineCap.Round), new Point(cx, cy), tip);
        context.DrawEllipse(accentBrush, null, new Point(cx, cy), 3.5 * s, 3.5 * s);

        // Active step label
        var activeLabel = steps is not null && Value >= 0 && Value < steps.Length
            ? steps[Value]
            : Value.ToString();
        var valueText = CreateText(activeLabel, ValueFontSize * s, FontWeight.SemiBold, valueBrush);
        context.DrawText(valueText, new Point(
            (bounds.Width - valueText.Width) / 2,
            cy + arcR + bottomPad));
    }

    private void SyncRangeToSteps()
    {
        var steps = Steps;
        if (steps is { Length: > 1 })
        {
            Minimum = 0;
            Maximum = steps.Length - 1;
        }
    }
}
