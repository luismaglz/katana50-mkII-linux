using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class SendReturnState
{
    public AmpControlState Sw = new(KatanaMkIIParameterCatalog.SendReturnSw);
    public AmpControlState Mode = new(KatanaMkIIParameterCatalog.SendReturnMode);
    public AmpControlState SendLevel = new(KatanaMkIIParameterCatalog.SendReturnSendLevel);
    public AmpControlState ReturnLevel = new(KatanaMkIIParameterCatalog.SendReturnReturnLevel);
}
