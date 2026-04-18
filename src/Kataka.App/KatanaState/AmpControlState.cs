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

    /// <summary>Fires when the user sets <see cref="Value"/>. Subscribe here for write-back to amp.</summary>
    public event Action? WriteRequested;

    /// <summary>
    ///     Updates the value from an amp read or push notification. Fires <see cref="ValueChanged"/> for UI
    ///     but NOT <see cref="WriteRequested"/>, breaking the circular write-back loop.
    /// </summary>
    public void SetFromAmp(int value)
    {
        if (_value == value) return;
        _value = value;
        ValueChanged?.Invoke();
    }
}
