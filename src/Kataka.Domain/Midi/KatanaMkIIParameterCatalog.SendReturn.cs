namespace Kataka.Domain.Midi;

public static partial class KatanaMkIIParameterCatalog
{
    public static KatanaParameterDefinition SendReturnSw { get; } =
        new("sr-sw", "Send/Return On",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary,
                KatanaAddressMap.PatchBlocks.Patch1, KatanaAddressMap.Patch1Params.SendReturnSw),
            maximum: 1);

    public static KatanaParameterDefinition SendReturnMode { get; } =
        new("sr-mode", "Send/Return Mode",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary,
                KatanaAddressMap.PatchBlocks.Patch1, KatanaAddressMap.Patch1Params.SendReturnMode),
            maximum: 1);

    public static KatanaParameterDefinition SendReturnSendLevel { get; } =
        new("sr-send-level", "Send Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary,
                KatanaAddressMap.PatchBlocks.Patch1, KatanaAddressMap.Patch1Params.SendReturnSendLevel),
            maximum: 100);

    public static KatanaParameterDefinition SendReturnReturnLevel { get; } =
        new("sr-return-level", "Return Level",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary,
                KatanaAddressMap.PatchBlocks.Patch1, KatanaAddressMap.Patch1Params.SendReturnReturnLevel),
            maximum: 100);

    public static KatanaParameterDefinition PatchEq2Position { get; } =
        new("patch-eq2-position", "EQ Position",
            KatanaAddressMap.ComputeAddress(KatanaAddressMap.Temporary,
                KatanaAddressMap.PatchBlocks.Patch1, KatanaAddressMap.Patch1Params.Eq2Position),
            maximum: 1);
}
