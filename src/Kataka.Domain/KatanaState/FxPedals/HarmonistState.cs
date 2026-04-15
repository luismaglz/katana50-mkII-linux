using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class HarmonistState
{
    public HarmonistState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModHarmonistParams : KatanaMkIIParameterCatalog.FxHarmonistParams;
        Voice     = new AmpControlState(p[0], description: "Selects the number of voices for the pitch shift sound (1VOICE or 2VOICE).");
        Harmony1  = new AmpControlState(p[1], description: "Determines the pitch of the sound added to the input sound for harmony 1.");
        Level1    = new AmpControlState(p[2], description: "Adjusts the volume of the harmony 1 sound.");
        Harmony2  = new AmpControlState(p[3], description: "Determines the pitch of the sound added to the input sound for harmony 2.");
        Level2    = new AmpControlState(p[4], description: "Adjusts the volume of the harmony 2 sound.");
        Feedback  = new AmpControlState(p[5], description: "Adjusts the feedback amount of the harmonist sound.");
        DirectMix = new AmpControlState(p[6], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Voice;
    public AmpControlState Harmony1;
    public AmpControlState Level1;
    public AmpControlState Harmony2;
    public AmpControlState Level2;
    public AmpControlState Feedback;
    public AmpControlState DirectMix;
}
