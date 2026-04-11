namespace Kataka.App.ViewModels;

public sealed class PedalboardItemViewModel
{
    public string Key { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Detail { get; init; } = string.Empty;

    public bool IsActive { get; init; }

    public bool IsEndpoint { get; init; }

    public bool IsAmp { get; init; }

    public bool IsConnectedFromPrevious { get; init; }

    public bool CanToggle { get; init; }
}
