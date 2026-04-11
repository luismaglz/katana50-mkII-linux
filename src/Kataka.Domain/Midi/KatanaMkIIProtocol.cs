using System.Linq;

namespace Kataka.Domain.Midi;

public static class KatanaMkIIProtocol
{
    private static readonly byte[] ModelId = [0x00, 0x00, 0x00, 0x33];
    private static readonly byte[] SingleByteSize = [0x00, 0x00, 0x00, 0x01];

    private const byte RolandManufacturerId = 0x41;
    private const byte DeviceId = 0x00;
    private const byte DataSetCommand = 0x12;

    public static SysExMessage CreateParameterReadRequest(KatanaParameterDefinition parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        return RolandSysExBuilder.BuildDataRequest1(DeviceId, ModelId, parameter.Address, SingleByteSize);
    }

    public static SysExMessage CreateParameterWriteRequest(KatanaParameterDefinition parameter, byte value)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        if (value < parameter.Minimum || value > parameter.Maximum)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"{parameter.DisplayName} must be between {parameter.Minimum} and {parameter.Maximum}.");
        }

        return RolandSysExBuilder.BuildDataSet1(DeviceId, ModelId, parameter.Address, [value]);
    }

    public static bool TryParseParameterReply(
        KatanaParameterDefinition parameter,
        SysExMessage message,
        out byte value)
    {
        ArgumentNullException.ThrowIfNull(parameter);
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

        if (!bytes.Skip(8).Take(4).SequenceEqual(parameter.Address))
        {
            return false;
        }

        var data = bytes.Skip(12).Take(1).ToArray();
        var checksum = bytes[^2];
        if (RolandChecksum.Calculate(parameter.Address.Concat(data).ToArray()) != checksum)
        {
            return false;
        }

        value = data[0];
        return true;
    }
}
