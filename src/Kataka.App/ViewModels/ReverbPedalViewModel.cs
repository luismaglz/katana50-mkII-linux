using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class ReverbPedalViewModel : PedalViewModel
{
    private static readonly KatanaPanelEffectDefinition OwnDefinition =
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "reverb");

    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.ReverbTypes;
    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    public ReverbPedalViewModel() : base(OwnDefinition)
    {
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();
    }

    // ── View-only properties ──────────────────────────────────────────────────────

    public IReadOnlyList<string> TypeOptions { get; }
    public bool HasTypeOptions => TypeOptions.Count > 0;
    public IBrush VariationBrush => GetVariationBrush(Variation);

    // ── IBasePedal abstract overrides ────────────────────────────────────────────

    private string? _selectedTypeOption;
    public override string? SelectedTypeOption
    {
        get => _selectedTypeOption;
        set
        {
            if (!SetProperty(ref _selectedTypeOption, value)) return;
            OnPropertyChanged(nameof(TypeCaption));
            if (SuppressingAmpApply || Definition.TypeParameter is null || !TryGetTypeValue(value, out var byteValue)) return;
            RaiseParameterChanged(Definition.TypeParameter.Key, byteValue);
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

    // ── Reverb-specific controls ───────────────────────────────────────────────────

    private int _time;
    public int Time
    {
        get => _time;
        set
        {
            if (!SetProperty(ref _time, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.ReverbTime.Key, value);
        }
    }

    private int _preDelay;
    public int PreDelay
    {
        get => _preDelay;
        set
        {
            if (!SetProperty(ref _preDelay, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.ReverbPreDelay.Key, value);
        }
    }

    private int _highCut;
    public int HighCut
    {
        get => _highCut;
        set
        {
            if (!SetProperty(ref _highCut, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.ReverbHighCut.Key, value);
        }
    }

    private bool _highDensity;
    public bool HighDensity
    {
        get => _highDensity;
        set
        {
            if (!SetProperty(ref _highDensity, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.ReverbHighDensity.Key, value ? 1 : 0);
        }
    }

    private int _effectLevel;
    public int EffectLevel
    {
        get => _effectLevel;
        set
        {
            if (!SetProperty(ref _effectLevel, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.ReverbEffectLevel.Key, value);
        }
    }

    private int _directMix;
    public int DirectMix
    {
        get => _directMix;
        set
        {
            if (!SetProperty(ref _directMix, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.ReverbDirectMix.Key, value);
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
        var list = new List<KatanaParameterDefinition> { Definition.SwitchParameter };
        if (Definition.TypeParameter is not null) list.Add(Definition.TypeParameter);
        if (Definition.LevelParameter is not null) list.Add(Definition.LevelParameter);
        if (Definition.VariationParameter is not null) list.Add(Definition.VariationParameter);
        list.Add(KatanaMkIIParameterCatalog.ReverbTime);
        list.Add(KatanaMkIIParameterCatalog.ReverbPreDelay);
        list.Add(KatanaMkIIParameterCatalog.ReverbHighCut);
        list.Add(KatanaMkIIParameterCatalog.ReverbHighDensity);
        list.Add(KatanaMkIIParameterCatalog.ReverbEffectLevel);
        list.Add(KatanaMkIIParameterCatalog.ReverbDirectMix);
        return list;
    }

    protected override void ApplyAmpValuesCore(IReadOnlyDictionary<string, int> values)
    {
        if (values.TryGetValue(Definition.SwitchParameter.Key, out var sw))
            IsEnabled = sw != 0;
        if (Definition.TypeParameter is not null && values.TryGetValue(Definition.TypeParameter.Key, out var typeVal))
            SelectedTypeOption = ToTypeOption((byte)typeVal);
        if (Definition.VariationParameter is not null && values.TryGetValue(Definition.VariationParameter.Key, out var variation))
            Variation = ToVariationString(variation);
        if (Definition.LevelParameter is not null && values.TryGetValue(Definition.LevelParameter.Key, out var level))
            Level = level;
        if (values.TryGetValue(KatanaMkIIParameterCatalog.ReverbTime.Key, out var time)) Time = Math.Clamp(time, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.ReverbPreDelay.Key, out var preDelay)) PreDelay = Math.Clamp(preDelay, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.ReverbHighCut.Key, out var highCut)) HighCut = Math.Clamp(highCut, 0, 17);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.ReverbHighDensity.Key, out var highDensity)) HighDensity = highDensity != 0;
        if (values.TryGetValue(KatanaMkIIParameterCatalog.ReverbEffectLevel.Key, out var effectLevel)) EffectLevel = Math.Clamp(effectLevel, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.ReverbDirectMix.Key, out var directMix)) DirectMix = Math.Clamp(directMix, 0, 127);
    }
}
