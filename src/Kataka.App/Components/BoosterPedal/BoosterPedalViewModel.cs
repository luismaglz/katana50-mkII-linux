using Avalonia.Media;

using Kataka.App.KatanaState;
using Kataka.Domain.Midi;
using Kataka.Domain.Models;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class BoosterPedalViewModel : PedalViewModel
{
    private static readonly KatanaPanelEffectDefinition OwnDefinition =
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == PedalPosition.Booster);

    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.BoosterTypes;

    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    private readonly BoostPedalState _state;

    public BoosterPedalViewModel(IKatanaState katanaState) : base(OwnDefinition)
    {
        _state = katanaState.BoostPedal;
        TypeOptions = TypeTable.Values.ToList().AsReadOnly();

        _state.EnabledState.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEnabled));
        _state.Type.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(SelectedTypeOption));
            this.RaisePropertyChanged(nameof(TypeCaption));
        };
        _state.Variation.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(Variation));
            this.RaisePropertyChanged(nameof(VariationBrush));
        };
        _state.Drive.ValueChanged += () => this.RaisePropertyChanged(nameof(Drive));
        _state.Tone.ValueChanged += () => this.RaisePropertyChanged(nameof(Tone));
        _state.Bottom.ValueChanged += () => this.RaisePropertyChanged(nameof(Bottom));
        _state.SoloSw.ValueChanged += () => this.RaisePropertyChanged(nameof(SoloSw));
        _state.SoloLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(SoloLevel));
        _state.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(EffectLevel));
        _state.BoosterDirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(DirectMix));
    }

    /// <summary> View-only properties ────────────────────────────────────────────────────── </summary>
    public IReadOnlyList<string> TypeOptions { get; }

    public bool HasTypeOptions => TypeOptions.Count > 0;
    public IBrush VariationBrush => GetVariationBrush(Variation);


    public override bool IsEnabled
    {
        get => _state.EnabledState.Value != 0;
        set => _state.EnabledState.Value = value ? 1 : 0;
    }

    public override string? SelectedTypeOption
    {
        get => TypeTable.TryGetValue((byte)_state.Type.Value, out var name) ? name : null;
        set
        {
            if (value is not null && ReverseTypeTable.TryGetValue(value, out var byteVal))
                _state.Type.Value = byteVal;
        }
    }

    public override string Variation
    {
        get => ToVariationString(_state.Variation.Value);
        set
        {
            // Reverse the string→int mapping
            var raw = value switch { "Green" => 0, "Red" => 1, "Yellow" => 2, _ => -1 };
            if (raw >= 0) _state.Variation.Value = raw;
        }
    }

    public override string TypeCaption => SelectedTypeOption ?? "—";

    /// <summary> Booster-specific controls ───────────────────────────────────────────────── </summary>
    public int Drive
    {
        get => _state.Drive.Value;
        set => _state.Drive.Value = value;
    }

    public int Tone
    {
        get => _state.Tone.Value;
        set => _state.Tone.Value = value;
    }

    public int Bottom
    {
        get => _state.Bottom.Value;
        set => _state.Bottom.Value = value;
    }

    public bool SoloSw
    {
        get => _state.SoloSw.Value != 0;
        set => _state.SoloSw.Value = value ? 1 : 0;
    }

    public int SoloLevel
    {
        get => _state.SoloLevel.Value;
        set => _state.SoloLevel.Value = value;
    }

    public int EffectLevel
    {
        get => _state.EffectLevel.Value;
        set => _state.EffectLevel.Value = value;
    }

    public int DirectMix
    {
        get => _state.BoosterDirectMix.Value;
        set => _state.BoosterDirectMix.Value = value;
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
