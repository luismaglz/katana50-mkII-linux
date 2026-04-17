using Avalonia.Media;

namespace Kataka.App.Controls;

/// <summary>
/// Single source of truth for all UI colors used in custom-drawn controls.
/// AXAML views reference the same values via App.axaml resources (same hex, different mechanism).
/// </summary>
internal static class KatanaPalette
{
    // ── Surfaces ──────────────────────────────────────────────────────────────

    public static readonly Color WindowBg     = Color.Parse("#111317");
    public static readonly Color PanelBg      = Color.Parse("#17191d");
    public static readonly Color ShellBg      = Color.Parse("#1a1d22");
    public static readonly Color SectionBg    = Color.Parse("#20242a");
    public static readonly Color SectionBgAlt = Color.Parse("#252a31");
    public static readonly Color CardBgTop    = Color.Parse("#2e3138");
    public static readonly Color CardBgBottom = Color.Parse("#1c1f24");
    public static readonly Color ControlBg    = Color.Parse("#272b31");
    public static readonly Color KnobFace     = Color.Parse("#2c2f35");
    public static readonly Color KnobBezel    = Color.Parse("#0f1114");
    public static readonly Color LogBg        = Color.Parse("#0d0f12");

    // ── Borders ───────────────────────────────────────────────────────────────

    public static readonly Color BorderDim    = Color.Parse("#3a3f47");
    public static readonly Color BorderMid    = Color.Parse("#454b54");
    public static readonly Color BorderTrim   = Color.Parse("#4f5661");
    public static readonly Color BorderStrong = Color.Parse("#565b64");
    public static readonly Color BorderBright = Color.Parse("#60656e");

    // ── Accent ────────────────────────────────────────────────────────────────

    public static readonly Color Accent       = Color.Parse("#ffb741");
    public static readonly Color AccentLit    = Color.Parse("#ffc95d");
    public static readonly Color AccentValue  = Color.Parse("#ffcf66");
    public static readonly Color AccentBorder = Color.Parse("#f7cf6d");
    public static readonly Color AccentMuted  = Color.Parse("#7a5d23");
    public static readonly Color AccentSelect = Color.Parse("#d4c87a");

    // ── Text ──────────────────────────────────────────────────────────────────

    public static readonly Color Text         = Color.Parse("#f2eee3");
    public static readonly Color TextControl  = Color.Parse("#d8d5cb");
    public static readonly Color TextMuted    = Color.Parse("#b9b3a7");
    public static readonly Color TextDim      = Color.Parse("#7a8494");
    public static readonly Color TextOnAccent = Color.Parse("#111317");
    public static readonly Color TextLight    = Color.Parse("#f1f1f1");

    // ── Status ────────────────────────────────────────────────────────────────

    public static readonly Color LedOn        = Color.Parse("#00e676");
    public static readonly Color LedOff       = Color.Parse("#333333");
    public static readonly Color Boost        = Color.Parse("#3dbf6c");
    public static readonly Color Cut          = Color.Parse("#e05050");
    public static readonly Color Danger       = Color.Parse("#ff7b6b");

    // ── Knob internals ────────────────────────────────────────────────────────

    public static readonly Color KnobBg       = Color.Parse("#D9D3C1");
    public static readonly Color KnobShadow   = Color.FromArgb(0x28, 0, 0, 0);

    // ── Pre-built brushes (allocated once, reused across all renders) ─────────

    public static readonly SolidColorBrush WindowBgBrush     = new(WindowBg);
    public static readonly SolidColorBrush ControlBgBrush    = new(ControlBg);
    public static readonly SolidColorBrush KnobFaceBrush     = new(KnobFace);
    public static readonly SolidColorBrush KnobBgBrush       = new(KnobBg);
    public static readonly SolidColorBrush KnobShadowBrush   = new(KnobShadow);
    public static readonly SolidColorBrush AccentLitBrush    = new(AccentLit);
    public static readonly SolidColorBrush AccentValueBrush  = new(AccentValue);
    public static readonly SolidColorBrush TextBrush         = new(Text);
    public static readonly SolidColorBrush TextControlBrush  = new(TextControl);
    public static readonly SolidColorBrush TextDimBrush      = new(TextDim);
    public static readonly SolidColorBrush TextOnAccentBrush = new(TextOnAccent);
    public static readonly SolidColorBrush BoostBrush        = new(Boost);
    public static readonly SolidColorBrush CutBrush          = new(Cut);
    public static readonly SolidColorBrush ShellBgBrush      = new(ShellBg);

    // ── Pre-built pens ────────────────────────────────────────────────────────

    public static readonly Pen BorderDimPen    = new(new SolidColorBrush(BorderDim),    1.0);
    public static readonly Pen BorderStrongPen = new(new SolidColorBrush(BorderStrong), 1.5);
    public static readonly Pen KnobBezelPen    = new(new SolidColorBrush(KnobBezel),    2.0);
    public static readonly Pen KnobRimPen      = new(new SolidColorBrush(BorderStrong), 1.5);
}
