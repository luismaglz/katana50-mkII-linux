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

    public ModFxPedalViewModel(string slot) : base(
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == slot))
    {
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();
        if (slot == "mod")
        {
            _chorusParams  = KatanaMkIIParameterCatalog.ModChorusParams;
            _flangerParams = KatanaMkIIParameterCatalog.ModFlangerParams;
            _phaserParams  = KatanaMkIIParameterCatalog.ModPhaserParams;
        }
        else
        {
            _chorusParams  = KatanaMkIIParameterCatalog.FxChorusParams;
            _flangerParams = KatanaMkIIParameterCatalog.FxFlangerParams;
            _phaserParams  = KatanaMkIIParameterCatalog.FxPhaserParams;
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
        }
    }

    public bool IsTypeChorus  => SelectedTypeIndex == 0;
    public bool IsTypeFlanger => SelectedTypeIndex == 1;
    public bool IsTypePhaser  => SelectedTypeIndex == 2;

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
    public override int Level
    {
        get => _level;
        set
        {
            if (!SetProperty(ref _level, value)) return;
            if (SuppressingAmpApply || Definition.LevelParameter is null) return;
            RaiseParameterChanged(Definition.LevelParameter.Key, value);
        }
    }

    public override bool HasLevel => Definition.LevelParameter is not null;
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

    // ── CHORUS params ─────────────────────────────────────────────────────────────

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
    }
}
