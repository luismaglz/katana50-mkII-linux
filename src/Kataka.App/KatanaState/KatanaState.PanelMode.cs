using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public partial class KatanaState
{
    public AmpControlState LedStateBooster { get; } = new(KatanaMkIIParameterCatalog.LedStateBooster);
    public AmpControlState LedStateMod { get; } = new(KatanaMkIIParameterCatalog.LedStateMod);
    public AmpControlState LedStateFx { get; } = new(KatanaMkIIParameterCatalog.LedStateFx);
    public AmpControlState LedStateDelay { get; } = new(KatanaMkIIParameterCatalog.LedStateDelay);
    public AmpControlState LedStateReverb { get; } = new(KatanaMkIIParameterCatalog.LedStateReverb);

    /// <summary> Panel-mode knob positions (Temporary + PatchStatus block) ─────────────── </summary>
    public AmpControlState AmpType { get; } = new(KatanaMkIIParameterCatalog.AmpType);

    public AmpControlState AmpVariation { get; } = new(KatanaMkIIParameterCatalog.AmpVariation);
    public AmpControlState Gain { get; } = new(KatanaMkIIParameterCatalog.AmpGain);
    public AmpControlState Volume { get; } = new(KatanaMkIIParameterCatalog.AmpVolume);
    public AmpControlState Bass { get; } = new(KatanaMkIIParameterCatalog.AmpBass);
    public AmpControlState Middle { get; } = new(KatanaMkIIParameterCatalog.AmpMiddle);
    public AmpControlState Treble { get; } = new(KatanaMkIIParameterCatalog.AmpTreble);
    public AmpControlState Presence { get; } = new(KatanaMkIIParameterCatalog.AmpPresence);
    public AmpControlState CabinetResonance { get; } = new(KatanaMkIIParameterCatalog.CabinetResonance);
    public AmpControlState PatchLevel { get; } = new(KatanaMkIIParameterCatalog.PatchLevel, 0, 200);

    /// <summary>Current channel as a raw SysEx byte value (see KatanaMkIIParameterCatalog.Channel* constants).</summary>
    public AmpControlState CurrentChannel { get; } = new(KatanaMkIIParameterCatalog.CurrentChannel);

    /// <summary>16-character patch name; public field so RegisterAll finds it via GetFields().</summary>
    public PatchNameState PatchName = new();

    public string CurrentPatchName => PatchName.CurrentName;

    public event Action? PatchNameChanged;

    partial void RegisterPanelMode()
    {
        RegisterAll(AmpType);
        RegisterAll(AmpVariation);
        RegisterAll(LedStateBooster);
        RegisterAll(LedStateMod);
        RegisterAll(LedStateFx);
        RegisterAll(LedStateDelay);
        RegisterAll(LedStateReverb);
        RegisterAll(Gain);
        RegisterAll(Volume);
        RegisterAll(Bass);
        RegisterAll(Middle);
        RegisterAll(Treble);
        RegisterAll(Presence);
        RegisterAll(CabinetResonance);
        RegisterAll(PatchLevel);
        RegisterAll(CurrentChannel);
        RegisterAll(PatchName);

        PatchName.NameChanged += () => PatchNameChanged?.Invoke();
    }
}

