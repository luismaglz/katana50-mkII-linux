namespace Kataka.App.KatanaState;

public partial class KatanaState
{
    public SendReturnState SendReturn { get; } = new();

    partial void RegisterSendReturn() => RegisterAll(SendReturn);
}
