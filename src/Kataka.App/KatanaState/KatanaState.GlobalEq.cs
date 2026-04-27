namespace Kataka.App.KatanaState;

public partial class KatanaState
{
    /// <summary> Global EQ (System block) ───────────────────────────────────────────────── </summary>
    public GlobalEqState GlobalEq { get; } = new();

    partial void RegisterGlobalEq() => RegisterAll(GlobalEq);
}
