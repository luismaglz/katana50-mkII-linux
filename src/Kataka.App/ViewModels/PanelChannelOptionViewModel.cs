using System.Windows.Input;

using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public class PanelChannelOptionViewModel(string displayName, byte channelValue) : ViewModelBase
{
    public string DisplayName { get; } = displayName;
    public byte ChannelValue { get; } = channelValue;

    [Reactive] public bool IsSelected { get; set; }

    public ICommand? SelectCommand { get; init; }
}
