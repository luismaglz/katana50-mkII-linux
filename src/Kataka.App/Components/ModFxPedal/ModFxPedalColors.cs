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

    public static PedalColorScheme GetColorScheme(string? typeName) => typeName switch
    {
        "CHORUS" or "DC-30" => new(Chorus),
        "FLANGER" or "FLANGER 117E" => new(Flanger),
        "PHASER" or "PHASER 90E" => new(Phaser),
        "UNI-V" => new(UniV),
        "TREMOLO" => new(Tremolo),
        "VIBRATO" => new(Vibrato),
        "ROTARY" => new(Rotary),
        "SLICER" => new(Slicer),
        "COMP" => new(Comp),
        "LIMITER" => new(Limiter),
        "GRAPHIC EQ" or "PARAMETRIC EQ" => new(Eq),
        "SLOW GEAR" => new(SlowGear),
        "T.WAH" or "AUTO WAH" => new(TouchWah),
        "PEDAL WAH" or "WAH 95E" => new(PedalWah),
        "HUMANIZER" => new(Humanizer),
        "PITCH SHIFTER" or "PEDAL BEND" => new(PitchShifter),
        "HARMONIST" => new(Harmonist),
        "OCTAVE" or "HEAVY OCTAVE" => new(Octave),
        "WAVE SYNTH" => new(WaveSynth),
        "RING MOD" => new(RingMod),
        "GUITAR SIM" => new(GuitarSim),
        "AC.GUITAR SIM" => new(AcGuitarSim),
        "AC.PROCESSOR" => new(AcProcessor),
        _ => default
    };

    private static IBrush Solid(string hex) => new SolidColorBrush(Color.Parse(hex));
}
