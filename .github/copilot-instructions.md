# Kataka – Copilot Instructions

This file documents the **established patterns** that every new feature must follow.
When in doubt, look at `BoostPedalState` / `PanelViewModel` / `AmpControlViewModel` as the ground-truth references.

---

## Build & test

```bash
dotnet build --no-incremental -v q   # build
dotnet test -v q                     # run tests
```

Both commands run from the repo root (`katana50-mkII-linux/`).

---

## Architecture overview

```
Amp (SysEx/MIDI)
  ↕  IKatanaSession (read/write/push notifications)
  ↕  AmpSyncService  ← the only place that touches the session
  ↕  KatanaState (IKatanaState)   ← shared domain state
  ↕  ViewModels  ← read state / write through AmpControlViewModel
  ↕  Views (Avalonia AXAML)
```

Data flows in **one direction** per path:
- **Amp → UI**: `IKatanaSession` push/read → `KatanaState.SetState(key, value)` → `AmpControlState.SetFromAmp(v)` → `ValueChanged` event → `AmpControlViewModel` raises `PropertyChanged` → UI binding refreshes.
- **UI → Amp**: UI sets `AmpControlViewModel.Value = v` → `AmpControlState.Value = v` → `WriteRequested` event → `AmpSyncService` queues the write → `IKatanaSession.WriteBlockAsync`.

---

## AmpControlState – the core primitive

```csharp
// Read from amp (no write-back):
state.SetFromAmp(value);   // fires ValueChanged only
// User/UI edit (triggers amp write):
state.Value = value;       // fires ValueChanged + WriteRequested
```

**Key rule:** `SetFromAmp` fires **only** `ValueChanged`; `WriteRequested` is **never** fired on inbound data.
The guard `if (_value == value) return;` in the `Value` setter prevents all circular loops.

---

## State classes – field conventions

`AmpControlState` members inside a state class **must be public fields, not properties**:

```csharp
// ✅ Correct – RegisterAll finds it via GetFields()
public AmpControlState Drive = new(KatanaMkIIParameterCatalog.BoosterDrive);

// ❌ Wrong – RegisterAll will NOT recurse into this
public AmpControlState Drive { get; } = new(...);
```

Nested state objects that contain `AmpControlState` fields **must also be public fields**, not properties, so `RegisterAll` can recurse into them:

```csharp
// ✅ Correct – RegisterAll recurses into this
public EqBankState Bank1 = new();

// ❌ Wrong – RegisterAll will NOT recurse into this
public EqBankState Bank1 { get; } = new();
```

Reference: `BoostPedalState.cs`, `PatchEqState.cs`, `SoloEqState.cs`.

---

## RegisterAll – how state registration works

`KatanaState.RegisterAll(object obj)` walks an object tree via **reflection on public instance fields only** and registers every `AmpControlState` it finds into `_stateFields` (keyed by `AddressString`).

- If `obj` is itself an `AmpControlState`, it is registered directly.
- Otherwise, every field whose value is an `AmpControlState` is registered.
- Nested objects whose type's namespace starts with `"Kataka"` are recursed into.

```csharp
// How to register a flat state object:
RegisterAll(BoostPedal);      // recurses into all AmpControlState fields

// How to register a single AmpControlState (defined as a property on KatanaState):
RegisterAll(AmpType);         // registers just this one state
```

Reference: `KatanaState.cs → RegisterAll`, `KatanaState.Pedals.cs`, `KatanaState.PanelMode.cs`.

---

## KatanaState partial-class layout

Each feature area gets its own `KatanaState.<Feature>.cs` partial file that declares:
1. The state object or individual `AmpControlState` properties.
2. A `partial void Register<Feature>()` implementation that calls `RegisterAll(...)` for each state.

The main `KatanaState.cs` calls `Register<Feature>()` from the constructor and that file is the only place to add new calls.

```csharp
// KatanaState.GlobalEq.cs
public partial class KatanaState
{
    public GlobalEqState GlobalEq { get; } = new();

    partial void RegisterGlobalEq()
    {
        RegisterAll(GlobalEq);  // recurses into all fields of GlobalEqState
    }
}
```

The matching feature must also be added to `IKatanaState` as a read-only property.

---

## Parameter catalog – KatanaMkIIParameterCatalog

Parameters are **static properties** (not fields):

```csharp
public static KatanaParameterDefinition BoosterDrive { get; } =
    new("booster-drive", "Drive", KatanaAddressMap.ComputeAddress(...), maximum: 120);
```

- Use `GetProperties(BindingFlags.Public | BindingFlags.Static)` (not `GetFields`) when collecting definitions via reflection.
- Each file is a `static partial class`; group related parameters into one file per feature area.

---

## Address map – KatanaAddressMap

