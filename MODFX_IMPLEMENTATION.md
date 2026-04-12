# Mod/FX Pedal Implementation

Tracks the design, MIDI addressing, and completion status for merging `ModPedalViewModel` + `FxPedalViewModel` into a single `ModFxPedalViewModel` with per-type view templates for all 31 effect types.

---

## Background

The Katana MkII has two identical effect slots ("MOD" and "FX") at different positions in the signal chain. Both slots support the same 31 effect types and share the same MIDI parameter layout — only their base addresses differ.

Previously, `ModPedalViewModel` and `FxPedalViewModel` were two separate but identical classes. Neither had type-specific controls. This refactor merges them and adds full parameter editing for every type.

**Reference source:** Boss Tone Studio for Katana MkII app  
Path on this machine: `../BOSS TONE STUDIO for KATANA MkII Installer/`  
Key files:
- `html/js/config/address_map.js` — all parameter addresses, min/max, offset
- `html/export/layout.div` — BTS UI controls mapped to parameters

---

## Architecture

```
ModFxPedalViewModel ("mod" | "fx")
  ├── SelectedTypeOption  → SelectedTypeIndex  → IsTypeChorus .. IsTypePedalBend
  ├── Variation, Level (shared for all types)
  ├── [ObservableProperty] per-type params (≈150 total)
  ├── GetSyncParameters() → switch + type + level + variation + ALL type params
  └── ApplyAmpValuesCore() → populates properties from MIDI readback dict

ModFxPedalView.axaml (outer shell)
  ├── Header: PedalHeaderControl (on/off)
  ├── TYPE: ComboBox → SelectedTypeOption
  ├── VARIATION: color bar
  ├── LEVEL: RotaryKnob
  └── Panel of 31 sub-views (one IsVisible=true at a time)
        Views/mods-fx/ChorusView.axaml          (IsTypeChorus)
        Views/mods-fx/FlangerView.axaml         (IsTypeFlanger)
        ... 31 total ...
        Views/mods-fx/PedalBendView.axaml       (IsTypePedalBend)
```

Both `IsMod` and `IsFx` slots in `PedalboardView.axaml` render `ModFxPedalView` with their respective `ModFxPedal` binding.

---

## MIDI Addressing

All 31 effect types share a common parameter block layout (`prm_prop_patch_fx`, section base comment `// 0x00000100`).

**Address formula for any type-specific parameter:**

| Slot | Base | Address bytes |
|------|------|---------------|
| MOD  | `[0x60, 0x00, 0x01, 0x00]` | `[0x60, 0x00, (0x01 + rel>>8), (rel & 0xFF)]` |
| FX   | `[0x60, 0x00, 0x03, 0x00]` | `[0x60, 0x00, (0x03 + rel>>8), (rel & 0xFF)]` |

Where `rel` is the hex offset listed in `address_map.js`.

Example: Phaser Rate at offset `0x0104`
- Mod → `[0x60, 0x00, 0x02, 0x04]`
- FX  → `[0x60, 0x00, 0x04, 0x04]`

---

## All 31 Effect Types with Parameters

### Type 0 — CHORUS (2×2 Chorus)

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Crossover Freq | `0x0137` | 0–16 | index |
| Low Rate | `0x0138` | 0–100 | 0–100 |
| Low Depth | `0x0139` | 0–100 | 0–100 |
| Low Pre-Delay | `0x013A` | 0–80 | 0–80 ms |
| Low Level | `0x013B` | 0–100 | 0–100 |
| High Rate | `0x013C` | 0–100 | 0–100 |
| High Depth | `0x013D` | 0–100 | 0–100 |
| High Pre-Delay | `0x013E` | 0–80 | 0–80 ms |
| High Level | `0x013F` | 0–100 | 0–100 |
| Direct Mix | `0x0140` | 0–100 | 0–100 |

### Type 1 — FLANGER

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Rate | `0x010B` | 0–100 | 0–100 |
| Depth | `0x010C` | 0–100 | 0–100 |
| Manual | `0x010D` | 0–100 | 0–100 |
| Resonance | `0x010E` | 0–100 | 0–100 |
| Low Cut | `0x0110` | 0–10 | index |
| Effect Level | `0x0111` | 0–100 | 0–100 |
| Direct Mix | `0x0112` | 0–100 | 0–100 |

