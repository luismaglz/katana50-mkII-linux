namespace Kataka.Domain.Midi;

public static class KatanaMkIIParameterCatalog
{
    public static KatanaParameterDefinition AmpGain { get; } =
        new("amp-gain", "Gain", [0x60, 0x00, 0x06, 0x51]);

    public static KatanaParameterDefinition AmpVolume { get; } =
        new("amp-volume", "Volume", [0x60, 0x00, 0x06, 0x52]);

    public static KatanaParameterDefinition Bass { get; } =
        new("amp-bass", "Bass", [0x60, 0x00, 0x00, 0x24]);

    public static KatanaParameterDefinition Middle { get; } =
        new("amp-middle", "Middle", [0x60, 0x00, 0x00, 0x25]);

    public static KatanaParameterDefinition Treble { get; } =
        new("amp-treble", "Treble", [0x60, 0x00, 0x00, 0x26]);

    public static KatanaParameterDefinition Presence { get; } =
        new("amp-presence", "Presence", [0x60, 0x00, 0x00, 0x28]);

    public static IReadOnlyList<KatanaParameterDefinition> AmpEditorControls { get; } =
    [
        AmpGain,
        AmpVolume,
        Bass,
        Middle,
        Treble,
        Presence,
    ];

    public static KatanaParameterDefinition BoosterSwitch { get; } =
        new("panel-booster-switch", "Booster", [0x60, 0x00, 0x00, 0x10], maximum: 1);

    public static KatanaParameterDefinition ModSwitch { get; } =
        new("panel-mod-switch", "Mod", [0x60, 0x00, 0x01, 0x00], maximum: 1);

    public static KatanaParameterDefinition FxSwitch { get; } =
        new("panel-fx-switch", "FX", [0x60, 0x00, 0x03, 0x00], maximum: 1);

    public static KatanaParameterDefinition DelaySwitch { get; } =
        new("panel-delay-switch", "Delay", [0x60, 0x00, 0x05, 0x00], maximum: 1);

    public static KatanaParameterDefinition Delay2Switch { get; } =
        new("panel-delay2-switch", "Delay 2", [0x60, 0x00, 0x05, 0x20], maximum: 1);

    public static KatanaParameterDefinition ReverbSwitch { get; } =
        new("panel-reverb-switch", "Reverb", [0x60, 0x00, 0x05, 0x40], maximum: 1);

    public static KatanaParameterDefinition BoosterVariation { get; } =
        new("panel-booster-variation", "Booster Variation", [0x60, 0x00, 0x06, 0x39], maximum: 2);

    public static KatanaParameterDefinition ModVariation { get; } =
        new("panel-mod-variation", "Mod Variation", [0x60, 0x00, 0x06, 0x3A], maximum: 2);

    public static KatanaParameterDefinition FxVariation { get; } =
        new("panel-fx-variation", "FX Variation", [0x60, 0x00, 0x06, 0x3B], maximum: 2);

    public static KatanaParameterDefinition DelayVariation { get; } =
        new("panel-delay-variation", "Delay Variation", [0x60, 0x00, 0x06, 0x3C], maximum: 2);

    public static KatanaParameterDefinition ReverbVariation { get; } =
        new("panel-reverb-variation", "Reverb Variation", [0x60, 0x00, 0x06, 0x3D], maximum: 2);

    public static IReadOnlyList<KatanaPanelEffectDefinition> PanelEffects { get; } =
    [
        new("booster", "Booster", BoosterSwitch, BoosterVariation),
        new("mod", "Mod", ModSwitch, ModVariation),
        new("fx", "FX", FxSwitch, FxVariation),
        new("delay", "Delay", DelaySwitch, DelayVariation),
        new("delay2", "Delay 2", Delay2Switch, ReverbVariation),
        new("reverb", "Reverb", ReverbSwitch, ReverbVariation),
    ];
}
