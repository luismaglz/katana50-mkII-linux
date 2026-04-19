using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;

using AvaloniaApplication = Avalonia.Application;

namespace Kataka.App.Controls;

public static class RuntimePaletteService
{
    private static Color _accent = KatanaPalette.PrimaryLit;
    private static Color _knobFace = KatanaPalette.KnobBg;
    private static Color _bgBase = KatanaPalette.BgBase;
    private static Color _bgSurface = KatanaPalette.BgSurface;
    private static Color _bgElevated = KatanaPalette.BgElevated;

    public static Color Accent
    {
        get => _accent;
        set
        {
            _accent = value;
            Publish();
        }
    }

    public static Color KnobFace
    {
        get => _knobFace;
        set
        {
            _knobFace = value;
            Publish();
        }
    }

    public static Color BgBase
    {
        get => _bgBase;
        set
        {
            _bgBase = value;
            Publish();
        }
    }

    public static Color BgSurface
    {
        get => _bgSurface;
        set
        {
            _bgSurface = value;
            Publish();
        }
    }

    public static Color BgElevated
    {
        get => _bgElevated;
        set
        {
            _bgElevated = value;
            Publish();
        }
    }

    public static event Action? Changed;

    private static void Publish()
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.Post(Publish);
            return;
        }

        ApplyToResources();
        Changed?.Invoke();
    }

    private static void ApplyToResources()
    {
        var res = AvaloniaApplication.Current?.Resources;
        if (res is null) return;

        SetBrush(res, "PrimaryLitBrush", _accent);
        SetBrush(res, "PrimaryBrush", _accent);
        SetBrush(res, "KatanaAccentBrush", _accent);
        SetBrush(res, "BgBaseBrush", _bgBase);
        SetBrush(res, "BgSurfaceBrush", _bgSurface);
        SetBrush(res, "BgElevatedBrush", _bgElevated);
    }

    private static void SetBrush(IResourceDictionary res, string key, Color color)
    {
        if (res.TryGetResource(key, null, out var obj) && obj is SolidColorBrush brush)
            brush.Color = color;
    }
}
