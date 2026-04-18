using System;

using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// Code-drawn stepped rotary knob (Scale-aware). No SVG dependency — ticks and face
/// are all Avalonia primitives using KatanaPalette colors. The active tick is lit in
/// amber; inactive ticks are dimmed. Minimum/Maximum are auto-set from Steps.Count.
/// </summary>
public sealed class SteppedKnobV2 : RotaryKnobBase
{
    private const double BaseWidth = 84.0;
    private const double BaseHeight = 120.0;
    private const double BaseKnobR = 26.0;
    private const double BasePad = 4.0;
    private const double BaseTopPad = 10.0;
    private const double BaseBottomPad = 6.0;
    private const double BaseTickInner = BaseKnobR + 5.0;
    private const double BaseTickOuter = BaseKnobR + 14.0;
    private const double BaseValueFontSize = 10.0;
    private const double StartDeg = 135.0;
    private const double SweepDeg = 270.0;

    private static readonly SolidColorBrush TickActiveBrush = KatanaPalette.PrimaryLitBrush;
    private static readonly SolidColorBrush TickInactiveBrush = new(Color.FromArgb(70, 255, 255, 255));
    private static readonly SolidColorBrush FaceBrush = KatanaPalette.KnobBgBrush;
    private static readonly SolidColorBrush BezelBrush = KatanaPalette.BgBaseBrush;

    public static readonly StyledProperty<string[]?> StepsProperty =
        AvaloniaProperty.Register<SteppedKnobV2, string[]?>(nameof(Steps));

    static SteppedKnobV2()
    {
        AffectsRender<SteppedKnobV2>(StepsProperty);
        AffectsMeasure<SteppedKnobV2>(StepsProperty);
        StepsProperty.Changed.AddClassHandler<SteppedKnobV2>((k, _) => k.SyncRangeToSteps());
    }

    public string[]? Steps
    {
        get => GetValue(StepsProperty);
        set => SetValue(StepsProperty, value);
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
        var topPad = BaseTopPad * s;
        var bottomPad = BaseBottomPad * s;
        var tickInner = BaseTickInner * s;
        var tickOuter = BaseTickOuter * s;
        var valueFontSize = BaseValueFontSize * s;
        var bounds = Bounds.Deflate(pad);
        var steps = Steps;

        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize * s, FontWeight.Bold, LabelBrush);
        var cx = bounds.Width / 2;
        var cy = labelText.Height + topPad + tickOuter + 2 * s;

        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        // Ticks at each step position
        if (steps is { Length: >= 2 })
        {
            for (var i = 0; i < steps.Length; i++)
            {
                var angleDeg = StartDeg + (double)i / (steps.Length - 1) * SweepDeg;
                var angleRad = DegreesToRadians(angleDeg);
                var sin = Math.Sin(angleRad);
                var cos = Math.Cos(angleRad);
                var isActive = i == Value;
                var p1 = new Point(cx + cos * tickInner, cy + sin * tickInner);
                var p2 = new Point(cx + cos * tickOuter, cy + sin * tickOuter);
                context.DrawLine(
                    new Pen(isActive ? TickActiveBrush : TickInactiveBrush,
                        isActive ? 2.5 * s : 1.2 * s, lineCap: PenLineCap.Round),
                    p1, p2);
            }
        }

        // Bezel + face
        context.DrawEllipse(BezelBrush, null, new Point(cx, cy), knobR + 3 * s, knobR + 3 * s);
        context.DrawEllipse(FaceBrush,
            new Pen(new SolidColorBrush(KatanaPalette.BorderLight), 1.2 * s),
            new Point(cx, cy), knobR, knobR);

        // Pointer snapped to step
        var pointerAngle = DegreesToRadians(StartDeg + NormalizedValue * SweepDeg);
        var pointerLen = knobR * 0.68;
        var tip = new Point(cx + Math.Cos(pointerAngle) * pointerLen, cy + Math.Sin(pointerAngle) * pointerLen);
        context.DrawLine(new Pen(ValueBrush, 2.5 * s, lineCap: PenLineCap.Round), new Point(cx, cy), tip);
        context.DrawEllipse(ValueBrush, null, new Point(cx, cy), 3 * s, 3 * s);

        // Active step label
        var activeLabel = (steps is not null && Value >= 0 && Value < steps.Length)
            ? steps[Value]
            : Value.ToString();
        var valueText = CreateText(activeLabel, valueFontSize, FontWeight.SemiBold, ValueBrush);
        context.DrawText(valueText, new Point(
            (bounds.Width - valueText.Width) / 2,
            cy + knobR + bottomPad));
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
