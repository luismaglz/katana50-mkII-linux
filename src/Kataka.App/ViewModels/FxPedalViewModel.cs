using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class FxPedalViewModel : PedalViewModel
{
    private static readonly KatanaPanelEffectDefinition OwnDefinition =
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "fx");

    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.ModFxTypes;
    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    public FxPedalViewModel() : base(OwnDefinition)
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
    }
}
