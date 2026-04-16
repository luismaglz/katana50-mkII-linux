using System;
using System.Collections.Generic;
using System.ComponentModel;

using Kataka.Domain.Midi;

namespace Kataka.App.ViewModels;

/// <summary>
/// Contract that MainWindowViewModel and views code against for all panel effect pedals.
/// Each pedal type owns its type tables, parameters, variation, and sync logic internally.
/// </summary>
public interface IBasePedal : INotifyPropertyChanged
{
    string DisplayName { get; }
    KatanaPanelEffectDefinition Definition { get; }
    bool IsEnabled { get; set; }

    // ── Type selection ────────────────────────────────────────────────────────────

    /// <summary>Human-readable name of the currently selected effect type, or null if unset.</summary>
    string? SelectedTypeOption { get; set; }

    /// <summary>Encodes a display name to the wire byte value for writing to the amp.</summary>
    bool TryGetTypeValue(string? option, out byte value);

    /// <summary>Decodes a raw wire byte value to a display name.</summary>
    string ToTypeOption(byte rawValue);

    /// <summary>Short caption for the current type, suitable for logging.</summary>
    string TypeCaption { get; }

    // ── Variation (FXBOX_SEL color) ───────────────────────────────────────────────

    /// <summary>Variation color string: "Green", "Red", "Yellow", or "N/A" if not supported.</summary>
    string Variation { get; set; }

    // ── Sync contract ─────────────────────────────────────────────────────────────

    /// <summary>Returns all parameter definitions this pedal needs read from the amp.</summary>
    IReadOnlyList<KatanaParameterDefinition> GetSyncParameters();

    /// <summary>
    /// Called by MWVM after reading amp values. Each pedal maps key→its own typed property.
    /// Implementations must NOT raise ParameterChanged during this call (use SuppressingAmpApply guard).
    /// </summary>
    void ApplyAmpValues(IReadOnlyDictionary<string, int> values);

    /// <summary>Raised when the user changes a parameter value. MWVM queues the write.</summary>
    event EventHandler<PedalParameterChangedEventArgs>? ParameterChanged;
}

public sealed class PedalParameterChangedEventArgs : EventArgs
{
    public PedalParameterChangedEventArgs(string key, int value)
    {
        Key = key;
        Value = value;
    }

    public string Key { get; }
    public int Value { get; }
}
