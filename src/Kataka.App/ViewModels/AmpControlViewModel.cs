using CommunityToolkit.Mvvm.ComponentModel;
using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

public partial class AmpControlViewModel : ViewModelBase
{
    public AmpControlViewModel(KatanaParameterDefinition parameter)
    {
        Parameter = parameter;
        Value = parameter.Minimum;
    }

    public KatanaParameterDefinition Parameter { get; }

    public string DisplayName => Parameter.DisplayName;

    public int Minimum => Parameter.Minimum;

    public int Maximum => Parameter.Maximum;

    [ObservableProperty]
    public partial int Value { get; set; }
}
