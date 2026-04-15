using Kataka.Domain.KatanaState.FxPedals;
using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState;

public class ModPedalState
{
    /// <summary>
    ///     On/Off state for the booster pedal
    /// </summary>
    public AmpControlState EnabledState = new(KatanaMkIIParameterCatalog.ModSwitch);

    /// <summary>
    ///     Type of boost pedal selected
    /// </summary>
    public AmpControlState Type = new(KatanaMkIIParameterCatalog.ModType);

    /// <summary>
    ///     Maps to the LED that maps to the variation, used in the app itself not necessarily useful to us
    /// </summary>
    public AmpControlState Variation = new(KatanaMkIIParameterCatalog.ModVariation);

    #region ModFxPedals State

    public AcGuitarSimState AcGuitarSim = new(isMod: true);
    public AcProcessorState AcProcessor = new(isMod: true);
    public AutoWahState AutoWah = new(isMod: true);
    public ChorusState Chorus = new(isMod: true);
    public CompState Comp = new(isMod: true);
    public DC30State DC30 = new(isMod: true);
    public Flanger117EState Flanger117E = new(isMod: true);
    public FlangerState Flanger = new(isMod: true);
    public GraphicEqState GraphicEq = new(isMod: true);
    public GuitarSimState GuitarSim = new(isMod: true);
    public HarmonistState Harmonist = new(isMod: true);
    public HeavyOctaveState HeavyOctave = new(isMod: true);
    public HumanizerState Humanizer = new(isMod: true);
    public LimiterState Limiter = new(isMod: true);
    public OctaveState Octave = new(isMod: true);
    public ParametricEQState ParametricEQ = new(isMod: true);
    public PedalBendState PedalBend = new(isMod: true);
    public PedalWahState PedalWah = new(isMod: true);
    public Phaser90EState Phaser90E = new(isMod: true);
    public PhaserState Phaser = new(isMod: true);
    public PitchShifterState PitchShifter = new(isMod: true);
    public RingModState RingMod = new(isMod: true);
    public RotaryState Rotary = new(isMod: true);
    public SlicerState Slicer = new(isMod: true);
    public SlowGearState SlowGear = new(isMod: true);
    public TremoloState Tremolo = new(isMod: true);
    public TWahState TWah = new(isMod: true);
    public UniVState UniV = new(isMod: true);
    public VibratoState Vibrato = new(isMod: true);
    public Wah95EState Wah95E = new(isMod: true);
    public WaveSynthState WaveSynth = new(isMod: true);

    #endregion
}
