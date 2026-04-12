using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class DelayPedalViewModel : PedalViewModel
{
    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.DelayTypes;
    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    private readonly KatanaPanelEffectDefinition _def;
    private readonly bool _hasVariation;

    // Slot-specific param definitions resolved once in constructor.
    private readonly KatanaParameterDefinition _feedbackParam;
    private readonly KatanaParameterDefinition _highCutParam;
    private readonly KatanaParameterDefinition _effectLevelParam;
    private readonly KatanaParameterDefinition _directMixParam;
    private readonly KatanaParameterDefinition _tapTimeParam;
    private readonly KatanaParameterDefinition _modRateParam;
    private readonly KatanaParameterDefinition _modDepthParam;
    private readonly KatanaParameterDefinition _rangeParam;
    private readonly KatanaParameterDefinition _filterParam;
    private readonly KatanaParameterDefinition _feedbackPhaseParam;
    private readonly KatanaParameterDefinition _delayPhaseParam;
    private readonly KatanaParameterDefinition _modSwParam;

    public DelayPedalViewModel(string slot) : base(
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == slot))
    {
        _def = Definition;
        _hasVariation = _def.VariationParameter is not null;
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();

        bool isSlot1 = string.Equals(slot, "delay", StringComparison.Ordinal);
        _feedbackParam      = isSlot1 ? KatanaMkIIParameterCatalog.DelayFeedback      : KatanaMkIIParameterCatalog.Delay2Feedback;
        _highCutParam       = isSlot1 ? KatanaMkIIParameterCatalog.DelayHighCut        : KatanaMkIIParameterCatalog.Delay2HighCut;
        _effectLevelParam   = isSlot1 ? KatanaMkIIParameterCatalog.DelayEffectLevel    : KatanaMkIIParameterCatalog.Delay2EffectLevel;
        _directMixParam     = isSlot1 ? KatanaMkIIParameterCatalog.DelayDirectMix      : KatanaMkIIParameterCatalog.Delay2DirectMix;
        _tapTimeParam       = isSlot1 ? KatanaMkIIParameterCatalog.DelayTapTime        : KatanaMkIIParameterCatalog.Delay2TapTime;
        _modRateParam       = isSlot1 ? KatanaMkIIParameterCatalog.DelayModRate        : KatanaMkIIParameterCatalog.Delay2ModRate;
        _modDepthParam      = isSlot1 ? KatanaMkIIParameterCatalog.DelayModDepth       : KatanaMkIIParameterCatalog.Delay2ModDepth;
        _rangeParam         = isSlot1 ? KatanaMkIIParameterCatalog.DelayRange          : KatanaMkIIParameterCatalog.Delay2Range;
        _filterParam        = isSlot1 ? KatanaMkIIParameterCatalog.DelayFilter         : KatanaMkIIParameterCatalog.Delay2Filter;
        _feedbackPhaseParam = isSlot1 ? KatanaMkIIParameterCatalog.DelayFeedbackPhase  : KatanaMkIIParameterCatalog.Delay2FeedbackPhase;
        _delayPhaseParam    = isSlot1 ? KatanaMkIIParameterCatalog.DelayDelayPhase     : KatanaMkIIParameterCatalog.Delay2DelayPhase;
        _modSwParam         = isSlot1 ? KatanaMkIIParameterCatalog.DelayModSw          : KatanaMkIIParameterCatalog.Delay2ModSw;
    }

    // ── View-only properties ──────────────────────────────────────────────────────

    public IReadOnlyList<string> TypeOptions { get; }
    public bool HasTypeOptions => TypeOptions.Count > 0;
    public bool HasVariation => _hasVariation;
    public IBrush VariationBrush => _hasVariation ? GetVariationBrush(Variation) : OffVariationBrush;

    // ── IBasePedal abstract overrides ────────────────────────────────────────────

    private string? _selectedTypeOption;
    public override string? SelectedTypeOption
    {
        get => _selectedTypeOption;
        set
        {
            if (!SetProperty(ref _selectedTypeOption, value)) return;
            OnPropertyChanged(nameof(TypeCaption));
            if (SuppressingAmpApply || _def.TypeParameter is null || !TryGetTypeValue(value, out var byteValue)) return;
            RaiseParameterChanged(_def.TypeParameter.Key, byteValue);
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
    public override int Level
    {
        get => _level;
        set
        {
            if (!SetProperty(ref _level, value)) return;
            if (SuppressingAmpApply || _def.LevelParameter is null) return;
            RaiseParameterChanged(_def.LevelParameter.Key, value);
        }
    }

    public override bool HasLevel => _def.LevelParameter is not null;
    public override string TypeCaption => SelectedTypeOption ?? "—";

    // ── Delay-specific controls ────────────────────────────────────────────────────

    private int _feedback;
    public int Feedback
    {
        get => _feedback;
        set
        {
            if (!SetProperty(ref _feedback, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_feedbackParam.Key, value);
        }
    }

    private int _highCut;
    public int HighCut
    {
        get => _highCut;
        set
        {
            if (!SetProperty(ref _highCut, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_highCutParam.Key, value);
        }
    }

    private int _effectLevel;
    public int EffectLevel
    {
        get => _effectLevel;
        set
        {
            if (!SetProperty(ref _effectLevel, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_effectLevelParam.Key, value);
        }
    }

    private int _directMix;
    public int DirectMix
    {
        get => _directMix;
        set
        {
            if (!SetProperty(ref _directMix, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_directMixParam.Key, value);
        }
    }

    private int _tapTime;
    public int TapTime
    {
        get => _tapTime;
        set
        {
            if (!SetProperty(ref _tapTime, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_tapTimeParam.Key, value);
        }
    }

    private int _modRate;
    public int ModRate
    {
        get => _modRate;
        set
        {
            if (!SetProperty(ref _modRate, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_modRateParam.Key, value);
        }
    }

    private int _modDepth;
    public int ModDepth
    {
        get => _modDepth;
        set
        {
            if (!SetProperty(ref _modDepth, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_modDepthParam.Key, value);
        }
    }

    private bool _rangeHigh;
    public bool RangeHigh
    {
        get => _rangeHigh;
        set
        {
            if (!SetProperty(ref _rangeHigh, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_rangeParam.Key, value ? 1 : 0);
        }
    }

    private bool _filterOn;
    public bool FilterOn
    {
        get => _filterOn;
        set
        {
            if (!SetProperty(ref _filterOn, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_filterParam.Key, value ? 1 : 0);
        }
    }

    private bool _feedbackPhaseInverse;
    public bool FeedbackPhaseInverse
    {
        get => _feedbackPhaseInverse;
        set
        {
            if (!SetProperty(ref _feedbackPhaseInverse, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_feedbackPhaseParam.Key, value ? 1 : 0);
        }
    }

    private bool _delayPhaseInverse;
    public bool DelayPhaseInverse
    {
        get => _delayPhaseInverse;
        set
        {
            if (!SetProperty(ref _delayPhaseInverse, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_delayPhaseParam.Key, value ? 1 : 0);
        }
    }

    private bool _modSwOn;
    public bool ModSwOn
    {
        get => _modSwOn;
        set
        {
            if (!SetProperty(ref _modSwOn, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(_modSwParam.Key, value ? 1 : 0);
        }
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

    public override IReadOnlyList<KatanaParameterDefinition> GetSyncParameters()
    {
        var list = new List<KatanaParameterDefinition> { _def.SwitchParameter };
        if (_def.TypeParameter is not null) list.Add(_def.TypeParameter);
        if (_def.LevelParameter is not null) list.Add(_def.LevelParameter);
        if (_def.VariationParameter is not null) list.Add(_def.VariationParameter);
        list.Add(_feedbackParam);
        list.Add(_highCutParam);
        list.Add(_effectLevelParam);
        list.Add(_directMixParam);
        list.Add(_tapTimeParam);
        list.Add(_modRateParam);
        list.Add(_modDepthParam);
        list.Add(_rangeParam);
        list.Add(_filterParam);
        list.Add(_feedbackPhaseParam);
        list.Add(_delayPhaseParam);
        list.Add(_modSwParam);
        return list;
    }

    protected override void ApplyAmpValuesCore(IReadOnlyDictionary<string, int> values)
    {
        if (values.TryGetValue(_def.SwitchParameter.Key, out var sw))
            IsEnabled = sw != 0;
        if (_def.TypeParameter is not null && values.TryGetValue(_def.TypeParameter.Key, out var typeVal))
            SelectedTypeOption = ToTypeOption((byte)typeVal);
        if (_def.VariationParameter is not null && values.TryGetValue(_def.VariationParameter.Key, out var variation))
            Variation = ToVariationString(variation);
        if (_def.LevelParameter is not null && values.TryGetValue(_def.LevelParameter.Key, out var level))
            Level = level;
        if (values.TryGetValue(_feedbackParam.Key, out var feedback))           Feedback             = Math.Clamp(feedback, 0, 100);
        if (values.TryGetValue(_highCutParam.Key, out var highCut))             HighCut              = Math.Clamp(highCut, 0, 14);
        if (values.TryGetValue(_effectLevelParam.Key, out var effectLevel))     EffectLevel          = Math.Clamp(effectLevel, 0, 120);
        if (values.TryGetValue(_directMixParam.Key, out var directMix))         DirectMix            = Math.Clamp(directMix, 0, 100);
        if (values.TryGetValue(_tapTimeParam.Key, out var tapTime))             TapTime              = Math.Clamp(tapTime, 0, 100);
        if (values.TryGetValue(_modRateParam.Key, out var modRate))             ModRate              = Math.Clamp(modRate, 0, 100);
        if (values.TryGetValue(_modDepthParam.Key, out var modDepth))           ModDepth             = Math.Clamp(modDepth, 0, 100);
        if (values.TryGetValue(_rangeParam.Key, out var range))                 RangeHigh            = range != 0;
        if (values.TryGetValue(_filterParam.Key, out var filter))               FilterOn             = filter != 0;
        if (values.TryGetValue(_feedbackPhaseParam.Key, out var fbPhase))       FeedbackPhaseInverse = fbPhase != 0;
        if (values.TryGetValue(_delayPhaseParam.Key, out var dlyPhase))         DelayPhaseInverse    = dlyPhase != 0;
        if (values.TryGetValue(_modSwParam.Key, out var modSw))                 ModSwOn              = modSw != 0;
    }
}
