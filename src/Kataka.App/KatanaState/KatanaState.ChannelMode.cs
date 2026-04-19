using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public partial class KatanaState
{
    // ── Channel-mode stored preamp values (Temporary + Patch0 / Mk2V2 blocks) ───
    public PreampState Preamp { get; } = new();
    public SoloEqState SoloEq { get; } = new();
    public PatchEqState PatchEq1 { get; } = new();
    public PatchEqState PatchEq2 { get; } = new(true);

    public ContourState Contour1 { get; } = new(KatanaMkIIParameterCatalog.Contour1Type,
        KatanaMkIIParameterCatalog.Contour1FreqShift);

    public ContourState Contour2 { get; } = new(KatanaMkIIParameterCatalog.Contour2Type,
        KatanaMkIIParameterCatalog.Contour2FreqShift);

    public ContourState Contour3 { get; } = new(KatanaMkIIParameterCatalog.Contour3Type,
        KatanaMkIIParameterCatalog.Contour3FreqShift);

    partial void RegisterChannelMode()
    {
        RegisterAll(Preamp);
        RegisterAll(SoloEq);
        RegisterAll(PatchEq1);
        RegisterAll(PatchEq2);
        RegisterAll(Contour1);
        RegisterAll(Contour2);
        RegisterAll(Contour3);
    }
}
