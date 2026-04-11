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
}
