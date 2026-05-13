namespace Kataka.App.KatanaState;

public partial class KatanaState
{
    public NoiseSuppressorState NoiseSuppressor { get; } = new();

    partial void RegisterNoiseSuppressor() => RegisterAll(NoiseSuppressor);
}
