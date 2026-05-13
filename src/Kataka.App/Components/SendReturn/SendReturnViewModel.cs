using Kataka.App.KatanaState;

using ReactiveUI;

namespace Kataka.App.ViewModels;

public class SendReturnViewModel : ViewModelBase
{
    public SendReturnViewModel(IKatanaState katanaState)
    {
        var sr = katanaState.SendReturn;

        SendLevel = new AmpControlViewModel(sr.SendLevel);
        ReturnLevel = new AmpControlViewModel(sr.ReturnLevel);
        Mode = new AmpControlViewModel(sr.Mode);

        _sw = sr.Sw;
        _sw.ValueChanged += () => this.RaisePropertyChanged(nameof(IsEnabled));
    }

    private readonly AmpControlState _sw;

    public bool IsEnabled
    {
        get => _sw.Value != 0;
        set => _sw.Value = value ? 1 : 0;
    }

    // 0 = SEND/RETURN, 1 = RETURN ONLY
    public AmpControlViewModel Mode { get; }
    public AmpControlViewModel SendLevel { get; }
    public AmpControlViewModel ReturnLevel { get; }
}
