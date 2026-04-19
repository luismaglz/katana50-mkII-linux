using System.Reflection;

using Microsoft.Extensions.Logging;

namespace Kataka.App.KatanaState;

public partial class KatanaState : IKatanaState
{
    private readonly ILogger<KatanaState> _logger;
    private readonly Dictionary<string, AmpControlState> _stateFields = new();

    public KatanaState(ILogger<KatanaState> logger)
    {
        _logger = logger;
        RegisterPanelMode();
        RegisterChannelMode();
        RegisterPedals();
        RegisterGlobalEq();
    }

    partial void RegisterPanelMode();
    partial void RegisterChannelMode();
    partial void RegisterPedals();
    partial void RegisterGlobalEq();

    public IReadOnlyDictionary<string, AmpControlState> GetAmpControlsByKey() =>
        new Dictionary<string, AmpControlState>(StringComparer.Ordinal)
        {
            [AmpType.Parameter.Key] = AmpType,
            [AmpVariation.Parameter.Key] = AmpVariation,
            [Gain.Parameter.Key] = Gain,
            [Volume.Parameter.Key] = Volume,
            [Bass.Parameter.Key] = Bass,
            [Middle.Parameter.Key] = Middle,
            [Treble.Parameter.Key] = Treble,
            [Presence.Parameter.Key] = Presence,
            [CabinetResonance.Parameter.Key] = CabinetResonance,
            [PedalChain.Parameter.Key] = PedalChain
        };

    public IReadOnlyDictionary<string, AmpControlState> GetAllRegisteredStates() => _stateFields;

    public void SetStates(IReadOnlyDictionary<string, byte> values)
    {
        foreach (var kvp in values) SetState(kvp.Key, kvp.Value);
    }

    public void SetState(string key, byte value)
    {
        if (_stateFields.TryGetValue(key, out var state))
        {
            state.SetFromAmp(value);
            _logger.LogDebug("{Name} ({Address}): {Value}", state.Parameter.DisplayName, key, value);
        }
        else
        {
            _logger.LogDebug("Untracked: {Address}", key);
        }
    }

    private void RegisterAll(object obj)
    {
        if (obj is AmpControlState direct)
        {
            _stateFields.TryAdd(direct.Parameter.AddressString, direct);
            return;
        }

        foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            if (field.GetValue(obj) is AmpControlState state)
                _stateFields.TryAdd(state.Parameter.AddressString, state);
            else if (field.FieldType.Namespace?.StartsWith("Kataka") == true
                     && field.GetValue(obj) is { } nested)
                RegisterAll(nested);
    }
}
