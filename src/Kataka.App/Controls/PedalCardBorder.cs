using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Kataka.App.Controls;

/// <summary>
///     A container control that renders the Boss Katana pedal card visual — coloured rounded-rect
///     with drop shadow, brushed-metal texture overlay, and a subtle top highlight — as a background
///     behind its single child. Behaves exactly like <see cref="Avalonia.Controls.Border"/> but
///     with the pedal-card aesthetic baked in.
///
///     Usage:
///     <code>
///         &lt;controls:PedalCardBorder Background="{Binding CardBackgroundBrush}" CornerRadius="8"&gt;
///             &lt;StackPanel&gt;…&lt;/StackPanel&gt;
///         &lt;/controls:PedalCardBorder&gt;
///     </code>
///
///     The drop shadow renders outside the control's layout bounds; add an appropriate
///     <see cref="Avalonia.Controls.Control.Margin"/> if you need the shadow to be fully visible.
/// </summary>
public sealed class PedalCardBorder : Decorator
{
    // Translated from the SVG filter values:
    //   feOffset dx="30.99" dy="15.49"
    //   feGaussianBlur stdDeviation="54.23"  → CSS blur = stdDev × 2 ≈ 108
    //   feColorMatrix → black at 43 % opacity (0.43 × 255 ≈ 110)
    private static readonly BoxShadows DropShadow = new(new BoxShadow
    {
        OffsetX = 31, OffsetY = 15, Blur = 108, Spread = 0,
        Color = Color.FromArgb(110, 0, 0, 0)
    });

    // Approximates the SVG inner shadow: dy=2.58, white at 25 % opacity
    private static readonly SolidColorBrush TopHighlightBrush =
        new(Color.FromArgb(64, 255, 255, 255));

    // Lazy-loaded so startup cost is deferred; shared across all instances.
    private static readonly Lazy<Bitmap?> TextureBitmap = new(LoadTexture);

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<PedalCardBorder, IBrush?>(
            nameof(Background),
            new SolidColorBrush(Color.Parse("#FE9213")));

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<PedalCardBorder, CornerRadius>(
            nameof(CornerRadius),
            new CornerRadius(24.5));

    static PedalCardBorder()
    {
        AffectsRender<PedalCardBorder>(BackgroundProperty, CornerRadiusProperty);
    }

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        var cr = CornerRadius;
        var bounds = new Rect(Bounds.Size);
        var rrect = new RoundedRect(
            bounds,
            new Point(cr.TopLeft, cr.TopLeft),
            new Point(cr.TopRight, cr.TopRight),
            new Point(cr.BottomRight, cr.BottomRight),
            new Point(cr.BottomLeft, cr.BottomLeft));

        // Layer 1: drop shadow + coloured background
        if (Background is not null)
            context.DrawRectangle(Background, null, rrect, DropShadow);

        // Layer 2: brushed-metal texture at 20 % opacity, clipped to card shape
        //          (approximates the SVG mix-blend-mode:overlay at fill-opacity:0.2)
        var texture = TextureBitmap.Value;
        if (texture is not null)
        {
            using (context.PushClip(rrect))
            using (context.PushOpacity(0.2))
                context.DrawImage(texture, bounds);
        }

        // Layer 3: 3 px white highlight strip at the top edge — approximates SVG inner shadow
        using (context.PushClip(rrect))
            context.DrawRectangle(TopHighlightBrush, null,
                new Rect(bounds.X, bounds.Y, bounds.Width, 3));

        // Children are rendered on top by the layout system after Render returns.
    }

    private static Bitmap? LoadTexture()
    {
        try
        {
            using var stream = AssetLoader.Open(
                new Uri("avares://Kataka.App/Assets/pedal-texture.jpg"));
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }
}
