namespace Kataka.App.ViewModels;

/// <summary>Standard Boss/Roland frequency step label tables for stepped knob controls.</summary>
public static class FrequencySteps
{
    /// <summary>Low-cut 11 steps (0–10): FLAT, 20Hz … 160Hz. Used by Flanger Low Cut.</summary>
    public static readonly string[] LowCut11 =
    [
        "FLAT", "20Hz", "25Hz", "31.5Hz", "40Hz", "50Hz",
        "63Hz", "80Hz", "100Hz", "125Hz", "160Hz"
    ];

    /// <summary>Low-cut 18 steps (0–17): FLAT, 20Hz … 800Hz. Used by Reverb/ParametricEQ Low Cut.</summary>
    public static readonly string[] LowCut18 =
    [
        "FLAT", "20Hz", "25Hz", "31.5Hz", "40Hz", "50Hz",
        "63Hz", "80Hz", "100Hz", "125Hz", "160Hz", "200Hz",
        "250Hz", "315Hz", "400Hz", "500Hz", "630Hz", "800Hz"
    ];

    /// <summary>High-cut 15 steps (0–14): 630Hz … FLAT. Used by Delay/ParametricEQ High Cut.</summary>
    public static readonly string[] HighCut15 =
    [
        "630Hz", "800Hz", "1.0kHz", "1.25kHz", "1.6kHz", "2.0kHz",
        "2.5kHz", "3.15kHz", "4.0kHz", "5.0kHz", "6.3kHz", "8.0kHz",
        "10kHz", "12.5kHz", "FLAT"
    ];

    /// <summary>High-cut 18 steps (0–17): 630Hz … FLAT. Used by Reverb High Cut.</summary>
    public static readonly string[] HighCut18 =
    [
        "630Hz", "800Hz", "1.0kHz", "1.25kHz", "1.6kHz", "2.0kHz",
        "2.5kHz", "3.15kHz", "4.0kHz", "5.0kHz", "6.3kHz", "8.0kHz",
        "10kHz", "12.5kHz", "16kHz", "20kHz", "25kHz", "FLAT"
    ];

    /// <summary>Crossover 17 steps (0–16): 63Hz … 2.5kHz. Used by Chorus XoverFreq.</summary>
    public static readonly string[] XoverFreq17 =
    [
        "63Hz", "80Hz", "100Hz", "125Hz", "160Hz", "200Hz", "250Hz",
        "315Hz", "400Hz", "500Hz", "630Hz", "800Hz",
        "1.0kHz", "1.25kHz", "1.6kHz", "2.0kHz", "2.5kHz"
    ];
}
