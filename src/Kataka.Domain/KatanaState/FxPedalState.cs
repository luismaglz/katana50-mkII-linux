using Kataka.Domain.KatanaState.FxPedals;
using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState;

public class FxPedalState
{
    /// <summary>
    ///     On/Off state for the FX pedal
    /// </summary>
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.FxSwitch);

    /// <summary>
    ///     Type of FX pedal selected
    /// </summary>
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.FxType);

    /// <summary>
    ///     Maps to the LED that maps to the variation, used in the app itself not necessarily useful to us
    /// </summary>
    public AmpControlState Variation = new(KatanaMkIIParameterCatalog.FxVariation);

    #region ModFxPedals State

    public AcGuitarSimState AcGuitarSim = new(isMod: false);
    public AcProcessorState AcProcessor = new(isMod: false);
    public AutoWahState AutoWah = new(isMod: false);
    public ChorusState Chorus = new(isMod: false);
    public CompState Comp = new(isMod: false);
    public DC30State DC30 = new(isMod: false);
    public Flanger117EState Flanger117E = new(isMod: false);
    public FlangerState Flanger = new(isMod: false);
    public GraphicEqState GraphicEq = new(isMod: false);
    public GuitarSimState GuitarSim = new(isMod: false);
    public HarmonistState Harmonist = new(isMod: false);
    public HeavyOctaveState HeavyOctave = new(isMod: false);
    public HumanizerState Humanizer = new(isMod: false);
    public LimiterState Limiter = new(isMod: false);
    public OctaveState Octave = new(isMod: false);
    public ParametricEQState ParametricEQ = new(isMod: false);
    public PedalBendState PedalBend = new(isMod: false);
    public PedalWahState PedalWah = new(isMod: false);
    public Phaser90EState Phaser90E = new(isMod: false);
    public PhaserState Phaser = new(isMod: false);
    public PitchShifterState PitchShifter = new(isMod: false);
    public RingModState RingMod = new(isMod: false);
    public RotaryState Rotary = new(isMod: false);
    public SlicerState Slicer = new(isMod: false);
    public SlowGearState SlowGear = new(isMod: false);
    public TremoloState Tremolo = new(isMod: false);
    public TWahState TWah = new(isMod: false);
    public UniVState UniV = new(isMod: false);
    public VibratoState Vibrato = new(isMod: false);
    public Wah95EState Wah95E = new(isMod: false);
    public WaveSynthState WaveSynth = new(isMod: false);

    #endregion
}
