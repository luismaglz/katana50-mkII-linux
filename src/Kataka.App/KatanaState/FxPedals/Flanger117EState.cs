using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class Flanger117EState
{
    public Flanger117EState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModFlanger117EParams : KatanaMkIIParameterCatalog.FxFlanger117EParams;
        Manual = new AmpControlState(p[0], description: "Adjusts the center frequency at which to apply the effect.");
        Width = new AmpControlState(p[1], description: "Determines the depth of the flanging effect.");
        Speed = new AmpControlState(p[2], description: "This sets the rate of the flanging effect.");
        Regen = new AmpControlState(p[3], description: "Determines the amount of feedback. Increasing the value will emphasize the effect, creating a more unusual sound.");
    }

    public AmpControlState Manual;
    public AmpControlState Width;
    public AmpControlState Speed;
    public AmpControlState Regen;
}
