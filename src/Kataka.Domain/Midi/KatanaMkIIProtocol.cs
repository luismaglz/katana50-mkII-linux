using System.Linq;

namespace Kataka.Domain.Midi;

public static class KatanaMkIIProtocol
{
    private static readonly byte[] ModelId = [0x00, 0x00, 0x00, 0x33];
    private static readonly byte[] SingleByteSize = [0x00, 0x00, 0x00, 0x01];
    private static readonly byte[] AmpVolumeAddress = [0x60, 0x00, 0x06, 0x52];

    private const byte RolandManufacturerId = 0x41;
    private const byte DeviceId = 0x00;
    private const byte DataSetCommand = 0x12;

    public static SysExMessage CreateAmpVolumeReadRequest() =>
        RolandSysExBuilder.BuildDataRequest1(DeviceId, ModelId, AmpVolumeAddress, SingleByteSize);

    public static SysExMessage CreateAmpVolumeWriteRequest(byte value)
    {
        if (value > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Katana amp volume must be between 0 and 100.");
        }

        return RolandSysExBuilder.BuildDataSet1(DeviceId, ModelId, AmpVolumeAddress, [value]);
    }

    public static bool TryParseAmpVolumeReply(SysExMessage message, out byte value)
    {
        ArgumentNullException.ThrowIfNull(message);

        value = 0;
        var bytes = message.Bytes;
        if (bytes.Count != 15)
        {
            return false;
        }

        if (bytes[0] != 0xF0 ||
            bytes[1] != RolandManufacturerId ||
            bytes[2] != DeviceId ||
            bytes[3] != ModelId[0] ||
            bytes[4] != ModelId[1] ||
            bytes[5] != ModelId[2] ||
            bytes[6] != ModelId[3] ||
            bytes[7] != DataSetCommand ||
            bytes[^1] != 0xF7)
        {
            return false;
        }

        if (!bytes.Skip(8).Take(4).SequenceEqual(AmpVolumeAddress))
        {
            return false;
        }

        var data = bytes.Skip(12).Take(1).ToArray();
        var checksum = bytes[^2];
        if (RolandChecksum.Calculate(AmpVolumeAddress.Concat(data).ToArray()) != checksum)
        {
            return false;
        }

        value = data[0];
        return true;
    }
}