Use the `ComputeAddress` helper to compute byte addresses from the symbolic constants:

```csharp
KatanaAddressMap.ComputeAddress(
    KatanaAddressMap.System,              // base area
    KatanaAddressMap.SystemBlocks.GlobalEq1,   // block offset
    KatanaAddressMap.GlobalEqParams.LowGain    // parameter offset within block
)
```

Never hardcode raw byte arrays inline in catalog definitions.

---

## ViewModel pattern – AmpControlViewModel

`AmpControlViewModel` is the standard adapter between `AmpControlState` and the UI:

```csharp
public class AmpControlViewModel : ViewModelBase
{
    private readonly AmpControlState _state;

    public AmpControlViewModel(AmpControlState state)
    {
        _state = state;
        // Inbound: amp change → raise PropertyChanged so UI refreshes
        _state.ValueChanged += () => this.RaisePropertyChanged(nameof(Value));
    }

    public int Value
    {
        get => _state.Value;
        // Outbound: UI edit → write to state → WriteRequested fires → amp write queued
        set => _state.Value = Math.Clamp(value, _state.Minimum, _state.Maximum);
    }
}
```

**No `WhenAnyValue` write-backs.** The write path is handled by the `Value` setter alone.
The `if (_value == value) return;` guard in `AmpControlState` prevents circular re-fires.

---

## ViewModel pattern – feature VMs (PanelViewModel)

Create one `AmpControlViewModel` per parameter and expose it as a property. Do **not** duplicate the value in a `[Reactive]` field on the VM:

```csharp
public class PanelViewModel : ViewModelBase
{
    public AmpControlViewModel Gain { get; }
    public AmpControlViewModel Volume { get; }

    public PanelViewModel(IKatanaState katanaState)
    {
        Gain   = new AmpControlViewModel(katanaState.Gain);
        Volume = new AmpControlViewModel(katanaState.Volume);
    }
}
```

The AXAML binds directly to `{Binding Gain.Value}`. No manual subscription needed.

---

## ViewModel pattern – command / event-driven VMs (ChannelSelectionViewModel)

For actions that go through `IAmpSyncService` (channel selection, global operations):

- **Inbound** (amp → UI): subscribe to `IKatanaState` events (e.g. `SelectedChannelChanged`) and update VM state in the handler.
- **Outbound** (UI → amp): use `[RelayCommand]` or a `Task`-returning method that calls `_ampSyncService.SomeActionAsync(...)`. Never write directly to the session.

```csharp
// Inbound
_katanaState.SelectedChannelChanged += UpdatePanelChannelSelection;

// Outbound
[RelayCommand]
private async Task SelectPanelChannel(string? channel)
    => await _ampSyncService.SelectChannelAsync(channelEnum);
```

---

## AmpSyncService responsibilities

| Responsibility | How |
|---|---|
| Seed state on connect | `ReadAllPatchStatesAsync()` → `SetStates()` |
| Seed system-level params (e.g. Global EQ) | explicit `ReadParametersAsync(defs)` with `GetProperties()` reflection on catalog |
| Route inbound push notifications | `OnAmpPushNotification` → `SetState(key, byte)` |
| Route outbound writes | Subscribe to `WriteRequested` on all `GetAllRegisteredStates()` entries; queue to write channel |
| Rate-limit writes | `PeriodicTimer(20 ms)` write loop; one write per tick |

`SubscribeToStateWrites()` is called **after** the seed read, and is guarded by `_stateWritesSubscribed` to prevent double-subscription across reconnects.

---

## Anti-patterns to avoid

| Anti-pattern | Why bad | Use instead |
|---|---|---|
| `[Reactive] int Foo` + `WhenAnyValue(x => x.Foo).Subscribe(v => state.Foo.Value = v)` | `WhenAnyValue` fires immediately on subscribe, causing writes during init; also fires on every `UpdateFromAmp()` call, bypassing the no-writeback guarantee of `SetFromAmp` | `AmpControlViewModel` wrapping `AmpControlState` |
| `AmpControlState` member declared as `{ get; }` property inside a nested state class | `RegisterAll` uses `GetFields()` only; properties are silently skipped | Use a public field: `public AmpControlState Foo = new(...)` |
| Nested state sub-object declared as `{ get; }` property | `RegisterAll` won't recurse into it | Declare as public field: `public SubState Sub = new()` |
| `GetFields()` on `KatanaMkIIParameterCatalog` | Catalog entries are static properties, `GetFields` returns 0 results | Use `GetProperties(Public \| Static)` |
| Writing to `IKatanaSession` directly from a ViewModel | Bypasses the rate-limiter and write loop | All writes go through `AmpControlState.Value` → `WriteRequested` → `AmpSyncService` |
