using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.Input;

using Kataka.App.KatanaState;
using Kataka.App.Services;
using Kataka.Domain.Midi;

using Microsoft.Extensions.Logging;

using ReactiveUI.Fody.Helpers;

namespace Kataka.App.ViewModels;

public partial class PatchViewModel : ViewModelBase
{
    // slot label → SysEx write-byte (matches KatanaPanelChannel program-change mapping)
    public static readonly IReadOnlyList<(string Label, byte SlotByte)> WriteSlots =
    [
        ("PANEL", 0),
        ("CH A1", 1),
        ("CH A2", 2),
        ("CH B1", 5),
        ("CH B2", 6),
    ];

    private static readonly TimeSpan TapResetThreshold = TimeSpan.FromSeconds(2.5);
    private readonly Action<string> _appendStatus;
    private readonly Func<bool> _isConnected;
    private readonly IKatanaSession _katanaSession;
    private readonly IKatanaState _katanaState;
    private readonly ILogger<PatchViewModel> _logger;

    private DateTimeOffset? _lastDelayTapAt;

    public PatchViewModel(
        IKatanaSession katanaSession,
        IKatanaState katanaState,
        IAmpSyncService syncService,
        Func<bool> isConnected,
        Action<string> appendStatus,
        ILogger<PatchViewModel> logger)
    {
        _katanaSession = katanaSession;
        _katanaState = katanaState;
        _isConnected = isConnected;
        _appendStatus = appendStatus;
        _logger = logger;

        katanaState.UserPatchNamesChanged += RefreshCurrentPatchName;
        katanaState.CurrentChannel.ValueChanged += RefreshCurrentPatchName;

        syncService.ReadCompleted.Subscribe(meta =>
        {
            if (meta.DelayTimeLoaded && meta.DelayTimeMs.HasValue)
            {
                DelayTimeMs = meta.DelayTimeMs;
                DelayTapStatus = $"Delay time loaded: {meta.DelayTimeMs} ms.";
            }
            else if (meta.PanelControlsStatus.Length > 0 && !meta.DelayTimeLoaded)
            {
                _lastDelayTapAt = null;
            }
        }).DisposeWith(Disposables);
    }

    [Reactive] public int PatchLevel { get; set; } = 100;
    public bool IsPatchLevelAvailable => false;
    [Reactive] public bool CanWritePatch { get; set; }
    [Reactive] public string DelayTapStatus { get; set; } = "Delay time has not been read yet.";
    [Reactive] public int? DelayTimeMs { get; set; }
    [Reactive] public string CurrentPatchName { get; set; } = "";

    public IStorageProvider? StorageProvider { get; set; }

