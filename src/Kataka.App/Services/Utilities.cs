using System;
using System.Collections.Generic;

using Kataka.Domain.Midi;

namespace Kataka.App.Services;

public sealed class Utilities
{
    public static bool CanBatchWrite(KatanaParameterDefinition previous, KatanaParameterDefinition current) =>
        previous.Address[0] == current.Address[0] &&
        previous.Address[1] == current.Address[1] &&
        previous.Address[2] == current.Address[2] &&
        previous.Address[3] + 1 == current.Address[3];

    public static string AddressToKey(IReadOnlyList<byte> address) =>
        $"{address[0]:X2}-{address[1]:X2}-{address[2]:X2}-{address[3]:X2}";

    public static int DecodeDelayTime(IReadOnlyList<byte> data)
    {
        if (data.Count != 2) throw new ArgumentException("Delay time data must contain exactly 2 bytes.", nameof(data));
        return (data[0] & 0x0F) << 7 | data[1] & 0x7F;
    }

    //  public Dictionary<string, Action<byte>> BuildPushHandlerLookup()
    // {
    //     var _pushHandlerLookup = new Dictionary<string, Action<byte>>(StringComparer.Ordinal);
    //
    //     List<KatanaParameterDefinition> controls = new(
    //     [
    //         KatanaMkIIParameterCatalog.AmpType,
    //         KatanaMkIIParameterCatalog.AmpVariation,
    //         KatanaMkIIParameterCatalog.AmpGain,
    //         KatanaMkIIParameterCatalog.AmpVolume,
    //         KatanaMkIIParameterCatalog.AmpBass,
    //         KatanaMkIIParameterCatalog.AmpMiddle,
    //         KatanaMkIIParameterCatalog.AmpTreble,
    //         KatanaMkIIParameterCatalog.AmpPresence,
    //         KatanaMkIIParameterCatalog.CabinetResonance
    //     ]);
    //
    //     // Amp controls → domain state (VMs observe via ValueChanged).
    //     foreach (var control in controls)
    //     {
    //         _pushHandlerLookup[Utilities.AddressToKey(control.Address)] = value =>
    //         {
    //             if (_domainAmpControlsByKey.TryGetValue(control.Parameter.Key, out var domainControl))
    //                 domainControl.Value = value;
    //         };
    //     }
    //
    //     // Panel effects: domain-migrated VMs use domain state; ModFx still uses ApplyAmpValues.
    //     foreach (var effect in _context.PanelEffects)
    //     {
    //         foreach (var param in effect.GetSyncParameters())
    //         {
    //             var capturedEffect = effect;
    //             var capturedKey = param.Key;
    //             _ampSyncService._pushHandlerLookup[Utilities.AddressToKey(param.Address)] = value =>
    //             {
    //                 if (_ampSyncService._domainPanelStatesByKey.TryGetValue(capturedKey, out var domainControl))
    //                     domainControl.Value = value;
    //                 else
    //                     capturedEffect.ApplyAmpValues(new Dictionary<string, int>(StringComparer.Ordinal)
    //                         { [capturedKey] = value });
    //             };
    //         }
    //     }
    //
    //     // AmpType / CabinetResonance / AmpVariation push → domain state.
    //     _pushHandlerLookup[Utilities.AddressToKey(KatanaMkIIParameterCatalog.AmpType.Address)] = value =>
    //     {
    //         if (_ampSyncService._domainAmpControlsByKey.TryGetValue(KatanaMkIIParameterCatalog.AmpType.Key, out var c)) c.Value = value;
    //     };
    //     _pushHandlerLookup[Utilities.AddressToKey(KatanaMkIIParameterCatalog.CabinetResonance.Address)] = value =>
    //     {
    //         if (_ampSyncService._domainAmpControlsByKey.TryGetValue(KatanaMkIIParameterCatalog.CabinetResonance.Key, out var c)) c.Value = value;
    //     };
    //     _pushHandlerLookup[Utilities.AddressToKey(KatanaMkIIParameterCatalog.AmpVariation.Address)] = value =>
    //     {
    //         if (_ampSyncService._domainAmpControlsByKey.TryGetValue(KatanaMkIIParameterCatalog.AmpVariation.Key, out var c)) c.Value = value;
    //     };
    //
    //     // Panel channel push.
    //     _pushHandlerLookup[Utilities.AddressToKey([0x00, 0x01, 0x00, 0x00])] = value =>
    //     {
    //         var displayName = value switch
    //         {
    //             0 => "Panel",
    //             1 => "CH A1",
    //             2 => "CH A2",
    //             5 => "CH B1",
    //             6 => "CH B2",
    //             _ => null
    //         };
    //         if (displayName is null) return;
    //
    //         _logger.LogInformation("Amp channel changed (push): {Channel}", displayName);
    //         PanelChannelPushed.OnNext(displayName);
    //
    //         _ = Dispatcher.UIThread.InvokeAsync(async () =>
    //         {
    //             await Task.Delay(150);
    //             await _ampSyncService.TryReadAmpControlsAsync();
    //             await _ampSyncService.TryReadPanelControlsAsync();
    //             await _ampSyncService.TryReadPedalControlsAsync();
    //         });
    //     };
    //
    //     return _pushHandlerLookup;
    // }
}
