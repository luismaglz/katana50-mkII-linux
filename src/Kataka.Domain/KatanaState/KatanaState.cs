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
}
