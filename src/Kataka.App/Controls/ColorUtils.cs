using Avalonia.Media;

namespace Kataka.App.Controls;

public static class ColorUtils
{
    // Rec. 601 perceived luminance — weights RGB by how sensitive human eyes are to each channel.
    public static float PerceivedLuminance(Color c) =>
        (0.299f * c.R + 0.587f * c.G + 0.114f * c.B) / 255f;

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
