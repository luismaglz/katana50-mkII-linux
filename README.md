# Kataka

A linux desktop editor for the **Boss Katana MK2** guitar amplifier, written in C# with [Avalonia UI](https://avaloniaui.net/).

![Kataka screenshot](screenshot.png)

---

## What it does

Kataka connects to your Katana MK2 over MIDI (USB) and gives you a graphical front panel to:

- Browse and select amp channels (Panel, A1–A4, B1–B4)
- Edit every effect in the signal chain — Booster, Mod, FX, Delay, Reverb, EQ, Noise Suppressor, and Preamp
- See live parameter values reflected as you turn knobs on the amp
- Save and load patches as `.tsl` files (compatible with Boss Tone Studio format)

All edits are sent to the amp in real time using Roland SysEx DT1/RQ1 messages.

---

## Platform

| Platform | Status |
|----------|--------|
| Linux    | ✅ Primary target (uses `amidi`) |

---

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- Boss Katana MK2 connected via USB
- Linux: `amidi` available (part of `alsa-utils`)

---

## Building

```bash
git clone <repo-url>
cd kataka-csharp
dotnet build src/Kataka.App/Kataka.App.csproj
```

## Running

```bash
dotnet run --project src/Kataka.App/Kataka.App.csproj
```

On first launch, Kataka will scan for MIDI ports and connect to the Katana automatically if it is plugged in.

---

## Project structure

```
src/
  Kataka.Domain/          # MIDI protocol, parameter catalog, patch model
  Kataka.Infrastructure/  # amidi transport, MIDI port discovery
  Kataka.Application/     # KatanaSession, write queue, patch load/save
  Kataka.App/             # Avalonia UI, ViewModels, custom controls
tests/
```

---

## License

Kataka is free software: you can redistribute it and/or modify it under the terms of the
[GNU General Public License v3.0](LICENSE) as published by the Free Software Foundation.

> Boss and Katana are trademarks of Roland Corporation. This project is not affiliated with or endorsed by Roland/Boss.
