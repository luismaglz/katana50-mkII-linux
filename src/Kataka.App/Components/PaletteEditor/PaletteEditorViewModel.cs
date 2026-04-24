using Avalonia.Media;

using Kataka.App.Controls;

namespace Kataka.App.ViewModels;

public class PaletteEditorViewModel : ViewModelBase
{
    private string _accentHex;
    private string _bgBaseHex;
    private string _bgElevatedHex;
    private string _bgSurfaceHex;
    private string _knobFaceHex;

    public PaletteEditorViewModel()
    {
        _accentHex = ToHex(RuntimePaletteService.Accent);
        _knobFaceHex = ToHex(RuntimePaletteService.KnobFace);
        _bgBaseHex = ToHex(RuntimePaletteService.BgBase);
        _bgSurfaceHex = ToHex(RuntimePaletteService.BgSurface);
        _bgElevatedHex = ToHex(RuntimePaletteService.BgElevated);

        AccentSwatch = new SolidColorBrush(RuntimePaletteService.Accent);
        KnobFaceSwatch = new SolidColorBrush(RuntimePaletteService.KnobFace);
        BgBaseSwatch = new SolidColorBrush(RuntimePaletteService.BgBase);
        BgSurfaceSwatch = new SolidColorBrush(RuntimePaletteService.BgSurface);
        BgElevatedSwatch = new SolidColorBrush(RuntimePaletteService.BgElevated);
    }

    /// <summary> Hex inputs ──────────────────────────────────────────────────────────── </summary>
    public string AccentHex
    {
        get => _accentHex;
        set => SetColor(ref _accentHex, value, nameof(AccentHex),
            c =>
            {
                RuntimePaletteService.Accent = c;
                AccentSwatch.Color = c;
            });
    }

    public string KnobFaceHex
    {
        get => _knobFaceHex;
        set => SetColor(ref _knobFaceHex, value, nameof(KnobFaceHex),
            c =>
            {
                RuntimePaletteService.KnobFace = c;
                KnobFaceSwatch.Color = c;
            });
    }

    public string BgBaseHex
    {
        get => _bgBaseHex;
        set => SetColor(ref _bgBaseHex, value, nameof(BgBaseHex),
            c =>
            {
                RuntimePaletteService.BgBase = c;
                BgBaseSwatch.Color = c;
            });
    }

    public string BgSurfaceHex
    {
        get => _bgSurfaceHex;
        set => SetColor(ref _bgSurfaceHex, value, nameof(BgSurfaceHex),
            c =>
            {
                RuntimePaletteService.BgSurface = c;
                BgSurfaceSwatch.Color = c;
            });
    }

    public string BgElevatedHex
    {
        get => _bgElevatedHex;
        set => SetColor(ref _bgElevatedHex, value, nameof(BgElevatedHex),
            c =>
            {
                RuntimePaletteService.BgElevated = c;
                BgElevatedSwatch.Color = c;
            });
    }

    /// <summary> Swatch brushes (mutated in-place so bound controls re-render) ───────── </summary>
    public SolidColorBrush AccentSwatch { get; }

    public SolidColorBrush KnobFaceSwatch { get; }
    public SolidColorBrush BgBaseSwatch { get; }
    public SolidColorBrush BgSurfaceSwatch { get; }
    public SolidColorBrush BgElevatedSwatch { get; }

    /// <summary> Helpers ─────────────────────────────────────────────────────────────── </summary>
    private void SetColor(ref string field, string value, string propName, Action<Color> apply)
    {
        if (!ChangeProperty(ref field, value, propName)) return;
        try
        {
            var hex = value.TrimStart();
            if (!hex.StartsWith('#')) hex = '#' + hex;
            apply(Color.Parse(hex));
        }
        catch
        {
            /* partial / invalid hex — ignore */
        }
    }

    private static string ToHex(Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}
