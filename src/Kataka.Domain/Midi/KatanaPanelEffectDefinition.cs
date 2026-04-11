namespace Kataka.Domain.Midi;

public sealed class KatanaPanelEffectDefinition
{
    public KatanaPanelEffectDefinition(
        string key,
        string displayName,
        KatanaParameterDefinition switchParameter,
        KatanaParameterDefinition? variationParameter = null,
        KatanaParameterDefinition? typeParameter = null,
        KatanaParameterDefinition? levelParameter = null)
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
    }

    public string Key { get; }

    public string DisplayName { get; }

    public KatanaParameterDefinition SwitchParameter { get; }

    public KatanaParameterDefinition? VariationParameter { get; }

    public KatanaParameterDefinition? TypeParameter { get; }

    public KatanaParameterDefinition? LevelParameter { get; }
}
