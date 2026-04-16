using Kataka.Domain.KatanaState.FxPedals;
using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState;

public class KatanaState
{
    #region Pedals

    public AmpControlState PedalChain = new(KatanaMkIIParameterCatalog.ChainPattern);
    public BoostPedalState BoostPedal = new();
    public ModPedalState ModPedal = new();
    public FxPedalState FxPedal = new();
    public DelayPedalState DelayPedal = new();
    public Delay2PedalState Delay2Pedal = new();
    public ReverbPedalState ReverbPedal = new();

    #endregion

    #region Amplifier Settings

    public AmpControlState AmpType = new(KatanaMkIIParameterCatalog.AmpType);
    public AmpControlState AmpVariation = new(KatanaMkIIParameterCatalog.AmpVariation);
    public AmpControlState Gain = new(KatanaMkIIParameterCatalog.AmpGain);
    public AmpControlState Volume = new(KatanaMkIIParameterCatalog.AmpVolume);

    #endregion

    #region Equalizer Settings

    public AmpControlState Bass = new(KatanaMkIIParameterCatalog.AmpBass);
    public AmpControlState Middle = new(KatanaMkIIParameterCatalog.AmpMiddle);
    public AmpControlState Treble = new(KatanaMkIIParameterCatalog.AmpTreble);

    #endregion

    #region Tone Settings

    public AmpControlState Presence = new(KatanaMkIIParameterCatalog.AmpPresence);
    public AmpControlState CabinetResonance = new(KatanaMkIIParameterCatalog.CabinetResonance);

    #endregion

    /// <summary>
    /// Returns all top-level amp control states (EQ, gain, tone) keyed by parameter key.
    /// Used by the service to keep domain state in sync with amp reads.
    /// </summary>
    public IReadOnlyDictionary<string, AmpControlState> GetAmpControlsByKey() =>
        new Dictionary<string, AmpControlState>(StringComparer.Ordinal)
        {
            [AmpType.Parameter.Key]          = AmpType,
            [AmpVariation.Parameter.Key]     = AmpVariation,
            [Gain.Parameter.Key]             = Gain,
            [Volume.Parameter.Key]           = Volume,
            [Bass.Parameter.Key]             = Bass,
            [Middle.Parameter.Key]           = Middle,
            [Treble.Parameter.Key]           = Treble,
            [Presence.Parameter.Key]         = Presence,
            [CabinetResonance.Parameter.Key] = CabinetResonance,
            [PedalChain.Parameter.Key]       = PedalChain,
        };

    /// <summary>
    /// Returns all parameter states from the domain-migrated pedal effects (Booster, Delay, Delay2, Reverb)
    /// keyed by parameter key. Used by the service to route amp reads and subscriptions without touching VMs.
    /// ModPedal and FxPedal are excluded — they remain on the VM path until Phase 5.
    /// </summary>
    public IReadOnlyDictionary<string, AmpControlState> GetPedalDomainControlsByKey()
    {
        var dict = new Dictionary<string, AmpControlState>(StringComparer.Ordinal);
        void Add(AmpControlState s) => dict[s.Parameter.Key] = s;

        // Booster
        Add(BoostPedal.EnabledState); Add(BoostPedal.Type); Add(BoostPedal.Variation);
        Add(BoostPedal.Drive); Add(BoostPedal.Tone); Add(BoostPedal.Bottom);
        Add(BoostPedal.SoloSw); Add(BoostPedal.SoloLevel); Add(BoostPedal.EffectLevel);
        Add(BoostPedal.BoosterDirectMix);

        // Delay
        Add(DelayPedal.EnabledState); Add(DelayPedal.Type); Add(DelayPedal.Variation); Add(DelayPedal.Level);
        Add(DelayPedal.Feedback); Add(DelayPedal.HighCut); Add(DelayPedal.EffectLevel);
        Add(DelayPedal.DirectMix); Add(DelayPedal.TapTime); Add(DelayPedal.ModRate);
        Add(DelayPedal.ModDepth); Add(DelayPedal.Range); Add(DelayPedal.Filter);
        Add(DelayPedal.FeedbackPhase); Add(DelayPedal.DelayPhase); Add(DelayPedal.ModSw);

        // Delay2
        Add(Delay2Pedal.EnabledState); Add(Delay2Pedal.Type);
        Add(Delay2Pedal.Feedback); Add(Delay2Pedal.HighCut); Add(Delay2Pedal.EffectLevel);
        Add(Delay2Pedal.DirectMix); Add(Delay2Pedal.TapTime); Add(Delay2Pedal.ModRate);
        Add(Delay2Pedal.ModDepth); Add(Delay2Pedal.Range); Add(Delay2Pedal.Filter);
        Add(Delay2Pedal.FeedbackPhase); Add(Delay2Pedal.DelayPhase); Add(Delay2Pedal.ModSw);

        // Reverb
        Add(ReverbPedal.EnabledState); Add(ReverbPedal.Type); Add(ReverbPedal.Variation); Add(ReverbPedal.Level);
        Add(ReverbPedal.Time); Add(ReverbPedal.PreDelay); Add(ReverbPedal.LowCut); Add(ReverbPedal.HighCut);
        Add(ReverbPedal.Density); Add(ReverbPedal.Color); Add(ReverbPedal.EffectLevel); Add(ReverbPedal.DirectMix);

        return dict;
    }
}
