using Avalonia.Media;

using Kataka.App.KatanaState;
using Kataka.App.KatanaState.FxPedals;
using Kataka.Domain.Midi;
using Kataka.Domain.Models;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class ModFxPedalViewModel : PedalViewModel
{
    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.ModFxTypes;

    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    private readonly AcGuitarSimState _acGuitarSim;
    private readonly AcProcessorState _acProcessor;
    private readonly AutoWahState _autoWah;

    /// <summary> Per-effect state ───────────────────────────────────────────────────────── </summary>
    private readonly ChorusState _chorus;

    private readonly CompState _comp;
    private readonly DC30State _dc30;

    /// <summary> Slot-level state ───────────────────────────────────────────────────────── </summary>
    private readonly AmpControlState _enabledState;

    private readonly FlangerState _flanger;
    private readonly Flanger117EState _flanger117E;
    private readonly GraphicEqState _graphicEq;
    private readonly GuitarSimState _guitarSim;
    private readonly HarmonistState _harmonist;
    private readonly HeavyOctaveState _heavyOctave;
    private readonly HumanizerState _humanizer;
    private readonly LimiterState _limiter;
    private readonly OctaveState _octave;
    private readonly ParametricEQState _parametricEq;
    private readonly PedalBendState _pedalBend;
    private readonly PedalWahState _pedalWah;
    private readonly PhaserState _phaser;
    private readonly Phaser90EState _phaser90E;
    private readonly PitchShifterState _pitchShifter;
    private readonly RingModState _ringMod;
    private readonly RotaryState _rotary;
    private readonly SlicerState _slicer;
    private readonly SlowGearState _slowGear;
    private readonly TremoloState _tremolo;
    private readonly TWahState _tWah;
    private readonly AmpControlState _typeState;
    private readonly UniVState _uniV;
    private readonly AmpControlState _variationState;
    private readonly VibratoState _vibrato;
    private readonly Wah95EState _wah95E;
    private readonly WaveSynthState _waveSynth;

    // Tracks type index for IsTypeXxx view switching.
    private int _selectedTypeIndex;

    private string? _selectedTypeOption;

    private string _variation = "N/A";

    public ModFxPedalViewModel(PedalPosition slot, IKatanaState katanaState) : base(
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == slot))
    {
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();

        if (slot == PedalPosition.Mod)
        {
            var s = katanaState.ModPedal;
            _enabledState = s.EnabledState;
            _typeState = s.Type;
            _variationState = s.Variation;
            _chorus = s.Chorus;
            _flanger = s.Flanger;
            _phaser = s.Phaser;
            _uniV = s.UniV;
            _tremolo = s.Tremolo;
            _vibrato = s.Vibrato;
            _rotary = s.Rotary;
            _ringMod = s.RingMod;
            _slowGear = s.SlowGear;
            _slicer = s.Slicer;
            _comp = s.Comp;
            _limiter = s.Limiter;
            _tWah = s.TWah;
            _autoWah = s.AutoWah;
            _pedalWah = s.PedalWah;
            _graphicEq = s.GraphicEq;
            _parametricEq = s.ParametricEQ;
            _guitarSim = s.GuitarSim;
            _acGuitarSim = s.AcGuitarSim;
            _acProcessor = s.AcProcessor;
            _waveSynth = s.WaveSynth;
            _octave = s.Octave;
            _heavyOctave = s.HeavyOctave;
            _pitchShifter = s.PitchShifter;
            _harmonist = s.Harmonist;
            _humanizer = s.Humanizer;
            _phaser90E = s.Phaser90E;
            _flanger117E = s.Flanger117E;
            _wah95E = s.Wah95E;
            _dc30 = s.DC30;
            _pedalBend = s.PedalBend;
        }
        else
        {
            var s = katanaState.FxPedal;
            _enabledState = s.EnabledState;
            _typeState = s.Type;
            _variationState = s.Variation;
            _chorus = s.Chorus;
            _flanger = s.Flanger;
            _phaser = s.Phaser;
            _uniV = s.UniV;
            _tremolo = s.Tremolo;
            _vibrato = s.Vibrato;
            _rotary = s.Rotary;
            _ringMod = s.RingMod;
            _slowGear = s.SlowGear;
            _slicer = s.Slicer;
            _comp = s.Comp;
            _limiter = s.Limiter;
            _tWah = s.TWah;
            _autoWah = s.AutoWah;
            _pedalWah = s.PedalWah;
            _graphicEq = s.GraphicEq;
            _parametricEq = s.ParametricEQ;
            _guitarSim = s.GuitarSim;
            _acGuitarSim = s.AcGuitarSim;
            _acProcessor = s.AcProcessor;
            _waveSynth = s.WaveSynth;
            _octave = s.Octave;
            _heavyOctave = s.HeavyOctave;
            _pitchShifter = s.PitchShifter;
            _harmonist = s.Harmonist;
            _humanizer = s.Humanizer;
            _phaser90E = s.Phaser90E;
            _flanger117E = s.Flanger117E;
            _wah95E = s.Wah95E;
            _dc30 = s.DC30;
            _pedalBend = s.PedalBend;
        }

        _enabledState.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEnabled));
        _typeState.ValueChanged += () =>
        {
            var idx = _typeState.Value;
            SelectedTypeIndex = idx;
            _selectedTypeOption = TypeTable.TryGetValue((byte)idx, out var name) ? name : null;
            this.RaisePropertyChanged(nameof(SelectedTypeOption));
            this.RaisePropertyChanged(nameof(TypeCaption));
        };
        _variationState.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(Variation));
            this.RaisePropertyChanged(nameof(VariationBrush));
        };

        _chorus.XoverFreq.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusXoverFreq));
        _chorus.LowRate.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusLowRate));
        _chorus.LowDepth.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusLowDepth));
        _chorus.LowPreDelay.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusLowPreDelay));
        _chorus.LowLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusLowLevel));
        _chorus.HighRate.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusHighRate));
        _chorus.HighDepth.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusHighDepth));
        _chorus.HighPreDelay.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusHighPreDelay));
        _chorus.HighLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusHighLevel));
        _chorus.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(ChorusDirectMix));

        _flanger.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(FlangerRate));
        _flanger.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(FlangerDepth));
        _flanger.Manual.ValueChanged += () => this.RaisePropertyChanged(nameof(FlangerManual));
        _flanger.Resonance.ValueChanged += () => this.RaisePropertyChanged(nameof(FlangerResonance));
        _flanger.LowCut.ValueChanged += () => this.RaisePropertyChanged(nameof(FlangerLowCut));
        _flanger.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(FlangerEffectLevel));
        _flanger.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(FlangerDirectMix));

        _phaser.Type.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserType));
        _phaser.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserRate));
        _phaser.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserDepth));
        _phaser.Manual.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserManual));
        _phaser.Resonance.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserResonance));
        _phaser.StepRate.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserStepRate));
        _phaser.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserEffectLevel));
        _phaser.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(PhaserDirectMix));

        _uniV.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(UniVRate));
        _uniV.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(UniVDepth));
        _uniV.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(UniVLevel));

        _tremolo.WaveShape.ValueChanged += () => this.RaisePropertyChanged(nameof(TremoloWaveShape));
        _tremolo.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(TremoloRate));
        _tremolo.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(TremoloDepth));
        _tremolo.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(TremoloLevel));

        _vibrato.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(VibratoRate));
        _vibrato.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(VibratoDepth));
        _vibrato.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(VibratoLevel));

        _rotary.RateFast.ValueChanged += () => this.RaisePropertyChanged(nameof(RotaryRateFast));
        _rotary.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(RotaryDepth));
        _rotary.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(RotaryLevel));

        _ringMod.Mode.ValueChanged += () => this.RaisePropertyChanged(nameof(RingModMode));
        _ringMod.Frequency.ValueChanged += () => this.RaisePropertyChanged(nameof(RingModFrequency));
        _ringMod.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(RingModEffectLevel));
        _ringMod.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(RingModDirectMix));

        _slowGear.Sens.ValueChanged += () => this.RaisePropertyChanged(nameof(SlowGearSens));
        _slowGear.RiseTime.ValueChanged += () => this.RaisePropertyChanged(nameof(SlowGearRiseTime));
        _slowGear.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(SlowGearLevel));

        _slicer.Pattern.ValueChanged += () => this.RaisePropertyChanged(nameof(SlicerPattern));
        _slicer.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(SlicerRate));
        _slicer.TriggerSens.ValueChanged += () => this.RaisePropertyChanged(nameof(SlicerTriggerSens));
        _slicer.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(SlicerEffectLevel));
        _slicer.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(SlicerDirectMix));

        _comp.Type.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(CompType));
            this.RaisePropertyChanged(nameof(CompTypeOption));
        };
        _comp.Sustain.ValueChanged += () => this.RaisePropertyChanged(nameof(CompSustain));
        _comp.Attack.ValueChanged += () => this.RaisePropertyChanged(nameof(CompAttack));
        _comp.Tone.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(CompTone));
            this.RaisePropertyChanged(nameof(CompToneDisplay));
        };
        _comp.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(CompLevel));

        _limiter.Type.ValueChanged += () => this.RaisePropertyChanged(nameof(LimiterType));
        _limiter.Attack.ValueChanged += () => this.RaisePropertyChanged(nameof(LimiterAttack));
        _limiter.Threshold.ValueChanged += () => this.RaisePropertyChanged(nameof(LimiterThreshold));
        _limiter.Ratio.ValueChanged += () => this.RaisePropertyChanged(nameof(LimiterRatio));
        _limiter.Release.ValueChanged += () => this.RaisePropertyChanged(nameof(LimiterRelease));
        _limiter.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(LimiterLevel));

        _tWah.Mode.ValueChanged += () => this.RaisePropertyChanged(nameof(TWahMode));
        _tWah.Polarity.ValueChanged += () => this.RaisePropertyChanged(nameof(TWahPolarity));
        _tWah.Sens.ValueChanged += () => this.RaisePropertyChanged(nameof(TWahSens));
        _tWah.Freq.ValueChanged += () => this.RaisePropertyChanged(nameof(TWahFreq));
        _tWah.Peak.ValueChanged += () => this.RaisePropertyChanged(nameof(TWahPeak));
        _tWah.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(TWahDirectMix));
        _tWah.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(TWahEffectLevel));

        _autoWah.Mode.ValueChanged += () => this.RaisePropertyChanged(nameof(AutoWahMode));
        _autoWah.Freq.ValueChanged += () => this.RaisePropertyChanged(nameof(AutoWahFreq));
        _autoWah.Peak.ValueChanged += () => this.RaisePropertyChanged(nameof(AutoWahPeak));
        _autoWah.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(AutoWahRate));
        _autoWah.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(AutoWahDepth));
        _autoWah.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(AutoWahDirectMix));
        _autoWah.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(AutoWahEffectLevel));

        _pedalWah.Type.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalWahType));
        _pedalWah.PedalPos.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalWahPedalPosition));
        _pedalWah.PedalMin.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalWahPedalMin));
        _pedalWah.PedalMax.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalWahPedalMax));
        _pedalWah.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalWahEffectLevel));
        _pedalWah.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalWahDirectMix));

        _graphicEq.Hz31.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq31Hz));
        _graphicEq.Hz62.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq62Hz));
        _graphicEq.Hz125.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq125Hz));
        _graphicEq.Hz250.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq250Hz));
        _graphicEq.Hz500.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq500Hz));
        _graphicEq.kHz1.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq1kHz));
        _graphicEq.kHz2.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq2kHz));
        _graphicEq.kHz4.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq4kHz));
        _graphicEq.kHz8.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq8kHz));
        _graphicEq.kHz16.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEq16kHz));
        _graphicEq.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(GraphicEqLevel));

        _parametricEq.LowCut.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqLowCut));
        _parametricEq.LowGain.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqLowGain));
        _parametricEq.LoMidFreq.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqLowMidFreq));
        _parametricEq.LoMidQ.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqLowMidQ));
        _parametricEq.LoMidGain.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqLowMidGain));
        _parametricEq.HiMidFreq.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqHighMidFreq));
        _parametricEq.HiMidQ.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqHighMidQ));
        _parametricEq.HiMidGain.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqHighMidGain));
        _parametricEq.HighGain.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqHighGain));
        _parametricEq.HighCut.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqHighCut));
        _parametricEq.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(ParametricEqLevel));

        _guitarSim.Type.ValueChanged += () => this.RaisePropertyChanged(nameof(GuitarSimType));
        _guitarSim.Low.ValueChanged += () => this.RaisePropertyChanged(nameof(GuitarSimLow));
        _guitarSim.High.ValueChanged += () => this.RaisePropertyChanged(nameof(GuitarSimHigh));
        _guitarSim.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(GuitarSimLevel));
        _guitarSim.Body.ValueChanged += () => this.RaisePropertyChanged(nameof(GuitarSimBody));

        _acGuitarSim.High.ValueChanged += () => this.RaisePropertyChanged(nameof(AcGuitarSimHigh));
        _acGuitarSim.Body.ValueChanged += () => this.RaisePropertyChanged(nameof(AcGuitarSimBody));
        _acGuitarSim.Low.ValueChanged += () => this.RaisePropertyChanged(nameof(AcGuitarSimLow));
        _acGuitarSim.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(AcGuitarSimLevel));

        _acProcessor.Type.ValueChanged += () => this.RaisePropertyChanged(nameof(AcProcessorType));
        _acProcessor.Bass.ValueChanged += () => this.RaisePropertyChanged(nameof(AcProcessorBass));
        _acProcessor.Mid.ValueChanged += () => this.RaisePropertyChanged(nameof(AcProcessorMid));
        _acProcessor.MidFreq.ValueChanged += () => this.RaisePropertyChanged(nameof(AcProcessorMidFreq));
        _acProcessor.Treble.ValueChanged += () => this.RaisePropertyChanged(nameof(AcProcessorTreble));
        _acProcessor.Presence.ValueChanged += () => this.RaisePropertyChanged(nameof(AcProcessorPresence));
        _acProcessor.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(AcProcessorLevel));

        _waveSynth.Wave.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthWave));
        _waveSynth.Cutoff.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthCutoff));
        _waveSynth.Resonance.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthResonance));
        _waveSynth.FilterSens.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthFilterSens));
        _waveSynth.FilterDecay.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthFilterDecay));
        _waveSynth.FilterDepth.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthFilterDepth));
        _waveSynth.SynthLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthSynthLevel));
        _waveSynth.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(WaveSynthDirectMix));

        _octave.Range.ValueChanged += () => this.RaisePropertyChanged(nameof(OctaveRange));
        _octave.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(OctaveEffectLevel));
        _octave.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(OctaveDirectMix));

        _heavyOctave.Oct1Level.ValueChanged += () => this.RaisePropertyChanged(nameof(HeavyOctave1OctLevel));
        _heavyOctave.Oct2Level.ValueChanged += () => this.RaisePropertyChanged(nameof(HeavyOctave2OctLevel));
        _heavyOctave.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(HeavyOctaveDirectMix));

        _pitchShifter.Voice.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterVoice));
        _pitchShifter.Ps1Mode.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS1Mode));
        _pitchShifter.Ps1Pitch.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS1Pitch));
        _pitchShifter.Ps1Fine.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS1Fine));
        _pitchShifter.Ps1Level.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS1Level));
        _pitchShifter.Ps2Mode.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS2Mode));
        _pitchShifter.Ps2Pitch.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS2Pitch));
        _pitchShifter.Ps2Fine.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS2Fine));
        _pitchShifter.Ps2Level.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterPS2Level));
        _pitchShifter.Feedback.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterFeedback));
        _pitchShifter.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(PitchShifterDirectMix));

        _harmonist.Voice.ValueChanged += () => this.RaisePropertyChanged(nameof(HarmonistVoice));
        _harmonist.Harmony1.ValueChanged += () => this.RaisePropertyChanged(nameof(HarmonistHarmony1));
        _harmonist.Level1.ValueChanged += () => this.RaisePropertyChanged(nameof(HarmonistLevel1));
        _harmonist.Harmony2.ValueChanged += () => this.RaisePropertyChanged(nameof(HarmonistHarmony2));
        _harmonist.Level2.ValueChanged += () => this.RaisePropertyChanged(nameof(HarmonistLevel2));
        _harmonist.Feedback.ValueChanged += () => this.RaisePropertyChanged(nameof(HarmonistFeedback));
        _harmonist.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(HarmonistDirectMix));

        _humanizer.Mode.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerMode));
        _humanizer.Vowel1.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerVowel1));
        _humanizer.Vowel2.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerVowel2));
        _humanizer.Sens.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerSens));
        _humanizer.Rate.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerRate));
        _humanizer.Depth.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerDepth));
        _humanizer.Manual.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerManual));
        _humanizer.Level.ValueChanged += () => this.RaisePropertyChanged(nameof(HumanizerLevel));

        _phaser90E.Script.ValueChanged += () => this.RaisePropertyChanged(nameof(Phaser90EScript));
        _phaser90E.Speed.ValueChanged += () => this.RaisePropertyChanged(nameof(Phaser90ESpeed));

        _flanger117E.Manual.ValueChanged += () => this.RaisePropertyChanged(nameof(Flanger117EManual));
        _flanger117E.Width.ValueChanged += () => this.RaisePropertyChanged(nameof(Flanger117EWidth));
        _flanger117E.Speed.ValueChanged += () => this.RaisePropertyChanged(nameof(Flanger117ESpeed));
        _flanger117E.Regen.ValueChanged += () => this.RaisePropertyChanged(nameof(Flanger117ERegen));

        _wah95E.PedalPos.ValueChanged += () => this.RaisePropertyChanged(nameof(Wah95EPedalPosition));
        _wah95E.PedalMin.ValueChanged += () => this.RaisePropertyChanged(nameof(Wah95EPedalMin));
        _wah95E.PedalMax.ValueChanged += () => this.RaisePropertyChanged(nameof(Wah95EPedalMax));
        _wah95E.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(Wah95EEffectLevel));
        _wah95E.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(Wah95EDirectMix));

        _dc30.Selector.ValueChanged += () => this.RaisePropertyChanged(nameof(DC30Selector));
        _dc30.InputVolume.ValueChanged += () => this.RaisePropertyChanged(nameof(DC30InputVolume));
        _dc30.ChorusIntensity.ValueChanged += () => this.RaisePropertyChanged(nameof(DC30ChorusIntensity));
        _dc30.EchoIntensity.ValueChanged += () => this.RaisePropertyChanged(nameof(DC30EchoIntensity));
        _dc30.EchoVolume.ValueChanged += () => this.RaisePropertyChanged(nameof(DC30EchoVolume));
        _dc30.Tone.ValueChanged += () => this.RaisePropertyChanged(nameof(DC30Tone));
        _dc30.Output.ValueChanged += () => this.RaisePropertyChanged(nameof(DC30Output));

        _pedalBend.Pitch.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalBendPitch));
        _pedalBend.PedalPos.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalBendPedalPosition));
        _pedalBend.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalBendEffectLevel));
        _pedalBend.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalBendDirectMix));
    }

    /// <summary> Common view properties ──────────────────────────────────────────────────── </summary>
    public IReadOnlyList<string> TypeOptions { get; }

    public bool HasTypeOptions => TypeOptions.Count > 0;
    public IBrush VariationBrush => GetVariationBrush(Variation);

    public int SelectedTypeIndex
    {
        get => _selectedTypeIndex;
        private set
        {
            if (!ChangeProperty(ref _selectedTypeIndex, value)) return;
            this.RaisePropertyChanged(nameof(IsTypeChorus));
            this.RaisePropertyChanged(nameof(IsTypeFlanger));
            this.RaisePropertyChanged(nameof(IsTypePhaser));
            this.RaisePropertyChanged(nameof(IsTypeUnivibe));
            this.RaisePropertyChanged(nameof(IsTypeTremolo));
            this.RaisePropertyChanged(nameof(IsTypeVibrato));
            this.RaisePropertyChanged(nameof(IsTypeRotary));
            this.RaisePropertyChanged(nameof(IsTypeRingMod));
            this.RaisePropertyChanged(nameof(IsTypeSlowGear));
            this.RaisePropertyChanged(nameof(IsTypeSlicer));
            this.RaisePropertyChanged(nameof(IsTypeComp));
            this.RaisePropertyChanged(nameof(IsTypeLimiter));
            this.RaisePropertyChanged(nameof(IsTypeTouchWah));
            this.RaisePropertyChanged(nameof(IsTypeAutoWah));
            this.RaisePropertyChanged(nameof(IsTypePedalWah));
            this.RaisePropertyChanged(nameof(IsTypeGraphicEq));
            this.RaisePropertyChanged(nameof(IsTypeParametricEq));
            this.RaisePropertyChanged(nameof(IsTypeGuitarSim));
            this.RaisePropertyChanged(nameof(IsTypeAcGuitarSim));
            this.RaisePropertyChanged(nameof(IsTypeAcProcessor));
            this.RaisePropertyChanged(nameof(IsTypeWaveSynth));
            this.RaisePropertyChanged(nameof(IsTypeOctave));
            this.RaisePropertyChanged(nameof(IsTypeHeavyOctave));
            this.RaisePropertyChanged(nameof(IsTypePitchShifter));
            this.RaisePropertyChanged(nameof(IsTypeHarmonist));
            this.RaisePropertyChanged(nameof(IsTypeHumanizer));
            this.RaisePropertyChanged(nameof(IsTypePhaser90E));
            this.RaisePropertyChanged(nameof(IsTypeFlanger117E));
            this.RaisePropertyChanged(nameof(IsTypeWah95E));
            this.RaisePropertyChanged(nameof(IsTypeDc30));
            this.RaisePropertyChanged(nameof(IsTypePedalBend));
        }
    }

    public bool IsTypeTouchWah => SelectedTypeIndex == 0;
    public bool IsTypeAutoWah => SelectedTypeIndex == 1;
    public bool IsTypePedalWah => SelectedTypeIndex == 2;
    public bool IsTypeComp => SelectedTypeIndex == 3;
    public bool IsTypeLimiter => SelectedTypeIndex == 4;
    public bool IsTypeGraphicEq => SelectedTypeIndex == 6;
    public bool IsTypeParametricEq => SelectedTypeIndex == 7;
    public bool IsTypeGuitarSim => SelectedTypeIndex == 9;
    public bool IsTypeSlowGear => SelectedTypeIndex == 10;
    public bool IsTypeWaveSynth => SelectedTypeIndex == 12;
    public bool IsTypeOctave => SelectedTypeIndex == 14;
    public bool IsTypePitchShifter => SelectedTypeIndex == 15;
    public bool IsTypeHarmonist => SelectedTypeIndex == 16;
    public bool IsTypeAcProcessor => SelectedTypeIndex == 18;
    public bool IsTypePhaser => SelectedTypeIndex == 19;
    public bool IsTypeFlanger => SelectedTypeIndex == 20;
    public bool IsTypeTremolo => SelectedTypeIndex == 21;
    public bool IsTypeRotary => SelectedTypeIndex == 22;
    public bool IsTypeUnivibe => SelectedTypeIndex == 23;
    public bool IsTypeSlicer => SelectedTypeIndex == 25;
    public bool IsTypeVibrato => SelectedTypeIndex == 26;
    public bool IsTypeRingMod => SelectedTypeIndex == 27;
    public bool IsTypeHumanizer => SelectedTypeIndex == 28;
    public bool IsTypeChorus => SelectedTypeIndex == 29;
    public bool IsTypeAcGuitarSim => SelectedTypeIndex == 31;
    public bool IsTypePhaser90E => SelectedTypeIndex == 35;
    public bool IsTypeFlanger117E => SelectedTypeIndex == 36;
    public bool IsTypeWah95E => SelectedTypeIndex == 37;
    public bool IsTypeDc30 => SelectedTypeIndex == 38;
    public bool IsTypeHeavyOctave => SelectedTypeIndex == 39;
    public bool IsTypePedalBend => SelectedTypeIndex == 40;

    /// <summary> PedalViewModel abstract overrides ──────────────────────────────────────── </summary>
    public override bool IsEnabled
    {
        get => _enabledState.Value != 0;
        set => _enabledState.Value = value ? 1 : 0;
    }

    public override string? SelectedTypeOption
    {
        get => TypeTable.TryGetValue((byte)_typeState.Value, out var name) ? name : null;
        set
        {
            if (!ChangeProperty(ref _selectedTypeOption, value)) return;
            this.RaisePropertyChanged(nameof(TypeCaption));
            if (value is not null && ReverseTypeTable.TryGetValue(value, out var byteValue))
            {
                SelectedTypeIndex = byteValue;
                _typeState.Value = byteValue;
            }
        }
    }

    public override string Variation
    {
        get => _variation;
        set
        {
            if (!ChangeProperty(ref _variation, value)) return;
            this.RaisePropertyChanged(nameof(VariationBrush));
            var raw = value switch { "Green" => 0, "Red" => 1, "Yellow" => 2, _ => -1 };
            if (raw >= 0) _variationState.Value = raw;
        }
    }

    public override string TypeCaption => SelectedTypeOption ?? "—";

    /// <summary> CHORUS params ───────────────────────────────────────────────────────────── </summary>
    public int ChorusXoverFreq
    {
        get => _chorus.XoverFreq.Value;
        set => _chorus.XoverFreq.Value = value;
    }

    public int ChorusLowRate
    {
        get => _chorus.LowRate.Value;
        set => _chorus.LowRate.Value = value;
    }

    public int ChorusLowDepth
    {
        get => _chorus.LowDepth.Value;
        set => _chorus.LowDepth.Value = value;
    }

    public int ChorusLowPreDelay
    {
        get => _chorus.LowPreDelay.Value;
        set => _chorus.LowPreDelay.Value = value;
    }

    public int ChorusLowLevel
    {
        get => _chorus.LowLevel.Value;
        set => _chorus.LowLevel.Value = value;
    }

    public int ChorusHighRate
    {
        get => _chorus.HighRate.Value;
        set => _chorus.HighRate.Value = value;
    }

    public int ChorusHighDepth
    {
        get => _chorus.HighDepth.Value;
        set => _chorus.HighDepth.Value = value;
    }

    public int ChorusHighPreDelay
    {
        get => _chorus.HighPreDelay.Value;
        set => _chorus.HighPreDelay.Value = value;
    }

    public int ChorusHighLevel
    {
        get => _chorus.HighLevel.Value;
        set => _chorus.HighLevel.Value = value;
    }

    public int ChorusDirectMix
    {
        get => _chorus.DirectMix.Value;
        set => _chorus.DirectMix.Value = value;
    }

    /// <summary> FLANGER params ──────────────────────────────────────────────────────────── </summary>
    public int FlangerRate
    {
        get => _flanger.Rate.Value;
        set => _flanger.Rate.Value = value;
    }

    public int FlangerDepth
    {
        get => _flanger.Depth.Value;
        set => _flanger.Depth.Value = value;
    }

    public int FlangerManual
    {
        get => _flanger.Manual.Value;
        set => _flanger.Manual.Value = value;
    }

    public int FlangerResonance
    {
        get => _flanger.Resonance.Value;
        set => _flanger.Resonance.Value = value;
    }

    public int FlangerLowCut
    {
        get => _flanger.LowCut.Value;
        set => _flanger.LowCut.Value = value;
    }

    public int FlangerEffectLevel
    {
        get => _flanger.EffectLevel.Value;
        set => _flanger.EffectLevel.Value = value;
    }

    public int FlangerDirectMix
    {
        get => _flanger.DirectMix.Value;
        set => _flanger.DirectMix.Value = value;
    }

    /// <summary> PHASER params ───────────────────────────────────────────────────────────── </summary>
    public int PhaserType
    {
        get => _phaser.Type.Value;
        set => _phaser.Type.Value = value;
    }

    public int PhaserRate
    {
        get => _phaser.Rate.Value;
        set => _phaser.Rate.Value = value;
    }

    public int PhaserDepth
    {
        get => _phaser.Depth.Value;
        set => _phaser.Depth.Value = value;
    }

    public int PhaserManual
    {
        get => _phaser.Manual.Value;
        set => _phaser.Manual.Value = value;
    }

    public int PhaserResonance
    {
        get => _phaser.Resonance.Value;
        set => _phaser.Resonance.Value = value;
    }

    public int PhaserStepRate
    {
        get => _phaser.StepRate.Value;
        set => _phaser.StepRate.Value = value;
    }

    public int PhaserEffectLevel
    {
        get => _phaser.EffectLevel.Value;
        set => _phaser.EffectLevel.Value = value;
    }

    public int PhaserDirectMix
    {
        get => _phaser.DirectMix.Value;
        set => _phaser.DirectMix.Value = value;
    }

    /// <summary> UNI-V params ────────────────────────────────────────────────────────────── </summary>
    public int UniVRate
    {
        get => _uniV.Rate.Value;
        set => _uniV.Rate.Value = value;
    }

    public int UniVDepth
    {
        get => _uniV.Depth.Value;
        set => _uniV.Depth.Value = value;
    }

    public int UniVLevel
    {
        get => _uniV.Level.Value;
        set => _uniV.Level.Value = value;
    }

    /// <summary> TREMOLO params ──────────────────────────────────────────────────────────── </summary>
    public int TremoloWaveShape
    {
        get => _tremolo.WaveShape.Value;
        set => _tremolo.WaveShape.Value = value;
    }

    public int TremoloRate
    {
        get => _tremolo.Rate.Value;
        set => _tremolo.Rate.Value = value;
    }

    public int TremoloDepth
    {
        get => _tremolo.Depth.Value;
        set => _tremolo.Depth.Value = value;
    }

    public int TremoloLevel
    {
        get => _tremolo.Level.Value;
        set => _tremolo.Level.Value = value;
    }

    /// <summary> VIBRATO params ──────────────────────────────────────────────────────────── </summary>
    public int VibratoRate
    {
        get => _vibrato.Rate.Value;
        set => _vibrato.Rate.Value = value;
    }

    public int VibratoDepth
    {
        get => _vibrato.Depth.Value;
        set => _vibrato.Depth.Value = value;
    }

    public int VibratoLevel
    {
        get => _vibrato.Level.Value;
        set => _vibrato.Level.Value = value;
    }

    /// <summary> ROTARY params ───────────────────────────────────────────────────────────── </summary>
    public int RotaryRateFast
    {
        get => _rotary.RateFast.Value;
        set => _rotary.RateFast.Value = value;
    }

    public int RotaryDepth
    {
        get => _rotary.Depth.Value;
        set => _rotary.Depth.Value = value;
    }

    public int RotaryLevel
    {
        get => _rotary.Level.Value;
        set => _rotary.Level.Value = value;
    }

    /// <summary> RING MOD params ─────────────────────────────────────────────────────────── </summary>
    public int RingModMode
    {
        get => _ringMod.Mode.Value;
        set => _ringMod.Mode.Value = value;
    }

    public int RingModFrequency
    {
        get => _ringMod.Frequency.Value;
        set => _ringMod.Frequency.Value = value;
    }

    public int RingModEffectLevel
    {
        get => _ringMod.EffectLevel.Value;
        set => _ringMod.EffectLevel.Value = value;
    }

    public int RingModDirectMix
    {
        get => _ringMod.DirectMix.Value;
        set => _ringMod.DirectMix.Value = value;
    }

    /// <summary> SLOW GEAR params ────────────────────────────────────────────────────────── </summary>
    public int SlowGearSens
    {
        get => _slowGear.Sens.Value;
        set => _slowGear.Sens.Value = value;
    }

    public int SlowGearRiseTime
    {
        get => _slowGear.RiseTime.Value;
        set => _slowGear.RiseTime.Value = value;
    }

    public int SlowGearLevel
    {
        get => _slowGear.Level.Value;
        set => _slowGear.Level.Value = value;
    }

    /// <summary> SLICER params ───────────────────────────────────────────────────────────── </summary>
    public int SlicerPattern
    {
        get => _slicer.Pattern.Value;
        set => _slicer.Pattern.Value = value;
    }

    public int SlicerRate
    {
        get => _slicer.Rate.Value;
        set => _slicer.Rate.Value = value;
    }

    public int SlicerTriggerSens
    {
        get => _slicer.TriggerSens.Value;
        set => _slicer.TriggerSens.Value = value;
    }

    public int SlicerEffectLevel
    {
        get => _slicer.EffectLevel.Value;
        set => _slicer.EffectLevel.Value = value;
    }

    public int SlicerDirectMix
    {
        get => _slicer.DirectMix.Value;
        set => _slicer.DirectMix.Value = value;
    }

    /// <summary> COMP params ─────────────────────────────────────────────────────────────── </summary>
    private static readonly string[] CompTypeOptionsList =
        ["BOSS COMP", "HI-BAND", "LIGHT", "D-COMP", "ORANGE", "FAT", "MILD"];

    public IReadOnlyList<string> CompTypeOptions => CompTypeOptionsList;

    public int CompType
    {
        get => _comp.Type.Value;
        set => _comp.Type.Value = value;
    }

    public string? CompTypeOption
    {
        get => _comp.Type.Value < CompTypeOptionsList.Length ? CompTypeOptionsList[_comp.Type.Value] : null;
        set
        {
            var idx = value is not null ? Array.IndexOf(CompTypeOptionsList, value) : -1;
            if (idx >= 0) _comp.Type.Value = idx;
        }
    }

    public int CompSustain
    {
        get => _comp.Sustain.Value;
        set => _comp.Sustain.Value = value;
    }

    public int CompAttack
    {
        get => _comp.Attack.Value;
        set => _comp.Attack.Value = value;
    }

    public int CompTone
    {
        get => _comp.Tone.Value;
        set => _comp.Tone.Value = value;
    }

    public int CompToneDisplay
    {
        get => _comp.Tone.Value - 50;
        set => _comp.Tone.Value = value + 50;
    }

    public int CompLevel
    {
        get => _comp.Level.Value;
        set => _comp.Level.Value = value;
    }

    /// <summary> LIMITER params ──────────────────────────────────────────────────────────── </summary>
    public int LimiterType
    {
        get => _limiter.Type.Value;
        set => _limiter.Type.Value = value;
    }

    public int LimiterAttack
    {
        get => _limiter.Attack.Value;
        set => _limiter.Attack.Value = value;
    }

    public int LimiterThreshold
    {
        get => _limiter.Threshold.Value;
        set => _limiter.Threshold.Value = value;
    }

    public int LimiterRatio
    {
        get => _limiter.Ratio.Value;
        set => _limiter.Ratio.Value = value;
    }

    public int LimiterRelease
    {
        get => _limiter.Release.Value;
        set => _limiter.Release.Value = value;
    }

    public int LimiterLevel
    {
        get => _limiter.Level.Value;
        set => _limiter.Level.Value = value;
    }

    /// <summary> T.WAH params ────────────────────────────────────────────────────────────── </summary>
    public int TWahMode
    {
        get => _tWah.Mode.Value;
        set => _tWah.Mode.Value = value;
    }

    public int TWahPolarity
    {
        get => _tWah.Polarity.Value;
        set => _tWah.Polarity.Value = value;
    }

    public int TWahSens
    {
        get => _tWah.Sens.Value;
        set => _tWah.Sens.Value = value;
    }

    public int TWahFreq
    {
        get => _tWah.Freq.Value;
        set => _tWah.Freq.Value = value;
    }

    public int TWahPeak
    {
        get => _tWah.Peak.Value;
        set => _tWah.Peak.Value = value;
    }

    public int TWahDirectMix
    {
        get => _tWah.DirectMix.Value;
        set => _tWah.DirectMix.Value = value;
    }

    public int TWahEffectLevel
    {
        get => _tWah.EffectLevel.Value;
        set => _tWah.EffectLevel.Value = value;
    }

    /// <summary> AUTO WAH params ─────────────────────────────────────────────────────────── </summary>
    public int AutoWahMode
    {
        get => _autoWah.Mode.Value;
        set => _autoWah.Mode.Value = value;
    }

    public int AutoWahFreq
    {
        get => _autoWah.Freq.Value;
        set => _autoWah.Freq.Value = value;
    }

    public int AutoWahPeak
    {
        get => _autoWah.Peak.Value;
        set => _autoWah.Peak.Value = value;
    }

    public int AutoWahRate
    {
        get => _autoWah.Rate.Value;
        set => _autoWah.Rate.Value = value;
    }

    public int AutoWahDepth
    {
        get => _autoWah.Depth.Value;
        set => _autoWah.Depth.Value = value;
    }

    public int AutoWahDirectMix
    {
        get => _autoWah.DirectMix.Value;
        set => _autoWah.DirectMix.Value = value;
    }

    public int AutoWahEffectLevel
    {
        get => _autoWah.EffectLevel.Value;
        set => _autoWah.EffectLevel.Value = value;
    }

    /// <summary> PEDAL WAH params ────────────────────────────────────────────────────────── </summary>
    public int PedalWahType
    {
        get => _pedalWah.Type.Value;
        set => _pedalWah.Type.Value = value;
    }

    public int PedalWahPedalPosition
    {
        get => _pedalWah.PedalPos.Value;
        set => _pedalWah.PedalPos.Value = value;
    }

    public int PedalWahPedalMin
    {
        get => _pedalWah.PedalMin.Value;
        set => _pedalWah.PedalMin.Value = value;
    }

    public int PedalWahPedalMax
    {
        get => _pedalWah.PedalMax.Value;
        set => _pedalWah.PedalMax.Value = value;
    }

    public int PedalWahEffectLevel
    {
        get => _pedalWah.EffectLevel.Value;
        set => _pedalWah.EffectLevel.Value = value;
    }

    public int PedalWahDirectMix
    {
        get => _pedalWah.DirectMix.Value;
        set => _pedalWah.DirectMix.Value = value;
    }

    /// <summary> GRAPHIC EQ params ───────────────────────────────────────────────────────── </summary>
    public int GraphicEq31Hz
    {
        get => _graphicEq.Hz31.Value;
        set => _graphicEq.Hz31.Value = value;
    }

    public int GraphicEq62Hz
    {
        get => _graphicEq.Hz62.Value;
        set => _graphicEq.Hz62.Value = value;
    }

    public int GraphicEq125Hz
    {
        get => _graphicEq.Hz125.Value;
        set => _graphicEq.Hz125.Value = value;
    }

    public int GraphicEq250Hz
    {
        get => _graphicEq.Hz250.Value;
        set => _graphicEq.Hz250.Value = value;
    }

    public int GraphicEq500Hz
    {
        get => _graphicEq.Hz500.Value;
        set => _graphicEq.Hz500.Value = value;
    }

    public int GraphicEq1kHz
    {
        get => _graphicEq.kHz1.Value;
        set => _graphicEq.kHz1.Value = value;
    }

    public int GraphicEq2kHz
    {
        get => _graphicEq.kHz2.Value;
        set => _graphicEq.kHz2.Value = value;
    }

    public int GraphicEq4kHz
    {
        get => _graphicEq.kHz4.Value;
        set => _graphicEq.kHz4.Value = value;
    }

    public int GraphicEq8kHz
    {
        get => _graphicEq.kHz8.Value;
        set => _graphicEq.kHz8.Value = value;
    }

    public int GraphicEq16kHz
    {
        get => _graphicEq.kHz16.Value;
        set => _graphicEq.kHz16.Value = value;
    }

    public int GraphicEqLevel
    {
        get => _graphicEq.Level.Value;
        set => _graphicEq.Level.Value = value;
    }

    /// <summary> PARAMETRIC EQ params ────────────────────────────────────────────────────── </summary>
    public int ParametricEqLowCut
    {
        get => _parametricEq.LowCut.Value;
        set => _parametricEq.LowCut.Value = value;
    }

    public int ParametricEqLowGain
    {
        get => _parametricEq.LowGain.Value;
        set => _parametricEq.LowGain.Value = value;
    }

    public int ParametricEqLowMidFreq
    {
        get => _parametricEq.LoMidFreq.Value;
        set => _parametricEq.LoMidFreq.Value = value;
    }

    public int ParametricEqLowMidQ
    {
        get => _parametricEq.LoMidQ.Value;
        set => _parametricEq.LoMidQ.Value = value;
    }

    public int ParametricEqLowMidGain
    {
        get => _parametricEq.LoMidGain.Value;
        set => _parametricEq.LoMidGain.Value = value;
    }

    public int ParametricEqHighMidFreq
    {
        get => _parametricEq.HiMidFreq.Value;
        set => _parametricEq.HiMidFreq.Value = value;
    }

    public int ParametricEqHighMidQ
    {
        get => _parametricEq.HiMidQ.Value;
        set => _parametricEq.HiMidQ.Value = value;
    }

    public int ParametricEqHighMidGain
    {
        get => _parametricEq.HiMidGain.Value;
        set => _parametricEq.HiMidGain.Value = value;
    }

    public int ParametricEqHighGain
    {
        get => _parametricEq.HighGain.Value;
        set => _parametricEq.HighGain.Value = value;
    }

    public int ParametricEqHighCut
    {
        get => _parametricEq.HighCut.Value;
        set => _parametricEq.HighCut.Value = value;
    }

    public int ParametricEqLevel
    {
        get => _parametricEq.Level.Value;
        set => _parametricEq.Level.Value = value;
    }

    /// <summary> GUITAR SIM params ───────────────────────────────────────────────────────── </summary>
    public int GuitarSimType
    {
        get => _guitarSim.Type.Value;
        set => _guitarSim.Type.Value = value;
    }

    public int GuitarSimLow
    {
        get => _guitarSim.Low.Value;
        set => _guitarSim.Low.Value = value;
    }

    public int GuitarSimHigh
    {
        get => _guitarSim.High.Value;
        set => _guitarSim.High.Value = value;
    }

    public int GuitarSimLevel
    {
        get => _guitarSim.Level.Value;
        set => _guitarSim.Level.Value = value;
    }

    public int GuitarSimBody
    {
        get => _guitarSim.Body.Value;
        set => _guitarSim.Body.Value = value;
    }

    /// <summary> AC. GUITAR SIM params ───────────────────────────────────────────────────── </summary>
    public int AcGuitarSimHigh
    {
        get => _acGuitarSim.High.Value;
        set => _acGuitarSim.High.Value = value;
    }

    public int AcGuitarSimBody
    {
        get => _acGuitarSim.Body.Value;
        set => _acGuitarSim.Body.Value = value;
    }

    public int AcGuitarSimLow
    {
        get => _acGuitarSim.Low.Value;
        set => _acGuitarSim.Low.Value = value;
    }

    public int AcGuitarSimLevel
    {
        get => _acGuitarSim.Level.Value;
        set => _acGuitarSim.Level.Value = value;
    }

    /// <summary> AC. PROCESSOR params ────────────────────────────────────────────────────── </summary>
    public int AcProcessorType
    {
        get => _acProcessor.Type.Value;
        set => _acProcessor.Type.Value = value;
    }

    public int AcProcessorBass
    {
        get => _acProcessor.Bass.Value;
        set => _acProcessor.Bass.Value = value;
    }

    public int AcProcessorMid
    {
        get => _acProcessor.Mid.Value;
        set => _acProcessor.Mid.Value = value;
    }

    public int AcProcessorMidFreq
    {
        get => _acProcessor.MidFreq.Value;
        set => _acProcessor.MidFreq.Value = value;
    }

    public int AcProcessorTreble
    {
        get => _acProcessor.Treble.Value;
        set => _acProcessor.Treble.Value = value;
    }

    public int AcProcessorPresence
    {
        get => _acProcessor.Presence.Value;
        set => _acProcessor.Presence.Value = value;
    }

    public int AcProcessorLevel
    {
        get => _acProcessor.Level.Value;
        set => _acProcessor.Level.Value = value;
    }

    /// <summary> WAVE SYNTH params ───────────────────────────────────────────────────────── </summary>
    public int WaveSynthWave
    {
        get => _waveSynth.Wave.Value;
        set => _waveSynth.Wave.Value = value;
    }

    public int WaveSynthCutoff
    {
        get => _waveSynth.Cutoff.Value;
        set => _waveSynth.Cutoff.Value = value;
    }

    public int WaveSynthResonance
    {
        get => _waveSynth.Resonance.Value;
        set => _waveSynth.Resonance.Value = value;
    }

    public int WaveSynthFilterSens
    {
        get => _waveSynth.FilterSens.Value;
        set => _waveSynth.FilterSens.Value = value;
    }

    public int WaveSynthFilterDecay
    {
        get => _waveSynth.FilterDecay.Value;
        set => _waveSynth.FilterDecay.Value = value;
    }

    public int WaveSynthFilterDepth
    {
        get => _waveSynth.FilterDepth.Value;
        set => _waveSynth.FilterDepth.Value = value;
    }

    public int WaveSynthSynthLevel
    {
        get => _waveSynth.SynthLevel.Value;
        set => _waveSynth.SynthLevel.Value = value;
    }

    public int WaveSynthDirectMix
    {
        get => _waveSynth.DirectMix.Value;
        set => _waveSynth.DirectMix.Value = value;
    }

    /// <summary> OCTAVE params ───────────────────────────────────────────────────────────── </summary>
    public int OctaveRange
    {
        get => _octave.Range.Value;
        set => _octave.Range.Value = value;
    }

    public int OctaveEffectLevel
    {
        get => _octave.EffectLevel.Value;
        set => _octave.EffectLevel.Value = value;
    }

    public int OctaveDirectMix
    {
        get => _octave.DirectMix.Value;
        set => _octave.DirectMix.Value = value;
    }

    /// <summary> HEAVY OCTAVE params ─────────────────────────────────────────────────────── </summary>
    public int HeavyOctave1OctLevel
    {
        get => _heavyOctave.Oct1Level.Value;
        set => _heavyOctave.Oct1Level.Value = value;
    }

    public int HeavyOctave2OctLevel
    {
        get => _heavyOctave.Oct2Level.Value;
        set => _heavyOctave.Oct2Level.Value = value;
    }

    public int HeavyOctaveDirectMix
    {
        get => _heavyOctave.DirectMix.Value;
        set => _heavyOctave.DirectMix.Value = value;
    }

    /// <summary> PITCH SHIFTER params ────────────────────────────────────────────────────── </summary>
    public int PitchShifterVoice
    {
        get => _pitchShifter.Voice.Value;
        set => _pitchShifter.Voice.Value = value;
    }

    public int PitchShifterPS1Mode
    {
        get => _pitchShifter.Ps1Mode.Value;
        set => _pitchShifter.Ps1Mode.Value = value;
    }

    public int PitchShifterPS1Pitch
    {
        get => _pitchShifter.Ps1Pitch.Value;
        set => _pitchShifter.Ps1Pitch.Value = value;
    }

    public int PitchShifterPS1Fine
    {
        get => _pitchShifter.Ps1Fine.Value;
        set => _pitchShifter.Ps1Fine.Value = value;
    }

    public int PitchShifterPS1Level
    {
        get => _pitchShifter.Ps1Level.Value;
        set => _pitchShifter.Ps1Level.Value = value;
    }

    public int PitchShifterPS2Mode
    {
        get => _pitchShifter.Ps2Mode.Value;
        set => _pitchShifter.Ps2Mode.Value = value;
    }

    public int PitchShifterPS2Pitch
    {
        get => _pitchShifter.Ps2Pitch.Value;
        set => _pitchShifter.Ps2Pitch.Value = value;
    }

    public int PitchShifterPS2Fine
    {
        get => _pitchShifter.Ps2Fine.Value;
        set => _pitchShifter.Ps2Fine.Value = value;
    }

    public int PitchShifterPS2Level
    {
        get => _pitchShifter.Ps2Level.Value;
        set => _pitchShifter.Ps2Level.Value = value;
    }

    public int PitchShifterFeedback
    {
        get => _pitchShifter.Feedback.Value;
        set => _pitchShifter.Feedback.Value = value;
    }

    public int PitchShifterDirectMix
    {
        get => _pitchShifter.DirectMix.Value;
        set => _pitchShifter.DirectMix.Value = value;
    }

    /// <summary> HARMONIST params ────────────────────────────────────────────────────────── </summary>
    public int HarmonistVoice
    {
        get => _harmonist.Voice.Value;
        set => _harmonist.Voice.Value = value;
    }

    public int HarmonistHarmony1
    {
        get => _harmonist.Harmony1.Value;
        set => _harmonist.Harmony1.Value = value;
    }

    public int HarmonistLevel1
    {
        get => _harmonist.Level1.Value;
        set => _harmonist.Level1.Value = value;
    }

    public int HarmonistHarmony2
    {
        get => _harmonist.Harmony2.Value;
        set => _harmonist.Harmony2.Value = value;
    }

    public int HarmonistLevel2
    {
        get => _harmonist.Level2.Value;
        set => _harmonist.Level2.Value = value;
    }

    public int HarmonistFeedback
    {
        get => _harmonist.Feedback.Value;
        set => _harmonist.Feedback.Value = value;
    }

    public int HarmonistDirectMix
    {
        get => _harmonist.DirectMix.Value;
        set => _harmonist.DirectMix.Value = value;
    }

    /// <summary> HUMANIZER params ────────────────────────────────────────────────────────── </summary>
    public int HumanizerMode
    {
        get => _humanizer.Mode.Value;
        set => _humanizer.Mode.Value = value;
    }

    public int HumanizerVowel1
    {
        get => _humanizer.Vowel1.Value;
        set => _humanizer.Vowel1.Value = value;
    }

    public int HumanizerVowel2
    {
        get => _humanizer.Vowel2.Value;
        set => _humanizer.Vowel2.Value = value;
    }

    public int HumanizerSens
    {
        get => _humanizer.Sens.Value;
        set => _humanizer.Sens.Value = value;
    }

    public int HumanizerRate
    {
        get => _humanizer.Rate.Value;
        set => _humanizer.Rate.Value = value;
    }

    public int HumanizerDepth
    {
        get => _humanizer.Depth.Value;
        set => _humanizer.Depth.Value = value;
    }

    public int HumanizerManual
    {
        get => _humanizer.Manual.Value;
        set => _humanizer.Manual.Value = value;
    }

    public int HumanizerLevel
    {
        get => _humanizer.Level.Value;
        set => _humanizer.Level.Value = value;
    }

    /// <summary> PHASER 90E params ───────────────────────────────────────────────────────── </summary>
    public int Phaser90EScript
    {
        get => _phaser90E.Script.Value;
        set => _phaser90E.Script.Value = value;
    }

    public int Phaser90ESpeed
    {
        get => _phaser90E.Speed.Value;
        set => _phaser90E.Speed.Value = value;
    }

    /// <summary> FLANGER 117E params ─────────────────────────────────────────────────────── </summary>
    public int Flanger117EManual
    {
        get => _flanger117E.Manual.Value;
        set => _flanger117E.Manual.Value = value;
    }

    public int Flanger117EWidth
    {
        get => _flanger117E.Width.Value;
        set => _flanger117E.Width.Value = value;
    }

    public int Flanger117ESpeed
    {
        get => _flanger117E.Speed.Value;
        set => _flanger117E.Speed.Value = value;
    }

    public int Flanger117ERegen
    {
        get => _flanger117E.Regen.Value;
        set => _flanger117E.Regen.Value = value;
    }

    /// <summary> WAH 95E params ──────────────────────────────────────────────────────────── </summary>
    public int Wah95EPedalPosition
    {
        get => _wah95E.PedalPos.Value;
        set => _wah95E.PedalPos.Value = value;
    }

    public int Wah95EPedalMin
    {
        get => _wah95E.PedalMin.Value;
        set => _wah95E.PedalMin.Value = value;
    }

    public int Wah95EPedalMax
    {
        get => _wah95E.PedalMax.Value;
        set => _wah95E.PedalMax.Value = value;
    }

    public int Wah95EEffectLevel
    {
        get => _wah95E.EffectLevel.Value;
        set => _wah95E.EffectLevel.Value = value;
    }

    public int Wah95EDirectMix
    {
        get => _wah95E.DirectMix.Value;
        set => _wah95E.DirectMix.Value = value;
    }

    /// <summary> DC-30 params ────────────────────────────────────────────────────────────── </summary>
    public int DC30Selector
    {
        get => _dc30.Selector.Value;
        set => _dc30.Selector.Value = value;
    }

    public int DC30InputVolume
    {
        get => _dc30.InputVolume.Value;
        set => _dc30.InputVolume.Value = value;
    }

    public int DC30ChorusIntensity
    {
        get => _dc30.ChorusIntensity.Value;
        set => _dc30.ChorusIntensity.Value = value;
    }

    public int DC30EchoIntensity
    {
        get => _dc30.EchoIntensity.Value;
        set => _dc30.EchoIntensity.Value = value;
    }

    public int DC30EchoVolume
    {
        get => _dc30.EchoVolume.Value;
        set => _dc30.EchoVolume.Value = value;
    }

    public int DC30Tone
    {
        get => _dc30.Tone.Value;
        set => _dc30.Tone.Value = value;
    }

    public int DC30Output
    {
        get => _dc30.Output.Value;
        set => _dc30.Output.Value = value;
    }

    /// <summary> PEDAL BEND params ───────────────────────────────────────────────────────── </summary>
    public int PedalBendPitch
    {
        get => _pedalBend.Pitch.Value;
        set => _pedalBend.Pitch.Value = value;
    }

    public int PedalBendPedalPosition
    {
        get => _pedalBend.PedalPos.Value;
        set => _pedalBend.PedalPos.Value = value;
    }

    public int PedalBendEffectLevel
    {
        get => _pedalBend.EffectLevel.Value;
        set => _pedalBend.EffectLevel.Value = value;
    }

    public int PedalBendDirectMix
    {
        get => _pedalBend.DirectMix.Value;
        set => _pedalBend.DirectMix.Value = value;
    }

    public override bool TryGetTypeValue(string? option, out byte value)
    {
        if (option is not null && ReverseTypeTable.TryGetValue(option, out value))
            return true;
        value = 0;
        return false;
    }

    public override string ToTypeOption(byte rawValue) =>
        TypeTable.TryGetValue(rawValue, out var name) ? name : $"Type {rawValue}";
}
