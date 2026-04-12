# Kataka — Technical Notes & Architecture Reference

> Living document. Updated as the rewrite progresses.

---

## Table of Contents

1. [Project Goal](#project-goal)
2. [Architecture Overview](#architecture-overview)
3. [Verified Katana MK2 Address Map](#verified-katana-mk2-address-map)
4. [MIDI Transport Strategy](#midi-transport-strategy)
5. [Read Performance Analysis](#read-performance-analysis)
6. [Protocol Details](#protocol-details)
7. [Write / Read Bounce Prevention](#write--read-bounce-prevention)
8. [Active Sync Model](#active-sync-model)
9. [UI Architecture & Design Decisions](#ui-architecture--design-decisions)
10. [Pedalboard UI](#pedalboard-ui)
11. [Future Phases](#future-phases)
12. [Reference Sources](#reference-sources)

---

## Project Goal

Produce a clean C# reimplementation of the Katana FloorBoard-style amp editor, keeping Linux and cross-platform support first-class while replacing the old Qt/C++ codebase with a layered Avalonia UI architecture.

**Status:** The critical unknown (reliable Katana SysEx on Linux) is proven on real hardware. The transport, checksum logic, addressing model, and request/reply workflow are all grounded in real hardware verification.

**Platform priority:** Linux-first until the editor core is solid; macOS/Windows second via backend abstraction.

---

## Architecture Overview

```
┌─────────────────────────────────┐
│  Avalonia UI (AXAML + MVVM)     │  ← MainWindow, custom controls
│  ViewModels (ObservableObject)  │  ← emit change intents only
├─────────────────────────────────┤
│  Application Services           │  ← coalescing write queue,
│  KatanaSession                  │    grouped refresh scheduler
├─────────────────────────────────┤
│  Infrastructure                 │  ← AmidiConnection (Linux)
│  MIDI Transport                 │    abstracted for cross-platform
└─────────────────────────────────┘
```

**Key decisions:**
- Keep Avalonia UI.
- Keep the layered architecture (UI emits intents, services own coalescing/flush, transport stays dumb).
- Keep Linux as the reference platform; preserve backend abstraction for future cross-platform work.
- Keep `amidi` as the active Linux transport.
- Do not attempt pixel-for-pixel recreation of the old UI — stylized front-panel direction.

---

## Verified Katana MK2 Address Map

All addresses below are **live state** (not patch-space), verified against the Katana MK2 source tree.

### Amp Knob Block — `KNOB_POS_*`

| Control   | Address         |
|-----------|-----------------|
| Gain      | `60 00 06 51`   |
| Volume    | `60 00 06 52`   |
| Bass      | `60 00 06 53`   |
| Middle    | `60 00 06 54`   |
| Treble    | `60 00 06 55`   |
| Presence  | `60 00 06 56`   |

These sit in one contiguous block → one SysEx round-trip reads all six.

### Panel Effect Switches

| Effect    | Address         |
|-----------|-----------------|
| Booster   | `60 00 00 10`   |
| Mod       | `60 00 01 00`   |
| FX        | `60 00 03 00`   |
| Delay     | `60 00 05 00`   |
| Delay 2   | `60 00 05 20`   |
| Reverb    | `60 00 05 40`   |

### Panel Effect Type Selectors

| Effect    | Address         |
|-----------|-----------------|
| Booster   | `60 00 00 11`   |
| Mod       | `60 00 01 01`   |
| FX        | `60 00 03 01`   |
| Delay     | `60 00 05 01`   |
| Delay 2   | `60 00 05 21`   |
| Reverb    | `60 00 05 41`   |

### Panel Variation Colors — `FXBOX_SEL_*`

| Effect    | Address         |
|-----------|-----------------|
| Booster   | `60 00 06 39`   |
| Mod       | `60 00 06 3A`   |
| FX        | `60 00 06 3B`   |
| Delay     | `60 00 06 3C`   |
| Reverb    | `60 00 06 3D`   |

> ⚠️ **Delay 2 has no variation color byte** in the confirmed MKII source. Do not borrow Reverb's byte for it.

### Delay

| Parameter  | Address         |
|------------|-----------------|
| Delay Time | `60 00 05 02`   |

> ⚠️ `60 00 05 62` is **not** delay time on MKII live state — it maps to send/return. Old GT-8 assumption was wrong.

### Current Channel

| Parameter       | Address         |
|-----------------|-----------------|
| Channel request | `00 01 00 00`   |

### Pedal FX Block

| Parameter              | Address         |
|------------------------|-----------------|
| Pedal FX switch        | `60 00 05 50`   |
| Pedal FX type          | `60 00 05 51`   |
| Pedal FX chain position| `60 00 06 23`   |
| Wah type               | `60 00 05 52`   |
| Wah position           | `60 00 05 53`   |
| Wah min                | `60 00 05 54`   |
| Wah max                | `60 00 05 55`   |
| Wah effect level       | `60 00 05 56`   |
| Wah direct mix         | `60 00 05 57`   |
| Foot volume            | `60 00 05 61`   |

Pedal FX type values:
- `0` = Wah
- `1` = Pedal Bend
- `2` = EVH WAH95

Pedal FX chain position values:
- `Input`
- `Post Amp`

### Patch Level

Currently **disabled** in the rewrite. The address `60 00 06 4C` is not yet source-backed for the live path. Re-enable only after confirmed against the MKII source and hardware behavior.

---

## MIDI Transport Strategy

Research compared all three codebases: GT-8 FloorBoard (C++/Qt), Boss Tone Studio (JS), and our C# app.

### GT-8 FloorBoard (C++/Qt) — write-immediately with busy spooler

- Global singleton `SysxIO` holds the entire patch as raw SysEx hex strings in memory.
- Every UI change mutates the hex buffer, recalculates checksum, and either sends immediately (`deviceReady`) or appends to a `sendSpooler` list.
- `resetDevice()` fires on each reply, drains the spooler and deduplicates consecutive writes to the same address prefix (last write wins at prefix level).
- No async — Qt signals/slots on main thread only. No timeout detection. No byte-rate pacing.
- **Verdict:** Simplest approach, most brittle — state is opaque hex bytes, hard to extend.

### Boss Tone Studio (JS) — byte-rate-paced FIFO with task queue

- `MIDIController` is a global FIFO drained by a `setInterval(10ms)` timer.
- `dt1(addr, data)` enqueues a write; `rq1(addr, size, callback)` enqueues a read with a reply callback matched by address.
- **Byte-rate pacing**: each message is sent only when `now > t0`; `t0 = now + (msgBytes / 3125 bytes/sec) + productInterval`. This models actual MIDI wire speed so the device is never flooded. Boss makes the hardware — they chose the right values.
- `ReadWriteLogic.readStart(tasks)` works through `{addr, size}` task arrays one at a time with a per-task timeout watchdog — aborts cleanly if no reply in N seconds.
- No write coalescing — rapid UI changes queue every intermediate value (pacing throttles delivery naturally).
- **Verdict:** Most disciplined transport; pacing + timeout watchdog are the standout features worth borrowing.

### Our C# App (Avalonia/MVVM) — debounce + coalesce + semaphore gate

- `pending*Writes` dictionaries collect changes per parameter key (last-write-wins, so fast knob spins produce one SysEx not many).
- `syncOperationGate` (SemaphoreSlim 1,1) prevents overlapping flush operations.
- `suppressChangeTracking` prevents device-read values from bouncing back as writes.
- `FlushPendingWritesAsync` snapshots all dicts atomically, clears originals, awaits each write; failures are re-merged.
- `ActiveReadSync` / `ActiveWriteSync` are user-toggleable.
- No byte-rate pacing (delegated to underlying MIDI library). No per-read timeout watchdog yet.
- **Verdict:** Best write efficiency and cleanest architecture. Missing BTS's pacing and timeout discipline.

### Head-to-Head Comparison

| Aspect                  | GT-8 FloorBoard (C++) | BTS (JS)                     | Ours (C#)                      |
|-------------------------|-----------------------|------------------------------|--------------------------------|
| Write coalescing        | Crude (prefix only)   | None                         | Full (last-write-wins per key) |
| Byte-rate pacing        | None                  | Yes — 3125 B/s explicit      | Delegated to library           |
| Read timeout watchdog   | None                  | Yes — per-task               | Not yet implemented            |
| Thread safety           | Qt main thread only   | JS single-threaded           | SemaphoreSlim gate             |
| Echo/bounce prevention  | None                  | Parameter model handles it   | suppressChangeTracking         |
| State representation    | Raw hex bytes         | Parameter objects            | Typed ViewModel properties     |
| Extensibility           | Poor                  | Medium (address_map config)  | Typed catalog + definitions    |
| Async                   | No (Qt signals)       | setInterval/callback         | async/await                    |

### Decision: Implement BTS-Style Byte-Rate Pacing

- BTS paces at `(msgBytes / 3125 bytes/sec) + productInterval` between sends.
- **3125 bytes/sec = MIDI 1.0 serial bandwidth** (31250 baud, 8N1 = 3.125 kB/s).
- Boss chose this formula for the hardware they built — it is the correct floor. **We defer to them here.**
- Implementation target: add a send queue + pacing timer to `KatanaSession` (or a new `MidiTransport` class) so all `WriteBlockAsync` and `ReadBlockAsync` calls flow through a paced FIFO rather than firing immediately.
- This also naturally serializes requests, making it straightforward to add read timeout watchdogs as a follow-on.

---

## Read Performance Analysis

### Why amp controls read faster than effects/variations

`KatanaSession.CreateReadGroups()` batches reads only when parameters share the same first three address bytes **and** fit inside an 8-byte span from the first offset:

- **Amp knobs:** `60 00 06 51-56` — all six fit in one grouped read. ~1 round-trip.
- **Panel effects:** spread across `60 00 00`, `60 00 01`, `60 00 03`, `60 00 05` — cannot batch together. ~8-9 round-trips.

### Current panel read breakdown

| Request                     | Round-trips |
|-----------------------------|-------------|
| Current channel             | 1           |
| Effect switches (×6)        | 6           |
| Variation colors (grouped)  | 1           |
| Delay time                  | 1           |
| **Total**                   | **~9**      |

vs **~2** for amp controls.

### Parallel requests: NOT safe with current transport

`AmidiConnection.RequestAsync()` launches one `amidi` request, waits for it to finish, reads the captured reply from a temp file, and returns the last SysEx frame. There is no request correlation layer. Concurrent reads on the same port would corrupt each other's replies.

**Rule: one in-flight SysEx request at a time per connection.**

### Recommended refresh scheduler shape

1. Amp knob group (cheapest, ~1 request)
2. Variation color group (~1 request)
3. Current channel (~1 request)
4. Effect switches (~6 requests)
5. Delay time (only when delay is active)

### Optimization options

| Option | Benefit | Risk |
|--------|---------|------|
| Reorder request groups | Better perceived responsiveness | Low |
| Widen `MaximumSpanLength` beyond 8 bytes | Fewer round-trips | Needs hardware validation |
| Fixed known-region reads | Simpler scheduling, deterministic timing | Needs region safety verification |

---

## Protocol Details

### How block reads work

A Katana read is a **block read**, not per-key:
1. Send 4-byte **start address** + 4-byte **size** via `RolandSysExBuilder.BuildDataRequest1()`.
2. Katana replies with the same start address followed by `size` bytes of contiguous data.
3. `KatanaMkIIProtocol.TryParseParameterBlockReply()` verifies the reply address and extracts the block.

When batching, `ReadParametersAsync()`:
1. Sorts parameters by address.
2. Groups parameters sharing the same 3-byte prefix within an 8-byte span.
3. Sends one block-read per group.
4. Maps returned bytes back to each parameter by offset.

Extra bytes in the middle of a requested span are simply ignored.

### Address families

| Family        | Meaning                                       | Example               |
|---------------|-----------------------------------------------|-----------------------|
| `KNOB_POS_*`  | Live front-panel knob positions               | `60 00 06 51` (Gain)  |
| `FXBOX_SEL_*` | Live effect variation/color LED state         | `60 00 06 39-3D`      |
| Switch blocks | Live effect on/off at scattered addresses     | `60 00 00 10`, etc.   |
| `PREAMP_A_*`  | Patch-space preamp data (not live state)      | Different region      |
| `SYS_PATCH_SEL` | Current channel / patch selection           | `00 01 00 00`         |

> **Key rule:** Do not conflate `PREAMP_A_*` (patch) with `KNOB_POS_*` (live). Do not infer a live mapping from a patch offset without a matching MKII source reference.

### Port contention / "device busy" errors

When active read sync polls while a write is flushing (or vice versa), `amidi` returns exit code 1 ("Device or resource busy"). The app's re-queue loop handles this correctly — writes are retried and eventually succeed. This is not a bug; it is an ALSA port contention artifact.

Future improvement: exponential backoff on retry + BTS-style port ownership mutex.

---

## Write / Read Bounce Prevention

Device-originated state updates must not be re-queued as writes. The guarantee is:

- Every device-originated state application wraps assignments in `suppressChangeTracking = true`.
- Change handlers (`AmpControlViewModel.Value`, `PanelEffectViewModel.IsEnabled`, `SelectedPanelChannel`) exit immediately when `suppressChangeTracking` is true.
- This is applied in: `RefreshWrittenStateAsync`, `TryReadAmpControlsAsync`, `TryReadPanelControlsAsync`, panel-channel readback flows.

**This is a code-path guarantee, not a protocol-level one.** Future reads paths must use the same suppression scope.

Recommended hardening: centralize device-to-UI state application behind one helper so new read paths cannot forget the guard.

---

## Active Sync Model

Two user-facing toggles (placed in the top device/transport strip):

- **Active Write Sync** — when enabled, queued control edits flush automatically to the amp.
- **Active Read Sync** — when enabled, grouped background refresh updates local state from the amp.

### Write path

1. UI control emits a change intent (property setter).
2. Change handler checks `suppressChangeTracking`, `ActiveWriteSync`, `IsConnected`.
3. If all clear, change is merged into the appropriate `pending*Writes` dictionary (last-write-wins per key).
4. `FlushPendingWritesAsync` is scheduled via debounce (~100-150ms for interactive controls like knob drags).
5. Flush snapshots all dicts atomically, clears originals, sends each write.
6. On failure, re-merges the failed writes back into the pending dicts.
7. `syncOperationGate` (SemaphoreSlim 1,1) prevents overlapping flush operations.

### Read path

- One refresh scheduler with prioritized groups (see [Read Performance](#recommended-refresh-scheduler-shape)).
- Polls ~250ms for lower-priority background refresh.
- All device reads go through `suppressChangeTracking` before applying to ViewModel properties.

---

## UI Architecture & Design Decisions

### Chosen direction: Stylized front-panel

Custom-drawn Avalonia knobs and illuminated buttons, a layout that resembles the Katana top panel, labels positioned like the amp rather than generic form rows.

**Not** a pixel-for-pixel recreation — that would be more brittle and asset-heavy than the current approach warrants.

### Main window layout (Row structure)

```
Row 0: Amp controls | Channel strip | Device status
Row 1: Pedalboard chain minimap (signal order overview)
Row 2: Full pedalboard (all effect cards always visible)
```

### Custom controls

- `RotaryKnob` — reusable custom rotary knob control for amp parameters.
- Illuminated channel/effect buttons — styled toggle buttons with variation color indicators.
- Stompbox cards — signal chain minimap with family accent colors and selection highlight.

### Selection highlight

Stompbox cards in the minimap get a warm gold (`#d4c87a`) border when selected via `Classes.selected` + `PedalboardItemViewModel.IsSelected`.

---

## Pedalboard UI

### Chain minimap (Row 1)

Shows signal order (Input → Booster → Mod → FX → Amp → Delay → Delay 2 → Reverb → Output). Each card shows the effect name and family accent color. Selecting a card highlights it in the minimap.

### Full pedalboard (Row 2)

All effect cards are always visible side-by-side in a horizontal `ScrollViewer`. No click-to-reveal — all controls are always open, like a real physical pedalboard.

Each panel effect card shows:
- Variation color accent bar at top
- On/Off toggle
- Type selector (ComboBox)
- Level slider (where applicable)

Pedal FX card (rightmost) shows:
- On/Off toggle
- Type selector (Wah / Pedal Bend / EVH WAH95)
- Chain position selector (Input / Post Amp)
- Foot Volume slider
- Wah subtype controls (type, position, min, max, level, direct mix)
- Pedal Bend / EVH WAH95 controls (conditional on selected type)

### Drag-and-drop ordering

Pedals are reordered by dragging stompbox cards to change their position in the signal chain.

---

## Future Phases

### Phase 4 — Performance pass (after mappings are trustworthy)

- Implement BTS-style byte-rate pacing (`3125 B/s`).
- Add per-read timeout watchdog.
- Revisit `MaximumSpanLength = 8` — widen if hardware tolerates it for denser panel regions.
- Prioritize refresh groups: cheap/high-value first.

### Phase 5 — Pedals

- Expand Pedal Bend and EVH WAH95 subtype parameter pages.
- Mine MKII source for remaining pedal-related control families.

### Phase 6 — Patch making

Implement in layers:
1. Reliable current patch editing in live memory.
2. Safe patch naming / metadata support.
3. Safe write-to-target workflow (channel/slot).
4. Broader patch file workflows (import/export).

**Keep live editing and saved patch workflows conceptually separate.** Prefer explicit save/apply for patch persistence — not implicit background saves.

### Known gaps / TODO

- **FX Type 21**: The `ModFxTypes` table has a gap at index 21 — user's amp reported this value for the FX effect. Needs hardware verification to name it.
- **Delay 2 variation color**: No color byte in confirmed MKII source. UI must not fake one.
- **Patch level**: Address `60 00 06 4C` not yet source-backed. Disabled until confirmed.
- **Exponential backoff on port-busy retries**: Currently retries immediately; add delay (~100-200ms with backoff) to reduce log noise.
- **Read timeout watchdog**: BTS has per-task timeout; we do not yet.

---

---

## Signal Chain Patterns (PRM_CHAIN_PTN)

**MIDI address:** `0x60 0x00 0x06 0x20` — values 0–6, default = 2

The chain pattern controls the routing order of effects relative to the preamp block. Reverb is always last in every pattern. Pedal FX position is independently configurable on top of the selected chain pattern; EQ1, EQ2, and S/R loop positions are also independently configurable.

| Value | Name | Signal Order (input → output) |
|-------|------|-------------------------------|
| 0 | CHAIN 1 | PDL → **OD** → *AMP* → MOD → FX → Delay → Delay 2 → Rev |
| 1 | CHAIN 2-1 | PDL → **OD** → MOD → *AMP* → FX → Delay → Delay 2 → Rev |
| 2 | CHAIN 3-1 *(default)* | PDL → **OD** → MOD → FX → *AMP* → Delay → Delay 2 → Rev |
| 3 | CHAIN 4-1 | PDL → **OD** → MOD → FX → Delay → Delay 2 → *AMP* → Rev |
| 4 | CHAIN 2-2 | PDL → MOD → **OD** → *AMP* → FX → Delay → Delay 2 → Rev |
| 5 | CHAIN 3-2 | PDL → MOD → **OD** → FX → *AMP* → Delay → Delay 2 → Rev |
| 6 | CHAIN 4-2 | PDL → MOD → **OD** → FX → Delay → Delay 2 → *AMP* → Rev |

**Naming convention:**
- The number (1–4) = how many effect blocks come **before the amp**
- Suffix **-1** = Booster/OD (OD) comes before MOD
- Suffix **-2** = MOD comes before Booster/OD

**Source:** `floorBoard.cpp` in the Katana MK2 FloorBoard community app — `chain_1_Set` through `chain_7_Set` functions; byte-to-name mapping comment at line 1047.

---

## Reference Sources

| Source | Location | Use |
|--------|----------|-----|
| Boss Tone Studio (macOS) | `~/BOSS TONE STUDIO for KATANA MkII Installer/` | Transport pacing strategy, UX reference |
| BTS `address_map.js` | Inside BTS app bundle | FX type count reference (FX1_FXTYPE max=40) |
| `docs/data/tsl-map-1.0.0.csv` | Sibling docs repo | Broad Katana parameter/address catalog |
| KATANA-Mk2 owner's manual | `/home/luisma/katana-git/kataka-csharp/KATANA-Mk2_eng02_W.pdf` | UX and hardware behavior reference |
| BTS parameter reference | `/home/luisma/katana-git/kataka-csharp/BTS_KTN-Mk2_eng07_W.pdf` | Parameter ranges and effect names |
