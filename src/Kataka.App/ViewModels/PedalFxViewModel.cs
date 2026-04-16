using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Kataka.Domain.Midi;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class PedalFxViewModel : ViewModelBase
{
    private const int BendPitchWireOffset = 24;

    public PedalFxViewModel()
    {
        TypeOptions.Add(ToPedalTypeOption(0));
        TypeOptions.Add(ToPedalTypeOption(1));
        TypeOptions.Add(ToPedalTypeOption(2));
        PositionOptions.Add("Input");
        PositionOptions.Add("Post Amp");

        foreach (var value in Enumerable.Range(
                     KatanaMkIIParameterCatalog.PedalFxWahType.Minimum,
                     KatanaMkIIParameterCatalog.PedalFxWahType.Maximum - KatanaMkIIParameterCatalog.PedalFxWahType.Minimum + 1))
        {
            WahTypeOptions.Add(ToWahTypeOption((byte)value));
        }

        SelectedTypeOption = TypeOptions[0];
        SelectedPositionOption = PositionOptions[0];
        SelectedWahTypeOption = WahTypeOptions[0];
        PedalPosition = KatanaMkIIParameterCatalog.PedalFxWahPedalPosition.Minimum;
        PedalMinimum = KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum.Minimum;
        PedalMaximum = KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum.Maximum;
        EffectLevel = KatanaMkIIParameterCatalog.PedalFxWahEffectLevel.Maximum;
        DirectMix = KatanaMkIIParameterCatalog.PedalFxWahDirectMix.Minimum;
        BendPitch = 12;
        BendPosition = 50;
        BendEffectLevel = 100;
        BendDirectMix = 0;
        Evh95Position = 100;
        Evh95Minimum = 0;
        Evh95Maximum = 100;
        Evh95EffectLevel = 100;
        Evh95DirectMix = 0;
        FootVolume = KatanaMkIIParameterCatalog.FootVolume.Maximum;

        this.WhenAnyValue(x => x.SelectedTypeOption)
            .Subscribe(_ =>
            {
                this.RaisePropertyChanged(nameof(IsWahMode));
                this.RaisePropertyChanged(nameof(IsBendMode));
                this.RaisePropertyChanged(nameof(IsEvh95Mode));
            });
    }

    public ObservableCollection<string> TypeOptions { get; } = [];

    public ObservableCollection<string> PositionOptions { get; } = [];

    public ObservableCollection<string> WahTypeOptions { get; } = [];

    [Reactive]
    public bool IsEnabled { get; set; }

    [Reactive]
    public string SelectedTypeOption { get; set; }

    [Reactive]
    public string SelectedPositionOption { get; set; }

    [Reactive]
    public string SelectedWahTypeOption { get; set; }

    [Reactive]
    public int PedalPosition { get; set; }

    [Reactive]
    public int PedalMinimum { get; set; }

    [Reactive]
    public int PedalMaximum { get; set; }

    [Reactive]
    public int EffectLevel { get; set; }

    [Reactive]
    public int DirectMix { get; set; }

    [Reactive]
    public int BendPitch { get; set; }

    [Reactive]
    public int BendPosition { get; set; }

    [Reactive]
    public int BendEffectLevel { get; set; }

    [Reactive]
    public int BendDirectMix { get; set; }

    [Reactive]
    public int Evh95Position { get; set; }

    [Reactive]
    public int Evh95Minimum { get; set; }

    [Reactive]
    public int Evh95Maximum { get; set; }

    [Reactive]
    public int Evh95EffectLevel { get; set; }

    [Reactive]
    public int Evh95DirectMix { get; set; }

    [Reactive]
    public int FootVolume { get; set; }

    public bool IsWahMode => TryParsePedalTypeOption(SelectedTypeOption, out var value) && value == 0;

    public bool IsBendMode => TryParsePedalTypeOption(SelectedTypeOption, out var value) && value == 1;

    public bool IsEvh95Mode => TryParsePedalTypeOption(SelectedTypeOption, out var value) && value == 2;

    public IReadOnlyList<KatanaParameterDefinition> GetReadParameters()
    {
        return KatanaMkIIParameterCatalog.PedalFxReadParameters;
    }

    public IReadOnlyList<KatanaParameterDefinition> GetManualWriteParameters()
    {
        var parameters = new List<KatanaParameterDefinition>
        {
            KatanaMkIIParameterCatalog.PedalFxSwitch,
            KatanaMkIIParameterCatalog.PedalFxType,
            KatanaMkIIParameterCatalog.PedalFxPosition,
            KatanaMkIIParameterCatalog.FootVolume,
        };

        if (IsWahMode)
        {
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxWahType);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxWahPedalPosition);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxWahEffectLevel);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxWahDirectMix);
        }
        else if (IsBendMode)
        {
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxBendPitch);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxBendPedalPosition);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxBendEffectLevel);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxBendDirectMix);
        }
        else if (IsEvh95Mode)
        {
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxEvh95Position);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxEvh95Minimum);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxEvh95Maximum);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxEvh95EffectLevel);
            parameters.Add(KatanaMkIIParameterCatalog.PedalFxEvh95DirectMix);
        }

        return parameters;
    }

    public KatanaParameterDefinition GetParameter(string key)
    {
        return key switch
        {
            "pedal-fx-switch" => KatanaMkIIParameterCatalog.PedalFxSwitch,
            "pedal-fx-type" => KatanaMkIIParameterCatalog.PedalFxType,
            "pedal-fx-position" => KatanaMkIIParameterCatalog.PedalFxPosition,
            "pedal-fx-wah-type" => KatanaMkIIParameterCatalog.PedalFxWahType,
            "pedal-fx-wah-position" => KatanaMkIIParameterCatalog.PedalFxWahPedalPosition,
            "pedal-fx-wah-min" => KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum,
            "pedal-fx-wah-max" => KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum,
            "pedal-fx-wah-effect-level" => KatanaMkIIParameterCatalog.PedalFxWahEffectLevel,
            "pedal-fx-wah-direct-mix" => KatanaMkIIParameterCatalog.PedalFxWahDirectMix,
            "pedal-fx-bend-pitch" => KatanaMkIIParameterCatalog.PedalFxBendPitch,
            "pedal-fx-bend-position" => KatanaMkIIParameterCatalog.PedalFxBendPedalPosition,
            "pedal-fx-bend-effect-level" => KatanaMkIIParameterCatalog.PedalFxBendEffectLevel,
            "pedal-fx-bend-direct-mix" => KatanaMkIIParameterCatalog.PedalFxBendDirectMix,
            "pedal-fx-evh95-position" => KatanaMkIIParameterCatalog.PedalFxEvh95Position,
            "pedal-fx-evh95-min" => KatanaMkIIParameterCatalog.PedalFxEvh95Minimum,
            "pedal-fx-evh95-max" => KatanaMkIIParameterCatalog.PedalFxEvh95Maximum,
            "pedal-fx-evh95-effect-level" => KatanaMkIIParameterCatalog.PedalFxEvh95EffectLevel,
            "pedal-fx-evh95-direct-mix" => KatanaMkIIParameterCatalog.PedalFxEvh95DirectMix,
            "pedal-fx-foot-volume" => KatanaMkIIParameterCatalog.FootVolume,
            _ => throw new KeyNotFoundException($"Unknown pedal parameter key '{key}'."),
        };
    }

    public void ApplyValues(IReadOnlyDictionary<string, byte> values)
    {
        IsEnabled = values[KatanaMkIIParameterCatalog.PedalFxSwitch.Key] != 0;
        SelectedTypeOption = ToPedalTypeOption(values[KatanaMkIIParameterCatalog.PedalFxType.Key]);
        SelectedPositionOption = ToPositionOption(values[KatanaMkIIParameterCatalog.PedalFxPosition.Key]);
        SelectedWahTypeOption = ToWahTypeOption(values[KatanaMkIIParameterCatalog.PedalFxWahType.Key]);
        PedalPosition = values[KatanaMkIIParameterCatalog.PedalFxWahPedalPosition.Key];
        PedalMinimum = values[KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum.Key];
        PedalMaximum = values[KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum.Key];
        EffectLevel = values[KatanaMkIIParameterCatalog.PedalFxWahEffectLevel.Key];
        DirectMix = values[KatanaMkIIParameterCatalog.PedalFxWahDirectMix.Key];
        BendPitch = (int)values[KatanaMkIIParameterCatalog.PedalFxBendPitch.Key] - BendPitchWireOffset;
        BendPosition = values[KatanaMkIIParameterCatalog.PedalFxBendPedalPosition.Key];
        BendEffectLevel = values[KatanaMkIIParameterCatalog.PedalFxBendEffectLevel.Key];
        BendDirectMix = values[KatanaMkIIParameterCatalog.PedalFxBendDirectMix.Key];
        Evh95Position = values[KatanaMkIIParameterCatalog.PedalFxEvh95Position.Key];
        Evh95Minimum = values[KatanaMkIIParameterCatalog.PedalFxEvh95Minimum.Key];
        Evh95Maximum = values[KatanaMkIIParameterCatalog.PedalFxEvh95Maximum.Key];
        Evh95EffectLevel = values[KatanaMkIIParameterCatalog.PedalFxEvh95EffectLevel.Key];
        Evh95DirectMix = values[KatanaMkIIParameterCatalog.PedalFxEvh95DirectMix.Key];
        FootVolume = values[KatanaMkIIParameterCatalog.FootVolume.Key];
    }

    public bool TryGetCurrentValue(string parameterKey, out byte value)
    {
        value = 0;
        switch (parameterKey)
        {
            case "pedal-fx-switch":
                value = IsEnabled ? (byte)1 : (byte)0;
                return true;
            case "pedal-fx-type":
                return TryParsePedalTypeOption(SelectedTypeOption, out value);
            case "pedal-fx-position":
                return TryParsePositionOption(SelectedPositionOption, out value);
            case "pedal-fx-wah-type":
                return TryParseWahTypeOption(SelectedWahTypeOption, out value);
            case "pedal-fx-wah-position":
                value = (byte)Math.Clamp(PedalPosition, KatanaMkIIParameterCatalog.PedalFxWahPedalPosition.Minimum, KatanaMkIIParameterCatalog.PedalFxWahPedalPosition.Maximum);
                return true;
            case "pedal-fx-wah-min":
                value = (byte)Math.Clamp(PedalMinimum, KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum.Minimum, KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum.Maximum);
                return true;
            case "pedal-fx-wah-max":
                value = (byte)Math.Clamp(PedalMaximum, KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum.Minimum, KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum.Maximum);
                return true;
            case "pedal-fx-wah-effect-level":
                value = (byte)Math.Clamp(EffectLevel, KatanaMkIIParameterCatalog.PedalFxWahEffectLevel.Minimum, KatanaMkIIParameterCatalog.PedalFxWahEffectLevel.Maximum);
                return true;
            case "pedal-fx-wah-direct-mix":
                value = (byte)Math.Clamp(DirectMix, KatanaMkIIParameterCatalog.PedalFxWahDirectMix.Minimum, KatanaMkIIParameterCatalog.PedalFxWahDirectMix.Maximum);
                return true;
            case "pedal-fx-bend-pitch":
                value = (byte)Math.Clamp(BendPitch + BendPitchWireOffset, 0, 48);
                return true;
            case "pedal-fx-bend-position":
                value = (byte)Math.Clamp(BendPosition, 0, 100);
                return true;
            case "pedal-fx-bend-effect-level":
                value = (byte)Math.Clamp(BendEffectLevel, 0, 100);
                return true;
            case "pedal-fx-bend-direct-mix":
                value = (byte)Math.Clamp(BendDirectMix, 0, 100);
                return true;
            case "pedal-fx-evh95-position":
                value = (byte)Math.Clamp(Evh95Position, 0, 100);
                return true;
            case "pedal-fx-evh95-min":
                value = (byte)Math.Clamp(Evh95Minimum, 0, 100);
                return true;
            case "pedal-fx-evh95-max":
                value = (byte)Math.Clamp(Evh95Maximum, 0, 100);
                return true;
            case "pedal-fx-evh95-effect-level":
                value = (byte)Math.Clamp(Evh95EffectLevel, 0, 100);
                return true;
            case "pedal-fx-evh95-direct-mix":
                value = (byte)Math.Clamp(Evh95DirectMix, 0, 100);
                return true;
            case "pedal-fx-foot-volume":
                value = (byte)Math.Clamp(FootVolume, KatanaMkIIParameterCatalog.FootVolume.Minimum, KatanaMkIIParameterCatalog.FootVolume.Maximum);
                return true;
            default:
                return false;
        }
    }

    public bool TryGetParameterValue(string? propertyName, out KatanaParameterDefinition parameter, out byte value, out string description)
    {
        parameter = KatanaMkIIParameterCatalog.PedalFxSwitch;
        value = 0;
        description = string.Empty;

        switch (propertyName)
        {
            case nameof(IsEnabled):
                parameter = KatanaMkIIParameterCatalog.PedalFxSwitch;
                value = IsEnabled ? (byte)1 : (byte)0;
                description = IsEnabled ? "On" : "Off";
                return true;
            case nameof(SelectedTypeOption):
                parameter = KatanaMkIIParameterCatalog.PedalFxType;
                description = SelectedTypeOption;
                return TryParsePedalTypeOption(SelectedTypeOption, out value);
            case nameof(SelectedPositionOption):
                parameter = KatanaMkIIParameterCatalog.PedalFxPosition;
                description = SelectedPositionOption;
                return TryParsePositionOption(SelectedPositionOption, out value);
            case nameof(SelectedWahTypeOption):
                parameter = KatanaMkIIParameterCatalog.PedalFxWahType;
                description = SelectedWahTypeOption;
                return TryParseWahTypeOption(SelectedWahTypeOption, out value);
            case nameof(PedalPosition):
                parameter = KatanaMkIIParameterCatalog.PedalFxWahPedalPosition;
                value = (byte)Math.Clamp(PedalPosition, parameter.Minimum, parameter.Maximum);
                description = value.ToString();
                return true;
            case nameof(PedalMinimum):
                parameter = KatanaMkIIParameterCatalog.PedalFxWahPedalMinimum;
                value = (byte)Math.Clamp(PedalMinimum, parameter.Minimum, parameter.Maximum);
                description = value.ToString();
                return true;
            case nameof(PedalMaximum):
                parameter = KatanaMkIIParameterCatalog.PedalFxWahPedalMaximum;
                value = (byte)Math.Clamp(PedalMaximum, parameter.Minimum, parameter.Maximum);
                description = value.ToString();
                return true;
            case nameof(EffectLevel):
                parameter = KatanaMkIIParameterCatalog.PedalFxWahEffectLevel;
                value = (byte)Math.Clamp(EffectLevel, parameter.Minimum, parameter.Maximum);
                description = value.ToString();
                return true;
            case nameof(DirectMix):
                parameter = KatanaMkIIParameterCatalog.PedalFxWahDirectMix;
                value = (byte)Math.Clamp(DirectMix, parameter.Minimum, parameter.Maximum);
                description = value.ToString();
                return true;
            case nameof(BendPitch):
                parameter = KatanaMkIIParameterCatalog.PedalFxBendPitch;
                value = (byte)Math.Clamp(BendPitch + BendPitchWireOffset, 0, 48);
                description = BendPitch >= 0 ? $"+{BendPitch}" : BendPitch.ToString();
                return true;
            case nameof(BendPosition):
                parameter = KatanaMkIIParameterCatalog.PedalFxBendPedalPosition;
                value = (byte)Math.Clamp(BendPosition, 0, 100);
                description = value.ToString();
                return true;
            case nameof(BendEffectLevel):
                parameter = KatanaMkIIParameterCatalog.PedalFxBendEffectLevel;
                value = (byte)Math.Clamp(BendEffectLevel, 0, 100);
                description = value.ToString();
                return true;
            case nameof(BendDirectMix):
                parameter = KatanaMkIIParameterCatalog.PedalFxBendDirectMix;
                value = (byte)Math.Clamp(BendDirectMix, 0, 100);
                description = value.ToString();
                return true;
            case nameof(Evh95Position):
                parameter = KatanaMkIIParameterCatalog.PedalFxEvh95Position;
                value = (byte)Math.Clamp(Evh95Position, 0, 100);
                description = value.ToString();
                return true;
            case nameof(Evh95Minimum):
                parameter = KatanaMkIIParameterCatalog.PedalFxEvh95Minimum;
                value = (byte)Math.Clamp(Evh95Minimum, 0, 100);
                description = value.ToString();
                return true;
            case nameof(Evh95Maximum):
                parameter = KatanaMkIIParameterCatalog.PedalFxEvh95Maximum;
                value = (byte)Math.Clamp(Evh95Maximum, 0, 100);
                description = value.ToString();
                return true;
            case nameof(Evh95EffectLevel):
                parameter = KatanaMkIIParameterCatalog.PedalFxEvh95EffectLevel;
                value = (byte)Math.Clamp(Evh95EffectLevel, 0, 100);
                description = value.ToString();
                return true;
            case nameof(Evh95DirectMix):
                parameter = KatanaMkIIParameterCatalog.PedalFxEvh95DirectMix;
                value = (byte)Math.Clamp(Evh95DirectMix, 0, 100);
                description = value.ToString();
                return true;
            case nameof(FootVolume):
                parameter = KatanaMkIIParameterCatalog.FootVolume;
                value = (byte)Math.Clamp(FootVolume, KatanaMkIIParameterCatalog.FootVolume.Minimum, KatanaMkIIParameterCatalog.FootVolume.Maximum);
                description = value.ToString();
                return true;
            default:
                return false;
        }
    }

    public static string ToPedalTypeOption(byte value)
    {
        return value switch
        {
            0 => "0 - Wah",
            1 => "1 - Pedal Bend",
            2 => "2 - EVH WAH95",
            _ => $"{value} - Value {value}",
        };
    }

    public static string ToPositionOption(byte value)
    {
        return value switch
        {
            0 => "Input",
            1 => "Post Amp",
            _ => $"Value {value}",
        };
    }

    public static string ToWahTypeOption(byte value) =>
        KatanaTypeNameTables.PedalWahTypes.TryGetValue(value, out var name) ? name : $"Type {value}";

    public static bool TryParsePedalTypeOption(string? option, out byte value)
    {
        return TryParseLeadingByte(option, out value);
    }

    public static bool TryParseWahTypeOption(string? option, out byte value)
    {
        // First try reverse-lookup in name table.
        if (!string.IsNullOrWhiteSpace(option))
        {
            foreach (var kvp in KatanaTypeNameTables.PedalWahTypes)
            {
                if (kvp.Value.Equals(option, StringComparison.Ordinal))
                {
                    value = kvp.Key;
                    return true;
                }
            }
        }

        // Fallback: legacy "Type N" format.
        value = 0;
        const string Prefix = "Type ";
        if (string.IsNullOrWhiteSpace(option) || !option.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        return byte.TryParse(option[Prefix.Length..], out value);
    }

    public static bool TryParsePositionOption(string? option, out byte value)
    {
        value = option switch
        {
            "Input" => (byte)0,
            "Post Amp" => (byte)1,
            _ => (byte)0,
        };

        return option is "Input" or "Post Amp";
    }

    private static bool TryParseLeadingByte(string? option, out byte value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(option))
        {
            return false;
        }

        var separatorIndex = option.IndexOf(' ');
        var candidate = separatorIndex < 0
            ? option
            : option[..separatorIndex];
        return byte.TryParse(candidate, out value);
    }
}
