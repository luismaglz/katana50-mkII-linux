using System;
using System.Collections.Generic;
using System.Reflection;

using Kataka.Domain.Midi;

using Microsoft.Extensions.Logging;

namespace Kataka.App.KatanaState;

public class KatanaState : IKatanaState
{
    private readonly ILogger<KatanaState> _logger;

    private readonly Dictionary<string, AmpControlState> _stateFields = new();

    public KatanaState(ILogger<KatanaState> logger)
    {
        _logger = logger;

        RegisterAll(AmpType);
        RegisterAll(AmpVariation);
        RegisterAll(Gain);
        RegisterAll(Volume);
        RegisterAll(Bass);
        RegisterAll(Middle);
        RegisterAll(Treble);
        RegisterAll(Presence);
        RegisterAll(CabinetResonance);
        RegisterAll(PatchLevel);
        RegisterAll(PedalChain);
        RegisterAll(Preamp);
        RegisterAll(BoostPedal);
        RegisterAll(ModPedal);
        RegisterAll(FxPedal);
        RegisterAll(DelayPedal);
        RegisterAll(Delay2Pedal);
        RegisterAll(ReverbPedal);
        RegisterAll(HardwarePedal);
        RegisterAll(SoloEq);
        RegisterAll(Contour1);
        RegisterAll(Contour2);
        RegisterAll(Contour3);
    }

    /// <summary>
    ///     Returns all top-level amp control states (EQ, gain, tone) keyed by parameter key.
    ///     Used by the service to keep domain state in sync with amp reads.
    /// </summary>
    public IReadOnlyDictionary<string, AmpControlState> GetAmpControlsByKey() =>
        new Dictionary<string, AmpControlState>(StringComparer.Ordinal)
        {
            [AmpType.Parameter.Key] = AmpType,
            [AmpVariation.Parameter.Key] = AmpVariation,
            [Gain.Parameter.Key] = Gain,
            [Volume.Parameter.Key] = Volume,
            [Bass.Parameter.Key] = Bass,
            [Middle.Parameter.Key] = Middle,
            [Treble.Parameter.Key] = Treble,
            [Presence.Parameter.Key] = Presence,
            [CabinetResonance.Parameter.Key] = CabinetResonance,
            [PedalChain.Parameter.Key] = PedalChain
        };

    /// <summary>
    ///     Returns all parameter states from the domain-migrated pedal effects (Booster, Delay, Delay2, Reverb)
    ///     keyed by parameter key. Used by the service to route amp reads and subscriptions without touching VMs.
    ///     ModPedal and FxPedal are excluded — they remain on the VM path until Phase 5.
    /// </summary>
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


    public void SetStates(IReadOnlyDictionary<string, byte> values)
    {
        foreach (var keyValuePair in values) SetState(keyValuePair.Key, keyValuePair.Value);
    }

    public void SetState(string key, byte value)
    {
        if (_stateFields.TryGetValue(key, out var state))
        {
            state.Value = value;
            _logger.LogInformation("{Name} : {Value} Refreshed", key, value);
        }
        else
        {
            _logger.LogWarning("Received update for unknown parameter key: {Key}", key);
        }
    }

    #region Amplifier Settings — Channel Mode (Preamp stored values)

    public PreampState Preamp { get; } = new();

    #endregion

    #region Hardware Pedal (Wah / Pedal Bend / EVH95)

    public HardwarePedalState HardwarePedal { get; } = new();

    #endregion

    #region Solo EQ + Solo Delay (Ver200+/Ver210+)

    public SoloEqState SoloEq { get; } = new();

    #endregion

    private void RegisterAll(object obj)
    {
        // Direct AmpControlState (top-level properties on KatanaState itself)
        if (obj is AmpControlState direct)
        {
            _stateFields.TryAdd(direct.Parameter.AddressString, direct);
            return;
        }

        foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            if (field.GetValue(obj) is AmpControlState state)
                _stateFields.TryAdd(state.Parameter.AddressString, state);
            else if (field.FieldType.Namespace?.StartsWith("Kataka") == true
                     && field.GetValue(obj) is { } nested)
                RegisterAll(nested);
    }

    #region Pedals

    public AmpControlState PedalChain { get; } = new(KatanaMkIIParameterCatalog.ChainPattern);
    public BoostPedalState BoostPedal { get; } = new();
    public ModPedalState ModPedal { get; } = new();
    public FxPedalState FxPedal { get; } = new();
    public DelayPedalState DelayPedal { get; } = new();
    public Delay2PedalState Delay2Pedal { get; } = new();
    public ReverbPedalState ReverbPedal { get; } = new();

    #endregion

    #region Amplifier Settings — Panel Mode (KNOB_POS)

    public AmpControlState AmpType { get; } = new(KatanaMkIIParameterCatalog.AmpType);
    public AmpControlState AmpVariation { get; } = new(KatanaMkIIParameterCatalog.AmpVariation);
    public AmpControlState Gain { get; } = new(KatanaMkIIParameterCatalog.AmpGain);
    public AmpControlState Volume { get; } = new(KatanaMkIIParameterCatalog.AmpVolume);
    public AmpControlState Bass { get; } = new(KatanaMkIIParameterCatalog.AmpBass);
    public AmpControlState Middle { get; } = new(KatanaMkIIParameterCatalog.AmpMiddle);
    public AmpControlState Treble { get; } = new(KatanaMkIIParameterCatalog.AmpTreble);
    public AmpControlState Presence { get; } = new(KatanaMkIIParameterCatalog.AmpPresence);
    public AmpControlState CabinetResonance { get; } = new(KatanaMkIIParameterCatalog.CabinetResonance);

    /// <summary>Patch output level (0-200).</summary>
    public AmpControlState PatchLevel { get; } = new(KatanaMkIIParameterCatalog.PatchLevel, 0, 200);

    #endregion

    #region Contour (Ver200+)

    public ContourState Contour1 { get; } = new(KatanaMkIIParameterCatalog.Contour1Type,
        KatanaMkIIParameterCatalog.Contour1FreqShift);

    public ContourState Contour2 { get; } = new(KatanaMkIIParameterCatalog.Contour2Type,
        KatanaMkIIParameterCatalog.Contour2FreqShift);

    public ContourState Contour3 { get; } = new(KatanaMkIIParameterCatalog.Contour3Type,
        KatanaMkIIParameterCatalog.Contour3FreqShift);

    #endregion
}
