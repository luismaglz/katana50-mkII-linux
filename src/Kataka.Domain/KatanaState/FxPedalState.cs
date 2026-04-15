using Kataka.Domain.KatanaState.FxPedals;
using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState;

public class FxPedalState
{
    /// <summary>
    ///     On/Off state for the booster pedal
    /// </summary>
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.FxSwitch);

    /// <summary>
    ///     Type of boost pedal selected
    /// </summary>
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.FxType);

    /// <summary>
    ///     Maps to the LED that maps to the variation, used in the app itself not necessarily useful to us
    /// </summary>
    public AmpControlState Variation = new(KatanaMkIIParameterCatalog.FxVariation);

    #region ModFxPedals State

    public AcGuitarSimState AcGuitarSim = new();
    public AcProcessorState AcProcessor = new();
    public AutoWahState AutoWah = new();
    public ChorusState Chorus = new();
    public CompState Comp = new();
    public DC30State DC30 = new();
    public Flanger117EState Flanger117E = new();
    public GraphicEqState GraphicEq = new();
    public GuitarSimState GuitarSim = new();
    public HarmonistState Harmonist = new();
    public HeavyOctaveState HeavyOctave = new();
    public HumanizerState Humanizer = new();
    public LimiterState Limiter = new();
    public OctaveState Octave = new();
    public ParametricEQState ParametricEQ = new();
    public Phaser90EState Phaser90E = new();
    public PitchShifterState PitchShifter = new();
    public RingModState RingMod = new();
    public RotaryState Rotary = new();
    public SlicerState Slicer = new();
    public SlowGearState SlowGear = new();
    public TremoloState Tremolo = new();
    public TWahState TWah = new();
    public UniVState UniV = new();
    public VibratoState Vibrato = new();
    public Wah95EState Wah95E = new();
    public WaveSynthState WaveSynth = new();

    #endregion
}
