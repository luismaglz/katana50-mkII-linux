using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class BoosterPedalViewModel : PedalViewModel
{
    private static readonly KatanaPanelEffectDefinition OwnDefinition =
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == "booster");

    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.BoosterTypes;
    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    public BoosterPedalViewModel() : base(OwnDefinition)
    {
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();
    }

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
            SetProperty(ref _level, value);
        }
    }

    public override bool HasLevel => Definition.LevelParameter is not null;
    public override string TypeCaption => SelectedTypeOption ?? "—";

    // ── Booster-specific controls ─────────────────────────────────────────────────

    private int _drive;
    public int Drive
    {
        get => _drive;
        set
        {
            if (!SetProperty(ref _drive, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.BoosterDrive.Key, value);
        }
    }

    private int _tone;
    public int Tone
    {
        get => _tone;
        set
        {
            if (!SetProperty(ref _tone, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.BoosterTone.Key, value);
        }
    }

    private int _bottom;
    public int Bottom
    {
        get => _bottom;
        set
        {
            if (!SetProperty(ref _bottom, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.BoosterBottom.Key, value);
        }
    }

    private bool _soloSw;
    public bool SoloSw
    {
        get => _soloSw;
        set
        {
            if (!SetProperty(ref _soloSw, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.BoosterSoloSw.Key, value ? 1 : 0);
        }
    }

    private int _soloLevel;
    public int SoloLevel
    {
        get => _soloLevel;
        set
        {
            if (!SetProperty(ref _soloLevel, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.BoosterSoloLevel.Key, value);
        }
    }

    private int _effectLevel;
    public int EffectLevel
    {
        get => _effectLevel;
        set
        {
            if (!SetProperty(ref _effectLevel, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.BoosterEffectLevel.Key, value);
        }
    }

    private int _directMix;
    public int DirectMix
    {
        get => _directMix;
        set
        {
            if (!SetProperty(ref _directMix, value)) return;
            if (!SuppressingAmpApply) RaiseParameterChanged(KatanaMkIIParameterCatalog.BoosterDirectMix.Key, value);
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
        list.Add(KatanaMkIIParameterCatalog.BoosterDrive);
        list.Add(KatanaMkIIParameterCatalog.BoosterTone);
        list.Add(KatanaMkIIParameterCatalog.BoosterBottom);
        list.Add(KatanaMkIIParameterCatalog.BoosterSoloSw);
        list.Add(KatanaMkIIParameterCatalog.BoosterSoloLevel);
        list.Add(KatanaMkIIParameterCatalog.BoosterEffectLevel);
        list.Add(KatanaMkIIParameterCatalog.BoosterDirectMix);
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
        if (values.TryGetValue(KatanaMkIIParameterCatalog.BoosterDrive.Key, out var drive)) Drive = Math.Clamp(drive, 0, 120);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.BoosterTone.Key, out var tone)) Tone = Math.Clamp(tone, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.BoosterBottom.Key, out var bottom)) Bottom = Math.Clamp(bottom, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.BoosterSoloSw.Key, out var soloSw)) SoloSw = soloSw != 0;
        if (values.TryGetValue(KatanaMkIIParameterCatalog.BoosterSoloLevel.Key, out var soloLevel)) SoloLevel = Math.Clamp(soloLevel, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.BoosterEffectLevel.Key, out var effectLevel)) EffectLevel = Math.Clamp(effectLevel, 0, 127);
        if (values.TryGetValue(KatanaMkIIParameterCatalog.BoosterDirectMix.Key, out var directMix)) DirectMix = Math.Clamp(directMix, 0, 127);
    }
}
