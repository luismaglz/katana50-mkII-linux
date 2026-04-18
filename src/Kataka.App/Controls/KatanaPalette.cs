using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
///     Single source of truth for all UI colors used in custom-drawn controls.
///     AXAML views reference the same values via App.axaml resources (same hex, different mechanism).
/// </summary>
internal static class KatanaPalette
{
    // ── Surfaces (by depth) ──────────────────────────────────────────────────P
    public static readonly Color BgBase = Color.Parse("#111317"); // Window (deepest)
    public static readonly Color BgSurface = Color.Parse("#1A1D22"); // Panels, sections
    public static readonly Color BgElevated = Color.Parse("#272B31"); // Cards, controls
    public static readonly Color BgContrast = Color.Parse("#0D0F12"); // Log, inset areas

    // ── Brand accent ─────────────────────────────────────────────────────────
    public static readonly Color Primary = Color.Parse("#FFB741"); // Main amber
    public static readonly Color PrimaryLit = Color.Parse("#FFCF66"); // Lit / active state

    // ── Semantic status ──────────────────────────────────────────────────────
    public static readonly Color Success = Color.Parse("#3DBF6C"); // Boost
    public static readonly Color Error = Color.Parse("#E05050"); // Cut / danger
    public static readonly Color LedOn = Color.Parse("#00E676"); // LED active
    public static readonly Color LedOff = Color.Parse("#333333"); // LED inactive

    // ── Text ─────────────────────────────────────────────────────────────────
    public static readonly Color TextMain = Color.Parse("#F2EEE3"); // Primary text
    public static readonly Color TextMuted = Color.Parse("#7A8494"); // Secondary / dim

    // ── Borders ──────────────────────────────────────────────────────────────
    public static readonly Color Border = Color.Parse("#3A3F47"); // Default stroke
    public static readonly Color BorderLight = Color.Parse("#565B64"); // Highlight stroke

    // ── Knob-specific (not exposed in AXAML palette) ─────────────────────────
    public static readonly Color KnobBg = Color.Parse("#D9D3C1"); // Knob face fill
    public static readonly Color KnobShadow = Color.FromArgb(0x28, 0, 0, 0);

    // ── Pre-built brushes ────────────────────────────────────────────────────
    public static readonly SolidColorBrush BgBaseBrush = new(BgBase);
    public static readonly SolidColorBrush BgSurfaceBrush = new(BgSurface);
    public static readonly SolidColorBrush BgElevatedBrush = new(BgElevated);
    public static readonly SolidColorBrush BgContrastBrush = new(BgContrast);
    public static readonly SolidColorBrush PrimaryBrush = new(Primary);
    public static readonly SolidColorBrush PrimaryLitBrush = new(PrimaryLit);
    public static readonly SolidColorBrush SuccessBrush = new(Success);
    public static readonly SolidColorBrush ErrorBrush = new(Error);
    public static readonly SolidColorBrush LedOnBrush = new(LedOn);
    public static readonly SolidColorBrush LedOffBrush = new(LedOff);
    public static readonly SolidColorBrush TextMainBrush = new(TextMain);
    public static readonly SolidColorBrush TextMutedBrush = new(TextMuted);
    public static readonly SolidColorBrush TextOnPrimaryBrush = new(BgBase);
    public static readonly SolidColorBrush KnobFaceBrush = new(BgElevated);
    public static readonly SolidColorBrush KnobBgBrush = new(KnobBg);
    public static readonly SolidColorBrush KnobShadowBrush = new(KnobShadow);

    // ── Pre-built pens ────────────────────────────────────────────────────────
    public static readonly Pen ThinBorderPen = new(new SolidColorBrush(Border));
    public static readonly Pen StrokePen = new(new SolidColorBrush(BorderLight), 1.5);
    public static readonly Pen KnobBezelPen = new(new SolidColorBrush(BgBase), 2.0);
    public static readonly Pen KnobRimPen = new(new SolidColorBrush(BorderLight), 1.5);
}
