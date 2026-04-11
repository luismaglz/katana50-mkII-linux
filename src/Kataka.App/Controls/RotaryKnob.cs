using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Kataka.App.Controls;

public class RotaryKnob : Control
{
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<RotaryKnob, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<int> MinimumProperty =
        AvaloniaProperty.Register<RotaryKnob, int>(nameof(Minimum), 0);

    public static readonly StyledProperty<int> MaximumProperty =
        AvaloniaProperty.Register<RotaryKnob, int>(nameof(Maximum), 100);

    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<RotaryKnob, int>(nameof(Value), 0, defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    private static readonly SolidColorBrush FaceBrush = new(Color.Parse("#2c2f35"));
    private static readonly SolidColorBrush BezelBrush = new(Color.Parse("#0f1114"));
    private static readonly SolidColorBrush TickBrush = new(Color.Parse("#4a4f57"));
    private static readonly SolidColorBrush LabelBrush = new(Color.Parse("#d8d5cb"));
    private static readonly SolidColorBrush ValueBrush = new(Color.Parse("#ffcf66"));
    private static readonly Pen BezelPen = new(new SolidColorBrush(Color.Parse("#0a0c0e")), 2);
    private static readonly Pen RimPen = new(new SolidColorBrush(Color.Parse("#565b64")), 1.5);
    private static readonly Pen PointerPen = new(ValueBrush, 4, lineCap: PenLineCap.Round);
    private static readonly Pen TickPen = new(TickBrush, 1.5, lineCap: PenLineCap.Round);

    private bool isDragging;
    private Point dragStart;
    private int dragStartValue;

    static RotaryKnob()
    {
        AffectsRender<RotaryKnob>(LabelProperty, MinimumProperty, MaximumProperty, ValueProperty);
        FocusableProperty.OverrideDefaultValue<RotaryKnob>(true);
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public int Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public int Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public int Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, Math.Clamp(value, Minimum, Maximum));
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(116, 156);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds.Deflate(4);
        var labelText = CreateText(Label.ToUpperInvariant(), 12, FontWeight.Bold, LabelBrush);
        var labelOrigin = new Point((bounds.Width - labelText.Width) / 2, bounds.Top);
        context.DrawText(labelText, labelOrigin);

        var diameter = Math.Min(bounds.Width - 12, bounds.Height - labelText.Height - 48);
        var radius = diameter / 2;
        var center = new Point(bounds.Width / 2, labelText.Height + 12 + radius);

        DrawTicks(context, center, radius + 4);
        context.DrawEllipse(BezelBrush, BezelPen, center, radius + 8, radius + 8);
        context.DrawEllipse(FaceBrush, RimPen, center, radius, radius);

        var normalized = Maximum == Minimum
            ? 0
            : (double)(Math.Clamp(Value, Minimum, Maximum) - Minimum) / (Maximum - Minimum);
        var angle = DegreesToRadians(135 + (normalized * 270));
        var indicatorLength = radius * 0.68;
        var pointerEnd = new Point(
            center.X + Math.Cos(angle) * indicatorLength,
            center.Y + Math.Sin(angle) * indicatorLength);
        context.DrawLine(PointerPen, center, pointerEnd);
        context.DrawEllipse(ValueBrush, null, center, 4, 4);

        var valueText = CreateText(Value.ToString(CultureInfo.InvariantCulture), 14, FontWeight.SemiBold, ValueBrush);
        var valueOrigin = new Point((bounds.Width - valueText.Width) / 2, center.Y + radius + 14);
        context.DrawText(valueText, valueOrigin);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            return;
        }

        Focus();
        isDragging = true;
        dragStart = e.GetPosition(this);
        dragStartValue = Value;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!isDragging)
        {
            return;
        }

        var current = e.GetPosition(this);
        var delta = (dragStart.Y - current.Y) + ((current.X - dragStart.X) * 0.35);
        var range = Math.Max(1, Maximum - Minimum);
        var adjusted = dragStartValue + (int)Math.Round(delta * range / 180d);
        Value = Math.Clamp(adjusted, Minimum, Maximum);
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!isDragging)
        {
            return;
        }

        isDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        var step = Math.Max(1, (Maximum - Minimum) / 40);
        Value = Math.Clamp(Value + ((e.Delta.Y > 0 ? 1 : -1) * step), Minimum, Maximum);
        e.Handled = true;
    }

    private static void DrawTicks(DrawingContext context, Point center, double radius)
    {
        for (var index = 0; index <= 10; index++)
        {
            var angle = DegreesToRadians(135 + (index * 27));
            var start = new Point(
                center.X + Math.Cos(angle) * (radius - 7),
                center.Y + Math.Sin(angle) * (radius - 7));
            var end = new Point(
                center.X + Math.Cos(angle) * radius,
                center.Y + Math.Sin(angle) * radius);
            context.DrawLine(TickPen, start, end);
        }
    }

    private static FormattedText CreateText(string text, double fontSize, FontWeight weight, IBrush brush)
    {
        return new FormattedText(
            text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily.Default, FontStyle.Normal, weight),
            fontSize,
            brush);
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * (Math.PI / 180d);
    }
}
