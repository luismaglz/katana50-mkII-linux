using Avalonia;
using Avalonia.Media;

namespace Kataka.App.Controls;

public class RotaryKnob : RotaryKnobBase
{
    private static readonly SolidColorBrush FaceBrush = KatanaPalette.KnobFaceBrush;
    private static readonly SolidColorBrush BezelBrush = KatanaPalette.BgBaseBrush;
    private static readonly SolidColorBrush TickBrush = new(KatanaPalette.BorderLight);
    private static readonly SolidColorBrush CenterTickBrush = KatanaPalette.PrimaryLitBrush;

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
        var resolvedLabelSize = LabelFontSize > 0 ? LabelFontSize : 12 * s;
        var labelText = CreateText(Label.ToUpperInvariant(), resolvedLabelSize, FontWeight.Bold, LabelBrush);
        var labelOrigin = new Point((bounds.Width - labelText.Width) / 2, bounds.Top);
        context.DrawText(labelText, labelOrigin);

        var diameter = Math.Min(bounds.Width - (12 * s), bounds.Height - labelText.Height - (56 * s));
        var radius = diameter / 2;
        var center = new Point(bounds.Width / 2, labelText.Height + (20 * s) + radius);

        DrawTicks(context, center, radius + (4 * s), s);
        context.DrawEllipse(BezelBrush, new Pen(new SolidColorBrush(KatanaPalette.BgBase), 2 * s), center,
            radius + (8 * s), radius + (8 * s));
        context.DrawEllipse(FaceBrush, new Pen(new SolidColorBrush(KatanaPalette.BorderLight), 1.5 * s), center, radius,
            radius);

        var angle = DegreesToRadians(135 + (NormalizedValue * 270));
        var indicatorLength = radius * 0.68;
        var pointerEnd = new Point(
            center.X + (Math.Cos(angle) * indicatorLength),
            center.Y + (Math.Sin(angle) * indicatorLength));
        context.DrawLine(new Pen(ValueBrush, 4 * s, lineCap: PenLineCap.Round), center, pointerEnd);
        context.DrawEllipse(ValueBrush, null, center, 4 * s, 4 * s);

        var valueText = CreateText(DisplayValueText, 14 * s, FontWeight.SemiBold, ValueBrush);
        var valueOrigin = new Point((bounds.Width - valueText.Width) / 2, center.Y + radius + (14 * s));
        context.DrawText(valueText, valueOrigin);
    }

    private void DrawTicks(DrawingContext context, Point center, double radius, double scale)
    {
        for (var index = 0; index <= 10; index++)
        {
            var isCenterTick = IsEffectivelyBipolar && index == 5;
            var pen = isCenterTick
                ? new Pen(CenterTickBrush, 2.5 * scale, lineCap: PenLineCap.Round)
                : new Pen(TickBrush, 1.5 * scale, lineCap: PenLineCap.Round);
            var angle = DegreesToRadians(135 + (index * 27));
            var start = new Point(
                center.X + (Math.Cos(angle) * (radius - (7 * scale))),
                center.Y + (Math.Sin(angle) * (radius - (7 * scale))));
            var end = new Point(
                center.X + (Math.Cos(angle) * radius),
                center.Y + (Math.Sin(angle) * radius));
            context.DrawLine(pen, start, end);
        }
    }
}
