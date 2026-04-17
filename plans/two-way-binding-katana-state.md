# Plan: Two-Way Binding via KatanaState — VM Pattern + Write-Back Loop

## Context

`KatanaState` is already the single source of truth: push notifications from the amp update it via `SetState()`, and all `AmpControlState` instances fire `ValueChanged` when their value changes. What is missing is the **write-back path**: when the user moves a control in the UI, nothing currently writes that change to the amp. The old service-aware-of-VMs write path (`IAmpSyncState`, `_context.AmpControls`, etc.) was commented out during the ReactiveUI migration and never replaced.

The goal is a clean bidirectional flow with no circular loop:

```
User moves control → VM setter → AmpControlState.Value = X
  → ValueChanged fires (value changed)
  → AmpSyncService queues write to amp (debounced 125ms)
  → Amp confirms with DT1 → KatanaState.SetState(key, X)
  → AmpControlState.Value = X  ← equality guard returns early (no-op)
  → Loop stops ✓
```

The equality guard in `AmpControlState.Value` setter (line 44: `if (_value == value) return;`) is already in place. Circular updates are impossible by construction.

## ViewModel Pattern (how new components should be written)

This pattern is already established in `AmpControlViewModel.cs` and `BoosterPedalViewModel.cs`. All new components follow the same rules:

```csharp
// 1. Inject IKatanaState; grab the specific state object(s) you need
public class MyPedalViewModel : ViewModelBase
{
    private readonly SomePedalState _state;

    public MyPedalViewModel(IKatanaState katanaState)
    {
        _state = katanaState.SomePedal;

        // 2. Subscribe ValueChanged → RaisePropertyChanged
        _state.SomeParam.ValueChanged += () => this.RaisePropertyChanged(nameof(SomeParam));
    }

    // 3. Property getter reads state; setter writes state (service handles amp write)
    public int SomeParam
    {
        get => _state.SomeParam.Value;
        set => _state.SomeParam.Value = value;
    }
}
```

Rules:
- No MIDI/session knowledge in VMs — they only touch `AmpControlState`
- No `SuppressChangeTracking` flag in VMs — that's gone
- `[Reactive]` (Fody) is fine for VM-only derived properties; don't use it for state-backed properties
- For complex properties (e.g. combobox selection mapped to an int), subscribe `ValueChanged` → update `[Reactive]` property; use `WhenAnyValue` on that property → write to `state.X.Value`

## Files to Modify

### 1. `src/Kataka.App/KatanaState/IKatanaState.cs`
Add one method to expose all registered states:
```csharp
/// <summary>Returns all registered parameter states, keyed by AddressString.</summary>
IReadOnlyDictionary<string, AmpControlState> GetAllRegisteredStates();
```

### 2. `src/Kataka.App/KatanaState/KatanaState.cs`
Implement it (trivial — `_stateFields` is already the right collection):
```csharp
public IReadOnlyDictionary<string, AmpControlState> GetAllRegisteredStates() => _stateFields;
```

### 3. `src/Kataka.App/Services/AmpSyncService.cs`

#### 3a. Add `_suppressWrites` flag
```csharp
private bool _suppressWrites;
```

#### 3b. Replace `_pendingWrites` type to carry address bytes
Change existing `Dictionary<string, byte>` to:
```csharp
private readonly Dictionary<string, (IReadOnlyList<byte> Address, byte Value)> _pendingWrites = [];
```
Key is still the address string (natural dedup — rapid drags to same param keep only the latest value).

#### 3c. Add `SubscribeToStateWrites()` — called once after connect+seed
```csharp
private void SubscribeToStateWrites()
{
    foreach (var (_, state) in _katanaState.GetAllRegisteredStates())
    {
        var captured = state;
        captured.ValueChanged += () => OnDomainStateChanged(captured);
    }
}

private void OnDomainStateChanged(AmpControlState state)
{
    if (_suppressWrites || !_session.IsConnected) return;
    _pendingWrites[state.Parameter.AddressString] = (state.Parameter.Address, (byte)state.Value);
    UpdateWriteSyncTimer(); // existing debounce timer (125ms)
}
```

#### 3d. Update connect flow to seed state and subscribe
In the method called on connect (currently `TryReadAmpControlsAsync` or its caller):
```csharp
_suppressWrites = true;
try
{
    var allStates = await _session.ReadAllPatchStatesAsync(cancellationToken);
    _katanaState.SetStates(allStates);
}
finally
{
    _suppressWrites = false;
}
SubscribeToStateWrites();
```

#### 3e. Re-enable flush using `WriteBlockAsync`
Replace the commented-out flush body with:
```csharp
var snapshot = _pendingWrites.ToList();
_pendingWrites.Clear();
foreach (var (_, (address, value)) in snapshot)
    await _session.WriteBlockAsync(address, [value]);
```

## Key Facts

- `AmpControlState.Value` setter line 44 has `if (_value == value) return;` — circular loop is impossible
- `_writeSyncTimer` with `WriteSyncDebounce = 125ms` already exists in `AmpSyncService` — reuse it
- `ReadAllPatchStatesAsync()` was added to `IKatanaSession` / `KatanaSession` (returns `Dictionary<string, byte>` keyed by address string)
- All `AmpControlState` instances are already registered in `_stateFields` (reflection-based `RegisterAll` is complete)
- `IAmpSyncState` interface and the old `_context`/`_ampControlsByKey` fields can be deleted once the new write path is wired — optional cleanup, can happen separately

## Verification

1. `dotnet build src/Kataka.App/Kataka.App.csproj` — 0 errors
2. Connect to amp → diagnostics show no `LogWarning("Received update for unknown parameter key")`
3. Move the Volume knob on the physical amp → UI control updates
4. Drag the Volume control in the UI → physical amp value changes, log shows write
5. Rapid drag in UI → only one write fires per 125ms burst (debounce works)
6. No infinite write loop in logs after moving either the physical knob or the UI control
