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
}
