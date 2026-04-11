using Kataka.Domain.Midi;

namespace Kataka.Tests;

public sealed class RolandSysExBuilderTests
{
    [Fact]
    public void BuildDataSet1_BuildsRolandFrameWithChecksum()
    {
        var message = RolandSysExBuilder.BuildDataSet1(
            deviceId: 0x10,
            modelId: [0x00, 0x00, 0x33],
            address: [0x60, 0x00, 0x00, 0x00],
            data: [0x01]);

        Assert.Equal(
            [0xF0, 0x41, 0x10, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x00, 0x00, 0x01, 0x1F, 0xF7],
            message.Bytes);
    }

    [Fact]
    public void IdentityRequest_MatchesReplyPattern()
    {
        var request = UniversalDeviceIdentity.CreateIdentityRequest();
        var reply = new SysExMessage([0xF0, 0x7E, 0x10, 0x06, 0x02, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x00, 0x00, 0xF7]);

        Assert.Equal([0xF0, 0x7E, 0x00, 0x06, 0x01, 0xF7], request.Bytes);
        Assert.True(UniversalDeviceIdentity.IsIdentityReply(reply));
    }

    [Fact]
    public void KatanaMkIIAmpVolumeReadRequest_UsesExpectedAddressAndChecksum()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpVolume);

        Assert.Equal(
            [0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x11, 0x60, 0x00, 0x06, 0x52, 0x00, 0x00, 0x00, 0x01, 0x47, 0xF7],
            request.Bytes);
    }

    [Fact]
    public void KatanaMkIIAmpVolumeReply_ParsesSingleByteValue()
    {
        var reply = new SysExMessage([0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x06, 0x52, 0x32, 0x16, 0xF7]);

        var parsed = KatanaMkIIProtocol.TryParseParameterReply(KatanaMkIIParameterCatalog.AmpVolume, reply, out var volume);

        Assert.True(parsed);
        Assert.Equal(50, volume);
    }

    [Fact]
    public void KatanaMkIIAmpGainReadRequest_UsesExpectedAddressAndChecksum()
    {
        var request = KatanaMkIIProtocol.CreateParameterReadRequest(KatanaMkIIParameterCatalog.AmpGain);

        Assert.Equal(
            [0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x11, 0x60, 0x00, 0x06, 0x51, 0x00, 0x00, 0x00, 0x01, 0x48, 0xF7],
            request.Bytes);
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
    }

    [Fact]
    public void KatanaMkIIParameterBlockReply_ParsesMultiBytePayload()
    {
        var reply = new SysExMessage([0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x06, 0x51, 0x4E, 0x38, 0x43, 0xF7]);

        var parsed = KatanaMkIIProtocol.TryParseParameterBlockReply([0x60, 0x00, 0x06, 0x51], 2, reply, out var data);

        Assert.True(parsed);
        Assert.Equal([0x4E, 0x38], data);
    }
}
