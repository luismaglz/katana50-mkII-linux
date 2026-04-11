using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Media;
using Kataka.Domain.Midi;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Kataka.App.ViewModels;

public partial class PanelEffectViewModel : ViewModelBase
{
    private static readonly IBrush OffVariationBrush = new SolidColorBrush(Color.Parse("#35383f"));
    private static readonly IBrush GreenVariationBrush = new SolidColorBrush(Color.Parse("#91ff92"));
    private static readonly IBrush RedVariationBrush = new SolidColorBrush(Color.Parse("#ff6f61"));
    private static readonly IBrush YellowVariationBrush = new SolidColorBrush(Color.Parse("#ffd65c"));

    // Bidirectional map: display string → wire byte value.
    private readonly Dictionary<string, byte> _typeValueByOption = [];

    public PanelEffectViewModel(KatanaPanelEffectDefinition definition)
    {
        Definition = definition;

        if (definition.TypeParameter is null)
        {
            SelectedTypeOption = "N/A";
            return;
        }

        var nameTable = KatanaTypeNameTables.GetTableForKey(definition.TypeParameter.Key);

        foreach (var value in Enumerable.Range(definition.TypeParameter.Minimum, definition.TypeParameter.Maximum - definition.TypeParameter.Minimum + 1)
                     .Select(index => (byte)index)
                     .Where(value => !definition.TypeParameter.SkippedValues.Contains(value)))
        {
            var option = ToTypeOption(nameTable, value);
            TypeOptions.Add(option);
            _typeValueByOption[option] = value;
        }

        SelectedTypeOption = TypeOptions.FirstOrDefault() ?? "N/A";
    }

    public KatanaPanelEffectDefinition Definition { get; }

    public string DisplayName => Definition.DisplayName;

    [ObservableProperty]
    public partial bool IsEnabled { get; set; }

    [ObservableProperty]
    public partial string Variation { get; set; } = "Unknown";

    public ObservableCollection<string> TypeOptions { get; } = [];

    [ObservableProperty]
    public partial string SelectedTypeOption { get; set; } = "N/A";

    public IBrush VariationBrush => Variation switch
    {
        "Green" => GreenVariationBrush,
        "Red" => RedVariationBrush,
        "Yellow" => YellowVariationBrush,
        _ => OffVariationBrush,
    };

    public string VariationCaption => Variation == "N/A" ? "No variation" : Variation;

    public string TypeCaption => SelectedTypeOption == "N/A" ? "No type" : SelectedTypeOption;

    partial void OnVariationChanged(string value)
    {
        OnPropertyChanged(nameof(VariationBrush));
        OnPropertyChanged(nameof(VariationCaption));
    }

    partial void OnSelectedTypeOptionChanged(string value)
    {
        OnPropertyChanged(nameof(TypeCaption));
    }

    /// <summary>
    /// Returns the wire byte value for a type option string, or false if unknown.
    /// Supports both named options (e.g., "PLATE") and legacy "Type N" format.
    /// </summary>
    public bool TryGetTypeValue(string option, [NotNullWhen(true)] out byte value)
    {
        if (_typeValueByOption.TryGetValue(option, out value))
            return true;

        // Fallback: parse legacy "Type N" format.
        const string prefix = "Type ";
        if (option.StartsWith(prefix, System.StringComparison.Ordinal) &&
            byte.TryParse(option[prefix.Length..], out value))
            return true;

        value = 0;
        return false;
    }

    /// <summary>Returns a display string for a wire byte value using the given name table.</summary>
    public static string ToTypeOption(IReadOnlyDictionary<byte, string>? nameTable, byte value) =>
        nameTable?.TryGetValue(value, out var name) == true ? name : $"Type {value}";

    /// <summary>Returns an option string for a wire value using this effect's type name table.</summary>
    public string ToTypeOption(byte value)
    {
        var nameTable = Definition.TypeParameter is not null
            ? KatanaTypeNameTables.GetTableForKey(Definition.TypeParameter.Key)
            : null;
        return ToTypeOption(nameTable, value);
    }
}
