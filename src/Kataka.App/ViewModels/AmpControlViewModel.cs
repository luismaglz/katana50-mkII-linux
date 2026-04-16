using ReactiveUI;
using System;
using Kataka.Domain.KatanaState;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public class AmpControlViewModel : ViewModelBase
{
    private readonly AmpControlState _state;

    public AmpControlViewModel(AmpControlState state)
    {
        _state = state;
        _state.ValueChanged += () => this.RaisePropertyChanged(nameof(Value));
    }

    public KatanaParameterDefinition Parameter => _state.Parameter;
    public string DisplayName => _state.DisplayName;
    public int Minimum => _state.Minimum;
    public int Maximum => _state.Maximum;

    public int Value
    {
        get => _state.Value;
        set => _state.Value = Math.Clamp(value, _state.Minimum, _state.Maximum);
    }
}
