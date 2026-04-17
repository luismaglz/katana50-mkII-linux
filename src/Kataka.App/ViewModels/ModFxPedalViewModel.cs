using System;
using System.Collections.Generic;
using System.Linq;

using Avalonia.Media;

using Kataka.Domain.Midi;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public partial class ModFxPedalViewModel : PedalViewModel
{
    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.ModFxTypes;
    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    private readonly IReadOnlyList<KatanaParameterDefinition> _chorusParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _flangerParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _phaserParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _uniVParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _tremoloParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _vibratoParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _rotaryParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _ringModParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _slowGearParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _slicerParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _compParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _limiterParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _tWahParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _autoWahParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _pedalWahParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _graphicEqParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _parametricEqParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _guitarSimParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _acGuitarSimParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _acProcessorParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _waveSynthParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _octaveParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _heavyOctaveParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _pitchShifterParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _harmonistParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _humanizerParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _phaser90EParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _flanger117EParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _wah95EParams;
    private readonly IReadOnlyList<KatanaParameterDefinition> _dc30Params;
    private readonly IReadOnlyList<KatanaParameterDefinition> _pedalBendParams;

    public ModFxPedalViewModel(string slot) : base(
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == slot))
    {
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();
        if (slot == "mod")
        {
            _chorusParams = KatanaMkIIParameterCatalog.ModChorusParams;
            _flangerParams = KatanaMkIIParameterCatalog.ModFlangerParams;
            _phaserParams = KatanaMkIIParameterCatalog.ModPhaserParams;
            _uniVParams = KatanaMkIIParameterCatalog.ModUniVParams;
            _tremoloParams = KatanaMkIIParameterCatalog.ModTremoloParams;
            _vibratoParams = KatanaMkIIParameterCatalog.ModVibratoParams;
            _rotaryParams = KatanaMkIIParameterCatalog.ModRotaryParams;
            _ringModParams = KatanaMkIIParameterCatalog.ModRingModParams;
            _slowGearParams = KatanaMkIIParameterCatalog.ModSlowGearParams;
            _slicerParams = KatanaMkIIParameterCatalog.ModSlicerParams;
            _compParams = KatanaMkIIParameterCatalog.ModCompParams;
            _limiterParams = KatanaMkIIParameterCatalog.ModLimiterParams;
            _tWahParams = KatanaMkIIParameterCatalog.ModTWahParams;
            _autoWahParams = KatanaMkIIParameterCatalog.ModAutoWahParams;
            _pedalWahParams = KatanaMkIIParameterCatalog.ModPedalWahParams;
            _graphicEqParams = KatanaMkIIParameterCatalog.ModGraphicEqParams;
            _parametricEqParams = KatanaMkIIParameterCatalog.ModParametricEqParams;
            _guitarSimParams = KatanaMkIIParameterCatalog.ModGuitarSimParams;
            _acGuitarSimParams = KatanaMkIIParameterCatalog.ModAcGuitarSimParams;
            _acProcessorParams = KatanaMkIIParameterCatalog.ModAcProcessorParams;
            _waveSynthParams = KatanaMkIIParameterCatalog.ModWaveSynthParams;
            _octaveParams = KatanaMkIIParameterCatalog.ModOctaveParams;
            _heavyOctaveParams = KatanaMkIIParameterCatalog.ModHeavyOctaveParams;
            _pitchShifterParams = KatanaMkIIParameterCatalog.ModPitchShifterParams;
            _harmonistParams = KatanaMkIIParameterCatalog.ModHarmonistParams;
            _humanizerParams = KatanaMkIIParameterCatalog.ModHumanizerParams;
            _phaser90EParams = KatanaMkIIParameterCatalog.ModPhaser90EParams;
            _flanger117EParams = KatanaMkIIParameterCatalog.ModFlanger117EParams;
            _wah95EParams = KatanaMkIIParameterCatalog.ModWah95EParams;
            _dc30Params = KatanaMkIIParameterCatalog.ModDC30Params;
            _pedalBendParams = KatanaMkIIParameterCatalog.ModPedalBendParams;
        }
        else
        {
            _chorusParams = KatanaMkIIParameterCatalog.FxChorusParams;
            _flangerParams = KatanaMkIIParameterCatalog.FxFlangerParams;
            _phaserParams = KatanaMkIIParameterCatalog.FxPhaserParams;
            _uniVParams = KatanaMkIIParameterCatalog.FxUniVParams;
            _tremoloParams = KatanaMkIIParameterCatalog.FxTremoloParams;
            _vibratoParams = KatanaMkIIParameterCatalog.FxVibratoParams;
            _rotaryParams = KatanaMkIIParameterCatalog.FxRotaryParams;
            _ringModParams = KatanaMkIIParameterCatalog.FxRingModParams;
            _slowGearParams = KatanaMkIIParameterCatalog.FxSlowGearParams;
            _slicerParams = KatanaMkIIParameterCatalog.FxSlicerParams;
            _compParams = KatanaMkIIParameterCatalog.FxCompParams;
            _limiterParams = KatanaMkIIParameterCatalog.FxLimiterParams;
            _tWahParams = KatanaMkIIParameterCatalog.FxTWahParams;
            _autoWahParams = KatanaMkIIParameterCatalog.FxAutoWahParams;
            _pedalWahParams = KatanaMkIIParameterCatalog.FxPedalWahParams;
            _graphicEqParams = KatanaMkIIParameterCatalog.FxGraphicEqParams;
            _parametricEqParams = KatanaMkIIParameterCatalog.FxParametricEqParams;
            _guitarSimParams = KatanaMkIIParameterCatalog.FxGuitarSimParams;
            _acGuitarSimParams = KatanaMkIIParameterCatalog.FxAcGuitarSimParams;
            _acProcessorParams = KatanaMkIIParameterCatalog.FxAcProcessorParams;
            _waveSynthParams = KatanaMkIIParameterCatalog.FxWaveSynthParams;
            _octaveParams = KatanaMkIIParameterCatalog.FxOctaveParams;
            _heavyOctaveParams = KatanaMkIIParameterCatalog.FxHeavyOctaveParams;
            _pitchShifterParams = KatanaMkIIParameterCatalog.FxPitchShifterParams;
            _harmonistParams = KatanaMkIIParameterCatalog.FxHarmonistParams;
            _humanizerParams = KatanaMkIIParameterCatalog.FxHumanizerParams;
            _phaser90EParams = KatanaMkIIParameterCatalog.FxPhaser90EParams;
            _flanger117EParams = KatanaMkIIParameterCatalog.FxFlanger117EParams;
            _wah95EParams = KatanaMkIIParameterCatalog.FxWah95EParams;
            _dc30Params = KatanaMkIIParameterCatalog.FxDC30Params;
            _pedalBendParams = KatanaMkIIParameterCatalog.FxPedalBendParams;
        }
    }

    // ── Common view properties ────────────────────────────────────────────────────

    public IReadOnlyList<string> TypeOptions { get; }
    public bool HasTypeOptions => TypeOptions.Count > 0;
    public IBrush VariationBrush => GetVariationBrush(Variation);

    // Tracks type index for IsTypeXxx view switching.
    private int _selectedTypeIndex;
    public int SelectedTypeIndex
    {
        get => _selectedTypeIndex;
        private set
        {
            var __old = _selectedTypeIndex;
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

    public bool IsTypeChorus => SelectedTypeIndex == 0;
    public bool IsTypeFlanger => SelectedTypeIndex == 1;
    public bool IsTypePhaser => SelectedTypeIndex == 2;
    public bool IsTypeUnivibe => SelectedTypeIndex == 3;
    public bool IsTypeTremolo => SelectedTypeIndex == 4;
    public bool IsTypeVibrato => SelectedTypeIndex == 5;
    public bool IsTypeRotary => SelectedTypeIndex == 6;
    public bool IsTypeRingMod => SelectedTypeIndex == 7;
    public bool IsTypeSlowGear => SelectedTypeIndex == 8;
    public bool IsTypeSlicer => SelectedTypeIndex == 9;
    public bool IsTypeComp => SelectedTypeIndex == 10;
    public bool IsTypeLimiter => SelectedTypeIndex == 11;
    public bool IsTypeTouchWah => SelectedTypeIndex == 12;
    public bool IsTypeAutoWah => SelectedTypeIndex == 13;
    public bool IsTypePedalWah => SelectedTypeIndex == 14;
    public bool IsTypeGraphicEq => SelectedTypeIndex == 15;
    public bool IsTypeParametricEq => SelectedTypeIndex == 16;
    public bool IsTypeGuitarSim => SelectedTypeIndex == 17;
    public bool IsTypeAcGuitarSim => SelectedTypeIndex == 18;
    public bool IsTypeAcProcessor => SelectedTypeIndex == 19;
    public bool IsTypeWaveSynth => SelectedTypeIndex == 20;
    public bool IsTypeOctave => SelectedTypeIndex == 21;
    public bool IsTypeHeavyOctave => SelectedTypeIndex == 22;
    public bool IsTypePitchShifter => SelectedTypeIndex == 23;
    public bool IsTypeHarmonist => SelectedTypeIndex == 24;
    public bool IsTypeHumanizer => SelectedTypeIndex == 25;
    public bool IsTypePhaser90E => SelectedTypeIndex == 26;
    public bool IsTypeFlanger117E => SelectedTypeIndex == 27;
    public bool IsTypeWah95E => SelectedTypeIndex == 28;
    public bool IsTypeDc30 => SelectedTypeIndex == 29;
    public bool IsTypePedalBend => SelectedTypeIndex == 30;

    private string? _selectedTypeOption;
    public override string? SelectedTypeOption
    {
        get => _selectedTypeOption;
        set
        {
            if (!ChangeProperty(ref _selectedTypeOption, value)) return;
            this.RaisePropertyChanged(nameof(TypeCaption));
            if (TryGetTypeValue(value, out var byteValue))
                SelectedTypeIndex = byteValue;
        }
    }

    private string _variation = "N/A";
    public override string Variation
    {
        get => _variation;
        set
        {
            if (!ChangeProperty(ref _variation, value)) return;
            this.RaisePropertyChanged(nameof(VariationBrush));
        }
    }

    public override string TypeCaption => SelectedTypeOption ?? "—";

    public override bool TryGetTypeValue(string? option, out byte value)
    {
        if (option is not null && ReverseTypeTable.TryGetValue(option, out value))
            return true;
        value = 0;
        return false;
    }

    public override string ToTypeOption(byte rawValue) =>
        TypeTable.TryGetValue(rawValue, out var name) ? name : $"Type {rawValue}";

    private int _chorusXoverFreq;
    public int ChorusXoverFreq
    {
        get => _chorusXoverFreq;
        set => ChangeProperty(ref _chorusXoverFreq, value);
    }

    private int _chorusLowRate;
    public int ChorusLowRate
    {
        get => _chorusLowRate;
        set => ChangeProperty(ref _chorusLowRate, value);
    }

    private int _chorusLowDepth;
    public int ChorusLowDepth
    {
        get => _chorusLowDepth;
        set => ChangeProperty(ref _chorusLowDepth, value);
    }

    private int _chorusLowPreDelay;
    public int ChorusLowPreDelay
    {
        get => _chorusLowPreDelay;
        set => ChangeProperty(ref _chorusLowPreDelay, value);
    }

    private int _chorusLowLevel;
    public int ChorusLowLevel
    {
        get => _chorusLowLevel;
        set => ChangeProperty(ref _chorusLowLevel, value);
    }

    private int _chorusHighRate;
    public int ChorusHighRate
    {
        get => _chorusHighRate;
        set => ChangeProperty(ref _chorusHighRate, value);
    }

    private int _chorusHighDepth;
    public int ChorusHighDepth
    {
        get => _chorusHighDepth;
        set => ChangeProperty(ref _chorusHighDepth, value);
    }

    private int _chorusHighPreDelay;
    public int ChorusHighPreDelay
    {
        get => _chorusHighPreDelay;
        set => ChangeProperty(ref _chorusHighPreDelay, value);
    }

    private int _chorusHighLevel;
    public int ChorusHighLevel
    {
        get => _chorusHighLevel;
        set => ChangeProperty(ref _chorusHighLevel, value);
    }

    private int _chorusDirectMix;
    public int ChorusDirectMix
    {
        get => _chorusDirectMix;
        set => ChangeProperty(ref _chorusDirectMix, value);
    }

    // ── FLANGER params ────────────────────────────────────────────────────────────

    private int _flangerRate;
    public int FlangerRate
    {
        get => _flangerRate;
        set => ChangeProperty(ref _flangerRate, value);
    }

    private int _flangerDepth;
    public int FlangerDepth
    {
        get => _flangerDepth;
        set => ChangeProperty(ref _flangerDepth, value);
    }

    private int _flangerManual;
    public int FlangerManual
    {
        get => _flangerManual;
        set => ChangeProperty(ref _flangerManual, value);
    }

    private int _flangerResonance;
    public int FlangerResonance
    {
        get => _flangerResonance;
        set => ChangeProperty(ref _flangerResonance, value);
    }

    private int _flangerLowCut;
    public int FlangerLowCut
    {
        get => _flangerLowCut;
        set => ChangeProperty(ref _flangerLowCut, value);
    }

    private int _flangerEffectLevel;
    public int FlangerEffectLevel
    {
        get => _flangerEffectLevel;
        set => ChangeProperty(ref _flangerEffectLevel, value);
    }

    private int _flangerDirectMix;
    public int FlangerDirectMix
    {
        get => _flangerDirectMix;
        set => ChangeProperty(ref _flangerDirectMix, value);
    }

    // ── PHASER params ─────────────────────────────────────────────────────────────

    private int _phaserType;
    public int PhaserType
    {
        get => _phaserType;
        set => ChangeProperty(ref _phaserType, value);
    }

    private int _phaserRate;
    public int PhaserRate
    {
        get => _phaserRate;
        set => ChangeProperty(ref _phaserRate, value);
    }

    private int _phaserDepth;
    public int PhaserDepth
    {
        get => _phaserDepth;
        set => ChangeProperty(ref _phaserDepth, value);
    }

    private int _phaserManual;
    public int PhaserManual
    {
        get => _phaserManual;
        set => ChangeProperty(ref _phaserManual, value);
    }

    private int _phaserResonance;
    public int PhaserResonance
    {
        get => _phaserResonance;
        set => ChangeProperty(ref _phaserResonance, value);
    }

    private int _phaserStepRate;
    public int PhaserStepRate
    {
        get => _phaserStepRate;
        set => ChangeProperty(ref _phaserStepRate, value);
    }

    private int _phaserEffectLevel;
    public int PhaserEffectLevel
    {
        get => _phaserEffectLevel;
        set => ChangeProperty(ref _phaserEffectLevel, value);
    }

    private int _phaserDirectMix;
    public int PhaserDirectMix
    {
        get => _phaserDirectMix;
        set => ChangeProperty(ref _phaserDirectMix, value);
    }

    // ── UNI-V params ──────────────────────────────────────────────────────────────

    private int _uniVRate;
    public int UniVRate
    {
        get => _uniVRate;
        set => ChangeProperty(ref _uniVRate, value);
    }

    private int _uniVDepth;
    public int UniVDepth
    {
        get => _uniVDepth;
        set => ChangeProperty(ref _uniVDepth, value);
    }

    private int _uniVLevel;
    public int UniVLevel
    {
        get => _uniVLevel;
        set => ChangeProperty(ref _uniVLevel, value);
    }

    // ── TREMOLO params ────────────────────────────────────────────────────────────

    private int _tremoloWaveShape;
    public int TremoloWaveShape
    {
        get => _tremoloWaveShape;
        set => ChangeProperty(ref _tremoloWaveShape, value);
    }

    private int _tremoloRate;
    public int TremoloRate
    {
        get => _tremoloRate;
        set => ChangeProperty(ref _tremoloRate, value);
    }

    private int _tremoloDepth;
    public int TremoloDepth
    {
        get => _tremoloDepth;
        set => ChangeProperty(ref _tremoloDepth, value);
    }

    private int _tremoloLevel;
    public int TremoloLevel
    {
        get => _tremoloLevel;
        set => ChangeProperty(ref _tremoloLevel, value);
    }

    // ── VIBRATO params ────────────────────────────────────────────────────────────

    private int _vibratoRate;
    public int VibratoRate
    {
        get => _vibratoRate;
        set => ChangeProperty(ref _vibratoRate, value);
    }

    private int _vibratoDepth;
    public int VibratoDepth
    {
        get => _vibratoDepth;
        set => ChangeProperty(ref _vibratoDepth, value);
    }

    private int _vibratoLevel;
    public int VibratoLevel
    {
        get => _vibratoLevel;
        set => ChangeProperty(ref _vibratoLevel, value);
    }

    // ── ROTARY params ─────────────────────────────────────────────────────────────

    private int _rotaryRateFast;
    public int RotaryRateFast
    {
        get => _rotaryRateFast;
        set => ChangeProperty(ref _rotaryRateFast, value);
    }

    private int _rotaryDepth;
    public int RotaryDepth
    {
        get => _rotaryDepth;
        set => ChangeProperty(ref _rotaryDepth, value);
    }

    private int _rotaryLevel;
    public int RotaryLevel
    {
        get => _rotaryLevel;
        set => ChangeProperty(ref _rotaryLevel, value);
    }

    // ── RING MOD params ───────────────────────────────────────────────────────────

    private int _ringModMode;
    public int RingModMode
    {
        get => _ringModMode;
        set => ChangeProperty(ref _ringModMode, value);
    }

    private int _ringModFrequency;
    public int RingModFrequency
    {
        get => _ringModFrequency;
        set => ChangeProperty(ref _ringModFrequency, value);
    }

    private int _ringModEffectLevel;
    public int RingModEffectLevel
    {
        get => _ringModEffectLevel;
        set => ChangeProperty(ref _ringModEffectLevel, value);
    }

    private int _ringModDirectMix;
    public int RingModDirectMix
    {
        get => _ringModDirectMix;
        set => ChangeProperty(ref _ringModDirectMix, value);
    }

    // ── SLOW GEAR params ──────────────────────────────────────────────────────────

    private int _slowGearSens;
    public int SlowGearSens
    {
        get => _slowGearSens;
        set => ChangeProperty(ref _slowGearSens, value);
    }

    private int _slowGearRiseTime;
    public int SlowGearRiseTime
    {
        get => _slowGearRiseTime;
        set => ChangeProperty(ref _slowGearRiseTime, value);
    }

    private int _slowGearLevel;
    public int SlowGearLevel
    {
        get => _slowGearLevel;
        set => ChangeProperty(ref _slowGearLevel, value);
    }

    // ── SLICER params ─────────────────────────────────────────────────────────────

    private int _slicerPattern;
    public int SlicerPattern
    {
        get => _slicerPattern;
        set => ChangeProperty(ref _slicerPattern, value);
    }

    private int _slicerRate;
    public int SlicerRate
    {
        get => _slicerRate;
        set => ChangeProperty(ref _slicerRate, value);
    }

    private int _slicerTriggerSens;
    public int SlicerTriggerSens
    {
        get => _slicerTriggerSens;
        set => ChangeProperty(ref _slicerTriggerSens, value);
    }

    private int _slicerEffectLevel;
    public int SlicerEffectLevel
    {
        get => _slicerEffectLevel;
        set => ChangeProperty(ref _slicerEffectLevel, value);
    }

    private int _slicerDirectMix;
    public int SlicerDirectMix
    {
        get => _slicerDirectMix;
        set => ChangeProperty(ref _slicerDirectMix, value);
    }

    // ── COMP params ───────────────────────────────────────────────────────────────

    private int _compType;
    public int CompType
    {
        get => _compType;
        set => ChangeProperty(ref _compType, value);
    }

    private int _compSustain;
    public int CompSustain
    {
        get => _compSustain;
        set => ChangeProperty(ref _compSustain, value);
    }

    private int _compAttack;
    public int CompAttack
    {
        get => _compAttack;
        set => ChangeProperty(ref _compAttack, value);
    }

    private int _compTone;
    public int CompTone
    {
        get => _compTone;
        set => ChangeProperty(ref _compTone, value);
    }

    private int _compLevel;
    public int CompLevel
    {
        get => _compLevel;
        set => ChangeProperty(ref _compLevel, value);
    }

    // ── LIMITER params ────────────────────────────────────────────────────────────

    private int _limiterType;
    public int LimiterType
    {
        get => _limiterType;
        set => ChangeProperty(ref _limiterType, value);
    }

    private int _limiterAttack;
    public int LimiterAttack
    {
        get => _limiterAttack;
        set => ChangeProperty(ref _limiterAttack, value);
    }

    private int _limiterThreshold;
    public int LimiterThreshold
    {
        get => _limiterThreshold;
        set => ChangeProperty(ref _limiterThreshold, value);
    }

    private int _limiterRatio;
    public int LimiterRatio
    {
        get => _limiterRatio;
        set => ChangeProperty(ref _limiterRatio, value);
    }

    private int _limiterRelease;
    public int LimiterRelease
    {
        get => _limiterRelease;
        set => ChangeProperty(ref _limiterRelease, value);
    }

    private int _limiterLevel;
    public int LimiterLevel
    {
        get => _limiterLevel;
        set => ChangeProperty(ref _limiterLevel, value);
    }

    // ── T.WAH params ──────────────────────────────────────────────────────────────

    private int _tWahMode;
    public int TWahMode
    {
        get => _tWahMode;
        set => ChangeProperty(ref _tWahMode, value);
    }

    private int _tWahPolarity;
    public int TWahPolarity
    {
        get => _tWahPolarity;
        set => ChangeProperty(ref _tWahPolarity, value);
    }

    private int _tWahSens;
    public int TWahSens
    {
        get => _tWahSens;
        set => ChangeProperty(ref _tWahSens, value);
    }

    private int _tWahFreq;
    public int TWahFreq
    {
        get => _tWahFreq;
        set => ChangeProperty(ref _tWahFreq, value);
    }

    private int _tWahPeak;
    public int TWahPeak
    {
        get => _tWahPeak;
        set => ChangeProperty(ref _tWahPeak, value);
    }

    private int _tWahDirectMix;
    public int TWahDirectMix
    {
        get => _tWahDirectMix;
        set => ChangeProperty(ref _tWahDirectMix, value);
    }

    private int _tWahEffectLevel;
    public int TWahEffectLevel
    {
        get => _tWahEffectLevel;
        set => ChangeProperty(ref _tWahEffectLevel, value);
    }

    // ── AUTO WAH params ───────────────────────────────────────────────────────────

    private int _autoWahMode;
    public int AutoWahMode
    {
        get => _autoWahMode;
        set => ChangeProperty(ref _autoWahMode, value);
    }

    private int _autoWahFreq;
    public int AutoWahFreq
    {
        get => _autoWahFreq;
        set => ChangeProperty(ref _autoWahFreq, value);
    }

    private int _autoWahPeak;
    public int AutoWahPeak
    {
        get => _autoWahPeak;
        set => ChangeProperty(ref _autoWahPeak, value);
    }

    private int _autoWahRate;
    public int AutoWahRate
    {
        get => _autoWahRate;
        set => ChangeProperty(ref _autoWahRate, value);
    }

    private int _autoWahDepth;
    public int AutoWahDepth
    {
        get => _autoWahDepth;
        set => ChangeProperty(ref _autoWahDepth, value);
    }

    private int _autoWahDirectMix;
    public int AutoWahDirectMix
    {
        get => _autoWahDirectMix;
        set => ChangeProperty(ref _autoWahDirectMix, value);
    }

    private int _autoWahEffectLevel;
    public int AutoWahEffectLevel
    {
        get => _autoWahEffectLevel;
        set => ChangeProperty(ref _autoWahEffectLevel, value);
    }

    // ── PEDAL WAH params ──────────────────────────────────────────────────────────

    private int _pedalWahType;
    public int PedalWahType
    {
        get => _pedalWahType;
        set => ChangeProperty(ref _pedalWahType, value);
    }

    private int _pedalWahPedalPosition;
    public int PedalWahPedalPosition
    {
        get => _pedalWahPedalPosition;
        set => ChangeProperty(ref _pedalWahPedalPosition, value);
    }

    private int _pedalWahPedalMin;
    public int PedalWahPedalMin
    {
        get => _pedalWahPedalMin;
        set => ChangeProperty(ref _pedalWahPedalMin, value);
    }

    private int _pedalWahPedalMax;
    public int PedalWahPedalMax
    {
        get => _pedalWahPedalMax;
        set => ChangeProperty(ref _pedalWahPedalMax, value);
    }

    private int _pedalWahEffectLevel;
    public int PedalWahEffectLevel
    {
        get => _pedalWahEffectLevel;
        set => ChangeProperty(ref _pedalWahEffectLevel, value);
    }

    private int _pedalWahDirectMix;
    public int PedalWahDirectMix
    {
        get => _pedalWahDirectMix;
        set => ChangeProperty(ref _pedalWahDirectMix, value);
    }

    // ── GRAPHIC EQ params ─────────────────────────────────────────────────────────

    private int _graphicEq31Hz;
    public int GraphicEq31Hz { get => _graphicEq31Hz; set => ChangeProperty(ref _graphicEq31Hz, value); }

    private int _graphicEq62Hz;
    public int GraphicEq62Hz { get => _graphicEq62Hz; set => ChangeProperty(ref _graphicEq62Hz, value); }

    private int _graphicEq125Hz;
    public int GraphicEq125Hz { get => _graphicEq125Hz; set => ChangeProperty(ref _graphicEq125Hz, value); }

    private int _graphicEq250Hz;
    public int GraphicEq250Hz { get => _graphicEq250Hz; set => ChangeProperty(ref _graphicEq250Hz, value); }

    private int _graphicEq500Hz;
    public int GraphicEq500Hz { get => _graphicEq500Hz; set => ChangeProperty(ref _graphicEq500Hz, value); }

    private int _graphicEq1kHz;
    public int GraphicEq1kHz { get => _graphicEq1kHz; set => ChangeProperty(ref _graphicEq1kHz, value); }

    private int _graphicEq2kHz;
    public int GraphicEq2kHz { get => _graphicEq2kHz; set => ChangeProperty(ref _graphicEq2kHz, value); }

    private int _graphicEq4kHz;
    public int GraphicEq4kHz { get => _graphicEq4kHz; set => ChangeProperty(ref _graphicEq4kHz, value); }

    private int _graphicEq8kHz;
    public int GraphicEq8kHz { get => _graphicEq8kHz; set => ChangeProperty(ref _graphicEq8kHz, value); }

    private int _graphicEq16kHz;
    public int GraphicEq16kHz { get => _graphicEq16kHz; set => ChangeProperty(ref _graphicEq16kHz, value); }

    private int _graphicEqLevel;
    public int GraphicEqLevel { get => _graphicEqLevel; set => ChangeProperty(ref _graphicEqLevel, value); }

    // ── PARAMETRIC EQ params ──────────────────────────────────────────────────────

    private int _parametricEqLowCut;
    public int ParametricEqLowCut { get => _parametricEqLowCut; set => ChangeProperty(ref _parametricEqLowCut, value); }

    private int _parametricEqLowGain;
    public int ParametricEqLowGain { get => _parametricEqLowGain; set => ChangeProperty(ref _parametricEqLowGain, value); }

    private int _parametricEqLowMidFreq;
    public int ParametricEqLowMidFreq { get => _parametricEqLowMidFreq; set => ChangeProperty(ref _parametricEqLowMidFreq, value); }

    private int _parametricEqLowMidQ;
    public int ParametricEqLowMidQ { get => _parametricEqLowMidQ; set => ChangeProperty(ref _parametricEqLowMidQ, value); }

    private int _parametricEqLowMidGain;
    public int ParametricEqLowMidGain { get => _parametricEqLowMidGain; set => ChangeProperty(ref _parametricEqLowMidGain, value); }

    private int _parametricEqHighMidFreq;
    public int ParametricEqHighMidFreq { get => _parametricEqHighMidFreq; set => ChangeProperty(ref _parametricEqHighMidFreq, value); }

    private int _parametricEqHighMidQ;
    public int ParametricEqHighMidQ { get => _parametricEqHighMidQ; set => ChangeProperty(ref _parametricEqHighMidQ, value); }

    private int _parametricEqHighMidGain;
    public int ParametricEqHighMidGain { get => _parametricEqHighMidGain; set => ChangeProperty(ref _parametricEqHighMidGain, value); }

    private int _parametricEqHighGain;
    public int ParametricEqHighGain { get => _parametricEqHighGain; set => ChangeProperty(ref _parametricEqHighGain, value); }

    private int _parametricEqHighCut;
    public int ParametricEqHighCut { get => _parametricEqHighCut; set => ChangeProperty(ref _parametricEqHighCut, value); }

    private int _parametricEqLevel;
    public int ParametricEqLevel { get => _parametricEqLevel; set => ChangeProperty(ref _parametricEqLevel, value); }

    // ── GUITAR SIM params ─────────────────────────────────────────────────────────

    private int _guitarSimType;
    public int GuitarSimType { get => _guitarSimType; set => ChangeProperty(ref _guitarSimType, value); }

    private int _guitarSimLow;
    public int GuitarSimLow { get => _guitarSimLow; set => ChangeProperty(ref _guitarSimLow, value); }

    private int _guitarSimHigh;
    public int GuitarSimHigh { get => _guitarSimHigh; set => ChangeProperty(ref _guitarSimHigh, value); }

    private int _guitarSimLevel;
    public int GuitarSimLevel { get => _guitarSimLevel; set => ChangeProperty(ref _guitarSimLevel, value); }

    private int _guitarSimBody;
    public int GuitarSimBody { get => _guitarSimBody; set => ChangeProperty(ref _guitarSimBody, value); }

    // ── AC.GUITAR SIM params ──────────────────────────────────────────────────────

    private int _acGuitarSimHigh;
    public int AcGuitarSimHigh { get => _acGuitarSimHigh; set => ChangeProperty(ref _acGuitarSimHigh, value); }

    private int _acGuitarSimBody;
    public int AcGuitarSimBody { get => _acGuitarSimBody; set => ChangeProperty(ref _acGuitarSimBody, value); }

    private int _acGuitarSimLow;
    public int AcGuitarSimLow { get => _acGuitarSimLow; set => ChangeProperty(ref _acGuitarSimLow, value); }

    private int _acGuitarSimLevel;
    public int AcGuitarSimLevel { get => _acGuitarSimLevel; set => ChangeProperty(ref _acGuitarSimLevel, value); }

    // ── AC.PROCESSOR params ───────────────────────────────────────────────────────

    private int _acProcessorType;
    public int AcProcessorType { get => _acProcessorType; set => ChangeProperty(ref _acProcessorType, value); }

    private int _acProcessorBass;
    public int AcProcessorBass { get => _acProcessorBass; set => ChangeProperty(ref _acProcessorBass, value); }

    private int _acProcessorMid;
    public int AcProcessorMid { get => _acProcessorMid; set => ChangeProperty(ref _acProcessorMid, value); }

    private int _acProcessorMidFreq;
    public int AcProcessorMidFreq { get => _acProcessorMidFreq; set => ChangeProperty(ref _acProcessorMidFreq, value); }

    private int _acProcessorTreble;
    public int AcProcessorTreble { get => _acProcessorTreble; set => ChangeProperty(ref _acProcessorTreble, value); }

    private int _acProcessorPresence;
    public int AcProcessorPresence { get => _acProcessorPresence; set => ChangeProperty(ref _acProcessorPresence, value); }

    private int _acProcessorLevel;
    public int AcProcessorLevel { get => _acProcessorLevel; set => ChangeProperty(ref _acProcessorLevel, value); }

    // ── WAVE SYNTH params ─────────────────────────────────────────────────────────

    private int _waveSynthWave;
    public int WaveSynthWave { get => _waveSynthWave; set => ChangeProperty(ref _waveSynthWave, value); }

    private int _waveSynthCutoff;
    public int WaveSynthCutoff { get => _waveSynthCutoff; set => ChangeProperty(ref _waveSynthCutoff, value); }

    private int _waveSynthResonance;
    public int WaveSynthResonance { get => _waveSynthResonance; set => ChangeProperty(ref _waveSynthResonance, value); }

    private int _waveSynthFilterSens;
    public int WaveSynthFilterSens { get => _waveSynthFilterSens; set => ChangeProperty(ref _waveSynthFilterSens, value); }

    private int _waveSynthFilterDecay;
    public int WaveSynthFilterDecay { get => _waveSynthFilterDecay; set => ChangeProperty(ref _waveSynthFilterDecay, value); }

    private int _waveSynthFilterDepth;
    public int WaveSynthFilterDepth { get => _waveSynthFilterDepth; set => ChangeProperty(ref _waveSynthFilterDepth, value); }

    private int _waveSynthSynthLevel;
    public int WaveSynthSynthLevel { get => _waveSynthSynthLevel; set => ChangeProperty(ref _waveSynthSynthLevel, value); }

    private int _waveSynthDirectMix;
    public int WaveSynthDirectMix { get => _waveSynthDirectMix; set => ChangeProperty(ref _waveSynthDirectMix, value); }

    // ── OCTAVE params ─────────────────────────────────────────────────────────────

    private int _octaveRange;
    public int OctaveRange { get => _octaveRange; set => ChangeProperty(ref _octaveRange, value); }

    private int _octaveEffectLevel;
    public int OctaveEffectLevel { get => _octaveEffectLevel; set => ChangeProperty(ref _octaveEffectLevel, value); }

    private int _octaveDirectMix;
    public int OctaveDirectMix { get => _octaveDirectMix; set => ChangeProperty(ref _octaveDirectMix, value); }

    // ── HEAVY OCTAVE params ───────────────────────────────────────────────────────

    private int _heavyOctave1OctLevel;
    public int HeavyOctave1OctLevel { get => _heavyOctave1OctLevel; set => ChangeProperty(ref _heavyOctave1OctLevel, value); }

    private int _heavyOctave2OctLevel;
    public int HeavyOctave2OctLevel { get => _heavyOctave2OctLevel; set => ChangeProperty(ref _heavyOctave2OctLevel, value); }

    private int _heavyOctaveDirectMix;
    public int HeavyOctaveDirectMix { get => _heavyOctaveDirectMix; set => ChangeProperty(ref _heavyOctaveDirectMix, value); }

    // ── PITCH SHIFTER params ──────────────────────────────────────────────────────

    private int _pitchShifterVoice;
    public int PitchShifterVoice { get => _pitchShifterVoice; set => ChangeProperty(ref _pitchShifterVoice, value); }

    private int _pitchShifterPS1Mode;
    public int PitchShifterPS1Mode { get => _pitchShifterPS1Mode; set => ChangeProperty(ref _pitchShifterPS1Mode, value); }

    private int _pitchShifterPS1Pitch;
    public int PitchShifterPS1Pitch { get => _pitchShifterPS1Pitch; set => ChangeProperty(ref _pitchShifterPS1Pitch, value); }

    private int _pitchShifterPS1Fine;
    public int PitchShifterPS1Fine { get => _pitchShifterPS1Fine; set => ChangeProperty(ref _pitchShifterPS1Fine, value); }

    private int _pitchShifterPS1Level;
    public int PitchShifterPS1Level { get => _pitchShifterPS1Level; set => ChangeProperty(ref _pitchShifterPS1Level, value); }

    private int _pitchShifterPS2Mode;
    public int PitchShifterPS2Mode { get => _pitchShifterPS2Mode; set => ChangeProperty(ref _pitchShifterPS2Mode, value); }

    private int _pitchShifterPS2Pitch;
    public int PitchShifterPS2Pitch { get => _pitchShifterPS2Pitch; set => ChangeProperty(ref _pitchShifterPS2Pitch, value); }

    private int _pitchShifterPS2Fine;
    public int PitchShifterPS2Fine { get => _pitchShifterPS2Fine; set => ChangeProperty(ref _pitchShifterPS2Fine, value); }

    private int _pitchShifterPS2Level;
    public int PitchShifterPS2Level { get => _pitchShifterPS2Level; set => ChangeProperty(ref _pitchShifterPS2Level, value); }

    private int _pitchShifterFeedback;
    public int PitchShifterFeedback { get => _pitchShifterFeedback; set => ChangeProperty(ref _pitchShifterFeedback, value); }

    private int _pitchShifterDirectMix;
    public int PitchShifterDirectMix { get => _pitchShifterDirectMix; set => ChangeProperty(ref _pitchShifterDirectMix, value); }

    // ── HARMONIST params ──────────────────────────────────────────────────────────

    private int _harmonistVoice;
    public int HarmonistVoice { get => _harmonistVoice; set => ChangeProperty(ref _harmonistVoice, value); }

    private int _harmonistHarmony1;
    public int HarmonistHarmony1 { get => _harmonistHarmony1; set => ChangeProperty(ref _harmonistHarmony1, value); }

    private int _harmonistLevel1;
    public int HarmonistLevel1 { get => _harmonistLevel1; set => ChangeProperty(ref _harmonistLevel1, value); }

    private int _harmonistHarmony2;
    public int HarmonistHarmony2 { get => _harmonistHarmony2; set => ChangeProperty(ref _harmonistHarmony2, value); }

    private int _harmonistLevel2;
    public int HarmonistLevel2 { get => _harmonistLevel2; set => ChangeProperty(ref _harmonistLevel2, value); }

    private int _harmonistFeedback;
    public int HarmonistFeedback { get => _harmonistFeedback; set => ChangeProperty(ref _harmonistFeedback, value); }

    private int _harmonistDirectMix;
    public int HarmonistDirectMix { get => _harmonistDirectMix; set => ChangeProperty(ref _harmonistDirectMix, value); }

    // ── HUMANIZER params ──────────────────────────────────────────────────────────

    private int _humanizerMode;
    public int HumanizerMode { get => _humanizerMode; set => ChangeProperty(ref _humanizerMode, value); }

    private int _humanizerVowel1;
    public int HumanizerVowel1 { get => _humanizerVowel1; set => ChangeProperty(ref _humanizerVowel1, value); }

    private int _humanizerVowel2;
    public int HumanizerVowel2 { get => _humanizerVowel2; set => ChangeProperty(ref _humanizerVowel2, value); }

    private int _humanizerSens;
    public int HumanizerSens { get => _humanizerSens; set => ChangeProperty(ref _humanizerSens, value); }

    private int _humanizerRate;
    public int HumanizerRate { get => _humanizerRate; set => ChangeProperty(ref _humanizerRate, value); }

    private int _humanizerDepth;
    public int HumanizerDepth { get => _humanizerDepth; set => ChangeProperty(ref _humanizerDepth, value); }

    private int _humanizerManual;
    public int HumanizerManual { get => _humanizerManual; set => ChangeProperty(ref _humanizerManual, value); }

    private int _humanizerLevel;
    public int HumanizerLevel { get => _humanizerLevel; set => ChangeProperty(ref _humanizerLevel, value); }

    // ── PHASER 90E params ─────────────────────────────────────────────────────────

    private int _phaser90EScript;
    public int Phaser90EScript { get => _phaser90EScript; set => ChangeProperty(ref _phaser90EScript, value); }

    private int _phaser90ESpeed;
    public int Phaser90ESpeed { get => _phaser90ESpeed; set => ChangeProperty(ref _phaser90ESpeed, value); }

    // ── FLANGER 117E params ───────────────────────────────────────────────────────

    private int _flanger117EManual;
    public int Flanger117EManual { get => _flanger117EManual; set => ChangeProperty(ref _flanger117EManual, value); }

    private int _flanger117EWidth;
    public int Flanger117EWidth { get => _flanger117EWidth; set => ChangeProperty(ref _flanger117EWidth, value); }

    private int _flanger117ESpeed;
    public int Flanger117ESpeed { get => _flanger117ESpeed; set => ChangeProperty(ref _flanger117ESpeed, value); }

    private int _flanger117ERegen;
    public int Flanger117ERegen { get => _flanger117ERegen; set => ChangeProperty(ref _flanger117ERegen, value); }

    // ── WAH 95E params ────────────────────────────────────────────────────────────

    private int _wah95EPedalPosition;
    public int Wah95EPedalPosition { get => _wah95EPedalPosition; set => ChangeProperty(ref _wah95EPedalPosition, value); }

    private int _wah95EPedalMin;
    public int Wah95EPedalMin { get => _wah95EPedalMin; set => ChangeProperty(ref _wah95EPedalMin, value); }

    private int _wah95EPedalMax;
    public int Wah95EPedalMax { get => _wah95EPedalMax; set => ChangeProperty(ref _wah95EPedalMax, value); }

    private int _wah95EEffectLevel;
    public int Wah95EEffectLevel { get => _wah95EEffectLevel; set => ChangeProperty(ref _wah95EEffectLevel, value); }

    private int _wah95EDirectMix;
    public int Wah95EDirectMix { get => _wah95EDirectMix; set => ChangeProperty(ref _wah95EDirectMix, value); }

    // ── DC-30 params ──────────────────────────────────────────────────────────────

    private int _dC30Selector;
    public int DC30Selector { get => _dC30Selector; set => ChangeProperty(ref _dC30Selector, value); }

    private int _dC30InputVolume;
    public int DC30InputVolume { get => _dC30InputVolume; set => ChangeProperty(ref _dC30InputVolume, value); }

    private int _dC30ChorusIntensity;
    public int DC30ChorusIntensity { get => _dC30ChorusIntensity; set => ChangeProperty(ref _dC30ChorusIntensity, value); }

    private int _dC30EchoIntensity;
    public int DC30EchoIntensity { get => _dC30EchoIntensity; set => ChangeProperty(ref _dC30EchoIntensity, value); }

    private int _dC30EchoVolume;
    public int DC30EchoVolume { get => _dC30EchoVolume; set => ChangeProperty(ref _dC30EchoVolume, value); }

    private int _dC30Tone;
    public int DC30Tone { get => _dC30Tone; set => ChangeProperty(ref _dC30Tone, value); }

    private int _dC30Output;
    public int DC30Output { get => _dC30Output; set => ChangeProperty(ref _dC30Output, value); }

    // ── PEDAL BEND params ─────────────────────────────────────────────────────────

    private int _pedalBendPitch;
    public int PedalBendPitch { get => _pedalBendPitch; set => ChangeProperty(ref _pedalBendPitch, value); }

    private int _pedalBendPedalPosition;
    public int PedalBendPedalPosition { get => _pedalBendPedalPosition; set => ChangeProperty(ref _pedalBendPedalPosition, value); }

    private int _pedalBendEffectLevel;
    public int PedalBendEffectLevel { get => _pedalBendEffectLevel; set => ChangeProperty(ref _pedalBendEffectLevel, value); }

    private int _pedalBendDirectMix;
    public int PedalBendDirectMix { get => _pedalBendDirectMix; set => ChangeProperty(ref _pedalBendDirectMix, value); }

}