    [RelayCommand]
    private async Task WritePatchToSlotAsync(object? slotParam)
    {
        if (!_isConnected())
        {
            _appendStatus("Connect to a MIDI port before writing a patch.");
            return;
        }

        if (!byte.TryParse(slotParam?.ToString(), out var slotByte)) return;

        var name = CurrentPatchName.Trim();
        if (name.Length == 0) name = "MY PATCH";

        try
        {
            _logger.LogInformation("Writing patch name '{Name}' then saving to slot {Slot}.", name, slotByte);
            await _katanaSession.WriteBlockAsync([0x60, 0x00, 0x00, 0x00], EncodePatchName(name));
            await _katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.PatchWriteAddress, [0x00, slotByte]);

            var slotLabel = WriteSlots.FirstOrDefault(s => s.SlotByte == slotByte).Label ?? slotByte.ToString();
            _katanaState.SetUserPatchName(slotByte, name);
            _appendStatus($"Patch '{name}' written to {slotLabel}.");
        }
        catch (Exception ex)
        {
            _appendStatus("Patch write failed.");
            _logger.LogError(ex, "Patch write to slot {Slot} failed.", slotByte);
        }
    }

    [RelayCommand]
    private async Task TapDelayAsync()
    {
        if (!_isConnected())
        {
            _appendStatus("Connect to a MIDI port before tapping delay time.");
            return;
        }

        var now = DateTimeOffset.Now;
        if (_lastDelayTapAt is null || now - _lastDelayTapAt.Value > TapResetThreshold)
        {
            _lastDelayTapAt = now;
            DelayTapStatus = "First tap registered. Tap again to set the delay time.";
            _appendStatus("Waiting for the second tap to calculate delay time.");
            _logger.LogInformation("Registered first delay tap.");
            return;
        }

        var tappedDelayTime = (int)Math.Clamp(
            Math.Round((now - _lastDelayTapAt.Value).TotalMilliseconds), 1, 2000);
        _lastDelayTapAt = now;

        try
        {
            _logger.LogInformation("Writing tapped delay time: {Ms} ms.", tappedDelayTime);
            await _katanaSession.WriteBlockAsync(
                KatanaMkIIParameterCatalog.DelayTimeAddress,
                EncodeDelayTime(tappedDelayTime));
            DelayTimeMs = tappedDelayTime;
            DelayTapStatus = $"Delay time tapped to {tappedDelayTime} ms.";
            _appendStatus("Delay time updated from tap tempo.");
        }
        catch (Exception ex)
        {
            DelayTapStatus = "Delay tap write failed.";
            _appendStatus(ex.Message);
            _logger.LogError(ex, "Delay tap write failed.");
        }
    }

    [RelayCommand]
    private async Task WritePatchAsync()
    {
        if (!_isConnected())
        {
            _appendStatus("Connect to a MIDI port before writing a patch.");
            return;
        }

        try
        {
            _logger.LogInformation("Sending WRITE PATCH command to Katana.");
            await _katanaSession.WriteBlockAsync(KatanaMkIIParameterCatalog.PatchWriteAddress, [0x00, 0x00]);
            _appendStatus("Patch written to Katana.");
            _logger.LogInformation("WRITE PATCH command sent.");
        }
        catch (Exception ex)
        {
            _appendStatus("Patch write failed.");
            _logger.LogError(ex, "Patch write command failed.");
        }
    }

    [RelayCommand]
    private async Task LoadPatchAsync()
    {
        if (StorageProvider is null)
        {
            _appendStatus("File dialog not available.");
            return;
        }

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Patch File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Boss Tone Studio Patch") { Patterns = ["*.tsl"] },
                new FilePickerFileType("All Files") { Patterns = ["*"] }
            ]
        });

        if (files.Count == 0) return;

        var file = files[0];
        try
        {
            _logger.LogInformation("Loading patch from {Name}...", file.Name);

            string json;
            await using (var stream = await file.OpenReadAsync())
            using (var reader = new StreamReader(stream))
            {
                json = await reader.ReadToEndAsync();
            }

            var patch = TslPatchSerializer.Deserialize(json);
            _logger.LogInformation("Patch '{Name}' parsed — {Count} block(s). Sending to amp...", patch.Name,
                patch.Blocks.Count);

            await _katanaSession.LoadPatchAsync(patch);
            _logger.LogInformation("Patch loaded. Refreshing display...");
            _appendStatus($"Patch '{patch.Name}' loaded.");
        }
        catch (Exception ex)
        {
            _appendStatus("Patch load failed.");
            _logger.LogError(ex, "Patch load failed.");
        }
    }

    [RelayCommand]
    private async Task SavePatchAsync()
    {
        if (StorageProvider is null)
        {
            _appendStatus("File dialog not available.");
            return;
        }

        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Patch File",
            SuggestedFileName = "patch.tsl",
            DefaultExtension = "tsl",
            FileTypeChoices = [new FilePickerFileType("Boss Tone Studio Patch") { Patterns = ["*.tsl"] }]
        });

        if (file is null) return;

        try
        {
            _logger.LogInformation("Reading all patch blocks from amp...");
            var patchName = Path.GetFileNameWithoutExtension(file.Name);
            var patch = await _katanaSession.ReadCurrentPatchAsync(patchName);
            var json = TslPatchSerializer.Serialize(patch);

            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(json);

            _logger.LogInformation("Patch '{Name}' saved to {File}.", patch.Name, file.Name);
            _appendStatus($"Patch saved as '{file.Name}'.");
        }
        catch (Exception ex)
        {
            _appendStatus("Patch save failed.");
            _logger.LogError(ex, "Patch save failed.");
        }
    }

    private void RefreshCurrentPatchName()
    {
        var channelByte = _katanaState.CurrentChannel.Value;
        if (_katanaState.UserPatchNames.TryGetValue(channelByte, out var name))
            CurrentPatchName = name;
    }

    private static byte[] EncodePatchName(string name)
    {
        var bytes = new byte[16];
        Array.Fill(bytes, (byte)' ');
        var chars = name.ToCharArray();
        for (var i = 0; i < Math.Min(chars.Length, 16); i++)
        {
            var c = chars[i];
            bytes[i] = c is >= ' ' and <= '~' ? (byte)c : (byte)' ';
        }
        return bytes;
    }

    private static byte[] EncodeDelayTime(int milliseconds)
    {
        var clamped = Math.Clamp(milliseconds, 1, 2000);
        return [(byte)((clamped >> 7) & 0x0F), (byte)(clamped & 0x7F)];
    }
}
