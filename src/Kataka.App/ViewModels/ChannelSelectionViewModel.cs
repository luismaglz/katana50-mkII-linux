using Kataka.App.KatanaState;
using Kataka.Domain.Midi;

using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class ChannelSelectionViewModel
{
    private readonly IKatanaState _katanaState;

    public ChannelSelectionViewModel(IKatanaState katanaState)
    {
        _katanaState = katanaState;

        Panel = new PanelChannelOptionViewModel("PANEL", KatanaMkIIParameterCatalog.ChannelPanel) { SelectCommand = new SelectChannelCommand(this) };
        CHA1 = new PanelChannelOptionViewModel("CH A1", KatanaMkIIParameterCatalog.ChannelChA1) { SelectCommand = new SelectChannelCommand(this) };
        CHA2 = new PanelChannelOptionViewModel("CH A2", KatanaMkIIParameterCatalog.ChannelChA2) { SelectCommand = new SelectChannelCommand(this) };
        CHB1 = new PanelChannelOptionViewModel("CH B1", KatanaMkIIParameterCatalog.ChannelChB1) { SelectCommand = new SelectChannelCommand(this) };
        CHB2 = new PanelChannelOptionViewModel("CH B2", KatanaMkIIParameterCatalog.ChannelChB2) { SelectCommand = new SelectChannelCommand(this) };

        _katanaState.CurrentChannel.ValueChanged += UpdateSelection;
        _katanaState.PatchNameChanged += () => CurrentPatchName = _katanaState.CurrentPatchName;

        UpdateSelection();
    }

    public PanelChannelOptionViewModel Panel { get; }
    public PanelChannelOptionViewModel CHA1 { get; }
    public PanelChannelOptionViewModel CHA2 { get; }
    public PanelChannelOptionViewModel CHB1 { get; }
    public PanelChannelOptionViewModel CHB2 { get; }

    [Reactive] public string CurrentPatchName { get; set; } = string.Empty;

    private IEnumerable<PanelChannelOptionViewModel> AllOptions => [Panel, CHA1, CHA2, CHB1, CHB2];

    internal void SelectChannel(byte channelValue)
    {
        _katanaState.CurrentChannel.Value = channelValue;
    }

    private void UpdateSelection()
    {
        var current = _katanaState.CurrentChannel.Value;
        foreach (var option in AllOptions)
            option.IsSelected = option.ChannelValue == current;
    }

    private sealed class SelectChannelCommand(ChannelSelectionViewModel vm) : System.Windows.Input.ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter)
        {
            var channelValue = parameter switch
            {
                byte b => b,
                int i => (byte)i,
                _ => (byte?)null
            };
            if (channelValue.HasValue) vm.SelectChannel(channelValue.Value);
        }
    }
}

