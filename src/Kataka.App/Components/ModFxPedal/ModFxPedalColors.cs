using Avalonia.Media;

using Kataka.App.ViewModels;

namespace Kataka.App.Components.ModFxPedal;

public static class ModFxPedalColors
{
    // Modulation & Movement
    public static readonly IBrush Chorus = Solid("#00CED1"); // CHORUS / DC-30
    public static readonly IBrush Flanger = Solid("#C0C0C0"); // FLANGER / 117E
    public static readonly IBrush Phaser = Solid("#FF8C00"); // PHASER / 90E
    public static readonly IBrush UniV = Solid("#2F4F4F"); // UNI-V
    public static readonly IBrush Tremolo = Solid("#006400"); // TREMOLO
    public static readonly IBrush Vibrato = Solid("#9370DB"); // VIBRATO
    public static readonly IBrush Rotary = Solid("#8B4513"); // ROTARY
    public static readonly IBrush Slicer = Solid("#B02220"); // SLICER

    // Dynamics & Utility
    public static readonly IBrush Comp = Solid("#F0F8FF"); // COMP
    public static readonly IBrush Limiter = Solid("#FF4500"); // LIMITER
    public static readonly IBrush Eq = Solid("#228B22"); // GRAPHIC EQ / PARAMETRIC EQ
    public static readonly IBrush SlowGear = Solid("#000000"); // SLOW GEAR

    // Filter & Expression
    public static readonly IBrush TouchWah = Solid("#E0D611"); // T.WAH / AUTO WAH
    public static readonly IBrush PedalWah = Solid("#DAA520"); // PEDAL WAH / WAH 95E
    public static readonly IBrush Humanizer = Solid("#FFB6C1"); // HUMANIZER

    // Pitch & Synth
    public static readonly IBrush PitchShifter = Solid("#4B0082"); // PITCH SHIFTER / PEDAL BEND
    public static readonly IBrush Harmonist = Solid("#8A2BE2"); // HARMONIST
    public static readonly IBrush Octave = Solid("#5D4037"); // OCTAVE / HEAVY OCTAVE
    public static readonly IBrush WaveSynth = Solid("#753E8C"); // WAVE SYNTH
    public static readonly IBrush RingMod = Solid("#800000"); // RING MOD

    // Simulation
    public static readonly IBrush GuitarSim = Solid("#708090"); // GUITAR SIM
    public static readonly IBrush AcGuitarSim = Solid("#D2B48C"); // AC.GUITAR SIM
    public static readonly IBrush AcProcessor = Solid("#BC8F8F"); // AC.PROCESSOR

    public static IBrush GetBackgroundBrush(string? typeName) => typeName switch
    {
        "CHORUS" or "DC-30" => Chorus,
        "FLANGER" or "FLANGER 117E" => Flanger,
        "PHASER" or "PHASER 90E" => Phaser,
        "UNI-V" => UniV,
        "TREMOLO" => Tremolo,
        "VIBRATO" => Vibrato,
        "ROTARY" => Rotary,
        "SLICER" => Slicer,
        "COMP" => Comp,
        "LIMITER" => Limiter,
        "GRAPHIC EQ" or "PARAMETRIC EQ" => Eq,
        "SLOW GEAR" => SlowGear,
        "T.WAH" or "AUTO WAH" => TouchWah,
        "PEDAL WAH" or "WAH 95E" => PedalWah,
        "HUMANIZER" => Humanizer,
        "PITCH SHIFTER" or "PEDAL BEND" => PitchShifter,
        "HARMONIST" => Harmonist,
        "OCTAVE" or "HEAVY OCTAVE" => Octave,
        "WAVE SYNTH" => WaveSynth,
        "RING MOD" => RingMod,
        "GUITAR SIM" => GuitarSim,
        "AC.GUITAR SIM" => AcGuitarSim,
        "AC.PROCESSOR" => AcProcessor,
        _ => PedalViewModel.DefaultCardBackground
    };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
