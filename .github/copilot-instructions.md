# Kataka – Copilot Instructions

This file documents the **established patterns** that every new feature must follow.
When in doubt, look at `BoostPedalState` / `PanelViewModel` / `AmpControlViewModel` as the ground-truth references.


# Kataka — Architecture & Development Guidelines

## Project overview

Kataka is an Avalonia desktop app for controlling a Boss Katana 50 MkII guitar amplifier over MIDI SysEx. The amp is the source of truth for hardware state; the app mirrors that state in `KatanaState` and writes changes back. Patches are stored as `.tsl` files (Boss Tone Studio format).

## Commands

```bash
dotnet build --no-incremental -v q           # build
dotnet run --project src/Kataka.App/Kataka.App.csproj   # run
dotnet watch run --project src/Kataka.App/Kataka.App.csproj  # hot reload
dotnet test --filter "FullyQualifiedName!~Integration"  # test (exclude ALSA hardware tests)
dotnet format                                # enforce .editorconfig rules (run after every C#/AXAML change)
```

Integration tests require a physical MIDI device and will hang without one — always use the filter.

## Solution layout

```
src/
  Kataka.Domain/          # Pure domain: parameter catalog, enums, KatanaParameterDefinition
  Kataka.Infrastructure/  # MIDI transport implementation (ALSA on Linux)
  Kataka.App/             # Avalonia UI: KatanaState, ViewModels, Views, Services
tests/
```

---

## Data flow

```
Amp (SysEx/MIDI)
  ↕  IKatanaSession (read/write/push notifications)
  ↕  AmpSyncService  ← the only place that touches the session
  ↕  KatanaState (IKatanaState)   ← shared domain state
  ↕  ViewModels  ← read state / write through AmpControlViewModel
  ↕  Views (Avalonia AXAML)
```

- **Amp → UI**: `IKatanaSession` push/read → `KatanaState.SetState(key, value)` → `AmpControlState.SetFromAmp(v)` → `ValueChanged` → `AmpControlViewModel` raises `PropertyChanged` → binding refreshes.
- **UI → Amp**: UI sets `AmpControlViewModel.Value = v` → `AmpControlState.Value = v` → `WriteRequested` → `AmpSyncService` queues the write → `IKatanaSession.WriteBlockAsync`.

---

## Dependency injection

`Microsoft.Extensions.DependencyInjection` wired through `CommunityToolkit.Mvvm.DependencyInjection.Ioc`.

Only infrastructure/service-level types are registered:

```csharp
services.AddSingleton<IMidiTransport>(...);
services.AddSingleton<IKatanaSession, KatanaSession>();
services.AddSingleton<IKatanaState, KatanaState>();
services.AddSingleton<IAmpSyncService, AmpSyncService>();
services.AddSingleton<MainWindowViewModel>();
```

ViewModels below `MainWindowViewModel` are created manually via constructor arguments — do not register them in the container. Pass dependencies down the constructor chain.

---

## KatanaState — the single source of truth

`IKatanaState` / `KatanaState` holds the in-memory mirror of every amp parameter as `AmpControlState` objects.

### AmpControlState

Each `AmpControlState` wraps one SysEx parameter:

```csharp
state.Value = 42;          // user action — fires ValueChanged AND WriteRequested
state.SetFromAmp(42);      // amp read — fires ValueChanged ONLY
```

**Never call `.Value = x` from an amp read path.** Use `SetFromAmp()` for values received from the amp. The `if (_value == value) return;` guard in the `Value` setter prevents circular loops.

### State classes — field conventions

`AmpControlState` members inside a state class **must be public fields, not properties**. Nested state objects that contain `AmpControlState` fields must also be public fields. Both rules apply so `KatanaState.RegisterAll()` can discover them via reflection (`GetFields()` only).

