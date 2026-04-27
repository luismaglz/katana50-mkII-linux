# Kataka

A Linux desktop editor for the **Boss Katana 50 MkII** guitar amplifier, written in C# with [Avalonia UI](https://avaloniaui.net/).

![Kataka screenshot](screenshot.png)

> **Work in progress.** Some functionality is still missing or untested. Use at your own risk.

> Yes, the name is a typo — it should be "Katana". By the time I noticed, it had grown on me, so I left it.

---

## What it does

Kataka connects to your Katana 50 MkII over MIDI (USB) and gives you a graphical front panel to:

- Browse and select amp channels (Panel, A1–A4, B1–B4)
- Edit effects in the signal chain — Booster, Mod, FX, Delay, Reverb, EQ, Noise Suppressor, and Preamp
- See live parameter values reflected as you turn knobs on the amp
- Save and load patches as `.tsl` files (compatible with Boss Tone Studio format)

All edits are sent to the amp in real time using Roland SysEx DT1/RQ1 messages.

---

## Getting started

When the app launches, it will not be connected to the amp automatically. Use the **Scan** button to discover available MIDI ports, then select your Katana to connect. The UI will populate with the amp's current state once connected.

---

## Platform and compatibility

Tested exclusively on **Fedora Linux** with a **Boss Katana 50 MkII**. It likely won't work with other Katana models or variants since I have no way to test on different hardware — the SysEx address maps are model-specific.

| Platform | Status |
|----------|--------|
| Linux    | Primary target — tested on Fedora |
| Windows / macOS | Not supported |

---

## Why not just use Boss Tone Studio?

I know Boss Tone Studio can be run in a Docker container on Linux. I wanted to take a crack at building a native Linux implementation. This is my weekend project.

---

## Requirements

- [.NET 10 runtime](https://dotnet.microsoft.com/download) (or SDK if building from source)
- Boss Katana 50 MkII connected via USB
- `amidi` available (`alsa-utils` package)

---

## Building and running

```bash
git clone <repo-url>
cd katana50-mkII-linux
dotnet run --project src/Kataka.App/Kataka.App.csproj
```

For hot reload during development:

```bash
dotnet watch run --project src/Kataka.App/Kataka.App.csproj
```

---

## Contributing

Contributions are welcome! If you have a Katana 50 MkII and want to help fill in missing parameters, fix bugs, or improve the UI, feel free to open a PR.

This is my first substantial Avalonia project — the UI and AXAML side is not my forte, so improvements there are especially welcome.

A note on AI: I used AI tools to help with boilerplate and implementation details, but the architecture, data flow design, MVVM patterns, UI layout, pedalboard model, signal chain ordering, and overall direction are mine. Every pattern in this codebase — how amp state is mirrored, how writes are routed, how the pedalboard is structured — was designed and driven by me. AI was a productivity tool for the parts that didn't need creative input, not a co-author.

---

## License

Kataka is free software licensed under the [GNU General Public License v3.0](LICENSE).

> Boss and Katana are trademarks of Roland Corporation. This project is not affiliated with or endorsed by Roland/Boss.
