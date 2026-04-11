namespace Kataka.Domain.Midi;

public sealed class KatanaPedalFxDefinition
{
    public KatanaPedalFxDefinition(
        KatanaParameterDefinition switchParameter,
        KatanaParameterDefinition typeParameter,
        KatanaParameterDefinition positionParameter,
        KatanaParameterDefinition wahTypeParameter,
        KatanaParameterDefinition wahPedalPositionParameter,
        KatanaParameterDefinition wahPedalMinimumParameter,
        KatanaParameterDefinition wahPedalMaximumParameter,
        KatanaParameterDefinition wahEffectLevelParameter,
        KatanaParameterDefinition wahDirectMixParameter,
        KatanaParameterDefinition bendPitchParameter,
        KatanaParameterDefinition bendPedalPositionParameter,
        KatanaParameterDefinition bendEffectLevelParameter,
        KatanaParameterDefinition bendDirectMixParameter,
        KatanaParameterDefinition evh95PositionParameter,
        KatanaParameterDefinition evh95MinimumParameter,
        KatanaParameterDefinition evh95MaximumParameter,
        KatanaParameterDefinition evh95EffectLevelParameter,
        KatanaParameterDefinition evh95DirectMixParameter,
        KatanaParameterDefinition footVolumeParameter)
    {
        SwitchParameter = switchParameter;
        TypeParameter = typeParameter;
        PositionParameter = positionParameter;
        WahTypeParameter = wahTypeParameter;
        WahPedalPositionParameter = wahPedalPositionParameter;
        WahPedalMinimumParameter = wahPedalMinimumParameter;
        WahPedalMaximumParameter = wahPedalMaximumParameter;
        WahEffectLevelParameter = wahEffectLevelParameter;
        WahDirectMixParameter = wahDirectMixParameter;
        BendPitchParameter = bendPitchParameter;
        BendPedalPositionParameter = bendPedalPositionParameter;
        BendEffectLevelParameter = bendEffectLevelParameter;
        BendDirectMixParameter = bendDirectMixParameter;
        Evh95PositionParameter = evh95PositionParameter;
        Evh95MinimumParameter = evh95MinimumParameter;
        Evh95MaximumParameter = evh95MaximumParameter;
        Evh95EffectLevelParameter = evh95EffectLevelParameter;
        Evh95DirectMixParameter = evh95DirectMixParameter;
        FootVolumeParameter = footVolumeParameter;
    }

    public KatanaParameterDefinition SwitchParameter { get; }

    public KatanaParameterDefinition TypeParameter { get; }

    public KatanaParameterDefinition PositionParameter { get; }

    public KatanaParameterDefinition WahTypeParameter { get; }

    public KatanaParameterDefinition WahPedalPositionParameter { get; }

    public KatanaParameterDefinition WahPedalMinimumParameter { get; }

    public KatanaParameterDefinition WahPedalMaximumParameter { get; }

    public KatanaParameterDefinition WahEffectLevelParameter { get; }

    public KatanaParameterDefinition WahDirectMixParameter { get; }

    public KatanaParameterDefinition BendPitchParameter { get; }

    public KatanaParameterDefinition BendPedalPositionParameter { get; }

    public KatanaParameterDefinition BendEffectLevelParameter { get; }

    public KatanaParameterDefinition BendDirectMixParameter { get; }

    public KatanaParameterDefinition Evh95PositionParameter { get; }

    public KatanaParameterDefinition Evh95MinimumParameter { get; }

    public KatanaParameterDefinition Evh95MaximumParameter { get; }

    public KatanaParameterDefinition Evh95EffectLevelParameter { get; }

    public KatanaParameterDefinition Evh95DirectMixParameter { get; }

    public KatanaParameterDefinition FootVolumeParameter { get; }
}
