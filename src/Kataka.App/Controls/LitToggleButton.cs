using System.Globalization;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// A fully custom-drawn toggle button that glows when on and goes dark when off.
/// Bind <see cref="IsChecked"/> (two-way) and set <see cref="Label"/> for the caption.
/// Use <see cref="OnColor"/> to theme the lit state (default: amber #ffc95d).
/// </summary>
public sealed class LitToggleButton : Control
{
    private const double CornerRadius = 6.0;
    private const double FontSize = 12.0;

    private static readonly Color DefaultOnColor = KatanaPalette.AccentLit;
    private static readonly SolidColorBrush OffFillBrush = KatanaPalette.ControlBgBrush;
    private static readonly Pen OffBorderPen = KatanaPalette.BorderDimPen;
    private static readonly SolidColorBrush OffTextBrush = KatanaPalette.TextDimBrush;
    private static readonly SolidColorBrush OnTextBrush = KatanaPalette.TextOnAccentBrush;
    private static readonly SolidColorBrush TopHighlightBrush = new(Color.FromArgb(45, 255, 255, 255));

    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<LitToggleButton, bool>(nameof(IsChecked),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<LitToggleButton, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<Color> OnColorProperty =
        AvaloniaProperty.Register<LitToggleButton, Color>(nameof(OnColor), DefaultOnColor);

    static LitToggleButton()
    {
        AffectsRender<LitToggleButton>(IsCheckedProperty, LabelProperty, OnColorProperty);
        FocusableProperty.OverrideDefaultValue<LitToggleButton>(true);
        CursorProperty.OverrideDefaultValue<LitToggleButton>(new Cursor(StandardCursorType.Hand));
    }

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public string Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public Color OnColor
    {
        get => GetValue(OnColorProperty);
        set => SetValue(OnColorProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize) => new(96, 36);

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            IsChecked = !IsChecked;
            e.Handled = true;
        }
        base.OnPointerPressed(e);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key is Key.Space or Key.Enter)
        {
            IsChecked = !IsChecked;
            e.Handled = true;
        }
        base.OnKeyDown(e);
    }

    public override void Render(DrawingContext context)
    {
        var w = Bounds.Width;
        var h = Bounds.Height;
        var rect = new Rect(0, 0, w, h);
        var isOn = IsChecked;

        if (isOn)
        {
            var c = OnColor;

            // Soft inner glow — two inset layers slightly larger than content
            context.DrawRectangle(
                new SolidColorBrush(Color.FromArgb(35, c.R, c.G, c.B)), null,
                rect.Inflate(3), CornerRadius + 3);
            context.DrawRectangle(
                new SolidColorBrush(Color.FromArgb(60, c.R, c.G, c.B)), null,
                rect.Inflate(1.5), CornerRadius + 1.5);

            // Button body
            context.DrawRectangle(new SolidColorBrush(c), null, rect, CornerRadius);

            // Top highlight — simulates light source from above
            context.DrawRectangle(TopHighlightBrush, null,
                new Rect(1, 1, w - 2, h * 0.45), CornerRadius - 1);

            DrawLabel(context, w, h, OnTextBrush, FontWeight.SemiBold);
        }
        else
        {
            context.DrawRectangle(OffFillBrush, OffBorderPen, rect, CornerRadius);
            DrawLabel(context, w, h, OffTextBrush, FontWeight.Normal);
        }
    }

    private void DrawLabel(DrawingContext context, double w, double h, IBrush brush, FontWeight weight)
    {
        var text = new FormattedText(
            Label,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily.Default, FontStyle.Normal, weight),
            FontSize,
            brush);
        context.DrawText(text, new Point((w - text.Width) / 2, (h - text.Height) / 2));
    }
}
