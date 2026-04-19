using System.Globalization;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace Kataka.App.Controls;

public enum LabelPosition
{
    Top,
    Bottom,
    Left,
    Right
}

/// <summary>
///     Small toggle button with an inset lit indicator. Label sits outside the button body.
///     Set <see cref="ButtonWidth" />/<see cref="ButtonHeight" /> and <see cref="LabelPosition" />.
/// </summary>
public sealed class LitToggleButton : Control
{
    private const double DefaultButtonWidth = 48.0;
    private const double DefaultButtonHeight = 28.0;
    private const double DefaultCornerRadius = 8.0;
    private const double InsetPadding = 5.0;
    private const double InsetCornerRadius = 4.0;
    private const double LabelFontSize = 11.0;
    private const double LabelGap = 4.0;

    private static readonly Color DefaultOnColor = KatanaPalette.PrimaryLit;
    private static readonly SolidColorBrush ButtonBodyBrush = new(Color.Parse("#1a1a1a"));
    private static readonly Pen ButtonBorderPen = new(new SolidColorBrush(Color.Parse("#444444")));
    private static readonly SolidColorBrush InsetOffBrush = new(Color.Parse("#0d0d0d"));
    private static readonly SolidColorBrush TopHighlightBrush = new(Color.FromArgb(40, 255, 255, 255));

    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<LitToggleButton, bool>(nameof(IsChecked),
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<string> LabelProperty =
        AvaloniaProperty.Register<LitToggleButton, string>(nameof(Label), string.Empty);

    public static readonly StyledProperty<Color> OnColorProperty =
        AvaloniaProperty.Register<LitToggleButton, Color>(nameof(OnColor), DefaultOnColor);

    public static readonly StyledProperty<LabelPosition> LabelPositionProperty =
        AvaloniaProperty.Register<LitToggleButton, LabelPosition>(nameof(LabelPosition), LabelPosition.Bottom);

    public static readonly StyledProperty<double> ButtonWidthProperty =
        AvaloniaProperty.Register<LitToggleButton, double>(nameof(ButtonWidth), DefaultButtonWidth);

    public static readonly StyledProperty<double> ButtonHeightProperty =
        AvaloniaProperty.Register<LitToggleButton, double>(nameof(ButtonHeight), DefaultButtonHeight);

    public static readonly StyledProperty<double> CornerRadiusProperty =
        AvaloniaProperty.Register<LitToggleButton, double>(nameof(CornerRadius), DefaultCornerRadius);

    static LitToggleButton()
    {
        AffectsRender<LitToggleButton>(IsCheckedProperty, LabelProperty, OnColorProperty,
            LabelPositionProperty, ButtonWidthProperty, ButtonHeightProperty, CornerRadiusProperty);
        AffectsMeasure<LitToggleButton>(LabelProperty, LabelPositionProperty,
            ButtonWidthProperty, ButtonHeightProperty);
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

    public LabelPosition LabelPosition
    {
        get => GetValue(LabelPositionProperty);
        set => SetValue(LabelPositionProperty, value);
    }

    public double ButtonWidth
    {
        get => GetValue(ButtonWidthProperty);
        set => SetValue(ButtonWidthProperty, value);
    }

    public double ButtonHeight
    {
        get => GetValue(ButtonHeightProperty);
        set => SetValue(ButtonHeightProperty, value);
    }

    public double CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var label = MakeLabel(KatanaPalette.TextMutedBrush, FontWeight.Normal);
        var bw = ButtonWidth;
        var bh = ButtonHeight;
        var pos = LabelPosition;

        return pos is LabelPosition.Top or LabelPosition.Bottom
            ? new Size(Math.Max(bw, label.Width), bh + LabelGap + label.Height)
            : new Size(bw + LabelGap + label.Width, Math.Max(bh, label.Height));
    }

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
        var isOn = IsChecked;
        var c = OnColor;
        var bw = ButtonWidth;
        var bh = ButtonHeight;
        var pos = LabelPosition;
        var labelBrush = isOn ? KatanaPalette.TextMainBrush : KatanaPalette.TextMutedBrush;
        var label = MakeLabel(labelBrush, isOn ? FontWeight.SemiBold : FontWeight.Normal);

        // Compute button origin based on label position
        double bx, by, lx, ly;
        switch (pos)
        {
            case LabelPosition.Top:
                bx = (Bounds.Width - bw) / 2;
                by = label.Height + LabelGap;
                lx = (Bounds.Width - label.Width) / 2;
                ly = 0;
                break;
            case LabelPosition.Left:
                bx = label.Width + LabelGap;
                by = (Bounds.Height - bh) / 2;
                lx = 0;
                ly = (Bounds.Height - label.Height) / 2;
                break;
            case LabelPosition.Right:
                bx = 0;
                by = (Bounds.Height - bh) / 2;
                lx = bw + LabelGap;
                ly = (Bounds.Height - label.Height) / 2;
                break;
            default: // Bottom
                bx = (Bounds.Width - bw) / 2;
                by = 0;
                lx = (Bounds.Width - label.Width) / 2;
                ly = bh + LabelGap;
                break;
        }

        var cr = CornerRadius;
        var buttonRect = new Rect(bx, by, bw, bh);

        // Button shell
        context.DrawRectangle(ButtonBodyBrush, ButtonBorderPen, buttonRect, cr);

        // Inset lit area
        var inset = buttonRect.Deflate(InsetPadding);
        var insetCr = Math.Max(0, cr - InsetPadding + 1);
        if (isOn)
        {
            // Outer glow
            context.DrawRectangle(
                new SolidColorBrush(Color.FromArgb(50, c.R, c.G, c.B)), null,
                inset.Inflate(2), insetCr + 2);

            context.DrawRectangle(new SolidColorBrush(c), null, inset, insetCr);

            // Top specular
            context.DrawRectangle(TopHighlightBrush, null,
                new Rect(inset.Left, inset.Top, inset.Width, inset.Height * 0.45),
                insetCr);
        }
        else
        {
            context.DrawRectangle(InsetOffBrush, null, inset, insetCr);
        }

        context.DrawText(label, new Point(lx, ly));
    }

    private FormattedText MakeLabel(IBrush brush, FontWeight weight) =>
        new(Label,
            CultureInfo.InvariantCulture,
            FlowDirection.LeftToRight,
            new Typeface(FontFamily.Default, FontStyle.Normal, weight),
            LabelFontSize,
            brush);
}
