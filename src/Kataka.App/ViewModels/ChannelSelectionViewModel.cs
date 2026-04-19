using CommunityToolkit.Mvvm.Input;

using Kataka.App.KatanaState;
using Kataka.App.Services;
using Kataka.Domain.Midi;

using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class ChannelSelectionViewModel
{
    private const string TextPanel = "PANEL";
    private readonly IAmpSyncService _ampSyncService;
    private readonly IKatanaState _katanaState;

    public ChannelSelectionViewModel(IAmpSyncService ampSyncService, IKatanaState katanaState)
    {
        _ampSyncService = ampSyncService;
        _katanaState = katanaState;
        CHA1 = new PanelChannelOptionViewModel("CH A1") { SelectCommand = SelectPanelChannelCommand };
        CHA2 = new PanelChannelOptionViewModel("CH A2") { SelectCommand = SelectPanelChannelCommand };
        CHB1 = new PanelChannelOptionViewModel("CH B1") { SelectCommand = SelectPanelChannelCommand };
        CHB2 = new PanelChannelOptionViewModel("CH B2") { SelectCommand = SelectPanelChannelCommand };
        Panel = new PanelChannelOptionViewModel(TextPanel) { SelectCommand = SelectPanelChannelCommand };

        _katanaState.SelectedChannelChanged += UpdatePanelChannelSelection;
    }

    public PanelChannelOptionViewModel CHA1 { get; internal set; }
    public PanelChannelOptionViewModel CHA2 { get; internal set; }
    public PanelChannelOptionViewModel CHB1 { get; internal set; }
    public PanelChannelOptionViewModel CHB2 { get; internal set; }
    public PanelChannelOptionViewModel Panel { get; internal set; }

    private List<PanelChannelOptionViewModel> PanelChannelOptions => new()
    {
        CHA1,
        CHA2,
        CHB1,
        CHB2,
        Panel
    };

    [Reactive] public string SelectedPanelChannel { get; set; } = TextPanel;

    [RelayCommand]
    private async Task SelectPanelChannel(string? channel)
    {
        var channelEnum = channel?.Trim() switch
        {
            "CH A1" => KatanaPanelChannel.ChA1,
            "CH A2" => KatanaPanelChannel.ChA2,
            "CH B1" => KatanaPanelChannel.ChB1,
            "CH B2" => KatanaPanelChannel.ChB2,
            TextPanel => KatanaPanelChannel.Panel,
            _ => throw new ArgumentOutOfRangeException(nameof(channel), $"Unknown channel: {channel}")
        };

        await _ampSyncService.SelectChannelAsync(channelEnum);
    }

    private void UpdatePanelChannelSelection(KatanaPanelChannel channel)
    {
        foreach (var option in PanelChannelOptions)
            option.IsSelected = false;

        switch (channel)
        {
            case KatanaPanelChannel.ChA1:
                CHA1.IsSelected = true;
                SelectedPanelChannel = "CH A1";
                break;
            case KatanaPanelChannel.ChA2:
                CHA2.IsSelected = true;
                SelectedPanelChannel = "CH A2";
                break;
            case KatanaPanelChannel.ChB1:
                CHB1.IsSelected = true;
                SelectedPanelChannel = "CH B1";
                break;
            case KatanaPanelChannel.ChB2:
                CHB2.IsSelected = true;
                SelectedPanelChannel = "CH B2";
                break;
            case KatanaPanelChannel.Panel:
                Panel.IsSelected = true;
                SelectedPanelChannel = TextPanel;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(channel), channel, null);
        }
    }
}