```csharp
// ✅ Correct
public AmpControlState Drive = new(KatanaMkIIParameterCatalog.BoosterDrive);
public EqBankState Bank1 = new();

// ❌ Wrong — RegisterAll will not find these
public AmpControlState Drive { get; } = new(...);
public EqBankState Bank1 { get; } = new();
```

### KatanaState partial-class layout

Each feature area gets its own `KatanaState.<Feature>.cs` partial file:

```csharp
// KatanaState.GlobalEq.cs
public partial class KatanaState
{
    public GlobalEqState GlobalEq { get; } = new();

    partial void RegisterGlobalEq()
    {
        RegisterAll(GlobalEq);
    }
}
```

The main `KatanaState.cs` calls `Register<Feature>()` from the constructor — that file is the only place to add new calls. Every new feature must also be added to `IKatanaState` as a read-only property.

### RegisterAll — how state registration works

`RegisterAll(obj)` walks the object tree via reflection on **public instance fields only** and registers every `AmpControlState` found into `_stateFields` (keyed by `AddressString`). Nested objects whose type's namespace starts with `"Kataka"` are recursed into.

---

## Parameter catalog

Parameters in `KatanaMkIIParameterCatalog` are **static properties** (not fields):

```csharp
public static KatanaParameterDefinition BoosterDrive { get; } =
    new("booster-drive", "Drive", KatanaAddressMap.ComputeAddress(...), maximum: 120);
```

Use `GetProperties(BindingFlags.Public | BindingFlags.Static)` when collecting definitions via reflection — `GetFields()` returns 0 results. Each file is a `static partial class`; group related parameters into one file per feature area.

### Address map

Use `KatanaAddressMap.ComputeAddress` — never hardcode raw byte arrays:

```csharp
KatanaAddressMap.ComputeAddress(
    KatanaAddressMap.System,
    KatanaAddressMap.SystemBlocks.GlobalEq1,
    KatanaAddressMap.GlobalEqParams.LowGain
)
```

---

## MVVM patterns

### AmpControlViewModel — standard adapter

`AmpControlViewModel` is the standard bridge between `AmpControlState` and the UI. Create one per parameter and expose it as a property; do not duplicate the value in a `[Reactive]` field on the VM:

```csharp
public class PanelViewModel : ViewModelBase
{
    public AmpControlViewModel Gain   { get; }
    public AmpControlViewModel Volume { get; }

    public PanelViewModel(IKatanaState katanaState)
    {
        Gain   = new AmpControlViewModel(katanaState.Gain);
        Volume = new AmpControlViewModel(katanaState.Volume);
    }
}
```

AXAML binds directly to `{Binding Gain.Value}`. No manual `ValueChanged` subscription needed. **No `WhenAnyValue` write-backs** — the write path is the `Value` setter alone.

### Command / event-driven VMs

For actions that go through `IAmpSyncService` (channel selection, global operations):

- **Inbound** (amp → UI): subscribe to `IKatanaState` events and update VM state in the handler.
- **Outbound** (UI → amp): use `[RelayCommand]` calling `_ampSyncService.SomeActionAsync(...)`. Never write directly to `IKatanaSession`.

```csharp
_katanaState.SelectedChannelChanged += UpdatePanelChannelSelection;

[RelayCommand]
private async Task SelectPanelChannel(string? channel)
    => await _ampSyncService.SelectChannelAsync(channelEnum);
```

### ViewModelBase

All ViewModels extend `ViewModelBase` (which extends `ReactiveObject`).

- Use `[Reactive]` (ReactiveUI.Fody) for simple observable properties.
- Use `[ObservableProperty]` (CommunityToolkit.Mvvm) where partial classes are preferred.

### ViewModel construction hierarchy

```
MainWindowViewModel
  ├── AmpEditorViewModel(IKatanaState, IAmpSyncService)
  │     ├── PedalboardViewModel(IKatanaState, pedalsByKey)
  │     ├── PanelViewModel(IKatanaState)
  │     └── [Pedal ViewModels: BoosterPedalViewModel, ModFxPedalViewModel, ...]
  ├── PedalboardMiniMapViewModel(IKatanaState)
  ├── GlobalEqViewModel(IKatanaState)
  └── ...
```