### Type 2 — PHASER

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Type | `0x0103` | 0–3 | index |
| Rate | `0x0104` | 0–100 | 0–100 |
| Depth | `0x0105` | 0–100 | 0–100 |
| Manual | `0x0106` | 0–100 | 0–100 |
| Resonance | `0x0107` | 0–100 | 0–100 |
| Step Rate | `0x0108` | 0–100 | 0–100 |
| Effect Level | `0x0109` | 0–100 | 0–100 |
| Direct Mix | `0x010A` | 0–100 | 0–100 |

### Type 3 — UNI-V

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Rate | `0x011E` | 0–100 |
| Depth | `0x011F` | 0–100 |
| Level | `0x0120` | 0–100 |

### Type 4 — TREMOLO

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Wave Shape | `0x0113` | 0–100 |
| Rate | `0x0114` | 0–100 |
| Depth | `0x0115` | 0–100 |
| Level | `0x0116` | 0–100 |

### Type 5 — VIBRATO

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Rate | `0x0126` | 0–100 |
| Depth | `0x0127` | 0–100 |
| Level | `0x012A` | 0–100 |

### Type 6 — ROTARY

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Rate (Fast) | `0x0119` | 0–100 |
| Depth | `0x011C` | 0–100 |
| Level | `0x011D` | 0–100 |

### Type 7 — RING MOD

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Mode | `0x012B` | 0–1 |
| Frequency | `0x012C` | 0–100 |
| Effect Level | `0x012D` | 0–100 |
| Direct Mix | `0x012E` | 0–100 |

### Type 8 — SLOW GEAR

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Sens | `0x003C` | 0–100 |
| Rise Time | `0x003D` | 0–100 |
| Level | `0x003E` | 0–100 |

### Type 9 — SLICER

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Pattern | `0x0121` | 0–19 |
| Rate | `0x0122` | 0–100 |
| Trigger Sens | `0x0123` | 0–100 |
| Effect Level | `0x0124` | 0–100 |
| Direct Mix | `0x0125` | 0–100 |

### Type 10 — COMP

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Type | `0x0016` | 0–6 | index |
| Sustain | `0x0017` | 0–100 | 0–100 |
| Attack | `0x0018` | 0–100 | 0–100 |
| Tone | `0x0019` | 0–100 | −50…+50 (ofs 50) |
| Level | `0x001A` | 0–100 | 0–100 |

### Type 11 — LIMITER

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Type | `0x001B` | 0–2 | index |
| Attack | `0x001C` | 0–100 | 0–100 |
| Threshold | `0x001D` | 0–100 | 0–100 |
| Ratio | `0x001E` | 0–17 | index |
| Release | `0x001F` | 0–100 | 0–100 |
| Level | `0x0020` | 0–100 | 0–100 |

### Type 12 — T.WAH

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Mode | `0x0002` | 0–1 |
| Polarity | `0x0003` | 0–1 |
| Sens | `0x0004` | 0–100 |
| Freq | `0x0005` | 0–100 |
| Peak | `0x0006` | 0–100 |
| Direct Mix | `0x0007` | 0–100 |
| Effect Level | `0x0008` | 0–100 |

### Type 13 — AUTO WAH

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Mode | `0x0009` | 0–1 |
| Freq | `0x000A` | 0–100 |
| Peak | `0x000B` | 0–100 |
| Rate | `0x000C` | 0–100 |
| Depth | `0x000D` | 0–100 |
| Direct Mix | `0x000E` | 0–100 |
| Effect Level | `0x000F` | 0–100 |

### Type 14 — PEDAL WAH

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Wah Type | `0x0010` | 0–5 |
| Pedal Position | `0x0011` | 0–100 |
| Pedal Min | `0x0012` | 0–100 |
| Pedal Max | `0x0013` | 0–100 |
| Effect Level | `0x0014` | 0–100 |
| Direct Mix | `0x0015` | 0–100 |

### Type 15 — GRAPHIC EQ

All band gains: wire 0–40, display −20…+20 dB (offset 20)

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| 31 Hz | `0x0021` | 0–40 |
| 62 Hz | `0x0022` | 0–40 |
| 125 Hz | `0x0023` | 0–40 |
| 250 Hz | `0x0024` | 0–40 |
| 500 Hz | `0x0025` | 0–40 |
| 1 kHz | `0x0026` | 0–40 |
| 2 kHz | `0x0027` | 0–40 |
| 4 kHz | `0x0028` | 0–40 |
| 8 kHz | `0x0029` | 0–40 |
| 16 kHz | `0x002A` | 0–40 |
| Level | `0x002B` | 0–40 |

### Type 16 — PARAMETRIC EQ

