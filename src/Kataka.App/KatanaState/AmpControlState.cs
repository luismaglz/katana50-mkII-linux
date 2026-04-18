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
        }
    }

    public event Action? ValueChanged;

    /// <summary>
    ///     Updates the value from an amp read or push notification without firing <see cref="ValueChanged" />,
    ///     preventing the write-back loop.
    /// </summary>
    public void SetFromAmp(int value) => _value = value;
}
