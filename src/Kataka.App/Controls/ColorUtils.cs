using Avalonia.Media;

namespace Kataka.App.Controls;

public static class ColorUtils
{
    // Returns pure black or white — whichever is more readable on the given background.
    public static Color GetBestContrast(Color bg)
    {
        double yiq = (bg.R * 299 + bg.G * 587 + bg.B * 114) / 1000.0;
        return yiq >= 128 ? Colors.Black : Colors.White;
    }

    public static Color DeriveLabel(Color bg) => GetBestContrast(bg);

    public static Color DeriveValue(Color bg) => GetBestContrast(bg);

    public static Color DeriveAccent(Color bg) => GetBestContrast(bg);
}
