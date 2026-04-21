using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public interface IKatanaState
{
    /// <summary> Panel-mode knob positions ──────────────────────────────────────────────── </summary>
    AmpControlState AmpType { get; }

    AmpControlState AmpVariation { get; }
    AmpControlState Gain { get; }
    AmpControlState Volume { get; }
    AmpControlState Bass { get; }
    AmpControlState Middle { get; }
    AmpControlState Treble { get; }
    AmpControlState Presence { get; }
    AmpControlState CabinetResonance { get; }
    AmpControlState PatchLevel { get; }

    // -- Channel
    AmpControlState CurrentChannel { get; }

    /// <summary>Currently displayed patch name, trimmed of trailing spaces.</summary>
    string CurrentPatchName { get; }

    event Action? PatchNameChanged;

    /// <summary> Channel-mode stored preamp values ──────────────────────────────────────── </summary>
    PreampState Preamp { get; }

    /// <summary> Pedals ─────────────────────────────────────────────────────────────────── </summary>
    AmpControlState PedalChain { get; }

    BoostPedalState BoostPedal { get; }
    ModPedalState ModPedal { get; }
    FxPedalState FxPedal { get; }
    DelayPedalState DelayPedal { get; }
    Delay2PedalState Delay2Pedal { get; }
    ReverbPedalState ReverbPedal { get; }
    HardwarePedalState HardwarePedal { get; }

    /// <summary> Ver200+ features ───────────────────────────────────────────────────────── </summary>
    SoloEqState SoloEq { get; }
    GlobalEqState GlobalEq { get; }

    ContourState Contour1 { get; }
    ContourState Contour2 { get; }
    ContourState Contour3 { get; }

    /// <summary>
    ///     Returns all top-level amp control states (EQ, gain, tone) keyed by parameter key.
    ///     Used by the service to keep domain state in sync with amp reads.
    /// </summary>
    IReadOnlyDictionary<string, AmpControlState> GetAmpControlsByKey();

    /// <summary>
    ///     Returns all parameter states from the domain-migrated pedal effects (Booster, Delay, Delay2, Reverb)
    ///     keyed by parameter key. Used by the service to route amp reads and subscriptions without touching VMs.
    ///     ModPedal and FxPedal are excluded — they remain on the VM path until Phase 5.
    /// </summary>
    IReadOnlyDictionary<string, AmpControlState> GetPedalDomainControlsByKey();

    /// <summary>
    ///     Set values from a response from the amp
    /// </summary>
    /// <param name="values"></param>
    public void SetStates(IReadOnlyDictionary<string, byte> values);

    /// <summary>
    ///     Set individual control value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetState(string key, byte value);

    /// <summary>
    ///     Set individual control value from a multi-byte SysEx payload.
    ///     For ByteSize==2 parameters: decodes INTEGER2x7 as (bytes[0] &lt;&lt; 7) | bytes[1].
    /// </summary>
    public void SetState(string key, byte[] bytes);

    /// <summary>Returns all registered parameter states keyed by AddressString (e.g. "60-00-06-52").</summary>
    IReadOnlyDictionary<string, AmpControlState> GetAllRegisteredStates();
}
