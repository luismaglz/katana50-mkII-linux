namespace Kataka.Domain.Midi;

/// <summary>Auto-generated: sealed class KatanaPanelEffectDefinition</summary>
public sealed class KatanaPanelEffectDefinition
{
    /// <summary>Auto-generated: KatanaPanelEffectDefinition(</summary>
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

    /// <summary>Auto-generated: string Key { get; }</summary>
    public string Key { get; }

    /// <summary>Auto-generated: string DisplayName { get; }</summary>
    public string DisplayName { get; }

    /// <summary>Auto-generated: KatanaParameterDefinition SwitchParameter { get; }</summary>
    public KatanaParameterDefinition SwitchParameter { get; }

    /// <summary>Auto-generated: KatanaParameterDefinition? VariationParameter { get; }</summary>
    public KatanaParameterDefinition? VariationParameter { get; }

    /// <summary>Auto-generated: KatanaParameterDefinition? TypeParameter { get; }</summary>
    public KatanaParameterDefinition? TypeParameter { get; }

    /// <summary>Auto-generated: KatanaParameterDefinition? LevelParameter { get; }</summary>
    public KatanaParameterDefinition? LevelParameter { get; }

    /// <summary>
    ///     Per-effect DSP parameters (Drive, Tone, Feedback, Time, etc.) polled and written
    ///     in addition to the top-level switch/type/level parameters.
    /// </summary>
    public IReadOnlyList<KatanaParameterDefinition> DetailParameters { get; }
}
