using System.Reflection;

using Kataka.App.Services;

using Microsoft.Extensions.Logging;

namespace Kataka.App.KatanaState;

public partial class KatanaState : IKatanaState
{
    private readonly ILogger<KatanaState> _logger;
    private readonly Dictionary<string, IMultiAddressState> _multiAddressStates = new();
    private readonly Dictionary<string, AmpControlState> _stateFields = new();

    public KatanaState(ILogger<KatanaState> logger)
    {
        _logger = logger;
        RegisterPanelMode();
        RegisterChannelMode();
        RegisterPedals();
        RegisterGlobalEq();
    }

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
        var consumed = new HashSet<string>(StringComparer.Ordinal);

        // First pass: multi-byte (INTEGER2x7) params — decode both bytes together.
        foreach (var (addr, state) in _stateFields)
        {
            if (state.Parameter.ByteSize != 2) continue;
            if (!values.TryGetValue(addr, out var b0)) continue;
            var lsbKey = Utilities.AddressToKey(Utilities.AddressOffset(state.Parameter.Address, 1));
            if (!values.TryGetValue(lsbKey, out var b1)) continue;
            SetState(addr, new[] { b0, b1 });
            consumed.Add(addr);
            consumed.Add(lsbKey);
        }

        // Second pass: single-byte params (skip addresses already decoded above).
        foreach (var kvp in values)
            if (!consumed.Contains(kvp.Key))
                SetState(kvp.Key, kvp.Value);
    }

    public void SetState(string key, byte value)
    {
        if (_stateFields.TryGetValue(key, out var state))
        {
            state.SetFromAmp(value);
            _logger.LogDebug("{Name} ({Address}): {Value}", state.Parameter.DisplayName, key, value);
        }
        else if (_multiAddressStates.TryGetValue(key, out var multi))
        {
            multi.SetByte(key, value);
            _logger.LogDebug("MultiAddress ({Address}): {Value}", key, value);
        }
        // _logger.LogDebug("Untracked: {Address}", key);
    }

    public void SetState(string key, byte[] bytes)
    {
        if (_stateFields.TryGetValue(key, out var state))
        {
            state.SetFromAmp(bytes);
            _logger.LogDebug("{Name} ({Address}): [{Bytes}]", state.Parameter.DisplayName, key,
                string.Join(" ", bytes.Select(b => b.ToString("X2"))));
        }
    }

    partial void RegisterPanelMode();
    partial void RegisterChannelMode();
    partial void RegisterPedals();
    partial void RegisterGlobalEq();

    private void RegisterAll(object obj)
    {
        if (obj is AmpControlState direct)
        {
            _stateFields.TryAdd(direct.Parameter.AddressString, direct);
            return;
        }

        if (obj is IMultiAddressState multi)
        {
            foreach (var addr in multi.AddressKeys)
                _multiAddressStates.TryAdd(addr, multi);
            return;
        }

        foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            if (field.GetValue(obj) is AmpControlState state)
                _stateFields.TryAdd(state.Parameter.AddressString, state);
            else if (field.GetValue(obj) is IMultiAddressState multiField)
                foreach (var addr in multiField.AddressKeys)
                    _multiAddressStates.TryAdd(addr, multiField);
            else if (field.FieldType.Namespace?.StartsWith("Kataka") == true
                     && field.GetValue(obj) is { } nested)
                RegisterAll(nested);
    }
}
