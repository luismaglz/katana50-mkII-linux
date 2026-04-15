using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class PitchShifterState
{
    public PitchShifterState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModPitchShifterParams : KatanaMkIIParameterCatalog.FxPitchShifterParams;
        Voice     = new AmpControlState(p[0],  description: "Selects the number of voices for the pitch shift sound (1VOICE or 2VOICE).");
        Ps1Mode   = new AmpControlState(p[1],  description: "Selection for the PS1 pitch shifter mode.");
        Ps1Pitch  = new AmpControlState(p[2],  description: "Adjusts the amount of pitch shift (the amount of interval) for PS1 in semitone steps.");
        Ps1Fine   = new AmpControlState(p[3],  description: "Make fine adjustments to the PS1 interval. The amount of the change in Fine 100 is equivalent to that of the Pitch 1.");
        Ps1Level  = new AmpControlState(p[4],  description: "Adjusts the volume of the PS1 pitch shifter.");
        Ps2Mode   = new AmpControlState(p[5],  description: "Selection for the PS2 pitch shifter mode.");
        Ps2Pitch  = new AmpControlState(p[6],  description: "Adjusts the amount of pitch shift for PS2 in semitone steps.");
        Ps2Fine   = new AmpControlState(p[7],  description: "Make fine adjustments to the PS2 interval.");
        Ps2Level  = new AmpControlState(p[8],  description: "Adjusts the volume of the PS2 pitch shifter.");
        Feedback  = new AmpControlState(p[9],  description: "Adjusts the feedback amount of the pitch shift sound.");
        DirectMix = new AmpControlState(p[10], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Voice;
    public AmpControlState Ps1Mode;
    public AmpControlState Ps1Pitch;
    public AmpControlState Ps1Fine;
    public AmpControlState Ps1Level;
    public AmpControlState Ps2Mode;
    public AmpControlState Ps2Pitch;
    public AmpControlState Ps2Fine;
    public AmpControlState Ps2Level;
    public AmpControlState Feedback;
    public AmpControlState DirectMix;
}
