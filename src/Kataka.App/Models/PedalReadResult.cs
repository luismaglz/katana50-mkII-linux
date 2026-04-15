namespace Kataka.App.Services;

public record PedalReadResult(bool Success, string StatusMessage, string PedalControlsStatus, bool IsWahMode);