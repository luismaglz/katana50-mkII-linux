using System.Globalization;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
///     Vertical slider designed for graphic-equaliser bands.
///     The track runs from bottom (Minimum) to top (Maximum). A centre-line marks the
///     midpoint so 0 dB is visually obvious. The filled bar extends from the centre
///     upward (boost) or downward (cut), and the numeric value shown is the offset from
///     the midpoint so that neutral always reads "0".
/// </summary>
public class EqBandSlider : Control
{
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<EqBandSlider, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<int> MinimumProperty =
        AvaloniaProperty.Register<EqBandSlider, int>(nameof(Minimum));

    public static readonly StyledProperty<int> MaximumProperty =
        AvaloniaProperty.Register<EqBandSlider, int>(nameof(Maximum), 40);

    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<EqBandSlider, int>(nameof(Value), 20,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> ScaleProperty =
        AvaloniaProperty.Register<EqBandSlider, double>(nameof(Scale), 1.0);

    /// <summary> Colours ─────────────────────────────────────────────────────────────── </summary>
    private static readonly SolidColorBrush TrackBrush = KatanaPalette.BgSurfaceBrush;

    private static readonly SolidColorBrush TrackBorderBrush = new(KatanaPalette.Border);
    private static readonly SolidColorBrush CentreBrush = new(KatanaPalette.BorderLight);
    private static readonly SolidColorBrush BoostBrush = KatanaPalette.SuccessBrush;
    private static readonly SolidColorBrush CutBrush = KatanaPalette.ErrorBrush;
    private static readonly SolidColorBrush NeutralBrush = new(KatanaPalette.BorderLight);
    private static readonly SolidColorBrush ThumbBrush = KatanaPalette.TextMainBrush;
    private static readonly SolidColorBrush LabelBrush = KatanaPalette.TextMutedBrush;
    private static readonly SolidColorBrush ValueBrush = KatanaPalette.PrimaryLitBrush;
    private int _dragStartValue;
    private double _dragStartY;

    private bool _isDragging;

    static EqBandSlider()
    {
        AffectsRender<EqBandSlider>(LabelProperty, MinimumProperty, MaximumProperty, ValueProperty, ScaleProperty);
        AffectsMeasure<EqBandSlider>(ScaleProperty);
        FocusableProperty.OverrideDefaultValue<EqBandSlider>(true);
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

    public double Scale
    {
        get => GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, Math.Max(0.1, value));
    }

    /// <summary> Layout ──────────────────────────────────────────────────────────────── </summary>
    private double TrackWidth => 14 * Scale;

    private double TrackHeight => 100 * Scale;
    private double TotalWidth => 44 * Scale;
    private double LabelHeight => 16 * Scale;

    private double ValueHeight => 16 * Scale;

    // Layout: [valueText] [track] [label]
    private double TotalHeight => ValueHeight + (6 * Scale) + TrackHeight + (4 * Scale) + LabelHeight;

    protected override Size MeasureOverride(Size availableSize) =>
        new(TotalWidth, TotalHeight);

    /// <summary> Rendering ───────────────────────────────────────────────────────────── </summary>
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var s = Scale;
        var tw = TrackWidth;
        var th = TrackHeight;
        var totalW = TotalWidth;
        var trackX = (totalW - tw) / 2;
        var trackTop = ValueHeight + (6 * s);

        // Track background
        var trackRect = new Rect(trackX, trackTop, tw, th);
        context.DrawRectangle(TrackBrush, new Pen(TrackBorderBrush), trackRect, 3, 3);

        // Centre line
        var range = Math.Max(1, Maximum - Minimum);
        var midVal = (Minimum + Maximum) / 2;
        var midNorm = (double)(midVal - Minimum) / range;
        var centreY = trackTop + (th * (1.0 - midNorm));
        context.DrawLine(new Pen(CentreBrush, 1.5 * s), new Point(trackX, centreY), new Point(trackX + tw, centreY));

        // Filled bar from centre to current value
        var norm = (double)(Math.Clamp(Value, Minimum, Maximum) - Minimum) / range;
        var thumbY = trackTop + (th * (1.0 - norm));
        var barTop = Math.Min(thumbY, centreY);
        var barBot = Math.Max(thumbY, centreY);
        var barHeight = barBot - barTop;
        if (barHeight > 0)
        {
            var barBrush = Value > midVal ? BoostBrush : Value < midVal ? CutBrush : NeutralBrush;
            context.DrawRectangle(barBrush, null, new Rect(trackX + 1, barTop, tw - 2, barHeight));
        }

        // Thumb
        var thumbH = 6 * s;
        var thumbRect = new Rect(trackX - (2 * s), thumbY - (thumbH / 2), tw + (4 * s), thumbH);
        context.DrawRectangle(ThumbBrush, null, thumbRect, 2, 2);

        // Value text (offset from centre, e.g. +12, 0, -8)
        var offset = Value - midVal;
        var valStr = offset > 0 ? $"+{offset}" : offset.ToString(CultureInfo.InvariantCulture);
        var valText = MakeText(valStr, 9 * s, FontWeight.SemiBold, ValueBrush);
        var valOrigin = new Point((totalW - valText.Width) / 2, 0);
        context.DrawText(valText, valOrigin);

        // Frequency label
        var lblText = MakeText(Label, 9 * s, FontWeight.Normal, LabelBrush);
        var lblOrigin = new Point((totalW - lblText.Width) / 2, trackTop + th + (4 * s));
        context.DrawText(lblText, lblOrigin);
    }

    /// <summary> Interaction ─────────────────────────────────────────────────────────── </summary>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        Focus();
        _isDragging = true;
        _dragStartY = e.GetPosition(this).Y;
        _dragStartValue = Value;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isDragging) return;
        var deltaY = _dragStartY - e.GetPosition(this).Y; // up = positive
        var range = Math.Max(1, Maximum - Minimum);
        var adjusted = _dragStartValue + (int)Math.Round(deltaY * range / TrackHeight);
        Value = Math.Clamp(adjusted, Minimum, Maximum);
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (!_isDragging) return;
        _isDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        Value = Math.Clamp(Value + (e.Delta.Y > 0 ? 1 : -1), Minimum, Maximum);
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        switch (e.Key)
        {
            case Key.Up:
                Value = Math.Clamp(Value + 1, Minimum, Maximum);
                e.Handled = true;
                break;
            case Key.Down:
                Value = Math.Clamp(Value - 1, Minimum, Maximum);
                e.Handled = true;
                break;
        }
    }

    private static FormattedText MakeText(string text, double size, FontWeight weight, IBrush brush) =>
        new(text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
            new Typeface(FontFamily.Default, FontStyle.Normal, weight), size, brush);
}
