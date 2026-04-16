using Kataka.Domain.Midi;

namespace Kataka.Domain.KatanaState;

public class AmpControlState
{
    public AmpControlState(KatanaParameterDefinition parameter, int displayMinimum = 0, int displayMaximum = 100, string displayUnit = "", string displayName = "", string description = "")
    {
        Parameter = parameter;
        _value = parameter.Minimum;
        DisplayMinimum = displayMinimum;
        DisplayMaximum = displayMaximum;
        DisplayUnit = displayUnit;
        DisplayName = displayName ?? parameter.DisplayName;
        Description = description ?? parameter.DisplayName;
    }

    public string DisplayName { get; set; }
    public string Description { get; set; }

    public KatanaParameterDefinition Parameter { get; set; }

    public int Minimum => Parameter.Minimum;

    public int Maximum => Parameter.Maximum;

    public int DisplayMinimum { get; set; }

    public int DisplayMaximum { get; set; }

    public string DisplayUnit { get; set; }

    private int _value;

    public event Action? ValueChanged;

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
}
