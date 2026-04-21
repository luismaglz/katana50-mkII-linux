using Kataka.Domain.Midi;

namespace Kataka.App.KatanaState;

public class AmpControlState
{
    private int _value;

    public AmpControlState(KatanaParameterDefinition parameter, int displayMinimum = 0, int displayMaximum = 100,
        string displayUnit = "", string displayName = "", string description = "")
    {
        Parameter = parameter;
        _value = parameter.Minimum;
        DisplayMinimum = displayMinimum;
        DisplayMaximum = displayMaximum;
        DisplayUnit = displayUnit;
        DisplayName = parameter.DisplayName ?? displayName;
        Description = parameter.Description ?? description;
    }

    public string DisplayName { get; set; }
    public string Description { get; set; }

    public KatanaParameterDefinition Parameter { get; set; }

    public int Minimum => Parameter.Minimum;

    public int Maximum => Parameter.Maximum;

    public int DisplayMinimum { get; set; }

    public int DisplayMaximum { get; set; }

    public string DisplayUnit { get; set; }

    public int Value
    {
        get => _value;
        set
        {
            if (_value == value) return;
            _value = value;
            ValueChanged?.Invoke();
            WriteRequested?.Invoke();
        }
    }

    public event Action? ValueChanged;

    /// <summary>Fires when the user sets <see cref="Value" />. Subscribe here for write-back to amp.</summary>
    public event Action? WriteRequested;

    /// <summary>
    ///     Updates the value from an amp read or push notification. Fires <see cref="ValueChanged" /> for UI
    ///     but NOT <see cref="WriteRequested" />, breaking the circular write-back loop.
    /// </summary>
    public void SetFromAmp(int value)
    {
        if (_value == value) return;
        _value = value;
        ValueChanged?.Invoke();
    }

    /// <summary>
    ///     Decodes a raw SysEx byte payload and updates from amp. For ByteSize==2 uses INTEGER2x7 decoding:
    ///     value = (bytes[0] &lt;&lt; 7) | bytes[1].
    /// </summary>
    public void SetFromAmp(byte[] bytes)
    {
        var decoded = Parameter.ByteSize == 2
            ? (bytes[0] << 7) | bytes[1]
            : bytes[0];
        SetFromAmp(decoded);
    }

    /// <summary>
    ///     Returns the SysEx byte encoding of <see cref="Value" /> for writing to the amp.
    ///     For ByteSize==2 uses INTEGER2x7: [value >> 7 &amp; 0x7F, value &amp; 0x7F].
    /// </summary>
    public byte[] GetWriteBytes()
    {
        if (Parameter.ByteSize == 2)
            return [(byte)((_value >> 7) & 0x7F), (byte)(_value & 0x7F)];
        return [(byte)_value];
    }
}
