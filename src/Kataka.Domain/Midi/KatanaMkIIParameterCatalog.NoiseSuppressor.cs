namespace Kataka.Domain.Midi;

public static partial class KatanaMkIIParameterCatalog
{
    public static KatanaParameterDefinition NoiseSuppressorSw { get; } =
        new("ns1-sw", "NS On",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary, KatanaAddressMap.PatchBlocks.Patch1,
                KatanaAddressMap.Patch1Params.Ns1Sw),
            maximum: 1);

    public static KatanaParameterDefinition NoiseSuppressorThreshold { get; } =
        new("ns1-threshold", "Threshold",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary, KatanaAddressMap.PatchBlocks.Patch1,
                KatanaAddressMap.Patch1Params.Ns1Threshold),
            maximum: 100);

    public static KatanaParameterDefinition NoiseSuppressorRelease { get; } =
        new("ns1-release", "Release",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary, KatanaAddressMap.PatchBlocks.Patch1,
                KatanaAddressMap.Patch1Params.Ns1Release),
            maximum: 100);
}
