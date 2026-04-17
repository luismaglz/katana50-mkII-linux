using System.Collections.Generic;

namespace Kataka.App.KatanaState;

public interface IKatanaState
{
    AmpControlState PedalChain { get; }
    BoostPedalState BoostPedal { get; }
    ModPedalState ModPedal { get; }
    FxPedalState FxPedal { get; }
    DelayPedalState DelayPedal { get; }
    Delay2PedalState Delay2Pedal { get; }
    ReverbPedalState ReverbPedal { get; }
    AmpControlState AmpType { get; }
    AmpControlState AmpVariation { get; }
    AmpControlState Gain { get; }
    AmpControlState Volume { get; }
    AmpControlState Bass { get; }
    AmpControlState Middle { get; }
    AmpControlState Treble { get; }
    AmpControlState Presence { get; }
    AmpControlState CabinetResonance { get; }

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
    /// Set values from a response from the amp
    /// </summary>
    /// <param name="values"></param>
    public void SetStates(IReadOnlyDictionary<string, byte> values);

    /// <summary>
    /// Set individual control value
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetState(string key, byte value);
}