Gain params: wire 0–40, display −20…+20 dB (offset 20)

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Low Cut | `0x002C` | 0–17 | index |
| Low Gain | `0x002D` | 0–40 | −20…+20 |
| Low-Mid Freq | `0x002E` | 0–27 | index |
| Low-Mid Q | `0x002F` | 0–5 | index |
| Low-Mid Gain | `0x0030` | 0–40 | −20…+20 |
| High-Mid Freq | `0x0031` | 0–27 | index |
| High-Mid Q | `0x0032` | 0–5 | index |
| High-Mid Gain | `0x0033` | 0–40 | −20…+20 |
| High Gain | `0x0034` | 0–40 | −20…+20 |
| High Cut | `0x0035` | 0–14 | index |
| Level | `0x0036` | 0–40 | −20…+20 |

### Type 17 — GUITAR SIM

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Type | `0x0037` | 0–7 | index |
| Low | `0x0038` | 0–100 | −50…+50 (ofs 50) |
| High | `0x0039` | 0–100 | −50…+50 (ofs 50) |
| Level | `0x003A` | 0–100 | 0–100 |
| Body | `0x003B` | 0–100 | 0–100 |

### Type 18 — AC.GUITAR SIM

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| High | `0x0141` | 0–100 | −50…+50 (ofs 50) |
| Body | `0x0142` | 0–100 | 0–100 |
| Low | `0x0143` | 0–100 | −50…+50 (ofs 50) |
| Level | `0x0145` | 0–100 | 0–100 |

> Note: address `0x0144` is padding — skip it.

### Type 19 — AC.PROCESSOR

> Note: spans a page boundary. `0x0100`–`0x0102` for Mod map to `[0x60, 0x00, 0x02, 0x00–0x02]`.

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Type | `0x007C` | 0–3 | index |
| Bass | `0x007D` | 0–100 | −50…+50 (ofs 50) |
| Mid | `0x007E` | 0–100 | −50…+50 (ofs 50) |
| Mid Freq | `0x007F` | 0–27 | index |
| Treble | `0x0100` | 0–100 | −50…+50 (ofs 50) |
| Presence | `0x0101` | 0–100 | −50…+50 (ofs 50) |
| Level | `0x0102` | 0–100 | 0–100 |

### Type 20 — WAVE SYNTH

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Wave | `0x003F` | 0–1 |
| Cutoff | `0x0040` | 0–100 |
| Resonance | `0x0041` | 0–100 |
| Filter Sens | `0x0042` | 0–100 |
| Filter Decay | `0x0043` | 0–100 |
| Filter Depth | `0x0044` | 0–100 |
| Synth Level | `0x0045` | 0–100 |
| Direct Mix | `0x0046` | 0–100 |

### Type 21 — OCTAVE

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Range | `0x0047` | 0–3 |
| Effect Level | `0x0048` | 0–100 |
| Direct Mix | `0x0049` | 0–100 |

### Type 22 — HEAVY OCTAVE

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| 1-Oct Level | `0x015A` | 0–100 |
| 2-Oct Level | `0x015B` | 0–100 |
| Direct Mix | `0x015C` | 0–100 |

### Type 23 — PITCH SHIFTER

> Pre-delay params skipped (INTEGER2×7, 2-byte values — future work).

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Voice | `0x004A` | 0–1 | index |
| PS1 Mode | `0x004B` | 0–3 | index |
| PS1 Pitch | `0x004C` | 0–48 | −24…+24 (ofs 24) |
| PS1 Fine | `0x004D` | 0–100 | −50…+50 (ofs 50) |
| PS1 Level | `0x0050` | 0–100 | 0–100 |
| PS2 Mode | `0x0051` | 0–3 | index |
| PS2 Pitch | `0x0052` | 0–48 | −24…+24 (ofs 24) |
| PS2 Fine | `0x0053` | 0–100 | −50…+50 (ofs 50) |
| PS2 Level | `0x0056` | 0–100 | 0–100 |
| Feedback | `0x0057` | 0–100 | 0–100 |
| Direct Mix | `0x0058` | 0–100 | 0–100 |

### Type 24 — HARMONIST

> Pre-delay params skipped (INTEGER2×7 — future work). Note-table harmony params (0x0064–0x007B) skipped for now.

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Voice | `0x0059` | 0–1 | index |
| Harmony 1 | `0x005A` | 0–29 | index |
| Level 1 | `0x005D` | 0–100 | 0–100 |
| Harmony 2 | `0x005E` | 0–29 | index |
| Level 2 | `0x0061` | 0–100 | 0–100 |
| Feedback | `0x0062` | 0–100 | 0–100 |
| Direct Mix | `0x0063` | 0–100 | 0–100 |

