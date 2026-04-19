# Global EQ Implementation Plan

## Overview

Add full Global EQ support to Kataka — address map, parameter catalog, state management, and UI. Global EQ lives in the **System memory area** (`0x00000000`), not the Temporary patch area, meaning it persists across all patches and channel switches.

---

## Completed

### Phase 1 — Address Map & Catalog

- [x] **`src/Kataka.Domain/Midi/KatanaAddressMap.cs`** (new)
  - C# mirror of `address_map.js`
  - Base addresses: `System (0x00)`, `Temporary (0x60)`, `HwStatus (0x7F010200)`
  - Block offsets: `SystemBlocks`, `PatchBlocks`
  - Parameter offsets: `GlobalEqParams`
  - `ComputeAddress(base, block, param)` helper — all new parameters use this instead of hardcoded byte arrays

- [x] **`src/Kataka.Domain/Midi/KatanaMkIIParameterCatalog.GlobalEq.cs`** (new partial class)
  - `GlobalEqSelect` — selects active bank (0–2)
  - 25 parameters × 3 banks = **75 parameters** total
  - Each bank: Sw, Type, Position, LowCut, LowGain, LowMidFreq/Q/Gain, HiMidFreq/Q/Gain, HighGain, HighCut, Level, Geq31Hz–Geq16kHz, GeqLevel

### Phase 2 — KatanaState Partial Class Split

- [x] **`KatanaState.cs`** — stripped to core machinery
  - Constructor calls `partial void Register*()` hooks
  - `RegisterAll`, `SetState(s)`, `GetAmpControlsByKey`, `GetAllRegisteredStates`

- [x] **`KatanaState.PanelMode.cs`** — `AmpType`, `Gain`, `Volume`, `Bass`, `Middle`, `Treble`, `Presence`, `AmpVariation`, LEDs, `CabinetResonance`, `PatchLevel`, `SelectedChannel`

- [x] **`KatanaState.ChannelMode.cs`** — `Preamp`, `SoloEq`, `PatchEq1`, `PatchEq2`, `Contour1/2/3`

- [x] **`KatanaState.Pedals.cs`** — `PedalChain`, `BoostPedal`, `ModPedal`, `FxPedal`, `DelayPedal`, `Delay2Pedal`, `ReverbPedal`, `HardwarePedal`, `GetPedalDomainControlsByKey`

- [x] **`KatanaState.GlobalEq.cs`** — stub with commented-out `GlobalEqState` registration

---

## Remaining

### Phase 3 — Global EQ State Class

**New file:** `src/Kataka.App/KatanaState/GlobalEqState.cs`

Model after `PatchEqState.cs`. Holds `AmpControlState` fields for all active-bank parameters plus the bank selector.

```csharp
public class GlobalEqState
{
    public AmpControlState Select = new(KatanaMkIIParameterCatalog.GlobalEqSelect);

    // Bank 1
    public AmpControlState Eq1Sw       = new(KatanaMkIIParameterCatalog.GlobalEq1Sw);
    public AmpControlState Eq1Type     = new(KatanaMkIIParameterCatalog.GlobalEq1Type);
    public AmpControlState Eq1LowCut   = new(KatanaMkIIParameterCatalog.GlobalEq1LowCut);
    public AmpControlState Eq1LowGain  = new(KatanaMkIIParameterCatalog.GlobalEq1LowGain);
    // ... all 25 params × 3 banks
}
```

**Then uncomment in `KatanaState.GlobalEq.cs`:**
```csharp
public GlobalEqState GlobalEq { get; } = new();

partial void RegisterGlobalEq()
{
    RegisterAll(GlobalEq);
}
```

**Update `IKatanaState.cs`:**
```csharp
GlobalEqState GlobalEq { get; }
```

### Phase 4 — Sync Service: System Block Reads

**File:** `src/Kataka.Application/Katana/KatanaSession.cs` (or equivalent sync service)

Global EQ parameters are in the **System** block, not Temporary. The amp likely pushes System block values on connect or they need to be explicitly requested.

- On connect: send RQ1 read requests for `GlobalEqSelect` address and all active-bank params
- Wire `GlobalEq` state fields into the sync service's `GetAllRegisteredStates` dictionary (already handled via `RegisterAll`)
- Determine if the amp pushes DT1 updates for System block changes or if a poll is needed

### Phase 5 — ViewModel

**New file:** `src/Kataka.App/ViewModels/GlobalEqViewModel.cs`

Pattern: inject `IKatanaState`, subscribe to `GlobalEq.*` state `ValueChanged` events, expose properties for binding.

Key behaviors:
- Show parametric EQ controls when `Eq1Type == 0`
- Show GE-10 graphic EQ sliders when `Eq1Type == 1`
- Bank selector (tabs or dropdown) switches which bank's controls are shown
- Writes go to System block — confirm amp accepts DT1 writes to `0x00...` addresses at runtime

### Phase 6 — UI (XAML)

**New file:** `src/Kataka.App/Views/GlobalEqView.axaml`

Suggested layout:
```
[ Bank 1 | Bank 2 | Bank 3 ]   EQ Select tabs
[ ON/OFF ]  [ Type: Parametric / GE-10 ]

-- Parametric mode --
Low Cut | Low Gain | Low-Mid Freq | Low-Mid Q | Low-Mid Gain
Hi-Mid Freq | Hi-Mid Q | Hi-Mid Gain | High Gain | High Cut | Level

-- GE-10 mode --
31 | 62 | 125 | 250 | 500 | 1k | 2k | 4k | 8k | 16k | Level
(vertical sliders, same style as PatchEq GE-10 view)
```

Integrate into main navigation alongside Patch EQ.

---

## Address Reference (spot-check)

| Parameter | Formula | Byte Array |
|-----------|---------|------------|
| GlobalEqSelect | `0x00 + 0x2E + 0x00` | `[0x00, 0x00, 0x00, 0x2E]` |
| GlobalEq1Sw | `0x00 + 0x30 + 0x00` | `[0x00, 0x00, 0x00, 0x30]` |
| GlobalEq1LowGain | `0x00 + 0x30 + 0x04` | `[0x00, 0x00, 0x00, 0x34]` |
| GlobalEq2Sw | `0x00 + 0x50 + 0x00` | `[0x00, 0x00, 0x00, 0x50]` |
| GlobalEq3Sw | `0x00 + 0x70 + 0x00` | `[0x00, 0x00, 0x00, 0x70]` |

---

## Notes

- **System vs Temporary**: System block writes persist globally. Unlike Temporary params, they don't reset on patch change. Verify the amp's SysEx spec allows DT1 writes to `0x00...` addresses.
- **Three EQ banks**: The amp stores 3 independent EQ configurations. `GlobalEqSelect` determines which is active. The UI should show all 3 but only one is "live" at a time.
- **No breaking changes**: All existing 200+ parameters and their consumers are untouched.
- **Test command**: `dotnet test tests/Kataka.Tests/ --filter "FullyQualifiedName!~Integration"`
