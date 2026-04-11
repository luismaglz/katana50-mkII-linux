using CommunityToolkit.Mvvm.ComponentModel;

namespace Kataka.App.ViewModels;

public partial class PanelChannelOptionViewModel(string displayName) : ViewModelBase
{
    public string DisplayName { get; } = displayName;

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}
