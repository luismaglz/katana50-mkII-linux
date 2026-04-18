using System.Windows.Input;

using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class PanelChannelOptionViewModel(string displayName) : ViewModelBase
{
    public string DisplayName { get; } = displayName;

    [Reactive]
    public bool IsSelected { get; set; }

    public ICommand? SelectCommand { get; init; }
}
