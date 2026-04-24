using System.Collections.ObjectModel;

using Kataka.App.KatanaState;
using Kataka.Domain.Midi;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class PedalFxViewModel : ViewModelBase
{
    private const int BendPitchWireOffset = 24;
    private readonly HardwarePedalState _state;

    public PedalFxViewModel(IKatanaState katanaState)
    {
        _state = katanaState.HardwarePedal;

        TypeOptions.Add(ToPedalTypeOption(0));
        TypeOptions.Add(ToPedalTypeOption(1));
        TypeOptions.Add(ToPedalTypeOption(2));
        PositionOptions.Add("Input");
        PositionOptions.Add("Post Amp");

        foreach (var value in Enumerable.Range(
                     KatanaMkIIParameterCatalog.PedalFxWahType.Minimum,
                     KatanaMkIIParameterCatalog.PedalFxWahType.Maximum -
                     KatanaMkIIParameterCatalog.PedalFxWahType.Minimum + 1))
            WahTypeOptions.Add(ToWahTypeOption((byte)value));

        _state.EnabledState.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEnabled));
        _state.Type.ValueChanged += () =>
        {
            this.RaisePropertyChanged(nameof(SelectedTypeOption));
            this.RaisePropertyChanged(nameof(IsWahMode));
            this.RaisePropertyChanged(nameof(IsBendMode));
            this.RaisePropertyChanged(nameof(IsEvh95Mode));
        };
        _state.Position.ValueChanged += () => this.RaisePropertyChanged(nameof(SelectedPositionOption));
        _state.WahType.ValueChanged += () => this.RaisePropertyChanged(nameof(SelectedWahTypeOption));
        _state.WahPedalPosition.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalPosition));
        _state.WahMinimum.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalMinimum));
        _state.WahMaximum.ValueChanged += () => this.RaisePropertyChanged(nameof(PedalMaximum));
        _state.WahEffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(EffectLevel));
        _state.WahDirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(DirectMix));
        _state.BendPitch.ValueChanged += () => this.RaisePropertyChanged(nameof(BendPitch));
        _state.BendPedalPosition.ValueChanged += () => this.RaisePropertyChanged(nameof(BendPosition));
        _state.BendEffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(BendEffectLevel));
        _state.BendDirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(BendDirectMix));
        _state.Evh95Position.ValueChanged += () => this.RaisePropertyChanged(nameof(Evh95Position));
        _state.Evh95Minimum.ValueChanged += () => this.RaisePropertyChanged(nameof(Evh95Minimum));
        _state.Evh95Maximum.ValueChanged += () => this.RaisePropertyChanged(nameof(Evh95Maximum));
        _state.Evh95EffectLevel.ValueChanged += () => this.RaisePropertyChanged(nameof(Evh95EffectLevel));
        _state.Evh95DirectMix.ValueChanged += () => this.RaisePropertyChanged(nameof(Evh95DirectMix));
        _state.FootVolume.ValueChanged += () => this.RaisePropertyChanged(nameof(FootVolume));
    }

    public ObservableCollection<string> TypeOptions { get; } = [];
    public ObservableCollection<string> PositionOptions { get; } = [];
    public ObservableCollection<string> WahTypeOptions { get; } = [];

    public bool IsEnabled
    {
        get => _state.EnabledState.Value != 0;
        set => _state.EnabledState.Value = value ? 1 : 0;
    }

    public string SelectedTypeOption
    {
        get => ToPedalTypeOption((byte)_state.Type.Value);
        set
        {
            if (TryParsePedalTypeOption(value, out var v)) _state.Type.Value = v;
        }
    }

    public string SelectedPositionOption
    {
        get => ToPositionOption((byte)_state.Position.Value);
        set
        {
            if (TryParsePositionOption(value, out var v)) _state.Position.Value = v;
        }
    }

    public string SelectedWahTypeOption
    {
        get => ToWahTypeOption((byte)_state.WahType.Value);
        set
        {
            if (TryParseWahTypeOption(value, out var v)) _state.WahType.Value = v;
        }
    }

    public int PedalPosition
    {
        get => _state.WahPedalPosition.Value;
        set => _state.WahPedalPosition.Value = value;
    }

    public int PedalMinimum
    {
        get => _state.WahMinimum.Value;
        set => _state.WahMinimum.Value = value;
    }

    public int PedalMaximum
    {
        get => _state.WahMaximum.Value;
        set => _state.WahMaximum.Value = value;
    }

    public int EffectLevel
    {
        get => _state.WahEffectLevel.Value;
        set => _state.WahEffectLevel.Value = value;
    }

    public int DirectMix
    {
        get => _state.WahDirectMix.Value;
        set => _state.WahDirectMix.Value = value;
    }

    public int BendPitch
    {
        get => _state.BendPitch.Value - BendPitchWireOffset;
        set => _state.BendPitch.Value = value + BendPitchWireOffset;
    }

    public int BendPosition
    {
        get => _state.BendPedalPosition.Value;
        set => _state.BendPedalPosition.Value = value;
    }

    public int BendEffectLevel
    {
        get => _state.BendEffectLevel.Value;
        set => _state.BendEffectLevel.Value = value;
    }

    public int BendDirectMix
    {
        get => _state.BendDirectMix.Value;
        set => _state.BendDirectMix.Value = value;
    }

    public int Evh95Position
    {
        get => _state.Evh95Position.Value;
        set => _state.Evh95Position.Value = value;
    }

    public int Evh95Minimum
    {
        get => _state.Evh95Minimum.Value;
        set => _state.Evh95Minimum.Value = value;
    }

    public int Evh95Maximum
    {
        get => _state.Evh95Maximum.Value;
        set => _state.Evh95Maximum.Value = value;
    }

    public int Evh95EffectLevel
    {
        get => _state.Evh95EffectLevel.Value;
        set => _state.Evh95EffectLevel.Value = value;
    }

    public int Evh95DirectMix
    {
        get => _state.Evh95DirectMix.Value;
        set => _state.Evh95DirectMix.Value = value;
    }

    public int FootVolume
    {
        get => _state.FootVolume.Value;
        set => _state.FootVolume.Value = value;
    }

    public bool IsWahMode => _state.Type.Value == 0;
    public bool IsBendMode => _state.Type.Value == 1;
    public bool IsEvh95Mode => _state.Type.Value == 2;

    public static string ToPedalTypeOption(byte value) => value switch
    {
        0 => "0 - Wah",
        1 => "1 - Pedal Bend",
        2 => "2 - EVH WAH95",
        _ => $"{value} - Value {value}"
    };

    public static string ToPositionOption(byte value) => value switch
    {
        0 => "Input",
        1 => "Post Amp",
        _ => $"Value {value}"
    };

    public static string ToWahTypeOption(byte value) =>
        KatanaTypeNameTables.PedalWahTypes.TryGetValue(value, out var name) ? name : $"Type {value}";

    public static bool TryParsePedalTypeOption(string? option, out byte value) =>
        TryParseLeadingByte(option, out value);

    public static bool TryParseWahTypeOption(string? option, out byte value)
    {
        if (!string.IsNullOrWhiteSpace(option))
            foreach (var kvp in KatanaTypeNameTables.PedalWahTypes)
                if (kvp.Value.Equals(option, StringComparison.Ordinal))
                {
                    value = kvp.Key;
                    return true;
                }

        value = 0;
        const string Prefix = "Type ";
        if (string.IsNullOrWhiteSpace(option) || !option.StartsWith(Prefix, StringComparison.Ordinal))
            return false;

        return byte.TryParse(option[Prefix.Length..], out value);
    }

    public static bool TryParsePositionOption(string? option, out byte value)
    {
        value = option switch
        {
            "Input" => 0,
            "Post Amp" => 1,
            _ => 0
        };
        return option is "Input" or "Post Amp";
    }

    private static bool TryParseLeadingByte(string? option, out byte value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(option)) return false;
        var separatorIndex = option.IndexOf(' ');
        var candidate = separatorIndex < 0 ? option : option[..separatorIndex];
        return byte.TryParse(candidate, out value);
    }
}
