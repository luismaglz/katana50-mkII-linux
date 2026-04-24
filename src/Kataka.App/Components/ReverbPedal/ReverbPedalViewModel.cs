using Avalonia.Media;

using Kataka.App.KatanaState;
using Kataka.Domain.Midi;
using Kataka.Domain.Models;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class ReverbPedalViewModel : PedalViewModel
{
    private static readonly KatanaPanelEffectDefinition OwnDefinition =
        KatanaMkIIParameterCatalog.PanelEffects.First(e => e.Key == PedalPosition.Reverb);

    private static readonly IReadOnlyDictionary<byte, string> TypeTable = KatanaTypeNameTables.ReverbTypes;

    private static readonly IReadOnlyDictionary<string, byte> ReverseTypeTable =
        TypeTable.ToDictionary(kvp => kvp.Value, kvp => kvp.Key, StringComparer.OrdinalIgnoreCase);

    private readonly ReverbPedalState _state;

    public ReverbPedalViewModel(IKatanaState katanaState) : base(OwnDefinition)
    {
        _state = katanaState.ReverbPedal;
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
        _state.Time.ValueChanged += () => this.RaisePropertyChanged(nameof(Time));
        _state.PreDelay.ValueChanged += () => this.RaisePropertyChanged(nameof(PreDelay));
        _state.LowCut.ValueChanged += () => this.RaisePropertyChanged(nameof(LowCut));
        _state.HighCut.ValueChanged += () => this.RaisePropertyChanged(nameof(HighCut));
        _state.Density.ValueChanged += () => this.RaisePropertyChanged(nameof(Density));
        _state.Color.ValueChanged += () => this.RaisePropertyChanged(nameof(Color));
        _state.EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(EffectLevel));
        _state.DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(DirectMix));
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
            var raw = value switch { "Green" => 0, "Red" => 1, "Yellow" => 2, _ => -1 };
            if (raw >= 0) _state.Variation.Value = raw;
        }
    }

    public override string TypeCaption => SelectedTypeOption ?? "—";

    /// <summary> Reverb-specific controls ────────────────────────────────────────────────── </summary>
    public int Time
    {
        get => _state.Time.Value;
        set => _state.Time.Value = value;
    }

    public int PreDelay
    {
        get => _state.PreDelay.Value;
        set => _state.PreDelay.Value = value;
    }

    public int LowCut
    {
        get => _state.LowCut.Value;
        set => _state.LowCut.Value = value;
    }

    public int HighCut
    {
        get => _state.HighCut.Value;
        set => _state.HighCut.Value = value;
    }

    public int Density
    {
        get => _state.Density.Value;
        set => _state.Density.Value = value;
    }

    public int Color
    {
        get => _state.Color.Value;
        set => _state.Color.Value = value;
    }

    public int EffectLevel
    {
        get => _state.EffectLevel.Value;
        set => _state.EffectLevel.Value = value;
    }

    public int DirectMix
    {
        get => _state.DirectMix.Value;
        set => _state.DirectMix.Value = value;
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
