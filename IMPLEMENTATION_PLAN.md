# Kataka C# Reimplementation Plan

## Goal

Create a clean, Linux-first, cross-platform C# reimplementation of the existing Qt/C++ floorboard app for Boss Katana devices.

## Architecture

1. **UI**: Avalonia desktop app
2. **Application**: device workflows, commands, view models, use cases
3. **Domain/Protocol**: SysEx framing, checksum logic, Katana parameter/address model
4. **Infrastructure**: MIDI transport, settings, logging, asset loading

## Stack decisions

- **UI**: Avalonia
- **Primary MIDI backend**: DryWetMIDI behind `IMidiTransport`
- **Fallback MIDI backend**: RtMidi-based adapter if Linux SysEx behavior is not reliable enough

## Vertical slice target

The first slice should prove the architecture against real hardware on Linux:

1. App starts
2. MIDI ports are enumerated
3. Boss Katana MKII can be selected and connected
4. Device detection succeeds
5. One safe parameter can be read/written end-to-end

## Execution plan

1. Create this repo-local plan and keep it updated
2. Inventory the old Qt/C++ app and identify only the code needed for the first slice
3. Bootstrap the .NET solution and projects
4. Define transport abstractions and session contracts
5. Implement the first MIDI transport adapter
6. Port the SysEx core and minimum Katana messages
7. Build the connection vertical slice in the UI
8. Add byte-level fixture tests
9. Run Linux hardware smoke tests against the Katana MKII

## Progress

| Item | Status | Notes |
| --- | --- | --- |
| Repo-local plan file | Done | Created as the first execution step |
| Old app inventory | Done | Source app responsibilities mapped into UI, protocol/data, and MIDI transport layers |
| Solution bootstrap | Done | Avalonia app, layered projects, tests, and solution wiring are in place |
| Transport contracts | Done | Basic MIDI port and SysEx abstractions added |
| DryWetMIDI adapter | Done | Kept as a non-Linux backend option; Linux no longer depends on its missing native library |
| SysEx core port | Done | Generic Roland/Boss checksum, identity, and frame builder helpers added |
| Connection vertical slice | Done | Quick-test UI now scans, auto-selects Katana-style ports, opens the selected MIDI pair, and exposes a Request identity action |
| Fixture tests | Done | Protocol helper tests are in place and passing |
| Hardware smoke test | Done | On this Linux machine the app enumerated KATANA MIDI 1/2/3 via `amidi`, auto-selected the ports, and Connect succeeded |

## Current next test point

Run the app, click **Scan MIDI ports**, click **Connect**, then click **Request identity**. The expected identity reply already observed from the shell on this machine is:

`F0 7E 00 06 02 41 33 03 00 00 05 00 00 00 F7`