Each effect slot has a single `PedalViewModel` instance shared between `AmpEditorViewModel.PanelEffects` and `PedalboardViewModel._pedalsByKey`.

---

## AmpSyncService responsibilities

| Responsibility | How |
|---|---|
| Seed state on connect | `ReadAllPatchStatesAsync()` → `SetStates()` |
| Seed system-level params (e.g. Global EQ) | `ReadParametersAsync(defs)` with `GetProperties()` reflection on catalog |
| Route inbound push notifications | `OnAmpPushNotification` → `SetState(key, byte)` |
| Route outbound writes | Subscribe to `WriteRequested` on all `GetAllRegisteredStates()` entries; queue to write channel |
| Rate-limit writes | `PeriodicTimer(20 ms)` write loop; one write per tick |

`SubscribeToStateWrites()` is called **after** the seed read and is guarded by `_stateWritesSubscribed` to prevent double-subscription across reconnects.

---

## Chain / pedalboard ordering

Chains are complete ordered arrays of slot keys. Each covers the full signal path from `"input"` to `"output"`, with `"amp"` placed at the chain-specific position:

```csharp
private static readonly List<string[]> Chains =
[
    ["input", "booster", "amp", "mod", "fx", "delay", "delay2", "reverb", "output"],
    ["input", "booster", "mod", "amp", "fx", "delay", "delay2", "reverb", "output"],
    // ...
];
```

`PedalboardViewModel.Refresh()` iterates the selected chain and builds `PedalboardItems` from it. `PedalboardMiniMapViewModel` follows the same pattern using `ChainNode[]` arrays.

---

## Avalonia view patterns

### Bindings

- Views use `x:CompileBindings="False"` for runtime binding flexibility (avoids `x:DataType` conflicts in nested templates).

### IDataTemplate selectors

Set the selector **directly** as `ItemsControl.ItemTemplate` — do not wrap it in an outer `DataTemplate` + `ContentControl` (breaks per-item DataContext propagation):

```xml
<ItemsControl.ItemTemplate>
    <vm:PedalboardItemTypeSelector>
        <vm:PedalboardItemTypeSelector.InputTemplate>
            <DataTemplate x:CompileBindings="False"> ... </DataTemplate>
        </vm:PedalboardItemTypeSelector.InputTemplate>
    </vm:PedalboardItemTypeSelector>
</ItemsControl.ItemTemplate>
```

### Pedal card background color by type

Each pedal view binds its card background to `CardBackgroundBrush` on `PedalViewModel`. The base class returns a neutral dark gradient (`#2E3138`→`#1C1F24`). To map colors to a specific pedal's types:

**1. Create a `<Pedal>Colors.cs` in the pedal's component folder** — define one `static readonly IBrush` per color category and a `GetBackgroundBrush(string? typeName)` switch. Use the `Gradient(top, bottom)` helper pattern:

```csharp
// src/Kataka.App/Components/BoosterPedal/BoosterPedalColors.cs
public static class BoosterPedalColors
{
    public static readonly IBrush Overdrive = Gradient("#1e3222", "#111e14");
    // ...

    public static IBrush GetBackgroundBrush(string? typeName) => typeName switch
    {
        "T-SCREAM" or "OVERDRIVE" => Overdrive,
        // ...
        _ => PedalViewModel.DefaultCardBackground,
    };

    private static IBrush Gradient(string top, string bottom) =>
        new LinearGradientBrush { ... };
}
```

**2. Override `CardBackgroundBrush` in the pedal ViewModel:**

```csharp
public override IBrush CardBackgroundBrush =>
    BoosterPedalColors.GetBackgroundBrush(SelectedTypeOption);
```