### Type 25 — HUMANIZER

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Mode | `0x012F` | 0–1 |
| Vowel 1 | `0x0130` | 0–4 |
| Vowel 2 | `0x0131` | 0–4 |
| Sens | `0x0132` | 0–100 |
| Rate | `0x0133` | 0–100 |
| Depth | `0x0134` | 0–100 |
| Manual | `0x0135` | 0–100 |
| Level | `0x0136` | 0–100 |

### Type 26 — PHASER 90E

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Script | `0x0146` | 0–1 |
| Speed | `0x0147` | 0–100 |

### Type 27 — FLANGER 117E

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Manual | `0x0148` | 0–100 |
| Width | `0x0149` | 0–100 |
| Speed | `0x014A` | 0–100 |
| Regen | `0x014B` | 0–100 |

### Type 28 — WAH 95E

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Pedal Position | `0x014C` | 0–100 |
| Pedal Min | `0x014D` | 0–100 |
| Pedal Max | `0x014E` | 0–100 |
| Effect Level | `0x014F` | 0–100 |
| Direct Mix | `0x0150` | 0–100 |

### Type 29 — DC-30

> Echo Repeat Rate skipped (INTEGER2×7, addr `0x0154`–`0x0155` — future work).

| Parameter | Relative Addr | Wire Range |
|-----------|--------------|-----------|
| Selector | `0x0151` | 0–1 |
| Input Volume | `0x0152` | 0–100 |
| Chorus Intensity | `0x0153` | 0–100 |
| Echo Intensity | `0x0156` | 0–100 |
| Echo Volume | `0x0157` | 0–100 |
| Tone | `0x0158` | 0–100 |
| Output | `0x0159` | 0–100 |

### Type 30 — PEDAL BEND

| Parameter | Relative Addr | Wire Range | Display |
|-----------|--------------|-----------|---------|
| Pitch | `0x015D` | 0–48 | −24…+24 (ofs 24) |
| Pedal Position | `0x015E` | 0–100 | 0–100 |
| Effect Level | `0x015F` | 0–100 | 0–100 |
| Direct Mix | `0x0160` | 0–100 | 0–100 |

---

## Future Work (deferred)

- **INTEGER2×7 pre-delay params**: PitchShifter PS1/PS2 pre-delay (0–300 ms), Harmonist HR1/HR2 pre-delay, DC-30 Echo Repeat Rate. These use 2-byte Roland 7-bit encoding and need special read/write handling.
- **Harmonist note tables**: per-key harmony assignments (0x0064–0x007B), 24 params per voice.
- **Slicer pattern editor**: pattern 0–19 select; a dedicated pattern editor UI could show the step grid.
- **GEQ/PEQ visual bar displays**: currently rendered as RotaryKnobs; a dedicated bar-graph EQ control would be more intuitive.

---

## Implementation Checklist

> **Phase 1 scope: Types 0–2 only (Chorus, Flanger, Phaser).**  
> Types 3–30 remain unimplemented — unrecognised type index shows no body panel.  
> Expand to remaining types after Phase 1 is validated on hardware.

### Phase 1 — Data / Parameter Catalog

- [ ] Add `partial` keyword to `KatanaMkIIParameterCatalog.cs`
- [ ] Create `src/Kataka.Domain/Midi/KatanaMkIIParameterCatalog.ModFx.cs`
  - [ ] Address helper `ModFxAddress(byte slotBase, int rel)` → `byte[4]`
  - [ ] `ModChorusParams[]` / `FxChorusParams[]` (10 params each, rel 0x0137–0x0140)
  - [ ] `ModFlangerParams[]` / `FxFlangerParams[]` (7 params, rel 0x010B–0x0112, skip 0x010F)
  - [ ] `ModPhaserParams[]` / `FxPhaserParams[]` (8 params, rel 0x0103–0x010A)

### Phase 2 — ViewModel

- [ ] Create `src/Kataka.App/ViewModels/ModFxPedalViewModel.cs`
  - [ ] Constructor `(string slot)` — slot is "mod" or "fx"
  - [ ] `SelectedTypeOption`, `SelectedTypeIndex`, `Variation`, `Level` (same as Mod/Fx VMs today)
  - [ ] `IsTypeChorus`, `IsTypeFlanger`, `IsTypePhaser` bool props (raised in `OnSelectedTypeIndexChanged`)
  - [ ] Named `[ObservableProperty]` fields: 10 Chorus + 7 Flanger + 8 Phaser params
  - [ ] `GetSyncParameters()` returns switch + type + level + variation + all 3 types' params
  - [ ] `ApplyAmpValuesCore()` populates all props from MIDI dict
