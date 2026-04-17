using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class Phaser90EState
{
    public Phaser90EState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModPhaser90EParams : KatanaMkIIParameterCatalog.FxPhaser90EParams;
        Script = new AmpControlState(p[0], description: "Switches the character of the phaser. OFF: Modern; ON: Vintage.");
        Speed = new AmpControlState(p[1], description: "Sets the rate and depth of the phaser effect.");
    }

    public AmpControlState Script;
    public AmpControlState Speed;
}
