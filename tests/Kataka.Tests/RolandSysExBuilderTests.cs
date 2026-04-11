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
    public void KatanaMkIIVolumePedalReadRequest_UsesExpectedAddressAndChecksum()
    {
        var request = KatanaMkIIProtocol.CreateVolumePedalReadRequest();

        Assert.Equal(
            [0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x11, 0x60, 0x00, 0x06, 0x33, 0x00, 0x00, 0x00, 0x01, 0x66, 0xF7],
            request.Bytes);
    }

    [Fact]
    public void KatanaMkIIVolumePedalReply_ParsesSingleByteValue()
    {
        var reply = new SysExMessage([0xF0, 0x41, 0x00, 0x00, 0x00, 0x00, 0x33, 0x12, 0x60, 0x00, 0x06, 0x33, 0x32, 0x35, 0xF7]);

        var parsed = KatanaMkIIProtocol.TryParseVolumePedalReply(reply, out var volume);

        Assert.True(parsed);
        Assert.Equal(50, volume);
    }
}