**3. Raise `CardBackgroundBrush` when type changes** (in the `_typeState.ValueChanged` handler):

```csharp
this.RaisePropertyChanged(nameof(CardBackgroundBrush));
```

**4. All pedal views already bind** `Background="{Binding CardBackgroundBrush}"` — no view changes needed.

See `src/Kataka.App/Components/BoosterPedal/BoosterPedalColors.cs` as the reference implementation.

---

### Design ViewModels

Complex views have a corresponding design VM in `ViewModels/Design/`. Design VMs extend the real VM, seed test values, and expose a static `Instance`:

```csharp
public sealed class DesignBoosterPedalViewModel : BoosterPedalViewModel
{
    public DesignBoosterPedalViewModel() : base(new KatanaState(NullLogger<KatanaState>.Instance)) { ... }
    public static DesignBoosterPedalViewModel Instance => new();
}
```

Reference via `d:DataContext="{x:Static dvm:DesignFooViewModel.Instance}"`.

---

## Code style

- 4-space indent for C# and AXAML; 2-space for XML/JSON/csproj.
- No comments unless the WHY is non-obvious (hidden constraint, workaround, subtle invariant).
- No XML doc blocks except on public API surface in `Kataka.Domain`.

---

## Tooling — Rider MCP

Prefer **Rider MCP tools** over the generic `Read`/`Bash` tools when exploring or modifying the codebase. Always pass `projectPath: "/home/luismaglz/Github/katana50-mkII-linux"` to all Rider MCP calls.

| Tool | When to use |
|---|---|
| `mcp__rider__find_files_by_name_keyword` | Locate a file when you know part of its name |
| `mcp__rider__find_files_by_glob` | Locate files by glob pattern (e.g. `src/**/*.axaml`) |
| `mcp__rider__list_directory_tree` | Browse directory structure |
| `mcp__rider__get_symbol_info` | Get declaration, type, and docs for a symbol at a file:line:col |
| `mcp__rider__search_in_files_by_text` | Full-text search across the solution |
| `mcp__rider__search_in_files_by_regex` | Regex search across the solution |
| `mcp__rider__get_file_text_by_path` | Read a file by project-relative path |
| `mcp__rider__replace_text_in_file` | Targeted find-and-replace (preferred over full rewrites) |
| `mcp__rider__create_new_file` | Create a new file with optional initial content |
| `mcp__rider__reformat_file` | Apply IDE formatting rules (alternative to `dotnet format`) |
| `mcp__rider__rename_refactoring` | Rename a symbol and update all references project-wide |
| `mcp__rider__get_file_problems` | Check a file for errors/warnings |
| `mcp__rider__execute_run_configuration` | Run a specific build/test/app configuration |
| `mcp__rider__execute_terminal_command` | Run a shell command in the IDE terminal |

---

## Anti-patterns to avoid

| Anti-pattern | Correct approach |
|---|---|
| `state.Value = x` from an amp read path | Use `state.SetFromAmp(x)` |
| `[Reactive] int Foo` + `WhenAnyValue(...).Subscribe(v => state.Foo.Value = v)` | Wrap in `AmpControlViewModel`; `WhenAnyValue` fires on subscribe and bypasses `SetFromAmp` guarantee |
| `AmpControlState` or nested state sub-object declared as `{ get; }` property | Use public fields — `RegisterAll` uses `GetFields()` only |
| `GetFields()` on `KatanaMkIIParameterCatalog` | Use `GetProperties(Public | Static)` — catalog entries are static properties |
| Writing to `IKatanaSession` directly from a ViewModel | All writes go through `AmpControlState.Value` → `WriteRequested` → `AmpSyncService` |
| Registering pedal VMs in the DI container | Construct manually in `AmpEditorViewModel` |
| Wrapping `IDataTemplate` selector in `ContentControl` | Set selector directly on `ItemsControl.ItemTemplate` |
| Defining split before/after-amp chain arrays | Define each chain as a single full-path array |
