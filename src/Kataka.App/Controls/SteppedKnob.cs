using System;

using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// Rotary knob that snaps to a discrete list of named steps.
/// Bind <see cref="Value"/> (0-based index) and supply <see cref="Steps"/> with the label for
/// each position. The control automatically sets Minimum=0 / Maximum=Steps.Count-1.
/// Tick marks are drawn around the arc; the selected tick and its label are highlighted.
/// The <see cref="RotaryKnobBase.Scale"/> property scales the entire control proportionally.
/// </summary>
public sealed class SteppedKnob : RotaryKnobBase
{
    private const double BaseImgSize = 64;
    private const double BasePadX = 4;
    private const double BaseTopPad = 10;
    private const double BaseBottomPad = 6;
    private const double BaseValueFontSize = 11;
    private const double BaseWidth = 80;
    private const double BaseHeight = 115;

    private const double BaseTickInner = BaseImgSize / 2 + 3;
    private const double BaseTickOuter = BaseImgSize / 2 + 10;

    public static readonly StyledProperty<string[]?> StepsProperty =
        AvaloniaProperty.Register<SteppedKnob, string[]?>(nameof(Steps));

    private static readonly SolidColorBrush TickActiveBrush = new(Color.Parse("#ffcf66"));
    private static readonly SolidColorBrush TickInactiveBrush = new(Color.Parse("#4a5060"));
    private static readonly SolidColorBrush KnobShadowBrush = new(Color.Parse("#28000000"));
    private static readonly SolidColorBrush KnobBgBrush = new(Color.Parse("#D9D3C1"));

    static SteppedKnob()
    {
        AffectsRender<SteppedKnob>(StepsProperty);
        StepsProperty.Changed.AddClassHandler<SteppedKnob>((k, _) => k.SyncRangeToSteps());
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
        var imgSize = BaseImgSize * s;
        var padX = BasePadX * s;
        var topPad = BaseTopPad * s;
        var bottomPad = BaseBottomPad * s;
        var valueFontSize = BaseValueFontSize * s;
        var tickInner = BaseTickInner * s;
        var tickOuter = BaseTickOuter * s;

        var steps = Steps;
        var bounds = Bounds.Deflate(padX);
        var cx = bounds.Width / 2;

        var labelText = CreateText(Label.ToUpperInvariant(), LabelFontSize * s, FontWeight.Bold, LabelBrush);
        context.DrawText(labelText, new Point((bounds.Width - labelText.Width) / 2, bounds.Top));

        var cy = labelText.Height + topPad + imgSize / 2;

        DrawTicks(context, steps, cx, cy, tickInner, tickOuter);

        context.DrawEllipse(KnobShadowBrush, null, new Point(cx + 1.5 * s, cy + 2 * s), imgSize / 2, imgSize / 2);
        context.DrawEllipse(KnobBgBrush, null, new Point(cx, cy), imgSize / 2, imgSize / 2);

        DrawRotatedKnob(context, cx, cy, imgSize);

        var valueLabel = (steps is not null && Value >= 0 && Value < steps.Length)
            ? steps[Value]
            : Value.ToString();
        var valueText = CreateText(valueLabel, valueFontSize, FontWeight.SemiBold, ValueBrush);
        context.DrawText(valueText, new Point((bounds.Width - valueText.Width) / 2, cy + imgSize / 2 + bottomPad));
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void SyncRangeToSteps()
    {
        var steps = Steps;
        if (steps is not null && steps.Length > 1)
        {
            Minimum = 0;
            Maximum = steps.Length - 1;
        }
    }

    private void DrawTicks(DrawingContext context, string[]? steps, double cx, double cy,
        double tickInner, double tickOuter)
    {
        if (steps is null || steps.Length < 2) return;

        for (var i = 0; i < steps.Length; i++)
        {
            var angleDeg = KnobImageAsset.MinAngleRad * (180.0 / Math.PI)
                           + i * KnobImageAsset.AngleSweep / (steps.Length - 1);
            var angleRad = DegreesToRadians(angleDeg);
            var sin = Math.Sin(angleRad);
            var cos = -Math.Cos(angleRad);

            var p1 = new Point(cx + sin * tickInner, cy + cos * tickInner);
            var p2 = new Point(cx + sin * tickOuter, cy + cos * tickOuter);

            var isSelected = i == Value;
            var s = Scale;
            context.DrawLine(
                new Pen(isSelected ? TickActiveBrush : TickInactiveBrush, isSelected ? 2.0 * s : 1.2 * s),
                p1, p2);
        }
    }

    private void DrawRotatedKnob(DrawingContext context, double cx, double cy, double imgSize)
    {
        var angleRad = KnobImageAsset.MinAngleRad + NormalizedValue * KnobImageAsset.AngleSweep * Math.PI / 180.0;
        var imgRect = new Rect(cx - imgSize / 2, cy - imgSize / 2, imgSize, imgSize);

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
                    new Pen(new SolidColorBrush(Color.Parse("#565b64")), 1.5 * Scale),
                    new Point(cx, cy), imgSize / 2, imgSize / 2);
        }
    }
}