- [ ] Create `src/Kataka.App/ViewModels/Design/DesignModFxPedalViewModel.cs`

### Phase 3 — Views

- [ ] Create `src/Kataka.App/Views/ModFxPedalView.axaml` + `.axaml.cs`
  - [ ] Header (PedalHeaderControl), TYPE ComboBox, Variation bar, Level knob
  - [ ] Three IsVisible panels hosting ChorusView / FlangerView / PhaserView
- [ ] Create `src/Kataka.App/Views/mods-fx/ChorusView.axaml` + `.axaml.cs`
- [ ] Create `src/Kataka.App/Views/mods-fx/FlangerView.axaml` + `.axaml.cs`
- [ ] Create `src/Kataka.App/Views/mods-fx/PhaserView.axaml` + `.axaml.cs`

### Phase 4 — Wiring

- [ ] `PedalboardItemViewModel.cs` — `ModFxPedal` accessor replaces `ModPedal` + `FxPedal`
- [ ] `PedalboardView.axaml` — both IsMod/IsFx slots use `ModFxPedalView` + `ModFxPedal`
- [ ] `MainWindowViewModel.cs` — factory: `new ModFxPedalViewModel("mod"/"fx")`

### Phase 5 — Cleanup

- [ ] Delete `ModPedalViewModel.cs`, `FxPedalViewModel.cs`
- [ ] Delete `Design/DesignModPedalViewModel.cs`, `Design/DesignFxPedalViewModel.cs`
- [ ] Delete `Views/ModPedalView.axaml(.cs)`, `Views/FxPedalView.axaml(.cs)`

### Phase 6 — Verification

- [ ] `dotnet build Kataka.slnx` — zero errors
- [ ] `dotnet test Kataka.slnx` — all tests pass
- [ ] Manual smoke: type-switch Mod/FX between Chorus/Flanger/Phaser, verify controls update

---

### Phase 2+ (future — after Phase 1 validated)

Types to add in future iterations:
- [ ] Type 3 — Uni-V
- [ ] Type 4 — Tremolo
- [ ] Type 5 — Vibrato
- [ ] Type 6 — Rotary
- [ ] Type 7 — Ring Mod
- [ ] Type 8 — Slow Gear
- [ ] Type 9 — Slicer
- [ ] Type 10 — Comp
- [ ] Type 11 — Limiter
- [ ] Type 12 — T.WAH
- [ ] Type 13 — Auto Wah
- [ ] Type 14 — Pedal Wah
- [ ] Type 15 — Graphic EQ
- [ ] Type 16 — Parametric EQ
- [ ] Type 17 — Guitar Sim
- [ ] Type 18 — AC Guitar Sim
- [ ] Type 19 — AC Processor
- [ ] Type 20 — Wave Synth
- [ ] Type 21 — Octave
- [ ] Type 22 — Heavy Octave
- [ ] Type 23 — Pitch Shifter *(INTEGER2×7 pre-delay deferred)*
- [ ] Type 24 — Harmonist *(INTEGER2×7 pre-delay + note tables deferred)*
- [ ] Type 25 — Humanizer
- [ ] Type 26 — Phaser 90E
- [ ] Type 27 — Flanger 117E
- [ ] Type 28 — Wah 95E
- [ ] Type 29 — DC-30 *(INTEGER2×7 echo repeat rate deferred)*
- [ ] Type 30 — Pedal Bend

---

## Key Decisions

| Decision | Choice | Reason |
|----------|--------|--------|
| Type body switching | 31 `IsVisible` panels driven by `IsTypeXxx` bool properties | Cleanest compile-time binding in Avalonia without converters |
| Parameter definitions | Arrays per type in partial catalog class | Keeps main catalog readable; arrays easy to iterate in `GetSyncParameters` |
| INTEGER2×7 params | Skipped (pre-delay, echo repeat rate) | 2-byte MIDI encoding not yet supported; deferred to future work |
| GetSyncParameters scope | Returns ALL type params (not just active type) | Simplifies MWVM read/write path; avoids stale-key issues on type change |
| Slot identity | Constructor arg `"mod"` or `"fx"` | Single class, two instances; keys differ by slot prefix |
