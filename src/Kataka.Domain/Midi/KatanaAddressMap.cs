namespace Kataka.Domain.Midi;

/// <summary>
/// Roland SysEx address map for the Katana MkII, mirroring the structure in address_map.js.
/// Absolute address = BaseArea + BlockOffset + ParameterOffset.
/// </summary>
public static class KatanaAddressMap
{
    // ── Memory area base addresses ──────────────────────────────────────────────
    public const uint System    = 0x00000000;
    public const uint Temporary = 0x60000000;
    public const uint HwStatus  = 0x7F010200;

    // ── System block offsets ────────────────────────────────────────────────────
    public static class SystemBlocks
    {
        public const uint Main           = 0x00000000; // prm_prop_system
        public const uint GlobalEqSelect = 0x0000002E; // prm_prop_sys_eq_sel
        public const uint GlobalEq1      = 0x00000030; // prm_prop_sys_eq (bank 1)
        public const uint GlobalEq2      = 0x00000050; // prm_prop_sys_eq (bank 2)
        public const uint GlobalEq3      = 0x00000070; // prm_prop_sys_eq (bank 3)
        public const uint Midi           = 0x00020000; // prm_prop_midi
    }

    // ── Patch block offsets (within Temporary or UserPatch areas) ───────────────
    public static class PatchBlocks
    {
        public const uint Patch0      = 0x00000010; // prm_prop_patch_0: preamp, booster stored values
        public const uint Eq2         = 0x00000060; // prm_prop_patch_eq2
        public const uint Mod         = 0x00000100; // prm_prop_patch_fx: Fx(1) / Mod
        public const uint Fx          = 0x00000300; // prm_prop_patch_fx: Fx(2) / FX
        public const uint Delay1      = 0x00000500; // prm_prop_patch_delay
        public const uint Delay2      = 0x00000520;
        public const uint Reverb      = 0x00000540; // prm_prop_patch_1
        public const uint PedalFx     = 0x00000620; // prm_prop_patch_2
        public const uint PatchStatus = 0x00000650; // prm_prop_patch_status: knob positions, LEDs
        public const uint Mk2V2       = 0x00000F10; // prm_prop_mk2_v2: Solo EQ/Delay (Ver200+)
        public const uint Contour1    = 0x00000F30; // prm_prop_contour (Ver200+)
        public const uint Contour2    = 0x00000F38;
        public const uint Contour3    = 0x00000F40;
    }

    // ── Global EQ parameter offsets (prm_prop_sys_eq) ──────────────────────────
    public static class GlobalEqParams
    {
        public const uint Sw         = 0x00;
        public const uint Type       = 0x01; // 0=PARAMETRIC EQ, 1=GE-10
        public const uint Position   = 0x02; // Ver200
        public const uint LowCut     = 0x03;
        public const uint LowGain    = 0x04;
        public const uint LowMidFreq = 0x05;
        public const uint LowMidQ    = 0x06;
        public const uint LowMidGain = 0x07;
        public const uint HiMidFreq  = 0x08;
        public const uint HiMidQ     = 0x09;
        public const uint HiMidGain  = 0x0A;
        public const uint HighGain   = 0x0B;
        public const uint HighCut    = 0x0C;
        public const uint Level      = 0x0D;
        public const uint Geq31Hz    = 0x0E;
        public const uint Geq62Hz    = 0x0F;
        public const uint Geq125Hz   = 0x10;
        public const uint Geq250Hz   = 0x11;
        public const uint Geq500Hz   = 0x12;
        public const uint Geq1kHz    = 0x13;
        public const uint Geq2kHz    = 0x14;
        public const uint Geq4kHz    = 0x15;
        public const uint Geq8kHz    = 0x16;
        public const uint Geq16kHz   = 0x17;
        public const uint GeqLevel   = 0x18;
    }

    // ── Address computation helper ──────────────────────────────────────────────
    /// <summary>
    /// Computes the 4-byte Roland SysEx address from the three address components.
    /// </summary>
    public static byte[] ComputeAddress(uint baseArea, uint blockOffset, uint paramOffset)
    {
        var abs = baseArea + blockOffset + paramOffset;
        return [(byte)(abs >> 24), (byte)(abs >> 16), (byte)(abs >> 8), (byte)abs];
    }
}
