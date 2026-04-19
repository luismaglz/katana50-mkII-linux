namespace Kataka.Domain.Midi;

public sealed class KatanaPanelEffectDefinition
{
    public KatanaPanelEffectDefinition(
        string key,
        string displayName,
        KatanaParameterDefinition switchParameter,
        KatanaParameterDefinition? variationParameter = null,
        KatanaParameterDefinition? typeParameter = null,
        KatanaParameterDefinition? levelParameter = null,
        IReadOnlyList<KatanaParameterDefinition>? detailParameters = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        ArgumentNullException.ThrowIfNull(switchParameter);

        Key = key;
        DisplayName = displayName;
        SwitchParameter = switchParameter;
        VariationParameter = variationParameter;
        TypeParameter = typeParameter;
        LevelParameter = levelParameter;
        DetailParameters = detailParameters ?? [];
    }

    public string Key { get; }

    public string DisplayName { get; }

    public KatanaParameterDefinition SwitchParameter { get; }

    public KatanaParameterDefinition? VariationParameter { get; }

    public KatanaParameterDefinition? TypeParameter { get; }

    public KatanaParameterDefinition? LevelParameter { get; }

    /// <summary>
    /// Per-effect DSP parameters (Drive, Tone, Feedback, Time, etc.) polled and written
    /// in addition to the top-level switch/type/level parameters.
    /// </summary>
    public IReadOnlyList<KatanaParameterDefinition> DetailParameters { get; }
}
