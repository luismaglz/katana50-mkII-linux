using Kataka.Domain.Midi;

namespace Kataka.Tests;

public sealed class RolandChecksumTests
{
    [Fact]
    public void Calculate_ReturnsExpectedRolandChecksum()
    {
        var checksum = RolandChecksum.Calculate(0x0D, 0x00, 0x12, 0x00, 0x00, 0x00, 0x00, 0x13);

        Assert.Equal(0x4E, checksum);
    }
}
