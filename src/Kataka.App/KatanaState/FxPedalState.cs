using Kataka.App.KatanaState.FxPedals;
using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class FxPedalState
{
    /// <summary>
    ///     On/Off state for the FX pedal
    /// </summary>
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.FxSwitch);

    /// <summary>Front-panel FX level knob position (PRM_KNOB_POS_FX).</summary>
    public AmpControlState Level = new(KatanaMkIIParameterCatalog.FxLevel);

    /// <summary>
    ///     Type of FX pedal selected
    /// </summary>
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.FxType);

    /// <summary>
    ///     Maps to the LED that maps to the variation, used in the app itself not necessarily useful to us
    /// </summary>
    public AmpControlState Variation = new(KatanaMkIIParameterCatalog.FxVariation);

    #region ModFxPedals State

    public AcGuitarSimState AcGuitarSim = new(false);
    public AcProcessorState AcProcessor = new(false);
    public AutoWahState AutoWah = new(false);
    public ChorusState Chorus = new(false);
    public CompState Comp = new(false);
    public DC30State DC30 = new(false);
    public Flanger117EState Flanger117E = new(false);
    public FlangerState Flanger = new(false);
    public GraphicEqState GraphicEq = new(false);
    public GuitarSimState GuitarSim = new(false);
    public HarmonistState Harmonist = new(false);
    public HeavyOctaveState HeavyOctave = new(false);
    public HumanizerState Humanizer = new(false);
    public LimiterState Limiter = new(false);
    public OctaveState Octave = new(false);
    public ParametricEQState ParametricEQ = new(false);
    public PedalBendState PedalBend = new(false);
    public PedalWahState PedalWah = new(false);
    public Phaser90EState Phaser90E = new(false);
    public PhaserState Phaser = new(false);
    public PitchShifterState PitchShifter = new(false);
    public RingModState RingMod = new(false);
    public RotaryState Rotary = new(false);
    public SlicerState Slicer = new(false);
    public SlowGearState SlowGear = new(false);
    public TremoloState Tremolo = new(false);
    public TWahState TWah = new(false);
    public UniVState UniV = new(false);
    public VibratoState Vibrato = new(false);
    public Wah95EState Wah95E = new(false);
    public WaveSynthState WaveSynth = new(false);

    #endregion
}
