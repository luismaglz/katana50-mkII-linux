using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Kataka.Domain.Midi;

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
            _chorusParams       = KatanaMkIIParameterCatalog.ModChorusParams;
            _flangerParams      = KatanaMkIIParameterCatalog.ModFlangerParams;
            _phaserParams       = KatanaMkIIParameterCatalog.ModPhaserParams;
            _uniVParams         = KatanaMkIIParameterCatalog.ModUniVParams;
            _tremoloParams      = KatanaMkIIParameterCatalog.ModTremoloParams;
            _vibratoParams      = KatanaMkIIParameterCatalog.ModVibratoParams;
            _rotaryParams       = KatanaMkIIParameterCatalog.ModRotaryParams;
            _ringModParams      = KatanaMkIIParameterCatalog.ModRingModParams;
            _slowGearParams     = KatanaMkIIParameterCatalog.ModSlowGearParams;
            _slicerParams       = KatanaMkIIParameterCatalog.ModSlicerParams;
            _compParams         = KatanaMkIIParameterCatalog.ModCompParams;
            _limiterParams      = KatanaMkIIParameterCatalog.ModLimiterParams;
            _tWahParams         = KatanaMkIIParameterCatalog.ModTWahParams;
            _autoWahParams      = KatanaMkIIParameterCatalog.ModAutoWahParams;
            _pedalWahParams     = KatanaMkIIParameterCatalog.ModPedalWahParams;
            _graphicEqParams    = KatanaMkIIParameterCatalog.ModGraphicEqParams;
            _parametricEqParams = KatanaMkIIParameterCatalog.ModParametricEqParams;
            _guitarSimParams    = KatanaMkIIParameterCatalog.ModGuitarSimParams;
            _acGuitarSimParams  = KatanaMkIIParameterCatalog.ModAcGuitarSimParams;
            _acProcessorParams  = KatanaMkIIParameterCatalog.ModAcProcessorParams;
            _waveSynthParams    = KatanaMkIIParameterCatalog.ModWaveSynthParams;
            _octaveParams       = KatanaMkIIParameterCatalog.ModOctaveParams;
            _heavyOctaveParams  = KatanaMkIIParameterCatalog.ModHeavyOctaveParams;
            _pitchShifterParams = KatanaMkIIParameterCatalog.ModPitchShifterParams;
            _harmonistParams    = KatanaMkIIParameterCatalog.ModHarmonistParams;
            _humanizerParams    = KatanaMkIIParameterCatalog.ModHumanizerParams;
            _phaser90EParams    = KatanaMkIIParameterCatalog.ModPhaser90EParams;
            _flanger117EParams  = KatanaMkIIParameterCatalog.ModFlanger117EParams;
            _wah95EParams       = KatanaMkIIParameterCatalog.ModWah95EParams;
            _dc30Params         = KatanaMkIIParameterCatalog.ModDC30Params;
            _pedalBendParams    = KatanaMkIIParameterCatalog.ModPedalBendParams;
        }
        else
        {
            _chorusParams       = KatanaMkIIParameterCatalog.FxChorusParams;
            _flangerParams      = KatanaMkIIParameterCatalog.FxFlangerParams;
            _phaserParams       = KatanaMkIIParameterCatalog.FxPhaserParams;
            _uniVParams         = KatanaMkIIParameterCatalog.FxUniVParams;
            _tremoloParams      = KatanaMkIIParameterCatalog.FxTremoloParams;
            _vibratoParams      = KatanaMkIIParameterCatalog.FxVibratoParams;
            _rotaryParams       = KatanaMkIIParameterCatalog.FxRotaryParams;
            _ringModParams      = KatanaMkIIParameterCatalog.FxRingModParams;
            _slowGearParams     = KatanaMkIIParameterCatalog.FxSlowGearParams;
            _slicerParams       = KatanaMkIIParameterCatalog.FxSlicerParams;
            _compParams         = KatanaMkIIParameterCatalog.FxCompParams;
            _limiterParams      = KatanaMkIIParameterCatalog.FxLimiterParams;
            _tWahParams         = KatanaMkIIParameterCatalog.FxTWahParams;
            _autoWahParams      = KatanaMkIIParameterCatalog.FxAutoWahParams;
            _pedalWahParams     = KatanaMkIIParameterCatalog.FxPedalWahParams;
            _graphicEqParams    = KatanaMkIIParameterCatalog.FxGraphicEqParams;
            _parametricEqParams = KatanaMkIIParameterCatalog.FxParametricEqParams;
            _guitarSimParams    = KatanaMkIIParameterCatalog.FxGuitarSimParams;
            _acGuitarSimParams  = KatanaMkIIParameterCatalog.FxAcGuitarSimParams;
            _acProcessorParams  = KatanaMkIIParameterCatalog.FxAcProcessorParams;
            _waveSynthParams    = KatanaMkIIParameterCatalog.FxWaveSynthParams;
            _octaveParams       = KatanaMkIIParameterCatalog.FxOctaveParams;
            _heavyOctaveParams  = KatanaMkIIParameterCatalog.FxHeavyOctaveParams;
            _pitchShifterParams = KatanaMkIIParameterCatalog.FxPitchShifterParams;
            _harmonistParams    = KatanaMkIIParameterCatalog.FxHarmonistParams;
            _humanizerParams    = KatanaMkIIParameterCatalog.FxHumanizerParams;
            _phaser90EParams    = KatanaMkIIParameterCatalog.FxPhaser90EParams;
            _flanger117EParams  = KatanaMkIIParameterCatalog.FxFlanger117EParams;
            _wah95EParams       = KatanaMkIIParameterCatalog.FxWah95EParams;
            _dc30Params         = KatanaMkIIParameterCatalog.FxDC30Params;
            _pedalBendParams    = KatanaMkIIParameterCatalog.FxPedalBendParams;
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
            if (!SetProperty(ref _selectedTypeIndex, value)) return;
            OnPropertyChanged(nameof(IsTypeChorus));
            OnPropertyChanged(nameof(IsTypeFlanger));
            OnPropertyChanged(nameof(IsTypePhaser));
            OnPropertyChanged(nameof(IsTypeUnivibe));
            OnPropertyChanged(nameof(IsTypeTremolo));
            OnPropertyChanged(nameof(IsTypeVibrato));
            OnPropertyChanged(nameof(IsTypeRotary));
            OnPropertyChanged(nameof(IsTypeRingMod));
            OnPropertyChanged(nameof(IsTypeSlowGear));
            OnPropertyChanged(nameof(IsTypeSlicer));
            OnPropertyChanged(nameof(IsTypeComp));
            OnPropertyChanged(nameof(IsTypeLimiter));
            OnPropertyChanged(nameof(IsTypeTouchWah));
            OnPropertyChanged(nameof(IsTypeAutoWah));
            OnPropertyChanged(nameof(IsTypePedalWah));
            OnPropertyChanged(nameof(IsTypeGraphicEq));
            OnPropertyChanged(nameof(IsTypeParametricEq));
            OnPropertyChanged(nameof(IsTypeGuitarSim));
            OnPropertyChanged(nameof(IsTypeAcGuitarSim));
            OnPropertyChanged(nameof(IsTypeAcProcessor));
            OnPropertyChanged(nameof(IsTypeWaveSynth));
            OnPropertyChanged(nameof(IsTypeOctave));
            OnPropertyChanged(nameof(IsTypeHeavyOctave));
            OnPropertyChanged(nameof(IsTypePitchShifter));
            OnPropertyChanged(nameof(IsTypeHarmonist));
            OnPropertyChanged(nameof(IsTypeHumanizer));
            OnPropertyChanged(nameof(IsTypePhaser90E));
            OnPropertyChanged(nameof(IsTypeFlanger117E));
            OnPropertyChanged(nameof(IsTypeWah95E));
            OnPropertyChanged(nameof(IsTypeDc30));
            OnPropertyChanged(nameof(IsTypePedalBend));
        }
    }

    public bool IsTypeChorus        => SelectedTypeIndex == 0;
    public bool IsTypeFlanger       => SelectedTypeIndex == 1;
    public bool IsTypePhaser        => SelectedTypeIndex == 2;
    public bool IsTypeUnivibe       => SelectedTypeIndex == 3;
    public bool IsTypeTremolo       => SelectedTypeIndex == 4;
    public bool IsTypeVibrato       => SelectedTypeIndex == 5;
    public bool IsTypeRotary        => SelectedTypeIndex == 6;
    public bool IsTypeRingMod       => SelectedTypeIndex == 7;
    public bool IsTypeSlowGear      => SelectedTypeIndex == 8;
    public bool IsTypeSlicer        => SelectedTypeIndex == 9;
    public bool IsTypeComp          => SelectedTypeIndex == 10;
    public bool IsTypeLimiter       => SelectedTypeIndex == 11;
    public bool IsTypeTouchWah      => SelectedTypeIndex == 12;
    public bool IsTypeAutoWah       => SelectedTypeIndex == 13;
    public bool IsTypePedalWah      => SelectedTypeIndex == 14;
    public bool IsTypeGraphicEq     => SelectedTypeIndex == 15;
    public bool IsTypeParametricEq  => SelectedTypeIndex == 16;
    public bool IsTypeGuitarSim     => SelectedTypeIndex == 17;
    public bool IsTypeAcGuitarSim   => SelectedTypeIndex == 18;
    public bool IsTypeAcProcessor   => SelectedTypeIndex == 19;
    public bool IsTypeWaveSynth     => SelectedTypeIndex == 20;
    public bool IsTypeOctave        => SelectedTypeIndex == 21;
    public bool IsTypeHeavyOctave   => SelectedTypeIndex == 22;
    public bool IsTypePitchShifter  => SelectedTypeIndex == 23;
    public bool IsTypeHarmonist     => SelectedTypeIndex == 24;
    public bool IsTypeHumanizer     => SelectedTypeIndex == 25;
    public bool IsTypePhaser90E     => SelectedTypeIndex == 26;
    public bool IsTypeFlanger117E   => SelectedTypeIndex == 27;
    public bool IsTypeWah95E        => SelectedTypeIndex == 28;
    public bool IsTypeDc30          => SelectedTypeIndex == 29;
    public bool IsTypePedalBend     => SelectedTypeIndex == 30;

    // ── IBasePedal abstract overrides ────────────────────────────────────────────

    private string? _selectedTypeOption;
    public override string? SelectedTypeOption
    {
        get => _selectedTypeOption;
        set
        {
            if (!SetProperty(ref _selectedTypeOption, value)) return;
            OnPropertyChanged(nameof(TypeCaption));
            if (TryGetTypeValue(value, out var byteValue))
                SelectedTypeIndex = byteValue;
            if (SuppressingAmpApply || Definition.TypeParameter is null || !TryGetTypeValue(value, out var bv)) return;
            RaiseParameterChanged(Definition.TypeParameter.Key, bv);
        }
    }

    private string _variation = "N/A";
    public override string Variation
    {
        get => _variation;
        set
        {
            if (!SetProperty(ref _variation, value)) return;
            OnPropertyChanged(nameof(VariationBrush));
        }
    }

    private int _level;
    public int Level
    {
        get => _level;
        set
        {
            if (!SetProperty(ref _level, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(Definition.LevelParameter!.Key, value);
        }
    }

    public bool HasLevel => Definition.LevelParameter is not null;
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
        set { if (SetProperty(ref _chorusXoverFreq, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[0].Key, value); }
    }

    private int _chorusLowRate;
    public int ChorusLowRate
    {
        get => _chorusLowRate;
        set { if (SetProperty(ref _chorusLowRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[1].Key, value); }
    }

    private int _chorusLowDepth;
    public int ChorusLowDepth
    {
        get => _chorusLowDepth;
        set { if (SetProperty(ref _chorusLowDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[2].Key, value); }
    }

    private int _chorusLowPreDelay;
    public int ChorusLowPreDelay
    {
        get => _chorusLowPreDelay;
        set { if (SetProperty(ref _chorusLowPreDelay, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[3].Key, value); }
    }

    private int _chorusLowLevel;
    public int ChorusLowLevel
    {
        get => _chorusLowLevel;
        set { if (SetProperty(ref _chorusLowLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[4].Key, value); }
    }

    private int _chorusHighRate;
    public int ChorusHighRate
    {
        get => _chorusHighRate;
        set { if (SetProperty(ref _chorusHighRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[5].Key, value); }
    }

    private int _chorusHighDepth;
    public int ChorusHighDepth
    {
        get => _chorusHighDepth;
        set { if (SetProperty(ref _chorusHighDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[6].Key, value); }
    }

    private int _chorusHighPreDelay;
    public int ChorusHighPreDelay
    {
        get => _chorusHighPreDelay;
        set { if (SetProperty(ref _chorusHighPreDelay, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[7].Key, value); }
    }

    private int _chorusHighLevel;
    public int ChorusHighLevel
    {
        get => _chorusHighLevel;
        set { if (SetProperty(ref _chorusHighLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[8].Key, value); }
    }

    private int _chorusDirectMix;
    public int ChorusDirectMix
    {
        get => _chorusDirectMix;
        set { if (SetProperty(ref _chorusDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_chorusParams[9].Key, value); }
    }

    // ── FLANGER params ────────────────────────────────────────────────────────────

    private int _flangerRate;
    public int FlangerRate
    {
        get => _flangerRate;
        set { if (SetProperty(ref _flangerRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_flangerParams[0].Key, value); }
    }

    private int _flangerDepth;
    public int FlangerDepth
    {
        get => _flangerDepth;
        set { if (SetProperty(ref _flangerDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_flangerParams[1].Key, value); }
    }

    private int _flangerManual;
    public int FlangerManual
    {
        get => _flangerManual;
        set { if (SetProperty(ref _flangerManual, value) && !SuppressingAmpApply) RaiseParameterChanged(_flangerParams[2].Key, value); }
    }

    private int _flangerResonance;
    public int FlangerResonance
    {
        get => _flangerResonance;
        set { if (SetProperty(ref _flangerResonance, value) && !SuppressingAmpApply) RaiseParameterChanged(_flangerParams[3].Key, value); }
    }

    private int _flangerLowCut;
    public int FlangerLowCut
    {
        get => _flangerLowCut;
        set { if (SetProperty(ref _flangerLowCut, value) && !SuppressingAmpApply) RaiseParameterChanged(_flangerParams[4].Key, value); }
    }

    private int _flangerEffectLevel;
    public int FlangerEffectLevel
    {
        get => _flangerEffectLevel;
        set { if (SetProperty(ref _flangerEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_flangerParams[5].Key, value); }
    }

    private int _flangerDirectMix;
    public int FlangerDirectMix
    {
        get => _flangerDirectMix;
        set { if (SetProperty(ref _flangerDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_flangerParams[6].Key, value); }
    }

    // ── PHASER params ─────────────────────────────────────────────────────────────

    private int _phaserType;
    public int PhaserType
    {
        get => _phaserType;
        set { if (SetProperty(ref _phaserType, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[0].Key, value); }
    }

    private int _phaserRate;
    public int PhaserRate
    {
        get => _phaserRate;
        set { if (SetProperty(ref _phaserRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[1].Key, value); }
    }

    private int _phaserDepth;
    public int PhaserDepth
    {
        get => _phaserDepth;
        set { if (SetProperty(ref _phaserDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[2].Key, value); }
    }

    private int _phaserManual;
    public int PhaserManual
    {
        get => _phaserManual;
        set { if (SetProperty(ref _phaserManual, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[3].Key, value); }
    }

    private int _phaserResonance;
    public int PhaserResonance
    {
        get => _phaserResonance;
        set { if (SetProperty(ref _phaserResonance, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[4].Key, value); }
    }

    private int _phaserStepRate;
    public int PhaserStepRate
    {
        get => _phaserStepRate;
        set { if (SetProperty(ref _phaserStepRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[5].Key, value); }
    }

    private int _phaserEffectLevel;
    public int PhaserEffectLevel
    {
        get => _phaserEffectLevel;
        set { if (SetProperty(ref _phaserEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[6].Key, value); }
    }

    private int _phaserDirectMix;
    public int PhaserDirectMix
    {
        get => _phaserDirectMix;
        set { if (SetProperty(ref _phaserDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaserParams[7].Key, value); }
    }

    // ── UNI-V params ──────────────────────────────────────────────────────────────

    private int _uniVRate;
    public int UniVRate
    {
        get => _uniVRate;
        set { if (SetProperty(ref _uniVRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_uniVParams[0].Key, value); }
    }

    private int _uniVDepth;
    public int UniVDepth
    {
        get => _uniVDepth;
        set { if (SetProperty(ref _uniVDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_uniVParams[1].Key, value); }
    }

    private int _uniVLevel;
    public int UniVLevel
    {
        get => _uniVLevel;
        set { if (SetProperty(ref _uniVLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_uniVParams[2].Key, value); }
    }

    // ── TREMOLO params ────────────────────────────────────────────────────────────

    private int _tremoloWaveShape;
    public int TremoloWaveShape
    {
        get => _tremoloWaveShape;
        set { if (SetProperty(ref _tremoloWaveShape, value) && !SuppressingAmpApply) RaiseParameterChanged(_tremoloParams[0].Key, value); }
    }

    private int _tremoloRate;
    public int TremoloRate
    {
        get => _tremoloRate;
        set { if (SetProperty(ref _tremoloRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_tremoloParams[1].Key, value); }
    }

    private int _tremoloDepth;
    public int TremoloDepth
    {
        get => _tremoloDepth;
        set { if (SetProperty(ref _tremoloDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_tremoloParams[2].Key, value); }
    }

    private int _tremoloLevel;
    public int TremoloLevel
    {
        get => _tremoloLevel;
        set { if (SetProperty(ref _tremoloLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_tremoloParams[3].Key, value); }
    }

    // ── VIBRATO params ────────────────────────────────────────────────────────────

    private int _vibratoRate;
    public int VibratoRate
    {
        get => _vibratoRate;
        set { if (SetProperty(ref _vibratoRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_vibratoParams[0].Key, value); }
    }

    private int _vibratoDepth;
    public int VibratoDepth
    {
        get => _vibratoDepth;
        set { if (SetProperty(ref _vibratoDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_vibratoParams[1].Key, value); }
    }

    private int _vibratoLevel;
    public int VibratoLevel
    {
        get => _vibratoLevel;
        set { if (SetProperty(ref _vibratoLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_vibratoParams[2].Key, value); }
    }

    // ── ROTARY params ─────────────────────────────────────────────────────────────

    private int _rotaryRateFast;
    public int RotaryRateFast
    {
        get => _rotaryRateFast;
        set { if (SetProperty(ref _rotaryRateFast, value) && !SuppressingAmpApply) RaiseParameterChanged(_rotaryParams[0].Key, value); }
    }

    private int _rotaryDepth;
    public int RotaryDepth
    {
        get => _rotaryDepth;
        set { if (SetProperty(ref _rotaryDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_rotaryParams[1].Key, value); }
    }

    private int _rotaryLevel;
    public int RotaryLevel
    {
        get => _rotaryLevel;
        set { if (SetProperty(ref _rotaryLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_rotaryParams[2].Key, value); }
    }

    // ── RING MOD params ───────────────────────────────────────────────────────────

    private int _ringModMode;
    public int RingModMode
    {
        get => _ringModMode;
        set { if (SetProperty(ref _ringModMode, value) && !SuppressingAmpApply) RaiseParameterChanged(_ringModParams[0].Key, value); }
    }

    private int _ringModFrequency;
    public int RingModFrequency
    {
        get => _ringModFrequency;
        set { if (SetProperty(ref _ringModFrequency, value) && !SuppressingAmpApply) RaiseParameterChanged(_ringModParams[1].Key, value); }
    }

    private int _ringModEffectLevel;
    public int RingModEffectLevel
    {
        get => _ringModEffectLevel;
        set { if (SetProperty(ref _ringModEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_ringModParams[2].Key, value); }
    }

    private int _ringModDirectMix;
    public int RingModDirectMix
    {
        get => _ringModDirectMix;
        set { if (SetProperty(ref _ringModDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_ringModParams[3].Key, value); }
    }

    // ── SLOW GEAR params ──────────────────────────────────────────────────────────

    private int _slowGearSens;
    public int SlowGearSens
    {
        get => _slowGearSens;
        set { if (SetProperty(ref _slowGearSens, value) && !SuppressingAmpApply) RaiseParameterChanged(_slowGearParams[0].Key, value); }
    }

    private int _slowGearRiseTime;
    public int SlowGearRiseTime
    {
        get => _slowGearRiseTime;
        set { if (SetProperty(ref _slowGearRiseTime, value) && !SuppressingAmpApply) RaiseParameterChanged(_slowGearParams[1].Key, value); }
    }

    private int _slowGearLevel;
    public int SlowGearLevel
    {
        get => _slowGearLevel;
        set { if (SetProperty(ref _slowGearLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_slowGearParams[2].Key, value); }
    }

    // ── SLICER params ─────────────────────────────────────────────────────────────

    private int _slicerPattern;
    public int SlicerPattern
    {
        get => _slicerPattern;
        set { if (SetProperty(ref _slicerPattern, value) && !SuppressingAmpApply) RaiseParameterChanged(_slicerParams[0].Key, value); }
    }

    private int _slicerRate;
    public int SlicerRate
    {
        get => _slicerRate;
        set { if (SetProperty(ref _slicerRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_slicerParams[1].Key, value); }
    }

    private int _slicerTriggerSens;
    public int SlicerTriggerSens
    {
        get => _slicerTriggerSens;
        set { if (SetProperty(ref _slicerTriggerSens, value) && !SuppressingAmpApply) RaiseParameterChanged(_slicerParams[2].Key, value); }
    }

    private int _slicerEffectLevel;
    public int SlicerEffectLevel
    {
        get => _slicerEffectLevel;
        set { if (SetProperty(ref _slicerEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_slicerParams[3].Key, value); }
    }

    private int _slicerDirectMix;
    public int SlicerDirectMix
    {
        get => _slicerDirectMix;
        set { if (SetProperty(ref _slicerDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_slicerParams[4].Key, value); }
    }

    // ── COMP params ───────────────────────────────────────────────────────────────

    private int _compType;
    public int CompType
    {
        get => _compType;
        set { if (SetProperty(ref _compType, value) && !SuppressingAmpApply) RaiseParameterChanged(_compParams[0].Key, value); }
    }

    private int _compSustain;
    public int CompSustain
    {
        get => _compSustain;
        set { if (SetProperty(ref _compSustain, value) && !SuppressingAmpApply) RaiseParameterChanged(_compParams[1].Key, value); }
    }

    private int _compAttack;
    public int CompAttack
    {
        get => _compAttack;
        set { if (SetProperty(ref _compAttack, value) && !SuppressingAmpApply) RaiseParameterChanged(_compParams[2].Key, value); }
    }

    private int _compTone;
    public int CompTone
    {
        get => _compTone;
        set { if (SetProperty(ref _compTone, value) && !SuppressingAmpApply) RaiseParameterChanged(_compParams[3].Key, value); }
    }

    private int _compLevel;
    public int CompLevel
    {
        get => _compLevel;
        set { if (SetProperty(ref _compLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_compParams[4].Key, value); }
    }

    // ── LIMITER params ────────────────────────────────────────────────────────────

    private int _limiterType;
    public int LimiterType
    {
        get => _limiterType;
        set { if (SetProperty(ref _limiterType, value) && !SuppressingAmpApply) RaiseParameterChanged(_limiterParams[0].Key, value); }
    }

    private int _limiterAttack;
    public int LimiterAttack
    {
        get => _limiterAttack;
        set { if (SetProperty(ref _limiterAttack, value) && !SuppressingAmpApply) RaiseParameterChanged(_limiterParams[1].Key, value); }
    }

    private int _limiterThreshold;
    public int LimiterThreshold
    {
        get => _limiterThreshold;
        set { if (SetProperty(ref _limiterThreshold, value) && !SuppressingAmpApply) RaiseParameterChanged(_limiterParams[2].Key, value); }
    }

    private int _limiterRatio;
    public int LimiterRatio
    {
        get => _limiterRatio;
        set { if (SetProperty(ref _limiterRatio, value) && !SuppressingAmpApply) RaiseParameterChanged(_limiterParams[3].Key, value); }
    }

    private int _limiterRelease;
    public int LimiterRelease
    {
        get => _limiterRelease;
        set { if (SetProperty(ref _limiterRelease, value) && !SuppressingAmpApply) RaiseParameterChanged(_limiterParams[4].Key, value); }
    }

    private int _limiterLevel;
    public int LimiterLevel
    {
        get => _limiterLevel;
        set { if (SetProperty(ref _limiterLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_limiterParams[5].Key, value); }
    }

    // ── T.WAH params ──────────────────────────────────────────────────────────────

    private int _tWahMode;
    public int TWahMode
    {
        get => _tWahMode;
        set { if (SetProperty(ref _tWahMode, value) && !SuppressingAmpApply) RaiseParameterChanged(_tWahParams[0].Key, value); }
    }

    private int _tWahPolarity;
    public int TWahPolarity
    {
        get => _tWahPolarity;
        set { if (SetProperty(ref _tWahPolarity, value) && !SuppressingAmpApply) RaiseParameterChanged(_tWahParams[1].Key, value); }
    }

    private int _tWahSens;
    public int TWahSens
    {
        get => _tWahSens;
        set { if (SetProperty(ref _tWahSens, value) && !SuppressingAmpApply) RaiseParameterChanged(_tWahParams[2].Key, value); }
    }

    private int _tWahFreq;
    public int TWahFreq
    {
        get => _tWahFreq;
        set { if (SetProperty(ref _tWahFreq, value) && !SuppressingAmpApply) RaiseParameterChanged(_tWahParams[3].Key, value); }
    }

    private int _tWahPeak;
    public int TWahPeak
    {
        get => _tWahPeak;
        set { if (SetProperty(ref _tWahPeak, value) && !SuppressingAmpApply) RaiseParameterChanged(_tWahParams[4].Key, value); }
    }

    private int _tWahDirectMix;
    public int TWahDirectMix
    {
        get => _tWahDirectMix;
        set { if (SetProperty(ref _tWahDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_tWahParams[5].Key, value); }
    }

    private int _tWahEffectLevel;
    public int TWahEffectLevel
    {
        get => _tWahEffectLevel;
        set { if (SetProperty(ref _tWahEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_tWahParams[6].Key, value); }
    }

    // ── AUTO WAH params ───────────────────────────────────────────────────────────

    private int _autoWahMode;
    public int AutoWahMode
    {
        get => _autoWahMode;
        set { if (SetProperty(ref _autoWahMode, value) && !SuppressingAmpApply) RaiseParameterChanged(_autoWahParams[0].Key, value); }
    }

    private int _autoWahFreq;
    public int AutoWahFreq
    {
        get => _autoWahFreq;
        set { if (SetProperty(ref _autoWahFreq, value) && !SuppressingAmpApply) RaiseParameterChanged(_autoWahParams[1].Key, value); }
    }

    private int _autoWahPeak;
    public int AutoWahPeak
    {
        get => _autoWahPeak;
        set { if (SetProperty(ref _autoWahPeak, value) && !SuppressingAmpApply) RaiseParameterChanged(_autoWahParams[2].Key, value); }
    }

    private int _autoWahRate;
    public int AutoWahRate
    {
        get => _autoWahRate;
        set { if (SetProperty(ref _autoWahRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_autoWahParams[3].Key, value); }
    }

    private int _autoWahDepth;
    public int AutoWahDepth
    {
        get => _autoWahDepth;
        set { if (SetProperty(ref _autoWahDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_autoWahParams[4].Key, value); }
    }

    private int _autoWahDirectMix;
    public int AutoWahDirectMix
    {
        get => _autoWahDirectMix;
        set { if (SetProperty(ref _autoWahDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_autoWahParams[5].Key, value); }
    }

    private int _autoWahEffectLevel;
    public int AutoWahEffectLevel
    {
        get => _autoWahEffectLevel;
        set { if (SetProperty(ref _autoWahEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_autoWahParams[6].Key, value); }
    }

    // ── PEDAL WAH params ──────────────────────────────────────────────────────────

    private int _pedalWahType;
    public int PedalWahType
    {
        get => _pedalWahType;
        set { if (SetProperty(ref _pedalWahType, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalWahParams[0].Key, value); }
    }

    private int _pedalWahPedalPosition;
    public int PedalWahPedalPosition
    {
        get => _pedalWahPedalPosition;
        set { if (SetProperty(ref _pedalWahPedalPosition, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalWahParams[1].Key, value); }
    }

    private int _pedalWahPedalMin;
    public int PedalWahPedalMin
    {
        get => _pedalWahPedalMin;
        set { if (SetProperty(ref _pedalWahPedalMin, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalWahParams[2].Key, value); }
    }

    private int _pedalWahPedalMax;
    public int PedalWahPedalMax
    {
        get => _pedalWahPedalMax;
        set { if (SetProperty(ref _pedalWahPedalMax, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalWahParams[3].Key, value); }
    }

    private int _pedalWahEffectLevel;
    public int PedalWahEffectLevel
    {
        get => _pedalWahEffectLevel;
        set { if (SetProperty(ref _pedalWahEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalWahParams[4].Key, value); }
    }

    private int _pedalWahDirectMix;
    public int PedalWahDirectMix
    {
        get => _pedalWahDirectMix;
        set { if (SetProperty(ref _pedalWahDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalWahParams[5].Key, value); }
    }

    // ── GRAPHIC EQ params ─────────────────────────────────────────────────────────

    private int _graphicEq31Hz;
    public int GraphicEq31Hz { get => _graphicEq31Hz; set { if (SetProperty(ref _graphicEq31Hz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[0].Key, value); } }

    private int _graphicEq62Hz;
    public int GraphicEq62Hz { get => _graphicEq62Hz; set { if (SetProperty(ref _graphicEq62Hz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[1].Key, value); } }

    private int _graphicEq125Hz;
    public int GraphicEq125Hz { get => _graphicEq125Hz; set { if (SetProperty(ref _graphicEq125Hz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[2].Key, value); } }

    private int _graphicEq250Hz;
    public int GraphicEq250Hz { get => _graphicEq250Hz; set { if (SetProperty(ref _graphicEq250Hz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[3].Key, value); } }

    private int _graphicEq500Hz;
    public int GraphicEq500Hz { get => _graphicEq500Hz; set { if (SetProperty(ref _graphicEq500Hz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[4].Key, value); } }

    private int _graphicEq1kHz;
    public int GraphicEq1kHz { get => _graphicEq1kHz; set { if (SetProperty(ref _graphicEq1kHz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[5].Key, value); } }

    private int _graphicEq2kHz;
    public int GraphicEq2kHz { get => _graphicEq2kHz; set { if (SetProperty(ref _graphicEq2kHz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[6].Key, value); } }

    private int _graphicEq4kHz;
    public int GraphicEq4kHz { get => _graphicEq4kHz; set { if (SetProperty(ref _graphicEq4kHz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[7].Key, value); } }

    private int _graphicEq8kHz;
    public int GraphicEq8kHz { get => _graphicEq8kHz; set { if (SetProperty(ref _graphicEq8kHz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[8].Key, value); } }

    private int _graphicEq16kHz;
    public int GraphicEq16kHz { get => _graphicEq16kHz; set { if (SetProperty(ref _graphicEq16kHz, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[9].Key, value); } }

    private int _graphicEqLevel;
    public int GraphicEqLevel { get => _graphicEqLevel; set { if (SetProperty(ref _graphicEqLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_graphicEqParams[10].Key, value); } }

    // ── PARAMETRIC EQ params ──────────────────────────────────────────────────────

    private int _parametricEqLowCut;
    public int ParametricEqLowCut { get => _parametricEqLowCut; set { if (SetProperty(ref _parametricEqLowCut, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[0].Key, value); } }

    private int _parametricEqLowGain;
    public int ParametricEqLowGain { get => _parametricEqLowGain; set { if (SetProperty(ref _parametricEqLowGain, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[1].Key, value); } }

    private int _parametricEqLowMidFreq;
    public int ParametricEqLowMidFreq { get => _parametricEqLowMidFreq; set { if (SetProperty(ref _parametricEqLowMidFreq, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[2].Key, value); } }

    private int _parametricEqLowMidQ;
    public int ParametricEqLowMidQ { get => _parametricEqLowMidQ; set { if (SetProperty(ref _parametricEqLowMidQ, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[3].Key, value); } }

    private int _parametricEqLowMidGain;
    public int ParametricEqLowMidGain { get => _parametricEqLowMidGain; set { if (SetProperty(ref _parametricEqLowMidGain, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[4].Key, value); } }

    private int _parametricEqHighMidFreq;
    public int ParametricEqHighMidFreq { get => _parametricEqHighMidFreq; set { if (SetProperty(ref _parametricEqHighMidFreq, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[5].Key, value); } }

    private int _parametricEqHighMidQ;
    public int ParametricEqHighMidQ { get => _parametricEqHighMidQ; set { if (SetProperty(ref _parametricEqHighMidQ, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[6].Key, value); } }

    private int _parametricEqHighMidGain;
    public int ParametricEqHighMidGain { get => _parametricEqHighMidGain; set { if (SetProperty(ref _parametricEqHighMidGain, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[7].Key, value); } }

    private int _parametricEqHighGain;
    public int ParametricEqHighGain { get => _parametricEqHighGain; set { if (SetProperty(ref _parametricEqHighGain, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[8].Key, value); } }

    private int _parametricEqHighCut;
    public int ParametricEqHighCut { get => _parametricEqHighCut; set { if (SetProperty(ref _parametricEqHighCut, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[9].Key, value); } }

    private int _parametricEqLevel;
    public int ParametricEqLevel { get => _parametricEqLevel; set { if (SetProperty(ref _parametricEqLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_parametricEqParams[10].Key, value); } }

    // ── GUITAR SIM params ─────────────────────────────────────────────────────────

    private int _guitarSimType;
    public int GuitarSimType { get => _guitarSimType; set { if (SetProperty(ref _guitarSimType, value) && !SuppressingAmpApply) RaiseParameterChanged(_guitarSimParams[0].Key, value); } }

    private int _guitarSimLow;
    public int GuitarSimLow { get => _guitarSimLow; set { if (SetProperty(ref _guitarSimLow, value) && !SuppressingAmpApply) RaiseParameterChanged(_guitarSimParams[1].Key, value); } }

    private int _guitarSimHigh;
    public int GuitarSimHigh { get => _guitarSimHigh; set { if (SetProperty(ref _guitarSimHigh, value) && !SuppressingAmpApply) RaiseParameterChanged(_guitarSimParams[2].Key, value); } }

    private int _guitarSimLevel;
    public int GuitarSimLevel { get => _guitarSimLevel; set { if (SetProperty(ref _guitarSimLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_guitarSimParams[3].Key, value); } }

    private int _guitarSimBody;
    public int GuitarSimBody { get => _guitarSimBody; set { if (SetProperty(ref _guitarSimBody, value) && !SuppressingAmpApply) RaiseParameterChanged(_guitarSimParams[4].Key, value); } }

    // ── AC.GUITAR SIM params ──────────────────────────────────────────────────────

    private int _acGuitarSimHigh;
    public int AcGuitarSimHigh { get => _acGuitarSimHigh; set { if (SetProperty(ref _acGuitarSimHigh, value) && !SuppressingAmpApply) RaiseParameterChanged(_acGuitarSimParams[0].Key, value); } }

    private int _acGuitarSimBody;
    public int AcGuitarSimBody { get => _acGuitarSimBody; set { if (SetProperty(ref _acGuitarSimBody, value) && !SuppressingAmpApply) RaiseParameterChanged(_acGuitarSimParams[1].Key, value); } }

    private int _acGuitarSimLow;
    public int AcGuitarSimLow { get => _acGuitarSimLow; set { if (SetProperty(ref _acGuitarSimLow, value) && !SuppressingAmpApply) RaiseParameterChanged(_acGuitarSimParams[2].Key, value); } }

    private int _acGuitarSimLevel;
    public int AcGuitarSimLevel { get => _acGuitarSimLevel; set { if (SetProperty(ref _acGuitarSimLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_acGuitarSimParams[3].Key, value); } }

    // ── AC.PROCESSOR params ───────────────────────────────────────────────────────

    private int _acProcessorType;
    public int AcProcessorType { get => _acProcessorType; set { if (SetProperty(ref _acProcessorType, value) && !SuppressingAmpApply) RaiseParameterChanged(_acProcessorParams[0].Key, value); } }

    private int _acProcessorBass;
    public int AcProcessorBass { get => _acProcessorBass; set { if (SetProperty(ref _acProcessorBass, value) && !SuppressingAmpApply) RaiseParameterChanged(_acProcessorParams[1].Key, value); } }

    private int _acProcessorMid;
    public int AcProcessorMid { get => _acProcessorMid; set { if (SetProperty(ref _acProcessorMid, value) && !SuppressingAmpApply) RaiseParameterChanged(_acProcessorParams[2].Key, value); } }

    private int _acProcessorMidFreq;
    public int AcProcessorMidFreq { get => _acProcessorMidFreq; set { if (SetProperty(ref _acProcessorMidFreq, value) && !SuppressingAmpApply) RaiseParameterChanged(_acProcessorParams[3].Key, value); } }

    private int _acProcessorTreble;
    public int AcProcessorTreble { get => _acProcessorTreble; set { if (SetProperty(ref _acProcessorTreble, value) && !SuppressingAmpApply) RaiseParameterChanged(_acProcessorParams[4].Key, value); } }

    private int _acProcessorPresence;
    public int AcProcessorPresence { get => _acProcessorPresence; set { if (SetProperty(ref _acProcessorPresence, value) && !SuppressingAmpApply) RaiseParameterChanged(_acProcessorParams[5].Key, value); } }

    private int _acProcessorLevel;
    public int AcProcessorLevel { get => _acProcessorLevel; set { if (SetProperty(ref _acProcessorLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_acProcessorParams[6].Key, value); } }

    // ── WAVE SYNTH params ─────────────────────────────────────────────────────────

    private int _waveSynthWave;
    public int WaveSynthWave { get => _waveSynthWave; set { if (SetProperty(ref _waveSynthWave, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[0].Key, value); } }

    private int _waveSynthCutoff;
    public int WaveSynthCutoff { get => _waveSynthCutoff; set { if (SetProperty(ref _waveSynthCutoff, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[1].Key, value); } }

    private int _waveSynthResonance;
    public int WaveSynthResonance { get => _waveSynthResonance; set { if (SetProperty(ref _waveSynthResonance, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[2].Key, value); } }

    private int _waveSynthFilterSens;
    public int WaveSynthFilterSens { get => _waveSynthFilterSens; set { if (SetProperty(ref _waveSynthFilterSens, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[3].Key, value); } }

    private int _waveSynthFilterDecay;
    public int WaveSynthFilterDecay { get => _waveSynthFilterDecay; set { if (SetProperty(ref _waveSynthFilterDecay, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[4].Key, value); } }

    private int _waveSynthFilterDepth;
    public int WaveSynthFilterDepth { get => _waveSynthFilterDepth; set { if (SetProperty(ref _waveSynthFilterDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[5].Key, value); } }

    private int _waveSynthSynthLevel;
    public int WaveSynthSynthLevel { get => _waveSynthSynthLevel; set { if (SetProperty(ref _waveSynthSynthLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[6].Key, value); } }

    private int _waveSynthDirectMix;
    public int WaveSynthDirectMix { get => _waveSynthDirectMix; set { if (SetProperty(ref _waveSynthDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_waveSynthParams[7].Key, value); } }

    // ── OCTAVE params ─────────────────────────────────────────────────────────────

    private int _octaveRange;
    public int OctaveRange { get => _octaveRange; set { if (SetProperty(ref _octaveRange, value) && !SuppressingAmpApply) RaiseParameterChanged(_octaveParams[0].Key, value); } }

    private int _octaveEffectLevel;
    public int OctaveEffectLevel { get => _octaveEffectLevel; set { if (SetProperty(ref _octaveEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_octaveParams[1].Key, value); } }

    private int _octaveDirectMix;
    public int OctaveDirectMix { get => _octaveDirectMix; set { if (SetProperty(ref _octaveDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_octaveParams[2].Key, value); } }

    // ── HEAVY OCTAVE params ───────────────────────────────────────────────────────

    private int _heavyOctave1OctLevel;
    public int HeavyOctave1OctLevel { get => _heavyOctave1OctLevel; set { if (SetProperty(ref _heavyOctave1OctLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_heavyOctaveParams[0].Key, value); } }

    private int _heavyOctave2OctLevel;
    public int HeavyOctave2OctLevel { get => _heavyOctave2OctLevel; set { if (SetProperty(ref _heavyOctave2OctLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_heavyOctaveParams[1].Key, value); } }

    private int _heavyOctaveDirectMix;
    public int HeavyOctaveDirectMix { get => _heavyOctaveDirectMix; set { if (SetProperty(ref _heavyOctaveDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_heavyOctaveParams[2].Key, value); } }

    // ── PITCH SHIFTER params ──────────────────────────────────────────────────────

    private int _pitchShifterVoice;
    public int PitchShifterVoice { get => _pitchShifterVoice; set { if (SetProperty(ref _pitchShifterVoice, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[0].Key, value); } }

    private int _pitchShifterPS1Mode;
    public int PitchShifterPS1Mode { get => _pitchShifterPS1Mode; set { if (SetProperty(ref _pitchShifterPS1Mode, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[1].Key, value); } }

    private int _pitchShifterPS1Pitch;
    public int PitchShifterPS1Pitch { get => _pitchShifterPS1Pitch; set { if (SetProperty(ref _pitchShifterPS1Pitch, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[2].Key, value); } }

    private int _pitchShifterPS1Fine;
    public int PitchShifterPS1Fine { get => _pitchShifterPS1Fine; set { if (SetProperty(ref _pitchShifterPS1Fine, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[3].Key, value); } }

    private int _pitchShifterPS1Level;
    public int PitchShifterPS1Level { get => _pitchShifterPS1Level; set { if (SetProperty(ref _pitchShifterPS1Level, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[4].Key, value); } }

    private int _pitchShifterPS2Mode;
    public int PitchShifterPS2Mode { get => _pitchShifterPS2Mode; set { if (SetProperty(ref _pitchShifterPS2Mode, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[5].Key, value); } }

    private int _pitchShifterPS2Pitch;
    public int PitchShifterPS2Pitch { get => _pitchShifterPS2Pitch; set { if (SetProperty(ref _pitchShifterPS2Pitch, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[6].Key, value); } }

    private int _pitchShifterPS2Fine;
    public int PitchShifterPS2Fine { get => _pitchShifterPS2Fine; set { if (SetProperty(ref _pitchShifterPS2Fine, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[7].Key, value); } }

    private int _pitchShifterPS2Level;
    public int PitchShifterPS2Level { get => _pitchShifterPS2Level; set { if (SetProperty(ref _pitchShifterPS2Level, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[8].Key, value); } }

    private int _pitchShifterFeedback;
    public int PitchShifterFeedback { get => _pitchShifterFeedback; set { if (SetProperty(ref _pitchShifterFeedback, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[9].Key, value); } }

    private int _pitchShifterDirectMix;
    public int PitchShifterDirectMix { get => _pitchShifterDirectMix; set { if (SetProperty(ref _pitchShifterDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_pitchShifterParams[10].Key, value); } }

    // ── HARMONIST params ──────────────────────────────────────────────────────────

    private int _harmonistVoice;
    public int HarmonistVoice { get => _harmonistVoice; set { if (SetProperty(ref _harmonistVoice, value) && !SuppressingAmpApply) RaiseParameterChanged(_harmonistParams[0].Key, value); } }

    private int _harmonistHarmony1;
    public int HarmonistHarmony1 { get => _harmonistHarmony1; set { if (SetProperty(ref _harmonistHarmony1, value) && !SuppressingAmpApply) RaiseParameterChanged(_harmonistParams[1].Key, value); } }

    private int _harmonistLevel1;
    public int HarmonistLevel1 { get => _harmonistLevel1; set { if (SetProperty(ref _harmonistLevel1, value) && !SuppressingAmpApply) RaiseParameterChanged(_harmonistParams[2].Key, value); } }

    private int _harmonistHarmony2;
    public int HarmonistHarmony2 { get => _harmonistHarmony2; set { if (SetProperty(ref _harmonistHarmony2, value) && !SuppressingAmpApply) RaiseParameterChanged(_harmonistParams[3].Key, value); } }

    private int _harmonistLevel2;
    public int HarmonistLevel2 { get => _harmonistLevel2; set { if (SetProperty(ref _harmonistLevel2, value) && !SuppressingAmpApply) RaiseParameterChanged(_harmonistParams[4].Key, value); } }

    private int _harmonistFeedback;
    public int HarmonistFeedback { get => _harmonistFeedback; set { if (SetProperty(ref _harmonistFeedback, value) && !SuppressingAmpApply) RaiseParameterChanged(_harmonistParams[5].Key, value); } }

    private int _harmonistDirectMix;
    public int HarmonistDirectMix { get => _harmonistDirectMix; set { if (SetProperty(ref _harmonistDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_harmonistParams[6].Key, value); } }

    // ── HUMANIZER params ──────────────────────────────────────────────────────────

    private int _humanizerMode;
    public int HumanizerMode { get => _humanizerMode; set { if (SetProperty(ref _humanizerMode, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[0].Key, value); } }

    private int _humanizerVowel1;
    public int HumanizerVowel1 { get => _humanizerVowel1; set { if (SetProperty(ref _humanizerVowel1, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[1].Key, value); } }

    private int _humanizerVowel2;
    public int HumanizerVowel2 { get => _humanizerVowel2; set { if (SetProperty(ref _humanizerVowel2, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[2].Key, value); } }

    private int _humanizerSens;
    public int HumanizerSens { get => _humanizerSens; set { if (SetProperty(ref _humanizerSens, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[3].Key, value); } }

    private int _humanizerRate;
    public int HumanizerRate { get => _humanizerRate; set { if (SetProperty(ref _humanizerRate, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[4].Key, value); } }

    private int _humanizerDepth;
    public int HumanizerDepth { get => _humanizerDepth; set { if (SetProperty(ref _humanizerDepth, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[5].Key, value); } }

    private int _humanizerManual;
    public int HumanizerManual { get => _humanizerManual; set { if (SetProperty(ref _humanizerManual, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[6].Key, value); } }

    private int _humanizerLevel;
    public int HumanizerLevel { get => _humanizerLevel; set { if (SetProperty(ref _humanizerLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_humanizerParams[7].Key, value); } }

    // ── PHASER 90E params ─────────────────────────────────────────────────────────

    private int _phaser90EScript;
    public int Phaser90EScript { get => _phaser90EScript; set { if (SetProperty(ref _phaser90EScript, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaser90EParams[0].Key, value); } }

    private int _phaser90ESpeed;
    public int Phaser90ESpeed { get => _phaser90ESpeed; set { if (SetProperty(ref _phaser90ESpeed, value) && !SuppressingAmpApply) RaiseParameterChanged(_phaser90EParams[1].Key, value); } }

    // ── FLANGER 117E params ───────────────────────────────────────────────────────

    private int _flanger117EManual;
    public int Flanger117EManual { get => _flanger117EManual; set { if (SetProperty(ref _flanger117EManual, value) && !SuppressingAmpApply) RaiseParameterChanged(_flanger117EParams[0].Key, value); } }

    private int _flanger117EWidth;
    public int Flanger117EWidth { get => _flanger117EWidth; set { if (SetProperty(ref _flanger117EWidth, value) && !SuppressingAmpApply) RaiseParameterChanged(_flanger117EParams[1].Key, value); } }

    private int _flanger117ESpeed;
    public int Flanger117ESpeed { get => _flanger117ESpeed; set { if (SetProperty(ref _flanger117ESpeed, value) && !SuppressingAmpApply) RaiseParameterChanged(_flanger117EParams[2].Key, value); } }

    private int _flanger117ERegen;
    public int Flanger117ERegen { get => _flanger117ERegen; set { if (SetProperty(ref _flanger117ERegen, value) && !SuppressingAmpApply) RaiseParameterChanged(_flanger117EParams[3].Key, value); } }

    // ── WAH 95E params ────────────────────────────────────────────────────────────

    private int _wah95EPedalPosition;
    public int Wah95EPedalPosition { get => _wah95EPedalPosition; set { if (SetProperty(ref _wah95EPedalPosition, value) && !SuppressingAmpApply) RaiseParameterChanged(_wah95EParams[0].Key, value); } }

    private int _wah95EPedalMin;
    public int Wah95EPedalMin { get => _wah95EPedalMin; set { if (SetProperty(ref _wah95EPedalMin, value) && !SuppressingAmpApply) RaiseParameterChanged(_wah95EParams[1].Key, value); } }

    private int _wah95EPedalMax;
    public int Wah95EPedalMax { get => _wah95EPedalMax; set { if (SetProperty(ref _wah95EPedalMax, value) && !SuppressingAmpApply) RaiseParameterChanged(_wah95EParams[2].Key, value); } }

    private int _wah95EEffectLevel;
    public int Wah95EEffectLevel { get => _wah95EEffectLevel; set { if (SetProperty(ref _wah95EEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_wah95EParams[3].Key, value); } }

    private int _wah95EDirectMix;
    public int Wah95EDirectMix { get => _wah95EDirectMix; set { if (SetProperty(ref _wah95EDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_wah95EParams[4].Key, value); } }

    // ── DC-30 params ──────────────────────────────────────────────────────────────

    private int _dC30Selector;
    public int DC30Selector { get => _dC30Selector; set { if (SetProperty(ref _dC30Selector, value) && !SuppressingAmpApply) RaiseParameterChanged(_dc30Params[0].Key, value); } }

    private int _dC30InputVolume;
    public int DC30InputVolume { get => _dC30InputVolume; set { if (SetProperty(ref _dC30InputVolume, value) && !SuppressingAmpApply) RaiseParameterChanged(_dc30Params[1].Key, value); } }

    private int _dC30ChorusIntensity;
    public int DC30ChorusIntensity { get => _dC30ChorusIntensity; set { if (SetProperty(ref _dC30ChorusIntensity, value) && !SuppressingAmpApply) RaiseParameterChanged(_dc30Params[2].Key, value); } }

    private int _dC30EchoIntensity;
    public int DC30EchoIntensity { get => _dC30EchoIntensity; set { if (SetProperty(ref _dC30EchoIntensity, value) && !SuppressingAmpApply) RaiseParameterChanged(_dc30Params[3].Key, value); } }

    private int _dC30EchoVolume;
    public int DC30EchoVolume { get => _dC30EchoVolume; set { if (SetProperty(ref _dC30EchoVolume, value) && !SuppressingAmpApply) RaiseParameterChanged(_dc30Params[4].Key, value); } }

    private int _dC30Tone;
    public int DC30Tone { get => _dC30Tone; set { if (SetProperty(ref _dC30Tone, value) && !SuppressingAmpApply) RaiseParameterChanged(_dc30Params[5].Key, value); } }

    private int _dC30Output;
    public int DC30Output { get => _dC30Output; set { if (SetProperty(ref _dC30Output, value) && !SuppressingAmpApply) RaiseParameterChanged(_dc30Params[6].Key, value); } }

    // ── PEDAL BEND params ─────────────────────────────────────────────────────────

    private int _pedalBendPitch;
    public int PedalBendPitch { get => _pedalBendPitch; set { if (SetProperty(ref _pedalBendPitch, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalBendParams[0].Key, value); } }

    private int _pedalBendPedalPosition;
    public int PedalBendPedalPosition { get => _pedalBendPedalPosition; set { if (SetProperty(ref _pedalBendPedalPosition, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalBendParams[1].Key, value); } }

    private int _pedalBendEffectLevel;
    public int PedalBendEffectLevel { get => _pedalBendEffectLevel; set { if (SetProperty(ref _pedalBendEffectLevel, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalBendParams[2].Key, value); } }

    private int _pedalBendDirectMix;
    public int PedalBendDirectMix { get => _pedalBendDirectMix; set { if (SetProperty(ref _pedalBendDirectMix, value) && !SuppressingAmpApply) RaiseParameterChanged(_pedalBendParams[3].Key, value); } }

    // ── Sync ─────────────────────────────────────────────────────────────────────

    public override IReadOnlyList<KatanaParameterDefinition> GetSyncParameters()
    {
        var list = new List<KatanaParameterDefinition> { Definition.SwitchParameter };
        if (Definition.TypeParameter is not null) list.Add(Definition.TypeParameter);
        if (Definition.LevelParameter is not null) list.Add(Definition.LevelParameter);
        if (Definition.VariationParameter is not null) list.Add(Definition.VariationParameter);
        list.AddRange(_chorusParams);
        list.AddRange(_flangerParams);
        list.AddRange(_phaserParams);
        list.AddRange(_uniVParams);
        list.AddRange(_tremoloParams);
        list.AddRange(_vibratoParams);
        list.AddRange(_rotaryParams);
        list.AddRange(_ringModParams);
        list.AddRange(_slowGearParams);
        list.AddRange(_slicerParams);
        list.AddRange(_compParams);
        list.AddRange(_limiterParams);
        list.AddRange(_tWahParams);
        list.AddRange(_autoWahParams);
        list.AddRange(_pedalWahParams);
        list.AddRange(_graphicEqParams);
        list.AddRange(_parametricEqParams);
        list.AddRange(_guitarSimParams);
        list.AddRange(_acGuitarSimParams);
        list.AddRange(_acProcessorParams);
        list.AddRange(_waveSynthParams);
        list.AddRange(_octaveParams);
        list.AddRange(_heavyOctaveParams);
        list.AddRange(_pitchShifterParams);
        list.AddRange(_harmonistParams);
        list.AddRange(_humanizerParams);
        list.AddRange(_phaser90EParams);
        list.AddRange(_flanger117EParams);
        list.AddRange(_wah95EParams);
        list.AddRange(_dc30Params);
        list.AddRange(_pedalBendParams);
        return list;
    }

    protected override void ApplyAmpValuesCore(IReadOnlyDictionary<string, int> values)
    {
        if (values.TryGetValue(Definition.SwitchParameter.Key, out var sw)) IsEnabled = sw != 0;
        if (Definition.TypeParameter is not null && values.TryGetValue(Definition.TypeParameter.Key, out var typeVal))
            SelectedTypeOption = ToTypeOption((byte)typeVal);
        if (Definition.VariationParameter is not null && values.TryGetValue(Definition.VariationParameter.Key, out var variation))
            Variation = ToVariationString(variation);
        if (Definition.LevelParameter is not null && values.TryGetValue(Definition.LevelParameter.Key, out var level))
            Level = level;

        if (values.TryGetValue(_chorusParams[0].Key, out var v)) ChorusXoverFreq    = v;
        if (values.TryGetValue(_chorusParams[1].Key, out v))     ChorusLowRate      = v;
        if (values.TryGetValue(_chorusParams[2].Key, out v))     ChorusLowDepth     = v;
        if (values.TryGetValue(_chorusParams[3].Key, out v))     ChorusLowPreDelay  = v;
        if (values.TryGetValue(_chorusParams[4].Key, out v))     ChorusLowLevel     = v;
        if (values.TryGetValue(_chorusParams[5].Key, out v))     ChorusHighRate     = v;
        if (values.TryGetValue(_chorusParams[6].Key, out v))     ChorusHighDepth    = v;
        if (values.TryGetValue(_chorusParams[7].Key, out v))     ChorusHighPreDelay = v;
        if (values.TryGetValue(_chorusParams[8].Key, out v))     ChorusHighLevel    = v;
        if (values.TryGetValue(_chorusParams[9].Key, out v))     ChorusDirectMix    = v;

        if (values.TryGetValue(_flangerParams[0].Key, out v)) FlangerRate        = v;
        if (values.TryGetValue(_flangerParams[1].Key, out v)) FlangerDepth       = v;
        if (values.TryGetValue(_flangerParams[2].Key, out v)) FlangerManual      = v;
        if (values.TryGetValue(_flangerParams[3].Key, out v)) FlangerResonance   = v;
        if (values.TryGetValue(_flangerParams[4].Key, out v)) FlangerLowCut      = v;
        if (values.TryGetValue(_flangerParams[5].Key, out v)) FlangerEffectLevel = v;
        if (values.TryGetValue(_flangerParams[6].Key, out v)) FlangerDirectMix   = v;

        if (values.TryGetValue(_phaserParams[0].Key, out v)) PhaserType        = v;
        if (values.TryGetValue(_phaserParams[1].Key, out v)) PhaserRate        = v;
        if (values.TryGetValue(_phaserParams[2].Key, out v)) PhaserDepth       = v;
        if (values.TryGetValue(_phaserParams[3].Key, out v)) PhaserManual      = v;
        if (values.TryGetValue(_phaserParams[4].Key, out v)) PhaserResonance   = v;
        if (values.TryGetValue(_phaserParams[5].Key, out v)) PhaserStepRate    = v;
        if (values.TryGetValue(_phaserParams[6].Key, out v)) PhaserEffectLevel = v;
        if (values.TryGetValue(_phaserParams[7].Key, out v)) PhaserDirectMix   = v;

        if (values.TryGetValue(_uniVParams[0].Key, out v)) UniVRate  = v;
        if (values.TryGetValue(_uniVParams[1].Key, out v)) UniVDepth = v;
        if (values.TryGetValue(_uniVParams[2].Key, out v)) UniVLevel = v;

        if (values.TryGetValue(_tremoloParams[0].Key, out v)) TremoloWaveShape = v;
        if (values.TryGetValue(_tremoloParams[1].Key, out v)) TremoloRate      = v;
        if (values.TryGetValue(_tremoloParams[2].Key, out v)) TremoloDepth     = v;
        if (values.TryGetValue(_tremoloParams[3].Key, out v)) TremoloLevel     = v;

        if (values.TryGetValue(_vibratoParams[0].Key, out v)) VibratoRate  = v;
        if (values.TryGetValue(_vibratoParams[1].Key, out v)) VibratoDepth = v;
        if (values.TryGetValue(_vibratoParams[2].Key, out v)) VibratoLevel = v;

        if (values.TryGetValue(_rotaryParams[0].Key, out v)) RotaryRateFast = v;
        if (values.TryGetValue(_rotaryParams[1].Key, out v)) RotaryDepth    = v;
        if (values.TryGetValue(_rotaryParams[2].Key, out v)) RotaryLevel    = v;

        if (values.TryGetValue(_ringModParams[0].Key, out v)) RingModMode        = v;
        if (values.TryGetValue(_ringModParams[1].Key, out v)) RingModFrequency   = v;
        if (values.TryGetValue(_ringModParams[2].Key, out v)) RingModEffectLevel = v;
        if (values.TryGetValue(_ringModParams[3].Key, out v)) RingModDirectMix   = v;

        if (values.TryGetValue(_slowGearParams[0].Key, out v)) SlowGearSens     = v;
        if (values.TryGetValue(_slowGearParams[1].Key, out v)) SlowGearRiseTime = v;
        if (values.TryGetValue(_slowGearParams[2].Key, out v)) SlowGearLevel    = v;

        if (values.TryGetValue(_slicerParams[0].Key, out v)) SlicerPattern     = v;
        if (values.TryGetValue(_slicerParams[1].Key, out v)) SlicerRate        = v;
        if (values.TryGetValue(_slicerParams[2].Key, out v)) SlicerTriggerSens = v;
        if (values.TryGetValue(_slicerParams[3].Key, out v)) SlicerEffectLevel = v;
        if (values.TryGetValue(_slicerParams[4].Key, out v)) SlicerDirectMix   = v;

        if (values.TryGetValue(_compParams[0].Key, out v)) CompType    = v;
        if (values.TryGetValue(_compParams[1].Key, out v)) CompSustain = v;
        if (values.TryGetValue(_compParams[2].Key, out v)) CompAttack  = v;
        if (values.TryGetValue(_compParams[3].Key, out v)) CompTone    = v;
        if (values.TryGetValue(_compParams[4].Key, out v)) CompLevel   = v;

        if (values.TryGetValue(_limiterParams[0].Key, out v)) LimiterType      = v;
        if (values.TryGetValue(_limiterParams[1].Key, out v)) LimiterAttack    = v;
        if (values.TryGetValue(_limiterParams[2].Key, out v)) LimiterThreshold = v;
        if (values.TryGetValue(_limiterParams[3].Key, out v)) LimiterRatio     = v;
        if (values.TryGetValue(_limiterParams[4].Key, out v)) LimiterRelease   = v;
        if (values.TryGetValue(_limiterParams[5].Key, out v)) LimiterLevel     = v;

        if (values.TryGetValue(_tWahParams[0].Key, out v)) TWahMode        = v;
        if (values.TryGetValue(_tWahParams[1].Key, out v)) TWahPolarity    = v;
        if (values.TryGetValue(_tWahParams[2].Key, out v)) TWahSens        = v;
        if (values.TryGetValue(_tWahParams[3].Key, out v)) TWahFreq        = v;
        if (values.TryGetValue(_tWahParams[4].Key, out v)) TWahPeak        = v;
        if (values.TryGetValue(_tWahParams[5].Key, out v)) TWahDirectMix   = v;
        if (values.TryGetValue(_tWahParams[6].Key, out v)) TWahEffectLevel = v;

        if (values.TryGetValue(_autoWahParams[0].Key, out v)) AutoWahMode        = v;
        if (values.TryGetValue(_autoWahParams[1].Key, out v)) AutoWahFreq        = v;
        if (values.TryGetValue(_autoWahParams[2].Key, out v)) AutoWahPeak        = v;
        if (values.TryGetValue(_autoWahParams[3].Key, out v)) AutoWahRate        = v;
        if (values.TryGetValue(_autoWahParams[4].Key, out v)) AutoWahDepth       = v;
        if (values.TryGetValue(_autoWahParams[5].Key, out v)) AutoWahDirectMix   = v;
        if (values.TryGetValue(_autoWahParams[6].Key, out v)) AutoWahEffectLevel = v;

        if (values.TryGetValue(_pedalWahParams[0].Key, out v)) PedalWahType        = v;
        if (values.TryGetValue(_pedalWahParams[1].Key, out v)) PedalWahPedalPosition = v;
        if (values.TryGetValue(_pedalWahParams[2].Key, out v)) PedalWahPedalMin    = v;
        if (values.TryGetValue(_pedalWahParams[3].Key, out v)) PedalWahPedalMax    = v;
        if (values.TryGetValue(_pedalWahParams[4].Key, out v)) PedalWahEffectLevel = v;
        if (values.TryGetValue(_pedalWahParams[5].Key, out v)) PedalWahDirectMix   = v;

        if (values.TryGetValue(_graphicEqParams[0].Key,  out v)) GraphicEq31Hz  = v;
        if (values.TryGetValue(_graphicEqParams[1].Key,  out v)) GraphicEq62Hz  = v;
        if (values.TryGetValue(_graphicEqParams[2].Key,  out v)) GraphicEq125Hz = v;
        if (values.TryGetValue(_graphicEqParams[3].Key,  out v)) GraphicEq250Hz = v;
        if (values.TryGetValue(_graphicEqParams[4].Key,  out v)) GraphicEq500Hz = v;
        if (values.TryGetValue(_graphicEqParams[5].Key,  out v)) GraphicEq1kHz  = v;
        if (values.TryGetValue(_graphicEqParams[6].Key,  out v)) GraphicEq2kHz  = v;
        if (values.TryGetValue(_graphicEqParams[7].Key,  out v)) GraphicEq4kHz  = v;
        if (values.TryGetValue(_graphicEqParams[8].Key,  out v)) GraphicEq8kHz  = v;
        if (values.TryGetValue(_graphicEqParams[9].Key,  out v)) GraphicEq16kHz = v;
        if (values.TryGetValue(_graphicEqParams[10].Key, out v)) GraphicEqLevel = v;

        if (values.TryGetValue(_parametricEqParams[0].Key,  out v)) ParametricEqLowCut      = v;
        if (values.TryGetValue(_parametricEqParams[1].Key,  out v)) ParametricEqLowGain     = v;
        if (values.TryGetValue(_parametricEqParams[2].Key,  out v)) ParametricEqLowMidFreq  = v;
        if (values.TryGetValue(_parametricEqParams[3].Key,  out v)) ParametricEqLowMidQ     = v;
        if (values.TryGetValue(_parametricEqParams[4].Key,  out v)) ParametricEqLowMidGain  = v;
        if (values.TryGetValue(_parametricEqParams[5].Key,  out v)) ParametricEqHighMidFreq = v;
        if (values.TryGetValue(_parametricEqParams[6].Key,  out v)) ParametricEqHighMidQ    = v;
        if (values.TryGetValue(_parametricEqParams[7].Key,  out v)) ParametricEqHighMidGain = v;
        if (values.TryGetValue(_parametricEqParams[8].Key,  out v)) ParametricEqHighGain    = v;
        if (values.TryGetValue(_parametricEqParams[9].Key,  out v)) ParametricEqHighCut     = v;
        if (values.TryGetValue(_parametricEqParams[10].Key, out v)) ParametricEqLevel       = v;

        if (values.TryGetValue(_guitarSimParams[0].Key, out v)) GuitarSimType  = v;
        if (values.TryGetValue(_guitarSimParams[1].Key, out v)) GuitarSimLow   = v;
        if (values.TryGetValue(_guitarSimParams[2].Key, out v)) GuitarSimHigh  = v;
        if (values.TryGetValue(_guitarSimParams[3].Key, out v)) GuitarSimLevel = v;
        if (values.TryGetValue(_guitarSimParams[4].Key, out v)) GuitarSimBody  = v;

        if (values.TryGetValue(_acGuitarSimParams[0].Key, out v)) AcGuitarSimHigh  = v;
        if (values.TryGetValue(_acGuitarSimParams[1].Key, out v)) AcGuitarSimBody  = v;
        if (values.TryGetValue(_acGuitarSimParams[2].Key, out v)) AcGuitarSimLow   = v;
        if (values.TryGetValue(_acGuitarSimParams[3].Key, out v)) AcGuitarSimLevel = v;

        if (values.TryGetValue(_acProcessorParams[0].Key, out v)) AcProcessorType     = v;
        if (values.TryGetValue(_acProcessorParams[1].Key, out v)) AcProcessorBass     = v;
        if (values.TryGetValue(_acProcessorParams[2].Key, out v)) AcProcessorMid      = v;
        if (values.TryGetValue(_acProcessorParams[3].Key, out v)) AcProcessorMidFreq  = v;
        if (values.TryGetValue(_acProcessorParams[4].Key, out v)) AcProcessorTreble   = v;
        if (values.TryGetValue(_acProcessorParams[5].Key, out v)) AcProcessorPresence = v;
        if (values.TryGetValue(_acProcessorParams[6].Key, out v)) AcProcessorLevel    = v;

        if (values.TryGetValue(_waveSynthParams[0].Key, out v)) WaveSynthWave        = v;
        if (values.TryGetValue(_waveSynthParams[1].Key, out v)) WaveSynthCutoff      = v;
        if (values.TryGetValue(_waveSynthParams[2].Key, out v)) WaveSynthResonance   = v;
        if (values.TryGetValue(_waveSynthParams[3].Key, out v)) WaveSynthFilterSens  = v;
        if (values.TryGetValue(_waveSynthParams[4].Key, out v)) WaveSynthFilterDecay = v;
        if (values.TryGetValue(_waveSynthParams[5].Key, out v)) WaveSynthFilterDepth = v;
        if (values.TryGetValue(_waveSynthParams[6].Key, out v)) WaveSynthSynthLevel  = v;
        if (values.TryGetValue(_waveSynthParams[7].Key, out v)) WaveSynthDirectMix   = v;

        if (values.TryGetValue(_octaveParams[0].Key, out v)) OctaveRange       = v;
        if (values.TryGetValue(_octaveParams[1].Key, out v)) OctaveEffectLevel = v;
        if (values.TryGetValue(_octaveParams[2].Key, out v)) OctaveDirectMix   = v;

        if (values.TryGetValue(_heavyOctaveParams[0].Key, out v)) HeavyOctave1OctLevel = v;
        if (values.TryGetValue(_heavyOctaveParams[1].Key, out v)) HeavyOctave2OctLevel = v;
        if (values.TryGetValue(_heavyOctaveParams[2].Key, out v)) HeavyOctaveDirectMix = v;

        if (values.TryGetValue(_pitchShifterParams[0].Key,  out v)) PitchShifterVoice     = v;
        if (values.TryGetValue(_pitchShifterParams[1].Key,  out v)) PitchShifterPS1Mode   = v;
        if (values.TryGetValue(_pitchShifterParams[2].Key,  out v)) PitchShifterPS1Pitch  = v;
        if (values.TryGetValue(_pitchShifterParams[3].Key,  out v)) PitchShifterPS1Fine   = v;
        if (values.TryGetValue(_pitchShifterParams[4].Key,  out v)) PitchShifterPS1Level  = v;
        if (values.TryGetValue(_pitchShifterParams[5].Key,  out v)) PitchShifterPS2Mode   = v;
        if (values.TryGetValue(_pitchShifterParams[6].Key,  out v)) PitchShifterPS2Pitch  = v;
        if (values.TryGetValue(_pitchShifterParams[7].Key,  out v)) PitchShifterPS2Fine   = v;
        if (values.TryGetValue(_pitchShifterParams[8].Key,  out v)) PitchShifterPS2Level  = v;
        if (values.TryGetValue(_pitchShifterParams[9].Key,  out v)) PitchShifterFeedback  = v;
        if (values.TryGetValue(_pitchShifterParams[10].Key, out v)) PitchShifterDirectMix = v;

        if (values.TryGetValue(_harmonistParams[0].Key, out v)) HarmonistVoice     = v;
        if (values.TryGetValue(_harmonistParams[1].Key, out v)) HarmonistHarmony1  = v;
        if (values.TryGetValue(_harmonistParams[2].Key, out v)) HarmonistLevel1    = v;
        if (values.TryGetValue(_harmonistParams[3].Key, out v)) HarmonistHarmony2  = v;
        if (values.TryGetValue(_harmonistParams[4].Key, out v)) HarmonistLevel2    = v;
        if (values.TryGetValue(_harmonistParams[5].Key, out v)) HarmonistFeedback  = v;
        if (values.TryGetValue(_harmonistParams[6].Key, out v)) HarmonistDirectMix = v;

        if (values.TryGetValue(_humanizerParams[0].Key, out v)) HumanizerMode   = v;
        if (values.TryGetValue(_humanizerParams[1].Key, out v)) HumanizerVowel1 = v;
        if (values.TryGetValue(_humanizerParams[2].Key, out v)) HumanizerVowel2 = v;
        if (values.TryGetValue(_humanizerParams[3].Key, out v)) HumanizerSens   = v;
        if (values.TryGetValue(_humanizerParams[4].Key, out v)) HumanizerRate   = v;
        if (values.TryGetValue(_humanizerParams[5].Key, out v)) HumanizerDepth  = v;
        if (values.TryGetValue(_humanizerParams[6].Key, out v)) HumanizerManual = v;
        if (values.TryGetValue(_humanizerParams[7].Key, out v)) HumanizerLevel  = v;

        if (values.TryGetValue(_phaser90EParams[0].Key, out v)) Phaser90EScript = v;
        if (values.TryGetValue(_phaser90EParams[1].Key, out v)) Phaser90ESpeed  = v;

        if (values.TryGetValue(_flanger117EParams[0].Key, out v)) Flanger117EManual = v;
        if (values.TryGetValue(_flanger117EParams[1].Key, out v)) Flanger117EWidth  = v;
        if (values.TryGetValue(_flanger117EParams[2].Key, out v)) Flanger117ESpeed  = v;
        if (values.TryGetValue(_flanger117EParams[3].Key, out v)) Flanger117ERegen  = v;

        if (values.TryGetValue(_wah95EParams[0].Key, out v)) Wah95EPedalPosition = v;
        if (values.TryGetValue(_wah95EParams[1].Key, out v)) Wah95EPedalMin      = v;
        if (values.TryGetValue(_wah95EParams[2].Key, out v)) Wah95EPedalMax      = v;
        if (values.TryGetValue(_wah95EParams[3].Key, out v)) Wah95EEffectLevel   = v;
        if (values.TryGetValue(_wah95EParams[4].Key, out v)) Wah95EDirectMix     = v;

        if (values.TryGetValue(_dc30Params[0].Key, out v)) DC30Selector       = v;
        if (values.TryGetValue(_dc30Params[1].Key, out v)) DC30InputVolume    = v;
        if (values.TryGetValue(_dc30Params[2].Key, out v)) DC30ChorusIntensity = v;
        if (values.TryGetValue(_dc30Params[3].Key, out v)) DC30EchoIntensity  = v;
        if (values.TryGetValue(_dc30Params[4].Key, out v)) DC30EchoVolume     = v;
        if (values.TryGetValue(_dc30Params[5].Key, out v)) DC30Tone           = v;
        if (values.TryGetValue(_dc30Params[6].Key, out v)) DC30Output         = v;

        if (values.TryGetValue(_pedalBendParams[0].Key, out v)) PedalBendPitch        = v;
        if (values.TryGetValue(_pedalBendParams[1].Key, out v)) PedalBendPedalPosition = v;
        if (values.TryGetValue(_pedalBendParams[2].Key, out v)) PedalBendEffectLevel  = v;
        if (values.TryGetValue(_pedalBendParams[3].Key, out v)) PedalBendDirectMix    = v;
    }
}
