using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

/// <summary>
/// ViewModel for the Delay 2 panel effect. Unlike the main Delay, Delay 2 has no variation
/// (FXBOX_SEL) and no front-panel level knob — only switch, type, and DSP detail params.
/// </summary>
public partial class Delay2PedalViewModel : PedalViewModel
{
    private static readonly KatanaPanelEffectDefinition OwnDefinition =
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "delay2");

    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.DelayTypes;
    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    public Delay2PedalViewModel() : base(OwnDefinition)
    {
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();
    }

    // ── View-only properties ──────────────────────────────────────────────────────

    public IReadOnlyList<string> TypeOptions { get; }
    public bool HasTypeOptions => TypeOptions.Count > 0;

    // Delay 2 has no variation — brush is always the off/inactive color.
    public IBrush VariationBrush => OffVariationBrush;

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

    // Delay 2 has no variation parameter. Variation is always "N/A".
    private string _variation = "N/A";
    public override string Variation
    {
        get => _variation;
        set => SetProperty(ref _variation, value);
    }

    // Delay 2 has no front-panel level knob (no LevelParameter in catalog).
    private int _level;
    public override int Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }

    public override bool HasLevel => Definition.LevelParameter is not null;
    public override string TypeCaption => SelectedTypeOption ?? "—";

    // ── Delay 2-specific controls ──────────────────────────────────────────────────

    private int _feedback;
    public int Feedback
    {
        get => _feedback;
        set
        {
            if (!SetProperty(ref _feedback, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.Delay2Feedback.Key, value);
        }
    }

    private int _highCut;
    public int HighCut
    {
        get => _highCut;
        set
        {
            if (!SetProperty(ref _highCut, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.Delay2HighCut.Key, value);
        }
    }

    private int _effectLevel;
    public int EffectLevel
    {
        get => _effectLevel;
        set
        {
            if (!SetProperty(ref _effectLevel, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.Delay2EffectLevel.Key, value);
        }
    }

    private int _directMix;
    public int DirectMix
    {
        get => _directMix;
        set
        {
            if (!SetProperty(ref _directMix, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.Delay2DirectMix.Key, value);
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
        list.Add(KatanaMkIIParameterCatalog.Delay2Feedback);
        list.Add(KatanaMkIIParameterCatalog.Delay2HighCut);
        list.Add(KatanaMkIIParameterCatalog.Delay2EffectLevel);
        list.Add(KatanaMkIIParameterCatalog.Delay2DirectMix);
        return list;
    }

    protected override void ApplyAmpValuesCore(IReadOnlyDictionary<string, int> values)
    {
        if (values.TryGetValue(Definition.SwitchParameter.Key, out var sw))
            IsEnabled = sw != 0;
        if (Definition.TypeParameter is not null && values.TryGetValue(Definition.TypeParameter.Key, out var typeVal))
            SelectedTypeOption = ToTypeOption((byte)typeVal);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.Delay2Feedback.Key, out var feedback)) Feedback = Math.Clamp(feedback, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.Delay2HighCut.Key, out var highCut)) HighCut = Math.Clamp(highCut, 0, 17);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.Delay2EffectLevel.Key, out var effectLevel)) EffectLevel = Math.Clamp(effectLevel, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.Delay2DirectMix.Key, out var directMix)) DirectMix = Math.Clamp(directMix, 0, 127);
    }
}
