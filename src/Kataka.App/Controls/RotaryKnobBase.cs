using System;
using System.Globalization;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// Shared base for <see cref="RotaryKnob"/> and <see cref="ImageRotaryKnob"/>.
/// Provides all styled properties, interaction handling (drag / scroll / keyboard),
/// tooltip management, and helper methods. Subclasses only need to implement
/// <see cref="Control.Render"/> and <see cref="Control.MeasureOverride"/>.
/// </summary>
public abstract class RotaryKnobBase : Control
{
    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<RotaryKnobBase, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<double> LabelFontSizeProperty =
        AvaloniaProperty.Register<RotaryKnobBase, double>(nameof(LabelFontSize), 12.0);

    public static readonly StyledProperty<int> MinimumProperty =
        AvaloniaProperty.Register<RotaryKnobBase, int>(nameof(Minimum), 0);

    public static readonly StyledProperty<int> MaximumProperty =
        AvaloniaProperty.Register<RotaryKnobBase, int>(nameof(Maximum), 100);

    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<RotaryKnobBase, int>(nameof(Value), 0,
            defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<double> ScaleProperty =
        AvaloniaProperty.Register<RotaryKnobBase, double>(nameof(Scale), 1.0);

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<RotaryKnobBase, string?>(nameof(Description));

    /// <summary>
    /// When true the knob is treated as bipolar: 12 o'clock = midpoint of [Minimum, Maximum]
    /// and the displayed value text is shown as an offset from that midpoint (e.g. +20 / -20).
    /// The visual sweep already centres naturally when IsBipolar is used with a symmetric range
    /// such as Minimum=-50, Maximum=50. A centre tick mark is also highlighted.
    /// </summary>
    public static readonly StyledProperty<bool> IsBipolarProperty =
        AvaloniaProperty.Register<RotaryKnobBase, bool>(nameof(IsBipolar), false);

    /// <summary>
    /// Optional display minimum. When both <see cref="DisplayMinimum"/> and
    /// <see cref="DisplayMaximum"/> are set (non-zero or explicitly different from the defaults),
    /// the value label is interpolated from the wire range [Minimum, Maximum] into
    /// [DisplayMinimum, DisplayMaximum]. A display range that crosses zero is inherently
    /// bipolar — the center tick is highlighted automatically without needing IsBipolar.
    /// </summary>
    public static readonly StyledProperty<int?> DisplayMinimumProperty =
        AvaloniaProperty.Register<RotaryKnobBase, int?>(nameof(DisplayMinimum), null);

    /// <summary>
    /// Optional display maximum. See <see cref="DisplayMinimumProperty"/>.
    /// </summary>
    public static readonly StyledProperty<int?> DisplayMaximumProperty =
        AvaloniaProperty.Register<RotaryKnobBase, int?>(nameof(DisplayMaximum), null);

    public static readonly StyledProperty<Color> AccentColorProperty =
        AvaloniaProperty.Register<RotaryKnobBase, Color>(nameof(AccentColor), KatanaPalette.PrimaryLit);

    public static readonly StyledProperty<Color> FaceColorProperty =
        AvaloniaProperty.Register<RotaryKnobBase, Color>(nameof(FaceColor), KatanaPalette.KnobBg);

    public static readonly StyledProperty<Color> BezelColorProperty =
        AvaloniaProperty.Register<RotaryKnobBase, Color>(nameof(BezelColor), KatanaPalette.BgBase);

    public static readonly StyledProperty<Color> TrackColorProperty =
        AvaloniaProperty.Register<RotaryKnobBase, Color>(nameof(TrackColor), Color.FromArgb(55, 255, 255, 255));

    public Color AccentColor
    {
        get => GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public Color FaceColor
    {
        get => GetValue(FaceColorProperty);
        set => SetValue(FaceColorProperty, value);
    }

    public Color BezelColor
    {
        get => GetValue(BezelColorProperty);
        set => SetValue(BezelColorProperty, value);
    }

    public Color TrackColor
    {
        get => GetValue(TrackColorProperty);
        set => SetValue(TrackColorProperty, value);
    }

    protected static readonly SolidColorBrush LabelBrush = KatanaPalette.TextMainBrush;
    protected static readonly SolidColorBrush ValueBrush = KatanaPalette.PrimaryLitBrush;

    private bool _isDragging;
    private Point _dragStart;
    private int _dragStartValue;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RuntimePaletteService.Changed += OnPaletteChanged;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        RuntimePaletteService.Changed -= OnPaletteChanged;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnPaletteChanged() => InvalidateVisual();

    static RotaryKnobBase()
    {
        AffectsRender<RotaryKnobBase>(
            LabelProperty, LabelFontSizeProperty,
            MinimumProperty, MaximumProperty, ValueProperty,
            ScaleProperty, IsBipolarProperty,
            DisplayMinimumProperty, DisplayMaximumProperty,
            AccentColorProperty, FaceColorProperty, BezelColorProperty, TrackColorProperty);
        AffectsMeasure<RotaryKnobBase>(ScaleProperty);
        FocusableProperty.OverrideDefaultValue<RotaryKnobBase>(true);

        LabelProperty.Changed.AddClassHandler<RotaryKnobBase>((k, _) => k.UpdateTooltip());
        DescriptionProperty.Changed.AddClassHandler<RotaryKnobBase>((k, _) => k.UpdateTooltip());
    }

    /// <summary> Properties ──────────────────────────────────────────────────────────── </summary>
    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public double LabelFontSize
    {
        get => GetValue(LabelFontSizeProperty);
        set => SetValue(LabelFontSizeProperty, Math.Max(1.0, value));
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

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool IsBipolar
    {
        get => GetValue(IsBipolarProperty);
        set => SetValue(IsBipolarProperty, value);
    }

    public int? DisplayMinimum
    {
        get => GetValue(DisplayMinimumProperty);
        set => SetValue(DisplayMinimumProperty, value);
    }

    public int? DisplayMaximum
    {
        get => GetValue(DisplayMaximumProperty);
        set => SetValue(DisplayMaximumProperty, value);
    }

    /// <summary>
    /// True when an explicit display range has been configured (both DisplayMinimum and
    /// DisplayMaximum are set) and the display range differs from the wire range.
    /// </summary>
    protected bool HasDisplayRange =>
        DisplayMinimum.HasValue && DisplayMaximum.HasValue &&
        (DisplayMinimum.Value != Minimum || DisplayMaximum.Value != Maximum);

    /// <summary>
    /// True when the label should show negative–zero–positive. Either an explicit display
    /// range that crosses zero, or the legacy IsBipolar flag.
    /// </summary>
    protected bool IsEffectivelyBipolar =>
        (HasDisplayRange && DisplayMinimum!.Value < 0 && DisplayMaximum!.Value > 0) || IsBipolar;

    /// <summary> Helpers for subclasses ───────────────────────────────────────────────── </summary>
    /// <summary>Value normalised to [0, 1] across [Minimum, Maximum].</summary>
    protected double NormalizedValue =>
        Maximum == Minimum
            ? 0.5
            : (double)(Math.Clamp(Value, Minimum, Maximum) - Minimum) / (Maximum - Minimum);

    /// <summary>
    /// Text shown inside / below the knob.
    /// <list type="bullet">
    ///   <item>When <see cref="HasDisplayRange"/> is true the raw <see cref="Value"/> is
    ///   linearly interpolated from [Minimum, Maximum] into [DisplayMinimum, DisplayMaximum]
    ///   and that mapped value is shown (with a "+" prefix when positive and the range crosses zero).</item>
    ///   <item>When <see cref="IsBipolar"/> is true (legacy mode) the value is shown as an
    ///   offset from the midpoint of [Minimum, Maximum].</item>
    ///   <item>Otherwise the raw <see cref="Value"/> is shown.</item>
    /// </list>
    /// </summary>
    protected string DisplayValueText
    {
        get
        {
            if (HasDisplayRange)
            {
                var wireRange = Math.Max(1, Maximum - Minimum);
                var displayRange = DisplayMaximum!.Value - DisplayMinimum!.Value;
                var displayValue = (int)Math.Round(
                    DisplayMinimum.Value + (double)(Math.Clamp(Value, Minimum, Maximum) - Minimum) / wireRange * displayRange);
                if (IsEffectivelyBipolar && displayValue > 0)
                    return $"+{displayValue}";
                return displayValue.ToString(CultureInfo.InvariantCulture);
            }

            if (!IsBipolar)
                return Value.ToString(CultureInfo.InvariantCulture);

            var mid = (Minimum + Maximum) / 2;
            var offset = Value - mid;
            return offset > 0
                ? $"+{offset}"
                : offset.ToString(CultureInfo.InvariantCulture);
        }
    }

    protected static FormattedText CreateText(string text, double fontSize, FontWeight weight, IBrush brush) =>
        new(text,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily.Default, FontStyle.Normal, weight),
            fontSize,
            brush);

    protected static double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);

    /// <summary> Tooltip ─────────────────────────────────────────────────────────────── </summary>
    private void UpdateTooltip()
    {
        var label = Label;
        var desc = Description;

        if (string.IsNullOrWhiteSpace(label) && string.IsNullOrWhiteSpace(desc))
        {
            ToolTip.SetTip(this, null);
            return;
        }

        var panel = new StackPanel { Spacing = 4 };
        if (!string.IsNullOrWhiteSpace(label))
            panel.Children.Add(new TextBlock
            {
                Text = label,
                FontSize = 13,
                FontWeight = FontWeight.SemiBold
            });
        if (!string.IsNullOrWhiteSpace(desc))
            panel.Children.Add(new TextBlock
            {
                Text = desc,
                FontSize = 11,
                MaxWidth = 260,
                TextWrapping = TextWrapping.Wrap
            });

        ToolTip.SetTip(this, panel);
    }

    /// <summary> Interaction ─────────────────────────────────────────────────────────── </summary>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed) return;
        Focus();
        _isDragging = true;
        _dragStart = e.GetPosition(this);
        _dragStartValue = Value;
        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        if (!_isDragging) return;
        var current = e.GetPosition(this);
        var delta = (_dragStart.Y - current.Y) + ((current.X - _dragStart.X) * 0.35);
        var range = Math.Max(1, Maximum - Minimum);
        var adjusted = _dragStartValue + (int)Math.Round(delta * range / 180.0);
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
        var step = Math.Max(1, (Maximum - Minimum) / 40);
        Value = Math.Clamp(Value + ((e.Delta.Y > 0 ? 1 : -1) * step), Minimum, Maximum);
        e.Handled = true;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        var step = Math.Max(1, (Maximum - Minimum) / 40);
        switch (e.Key)
        {
            case Key.Up:
            case Key.Right:
                Value = Math.Clamp(Value + step, Minimum, Maximum);
                e.Handled = true;
                break;
            case Key.Down:
            case Key.Left:
                Value = Math.Clamp(Value - step, Minimum, Maximum);
                e.Handled = true;
                break;
        }
    }
}
