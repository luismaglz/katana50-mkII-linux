using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState.FxPedals;

public class GraphicEqState
{
    public AmpControlState Hz125;
    public AmpControlState Hz250;

    public AmpControlState Hz31;
    public AmpControlState Hz500;
    public AmpControlState Hz62;
    public AmpControlState kHz1;
    public AmpControlState kHz16;
    public AmpControlState kHz2;
    public AmpControlState kHz4;
    public AmpControlState kHz8;
    public AmpControlState Level;

    public GraphicEqState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModGraphicEqParams : KatanaMkIIParameterCatalog.FxGraphicEqParams;
        Hz31 = new AmpControlState(p[0], description: "Adjusts the 31 Hz band level.");
        Hz62 = new AmpControlState(p[1], description: "Adjusts the 62 Hz band level.");
        Hz125 = new AmpControlState(p[2], description: "Adjusts the 125 Hz band level.");
        Hz250 = new AmpControlState(p[3], description: "Adjusts the 250 Hz band level.");
        Hz500 = new AmpControlState(p[4], description: "Adjusts the 500 Hz band level.");
        kHz1 = new AmpControlState(p[5], description: "Adjusts the 1 kHz band level.");
        kHz2 = new AmpControlState(p[6], description: "Adjusts the 2 kHz band level.");
        kHz4 = new AmpControlState(p[7], description: "Adjusts the 4 kHz band level.");
        kHz8 = new AmpControlState(p[8], description: "Adjusts the 8 kHz band level.");
        kHz16 = new AmpControlState(p[9], description: "Adjusts the 16 kHz band level.");
        Level = new AmpControlState(p[10], description: "Adjusts the overall volume level of the equalizer.");
    }
}
