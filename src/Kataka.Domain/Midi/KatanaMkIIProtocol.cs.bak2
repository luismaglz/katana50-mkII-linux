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
        return CreateDataReadRequest(parameter.Address, 1);
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

        return CreateDataWriteRequest(parameter.Address, [value]);
    }

    public static SysExMessage CreateDataReadRequest(IReadOnlyList<byte> address, int length)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (address.Count != 4)
        {
            throw new ArgumentException("Katana parameter addresses must contain exactly 4 bytes.", nameof(address));
        }

        if (length <= 0 || length > 0x7F)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, "Requested read length must be in the 1..127 byte range.");
        }

        return RolandSysExBuilder.BuildDataRequest1(DeviceId, ModelId, address, [0x00, 0x00, 0x00, (byte)length]);
    }

    public static SysExMessage CreateDataWriteRequest(IReadOnlyList<byte> address, IReadOnlyList<byte> data)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(data);

        if (address.Count != 4)
        {
            throw new ArgumentException("Katana parameter addresses must contain exactly 4 bytes.", nameof(address));
        }

        if (data.Count == 0)
        {
            throw new ArgumentException("Katana data writes must contain at least one payload byte.", nameof(data));
        }

        if (data.Any(value => value > 0x7F))
        {
            throw new ArgumentOutOfRangeException(nameof(data), "Katana SysEx data bytes must be in the 0..127 range.");
        }

        return RolandSysExBuilder.BuildDataSet1(DeviceId, ModelId, address, data);
    }

    public static bool TryParseParameterReply(
        KatanaParameterDefinition parameter,
        SysExMessage message,
        out byte value)
    {
        ArgumentNullException.ThrowIfNull(parameter);
        value = 0;
        if (!TryParseParameterBlockReply(parameter.Address, 1, message, out var data))
        {
            return false;
        }

        value = data[0];
        return true;
    }

    public static bool TryParseParameterBlockReply(
        IReadOnlyList<byte> address,
        int expectedLength,
        SysExMessage message,
        out byte[] data)
    {
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(message);

        data = [];
        if (address.Count != 4 || expectedLength <= 0)
        {
            return false;
        }

        var bytes = message.Bytes;
        if (bytes.Count != 14 + expectedLength)
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

        if (!bytes.Skip(8).Take(4).SequenceEqual(address))
        {
            return false;
        }

        data = bytes.Skip(12).Take(expectedLength).ToArray();
        var checksum = bytes[^2];
        if (RolandChecksum.Calculate(address.Concat(data).ToArray()) != checksum)
        {
            data = [];
            return false;
        }

        return true;
    }
}
