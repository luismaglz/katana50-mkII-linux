using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public partial class KatanaState
{
    /// <summary> Effect pedals (Temporary + Delay/Mod/FX/Reverb/PedalFx blocks) ────────── </summary>
    public AmpControlState PedalChain { get; } = new(KatanaMkIIParameterCatalog.ChainPattern);

    public BoostPedalState BoostPedal { get; } = new();
    public ModPedalState ModPedal { get; } = new();
    public FxPedalState FxPedal { get; } = new();
    public DelayPedalState DelayPedal { get; } = new();
    public Delay2PedalState Delay2Pedal { get; } = new();
    public ReverbPedalState ReverbPedal { get; } = new();
    public HardwarePedalState HardwarePedal { get; } = new();

    public IReadOnlyDictionary<string, AmpControlState> GetPedalDomainControlsByKey()
    {
        var dict = new Dictionary<string, AmpControlState>(StringComparer.Ordinal);

        void Add(AmpControlState s)
        {
            dict[s.Parameter.Key] = s;
        }

        // Booster
        Add(BoostPedal.EnabledState);
        Add(BoostPedal.Type);
        Add(BoostPedal.Variation);
        Add(BoostPedal.Drive);
        Add(BoostPedal.Tone);
        Add(BoostPedal.Bottom);
        Add(BoostPedal.SoloSw);
        Add(BoostPedal.SoloLevel);
        Add(BoostPedal.EffectLevel);
        Add(BoostPedal.BoosterDirectMix);

        // Delay
        Add(DelayPedal.EnabledState);
        Add(DelayPedal.Type);
        Add(DelayPedal.Variation);
        Add(DelayPedal.Level);
        Add(DelayPedal.Feedback);
        Add(DelayPedal.HighCut);
        Add(DelayPedal.EffectLevel);
        Add(DelayPedal.DirectMix);
        Add(DelayPedal.TapTime);
        Add(DelayPedal.ModRate);
        Add(DelayPedal.ModDepth);
        Add(DelayPedal.Range);
        Add(DelayPedal.Filter);
        Add(DelayPedal.FeedbackPhase);
        Add(DelayPedal.DelayPhase);
        Add(DelayPedal.ModSw);

        // Delay2
        Add(Delay2Pedal.EnabledState);
        Add(Delay2Pedal.Type);
        Add(Delay2Pedal.Feedback);
        Add(Delay2Pedal.HighCut);
        Add(Delay2Pedal.EffectLevel);
        Add(Delay2Pedal.DirectMix);
        Add(Delay2Pedal.TapTime);
        Add(Delay2Pedal.ModRate);
        Add(Delay2Pedal.ModDepth);
        Add(Delay2Pedal.Range);
        Add(Delay2Pedal.Filter);
        Add(Delay2Pedal.FeedbackPhase);
        Add(Delay2Pedal.DelayPhase);
        Add(Delay2Pedal.ModSw);

        // Reverb
        Add(ReverbPedal.EnabledState);
        Add(ReverbPedal.Type);
        Add(ReverbPedal.Variation);
        Add(ReverbPedal.Level);
        Add(ReverbPedal.Time);
        Add(ReverbPedal.PreDelay);
        Add(ReverbPedal.LowCut);
        Add(ReverbPedal.HighCut);
        Add(ReverbPedal.Density);
        Add(ReverbPedal.Color);
        Add(ReverbPedal.EffectLevel);
        Add(ReverbPedal.DirectMix);

        return dict;
    }

    partial void RegisterPedals()
    {
        RegisterAll(PedalChain);
        RegisterAll(BoostPedal);
        RegisterAll(ModPedal);
        RegisterAll(FxPedal);
        RegisterAll(DelayPedal);
        RegisterAll(Delay2Pedal);
        RegisterAll(ReverbPedal);
        RegisterAll(HardwarePedal);
    }
}
