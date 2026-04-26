using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Kataka.App.Controls;

/// <summary>
///     A container control that renders the Boss Katana pedal card visual — coloured rounded-rect
///     with drop shadow, brushed-metal texture overlay, and an inset bevel — as a background
///     behind its single child. Corner radius is enforced via a <see cref="Visual.Clip" /> geometry
///     set in <see cref="ArrangeOverride" /> so it reliably clips both the background fill and child
///     content regardless of the rendering backend.
/// </summary>
public sealed class PedalCardBorder : Decorator
{
    private static readonly BoxShadows DropShadow = new(
        new BoxShadow { OffsetX = 31, OffsetY = 15, Blur = 108, Spread = 0, Color = Color.FromArgb(110, 0, 0, 0) });

    private static readonly BoxShadows InsetShadows = new(
        new BoxShadow { OffsetX = 0, OffsetY = 0, Blur = 10, Spread = 2, IsInset = true, Color = Color.FromArgb(120, 255, 255, 255) },
        [new BoxShadow { OffsetX = 0, OffsetY = 0, Blur = 10, Spread = 2, IsInset = true, Color = Color.FromArgb(120, 0, 0, 0) }]);

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
        // CornerRadius change must re-run ArrangeOverride so the Clip geometry is rebuilt.
        AffectsArrange<PedalCardBorder>(CornerRadiusProperty);
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

    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);

        // Build a rounded-rect clip geometry. This is the most reliable way to get rounded
        // corners: the compositor applies it after Render, clipping both the fill and children.
        var r = CornerRadius.TopLeft;
        var w = result.Width;
        var h = result.Height;
        var geo = new StreamGeometry();
        using (var ctx = geo.Open())
        {
            ctx.BeginFigure(new Point(r, 0), true);
            ctx.LineTo(new Point(w - r, 0));
            ctx.ArcTo(new Point(w, r), new Size(r, r), 0, false, SweepDirection.Clockwise);
            ctx.LineTo(new Point(w, h - r));
            ctx.ArcTo(new Point(w - r, h), new Size(r, r), 0, false, SweepDirection.Clockwise);
            ctx.LineTo(new Point(r, h));
            ctx.ArcTo(new Point(0, h - r), new Size(r, r), 0, false, SweepDirection.Clockwise);
            ctx.LineTo(new Point(0, r));
            ctx.ArcTo(new Point(r, 0), new Size(r, r), 0, false, SweepDirection.Clockwise);
        }
        Clip = geo;

        return result;
    }

    public override void Render(DrawingContext context)
    {
        var bounds = new Rect(Bounds.Size);
        var r = CornerRadius.TopLeft;
        var rrect = new RoundedRect(bounds, r, r);

        // Outer drop shadow — draw before the clip so it isn't masked.
        context.DrawRectangle(null, null, bounds, r, r, DropShadow);

        // Everything inside the rounded clip: background, inset bevel, texture.
        using (context.PushClip(rrect))
        {
            if (Background is not null)
                context.DrawRectangle(Background, null, bounds);

            context.DrawRectangle(Brushes.Transparent, null, bounds, r, r, InsetShadows);

            var texture = TextureBitmap.Value;
            if (texture is not null)
                using (context.PushOpacity(0.2))
                    context.DrawImage(texture, bounds);
        }
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
