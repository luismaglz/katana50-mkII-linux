using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

/// <summary>
///     A single Contour band (Ver200+). Three instances exist: Contour1, Contour2, Contour3.
/// </summary>
public class ContourState
{
    public AmpControlState FreqShift;

    public AmpControlState Type;

    public ContourState(KatanaParameterDefinition typeParam, KatanaParameterDefinition freqShiftParam)
    {
        Type = new AmpControlState(typeParam, description: "Contour type.");
        FreqShift = new AmpControlState(freqShiftParam, -50, 50,
            description: "Frequency shift. Raw 0-100, display -50 to +50.");
    }
}
