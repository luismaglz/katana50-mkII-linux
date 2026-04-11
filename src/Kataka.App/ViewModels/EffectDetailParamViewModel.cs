using CommunityToolkit.Mvvm.ComponentModel;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public sealed partial class EffectDetailParamViewModel : ObservableObject
{
    public EffectDetailParamViewModel(KatanaParameterDefinition definition)
    {
        Definition = definition;
        value = definition.Minimum;
    }

    public KatanaParameterDefinition Definition { get; }

    public string Label => Definition.DisplayName;
    public int Minimum => Definition.Minimum;
    public int Maximum => Definition.Maximum;

    /// <summary>True when the parameter is a two-position toggle (e.g., Solo SW, Hi Density).</summary>
    public bool IsToggle => Maximum - Minimum <= 1;

    [ObservableProperty]
    private int value;

    /// <summary>Boolean accessor for toggle parameters; setting it flips Value between Min and Max.</summary>
    public bool IsOn
    {
        get => Value != Minimum;
        set
        {
            Value = value ? Maximum : Minimum;
            OnPropertyChanged();
        }
    }

    partial void OnValueChanged(int oldValue, int newValue)
    {
        if (IsToggle)
            OnPropertyChanged(nameof(IsOn));
    }
}
