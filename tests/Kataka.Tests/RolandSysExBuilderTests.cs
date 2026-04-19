using Kataka.Domain.Midi;

namespace Kataka.Tests;

public sealed class RolandSysExBuilderTests
{
    [Fact]
    public void BuildDataSet1_BuildsRolandFrameWithChecksum()
    {
        var message = RolandSysExBuilder.BuildDataSet1(
            0x10,
            [0x00, 0x00, 0x33],
            [0x60, 0x00, 0x00, 0x00],
            [0x01]);

        Assert.Equal(
            [0xF0, 0x41, 0x10, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x00, 0x00, 0x01, 0x1F, 0xF7],
            message.Bytes);
    }

    [Fact]
    public void IdentityRequest_MatchesReplyPattern()
    {
        var request = UniversalDeviceIdentity.CreateIdentityRequest();
        var reply = new SysExMessage([
            0xF0, 0x7E, 0x10, 0x06, 0x02, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0xF7
        ]);

        Assert.Equal([0xF0, 0x7E, 0x00, 0x06, 0x01, 0xF7], request.Bytes);
        Assert.True(UniversalDeviceIdentity.IsIdentityReply(reply));
    }

    [Fact]
    public void KatanaMkIIAmpVolumeReadRequest_UsesExpectedAddressAndChecksum()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpVolume);

        Assert.Equal(
            [
                0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x11, 0x60, 0x00, 0x06, 0x52, 0x00, 0x00, 0x00, 0x01, 0x47,
                0xF7
            ],
            request.Bytes);
    }

    [Fact]
    public void KatanaMkIIAmpVolumeReply_ParsesSingleByteValue()
    {
        var reply = new SysExMessage([
            0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x06, 0x52, 0x32, 0x16, 0xF7
        ]);

        var parsed =
            KatanaMkIIProtocol.TryParseParameterReply(KatanaMkIIParameterCatalog.AmpVolume, reply, out var volume);

        Assert.True(parsed);
        Assert.Equal(50, volume);
    }

    [Fact]
    public void KatanaMkIIAmpGainReadRequest_UsesExpectedAddressAndChecksum()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpGain);

        Assert.Equal(
            [
                0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x11, 0x60, 0x00, 0x06, 0x51, 0x00, 0x00, 0x00, 0x01, 0x48,
                0xF7
            ],
            request.Bytes);
    }

    [Fact]
    public void KatanaMkIIPatchLevelReadRequest_UsesExpectedAddressAndChecksum()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.PatchLevel);

        Assert.Equal(
            [
                0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x11, 0x60, 0x00, 0x06, 0x4C, 0x00, 0x00, 0x00, 0x01, 0x4D,
                0xF7
            ],
            request.Bytes);
    }

    [Fact]
    public void KatanaMkIIDelayTimeWriteRequest_BuildsMultiBytePayload()
    {
        var message = KatanaMkIIProtocol.CreateDataWriteRequest(
            KatanaMkIIParameterCatalog.DelayTimeAddress,
            [0x03, 0x74]);

        Assert.Equal(
            [0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x05, 0x02, 0x03, 0x74, 0x22, 0xF7],
            message.Bytes);
    }

    [Fact]
    public void KatanaMkIIParameterCatalog_ExposesAmpEditorControls()
    {
        Assert.Collection(
            KatanaMkIIParameterCatalog.AmpEditorControls,
            control => Assert.Equal("Gain", control.DisplayName),
            control => Assert.Equal("Volume", control.DisplayName),
            control => Assert.Equal("Bass", control.DisplayName),
            control => Assert.Equal("Middle", control.DisplayName),
            control => Assert.Equal("Treble", control.DisplayName),
            control => Assert.Equal("Presence", control.DisplayName));
    }

    [Fact]
    public void KatanaMkIIParameterCatalog_ExposesPanelEffects()
    {
        Assert.Collection(
            KatanaMkIIParameterCatalog.PanelEffects,
            effect => Assert.Equal("Booster", effect.DisplayName),
            effect => Assert.Equal("Mod", effect.DisplayName),
            effect => Assert.Equal("FX", effect.DisplayName),
            effect => Assert.Equal("Delay", effect.DisplayName),
            effect => Assert.Equal("Delay 2", effect.DisplayName),
            effect => Assert.Equal("Reverb", effect.DisplayName));

        Assert.Null(KatanaMkIIParameterCatalog.PanelEffects[4].VariationParameter);
        Assert.NotNull(KatanaMkIIParameterCatalog.PanelEffects[0].TypeParameter);
        Assert.Equal([0x60, 0x00, 0x05, 0x01], KatanaMkIIParameterCatalog.DelayType.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x21], KatanaMkIIParameterCatalog.Delay2Type.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x41], KatanaMkIIParameterCatalog.ReverbType.Address);
    }

    [Fact]
    public void KatanaMkIIParameterCatalog_ExposesConfirmedPanelVariationAddresses()
    {
        Assert.Equal([0x60, 0x00, 0x06, 0x39], KatanaMkIIParameterCatalog.BoosterVariation.Address);
        Assert.Equal([0x60, 0x00, 0x06, 0x3A], KatanaMkIIParameterCatalog.ModVariation.Address);
        Assert.Equal([0x60, 0x00, 0x06, 0x3B], KatanaMkIIParameterCatalog.FxVariation.Address);
        Assert.Equal([0x60, 0x00, 0x06, 0x3C], KatanaMkIIParameterCatalog.DelayVariation.Address);
        Assert.Equal([0x60, 0x00, 0x06, 0x3D], KatanaMkIIParameterCatalog.ReverbVariation.Address);
        Assert.Null(KatanaMkIIParameterCatalog.PanelEffects.Single(effect => effect.Key == "delay2")
            .VariationParameter);
    }

    [Fact]
    public void KatanaMkIIParameterCatalog_ExposesConfirmedTypeRangesAndSkippedValues()
    {
        Assert.Equal((byte)22, KatanaMkIIParameterCatalog.BoosterType.Maximum);
        Assert.Equal<byte>([0x07], KatanaMkIIParameterCatalog.BoosterType.SkippedValues);

        Assert.Equal((byte)39, KatanaMkIIParameterCatalog.ModType.Maximum);
        Assert.Equal<byte>([0x05, 0x08, 0x0B, 0x0D, 0x11, 0x18, 0x1E, 0x20, 0x21, 0x22],
            KatanaMkIIParameterCatalog.ModType.SkippedValues);
        Assert.Equal(KatanaMkIIParameterCatalog.ModType.SkippedValues, KatanaMkIIParameterCatalog.FxType.SkippedValues);

        Assert.Equal((byte)10, KatanaMkIIParameterCatalog.DelayType.Maximum);
        Assert.Equal((byte)10, KatanaMkIIParameterCatalog.Delay2Type.Maximum);
        Assert.Equal((byte)6, KatanaMkIIParameterCatalog.ReverbType.Maximum);
    }

    [Fact]
    public void KatanaMkIIParameterCatalog_ExposesPedalFxBlock()
    {
        Assert.Equal([0x60, 0x00, 0x05, 0x50], KatanaMkIIParameterCatalog.PedalFxSwitch.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x51], KatanaMkIIParameterCatalog.PedalFxType.Address);
        Assert.Equal([0x60, 0x00, 0x06, 0x23], KatanaMkIIParameterCatalog.PedalFxPosition.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x52], KatanaMkIIParameterCatalog.PedalFxWahType.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x53], KatanaMkIIParameterCatalog.PedalFxWahPedalPosition.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x57], KatanaMkIIParameterCatalog.PedalFxWahDirectMix.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x58], KatanaMkIIParameterCatalog.PedalFxBendPitch.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x59], KatanaMkIIParameterCatalog.PedalFxBendPedalPosition.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x5A], KatanaMkIIParameterCatalog.PedalFxBendEffectLevel.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x5B], KatanaMkIIParameterCatalog.PedalFxBendDirectMix.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x5C], KatanaMkIIParameterCatalog.PedalFxEvh95Position.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x60], KatanaMkIIParameterCatalog.PedalFxEvh95DirectMix.Address);
        Assert.Equal([0x60, 0x00, 0x05, 0x61], KatanaMkIIParameterCatalog.FootVolume.Address);
        Assert.Collection(
            KatanaMkIIParameterCatalog.PedalFxReadParameters,
            parameter => Assert.Equal("pedal-fx-switch", parameter.Key),
            parameter => Assert.Equal("pedal-fx-type", parameter.Key),
            parameter => Assert.Equal("pedal-fx-position", parameter.Key),
            parameter => Assert.Equal("pedal-fx-wah-type", parameter.Key),
            parameter => Assert.Equal("pedal-fx-wah-position", parameter.Key),
            parameter => Assert.Equal("pedal-fx-wah-min", parameter.Key),
            parameter => Assert.Equal("pedal-fx-wah-max", parameter.Key),
            parameter => Assert.Equal("pedal-fx-wah-effect-level", parameter.Key),
            parameter => Assert.Equal("pedal-fx-wah-direct-mix", parameter.Key),
            parameter => Assert.Equal("pedal-fx-bend-pitch", parameter.Key),
            parameter => Assert.Equal("pedal-fx-bend-position", parameter.Key),
            parameter => Assert.Equal("pedal-fx-bend-effect-level", parameter.Key),
            parameter => Assert.Equal("pedal-fx-bend-direct-mix", parameter.Key),
            parameter => Assert.Equal("pedal-fx-evh95-position", parameter.Key),
            parameter => Assert.Equal("pedal-fx-evh95-min", parameter.Key),
            parameter => Assert.Equal("pedal-fx-evh95-max", parameter.Key),
            parameter => Assert.Equal("pedal-fx-evh95-effect-level", parameter.Key),
            parameter => Assert.Equal("pedal-fx-evh95-direct-mix", parameter.Key),
            parameter => Assert.Equal("pedal-fx-foot-volume", parameter.Key));
    }

    [Fact]
    public void KatanaMkIICurrentPanelChannelReadRequest_UsesExpectedAddressAndChecksum()
    {
        var request = KatanaMkIIProtocol.CreateDataReadRequest([0x00, 0x01, 0x00, 0x00], 2);

        Assert.Equal(
            [
                0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x11, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x7D,
                0xF7
            ],
            request.Bytes);
    }

    [Fact]
    public void KatanaMkIIParameterBlockReply_ParsesMultiBytePayload()
    {
        var reply = new SysExMessage([
            0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x06, 0x51, 0x4E, 0x38, 0x43, 0xF7
        ]);

        var parsed = KatanaMkIIProtocol.TryParseParameterBlockReply([0x60, 0x00, 0x06, 0x51], 2, reply, out var data);

        Assert.True(parsed);
        Assert.Equal([0x4E, 0x38], data);
    }

    [Fact]
    public void KatanaMkIIAmpKnobBlockReply_ParsesLiveKnobPositions()
    {
        var reply = new SysExMessage([
            0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x06, 0x51, 0x36, 0x2A, 0x2C, 0x32, 0x2B, 0x2A,
            0x36, 0xF7
        ]);

        var parsed = KatanaMkIIProtocol.TryParseParameterBlockReply([0x60, 0x00, 0x06, 0x51], 6, reply, out var data);

        Assert.True(parsed);
        Assert.Equal([0x36, 0x2A, 0x2C, 0x32, 0x2B, 0x2A], data);
        Assert.Equal([0x60, 0x00, 0x06, 0x53], KatanaMkIIParameterCatalog.AmpBass.Address);
        Assert.Equal([0x60, 0x00, 0x06, 0x56], KatanaMkIIParameterCatalog.AmpPresence.Address);
    }
}
