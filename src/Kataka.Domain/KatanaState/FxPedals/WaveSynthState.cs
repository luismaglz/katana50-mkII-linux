using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState.FxPedals;

public class WaveSynthState
{
    public WaveSynthState(bool isMod)
    {
        var p = isMod ? KatanaMkIIParameterCatalog.ModWaveSynthParams : KatanaMkIIParameterCatalog.FxWaveSynthParams;
        Wave        = new AmpControlState(p[0], description: "Selects a wave type which the synth sound is based on (SAW or SQUARE).");
        Cutoff      = new AmpControlState(p[1], description: "Adjusts the frequency where the harmonics contents of the sound are cut off.");
        Resonance   = new AmpControlState(p[2], description: "Adjusts the amount of resonance. The higher the value, the more the synth tone coloration is emphasized.");
        FilterSens  = new AmpControlState(p[3], description: "Adjusts the amount of filtering applied in response to the input.");
        FilterDecay = new AmpControlState(p[4], description: "This sets the time needed for the filter to finish its sweep.");
        FilterDepth = new AmpControlState(p[5], description: "Adjusts the depth of the filter. When the value is higher, the filter will change more drastically.");
        SynthLevel  = new AmpControlState(p[6], description: "Adjusts the volume of the synth sound.");
        DirectMix   = new AmpControlState(p[7], description: "Adjusts the volume of the direct sound.");
    }

    public AmpControlState Wave;
    public AmpControlState Cutoff;
    public AmpControlState Resonance;
    public AmpControlState FilterSens;
    public AmpControlState FilterDecay;
    public AmpControlState FilterDepth;
    public AmpControlState SynthLevel;
    public AmpControlState DirectMix;
}
