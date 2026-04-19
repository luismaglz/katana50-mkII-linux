namespace Kataka.Domain.Midi;

/// <summary>RolandSysExBuilder - auto-generated summary.</summary>
public static class RolandSysExBuilder
{
    private const byte RolandManufacturerId = 0x41;
    private const byte DataRequestCommand = 0x11;
    private const byte DataSetCommand = 0x12;

    /// <summary>Auto-generated: static SysExMessage BuildDataRequest1(</summary>
    public static SysExMessage BuildDataRequest1(
        byte deviceId,
        IReadOnlyList<byte> modelId,
        IReadOnlyList<byte> address,
        IReadOnlyList<byte> size)
    {
        return Build(deviceId, modelId, DataRequestCommand, address.Concat(size).ToArray());
    }

    /// <summary>Auto-generated: static SysExMessage BuildDataSet1(</summary>
    public static SysExMessage BuildDataSet1(
        byte deviceId,
        IReadOnlyList<byte> modelId,
        IReadOnlyList<byte> address,
        IReadOnlyList<byte> data)
    {
        return Build(deviceId, modelId, DataSetCommand, address.Concat(data).ToArray());
    }

    private static SysExMessage Build(
        byte deviceId,
        IReadOnlyList<byte> modelId,
        byte commandId,
        IReadOnlyList<byte> addressAndData)
    {
        ArgumentNullException.ThrowIfNull(modelId);
        ArgumentNullException.ThrowIfNull(addressAndData);

        if (modelId.Count == 0)
        {
            throw new ArgumentException("Model ID must contain at least one byte.", nameof(modelId));
        }

        if (addressAndData.Count == 0)
        {
            throw new ArgumentException("Address/data payload must contain at least one byte.", nameof(addressAndData));
        }

        var payload = addressAndData.ToArray();
        var checksum = RolandChecksum.Calculate(payload);

        var bytes = new List<byte>(2 + 1 + 1 + modelId.Count + 1 + payload.Length + 2)
        {
            0xF0,
            RolandManufacturerId,
            deviceId,
        };

        bytes.AddRange(modelId);
        bytes.Add(commandId);
        bytes.AddRange(payload);
        bytes.Add(checksum);
        bytes.Add(0xF7);

        return new SysExMessage(bytes);
    }
}
