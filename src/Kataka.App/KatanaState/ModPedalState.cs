using Kataka.App.KatanaState.FxPedals;
using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class ModPedalState
{
    /// <summary>
    ///     On/Off state for the booster pedal
    /// </summary>
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.ModSwitch);

    /// <summary>Front-panel mod level knob position (PRM_KNOB_POS_MOD).</summary>
    public AmpControlState Level = new(KatanaMkIIParameterCatalog.ModLevel);

    /// <summary>
    ///     Type of boost pedal selected
    /// </summary>
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.ModType);

    /// <summary>
    ///     Maps to the LED that maps to the variation, used in the app itself not necessarily useful to us
    /// </summary>
    public AmpControlState Variation = new(KatanaMkIIParameterCatalog.ModVariation);

    #region ModFxPedals State

    public AcGuitarSimState AcGuitarSim = new(true);
    public AcProcessorState AcProcessor = new(true);
    public AutoWahState AutoWah = new(true);
    public ChorusState Chorus = new(true);
    public CompState Comp = new(true);
    public DC30State DC30 = new(true);
    public Flanger117EState Flanger117E = new(true);
    public FlangerState Flanger = new(true);
    public GraphicEqState GraphicEq = new(true);
    public GuitarSimState GuitarSim = new(true);
    public HarmonistState Harmonist = new(true);
    public HeavyOctaveState HeavyOctave = new(true);
    public HumanizerState Humanizer = new(true);
    public LimiterState Limiter = new(true);
    public OctaveState Octave = new(true);
    public ParametricEQState ParametricEQ = new(true);
    public PedalBendState PedalBend = new(true);
    public PedalWahState PedalWah = new(true);
    public Phaser90EState Phaser90E = new(true);
    public PhaserState Phaser = new(true);
    public PitchShifterState PitchShifter = new(true);
    public RingModState RingMod = new(true);
    public RotaryState Rotary = new(true);
    public SlicerState Slicer = new(true);
    public SlowGearState SlowGear = new(true);
    public TremoloState Tremolo = new(true);
    public TWahState TWah = new(true);
    public UniVState UniV = new(true);
    public VibratoState Vibrato = new(true);
    public Wah95EState Wah95E = new(true);
    public WaveSynthState WaveSynth = new(true);

    #endregion
}
