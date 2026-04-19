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

    // User patch slots (stored patches, not live-edit)
    public const uint UserPatch1 = 0x10000000;
    public const uint UserPatch2 = 0x10010000;
    public const uint UserPatch3 = 0x10020000;
    public const uint UserPatch4 = 0x10030000;
    public const uint UserPatch5 = 0x10040000;
    public const uint UserPatch6 = 0x10050000;
    public const uint UserPatch7 = 0x10060000;
    public const uint UserPatch8 = 0x10070000;
    public const uint UserPatch9 = 0x10080000;

    // ── System block offsets ────────────────────────────────────────────────────
    public static class SystemBlocks
    {
        public const uint Main           = 0x00000000; // prm_prop_system (USB, system EQ)
        public const uint GlobalEqSelect = 0x0000002E; // prm_prop_sys_eq_sel
        public const uint GlobalEq1      = 0x00000030; // prm_prop_sys_eq (bank 1)
        public const uint GlobalEq2      = 0x00000050; // prm_prop_sys_eq (bank 2)
        public const uint GlobalEq3      = 0x00000070; // prm_prop_sys_eq (bank 3)
        public const uint Info           = 0x00010000; // prm_prop_info (patch num)
        public const uint Midi           = 0x00020000; // prm_prop_midi
    }

    // ── Patch block offsets (within Temporary or UserPatch areas) ───────────────
    public static class PatchBlocks
    {
        public const uint PatchName  = 0x00000000; // prm_prop_patch_name (16-byte name)
        public const uint Patch0     = 0x00000010; // prm_prop_patch_0: booster + preamp stored values + EQ1
        public const uint Eq2        = 0x00000060; // prm_prop_patch_eq2
        public const uint Mod        = 0x00000100; // prm_prop_patch_fx: Fx(1) / Mod
        public const uint Fx         = 0x00000300; // prm_prop_patch_fx: Fx(2) / FX
        public const uint Delay1     = 0x00000500; // prm_prop_patch_delay
        public const uint Delay2     = 0x00000520;
        public const uint Patch1     = 0x00000540; // prm_prop_patch_1: Reverb + PedalFX + SendReturn + NS
        public const uint Patch2     = 0x00000620; // prm_prop_patch_2: Chain, CabinetResonance, FxBox assign
        public const uint PatchStatus = 0x00000650; // prm_prop_patch_status: knob positions, LEDs
        public const uint Mk2V2      = 0x00000F10; // prm_prop_mk2_v2: Solo EQ/Delay (Ver200+)
        public const uint Contour1   = 0x00000F30; // prm_prop_contour (Ver200+)
        public const uint Contour2   = 0x00000F38;
        public const uint Contour3   = 0x00000F40;
    }

    // ── System Main parameter offsets (prm_prop_system at SystemBlocks.Main) ────
    public static class SystemMainParams
    {
        // USB
        public const uint UsbInOutMode  = 0x00;
        public const uint UsbInputLevel = 0x01;
        public const uint UsbOutLevel   = 0x02;
        public const uint UsbMixLevel   = 0x03;
        public const uint UsbLoopback   = 0x06;
        public const uint Usb2OutLevel  = 0x07;

        // Built-in system EQ (always-active EQ, separate from the 3 Global EQ banks)
        public const uint EqSw        = 0x10;
        public const uint EqType      = 0x11;
        public const uint EqPosition  = 0x12;
        public const uint EqLowCut    = 0x13;
        public const uint EqLowGain   = 0x14;
        public const uint EqLowMidFreq = 0x15;
        public const uint EqLowMidQ   = 0x16;
        public const uint EqLowMidGain = 0x17;
        public const uint EqHiMidFreq  = 0x18;
        public const uint EqHiMidQ     = 0x19;
        public const uint EqHiMidGain  = 0x1A;
        public const uint EqHighGain   = 0x1B;
        public const uint EqHighCut    = 0x1C;
        public const uint EqLevel      = 0x1D;
        public const uint EqGeq31Hz    = 0x1E;
        public const uint EqGeq62Hz    = 0x1F;
        public const uint EqGeq125Hz   = 0x20;
        public const uint EqGeq250Hz   = 0x21;
        public const uint EqGeq500Hz   = 0x22;
        public const uint EqGeq1kHz    = 0x23;
        public const uint EqGeq2kHz    = 0x24;
        public const uint EqGeq4kHz    = 0x25;
        public const uint EqGeq8kHz    = 0x26;
        public const uint EqGeq16kHz   = 0x27;
        public const uint EqGeqLevel   = 0x28;

        public const uint LineOutAirFeel = 0x29;
        public const uint ExpandMode     = 0x2A;
    }

    // ── Global EQ bank parameter offsets (prm_prop_sys_eq) ─────────────────────
    // NOTE: these banks (SysEq1/2/3) have NO per-bank SW — the on/off is
    // SystemMainParams.EqSw. Each bank stores only the EQ shape settings.
    public static class GlobalEqParams
    {
        public const uint Type       = 0x00; // 0=PARAMETRIC EQ, 1=GE-10
        public const uint Position   = 0x01; // Ver200
        public const uint LowCut     = 0x02;
        public const uint LowGain    = 0x03;
        public const uint LowMidFreq = 0x04;
        public const uint LowMidQ    = 0x05;
        public const uint LowMidGain = 0x06;
        public const uint HiMidFreq  = 0x07;
        public const uint HiMidQ     = 0x08;
        public const uint HiMidGain  = 0x09;
        public const uint HighGain   = 0x0A;
        public const uint HighCut    = 0x0B;
        public const uint Level      = 0x0C;
        public const uint Geq31Hz    = 0x0D;
        public const uint Geq62Hz    = 0x0E;
        public const uint Geq125Hz   = 0x0F;
        public const uint Geq250Hz   = 0x10;
        public const uint Geq500Hz   = 0x11;
        public const uint Geq1kHz    = 0x12;
        public const uint Geq2kHz    = 0x13;
        public const uint Geq4kHz    = 0x14;
        public const uint Geq8kHz    = 0x15;
        public const uint Geq16kHz   = 0x16;
        public const uint GeqLevel   = 0x17;
    }

    // ── Patch0 parameter offsets (prm_prop_patch_0) ─────────────────────────────
    public static class Patch0Params
    {
        // Booster (OD/DS)
        public const uint BoosterSw          = 0x00;
        public const uint BoosterType        = 0x01;
        public const uint BoosterDrive       = 0x02;
        public const uint BoosterBottom      = 0x03;
        public const uint BoosterTone        = 0x04;
        public const uint BoosterSoloSw      = 0x05;
        public const uint BoosterSoloLevel   = 0x06;
        public const uint BoosterEffectLevel = 0x07;
        public const uint BoosterDirectMix   = 0x08;

        // Preamp
        public const uint PreampType     = 0x11;
        public const uint PreampGain     = 0x12;
        public const uint PreampBass     = 0x14;
        public const uint PreampMiddle   = 0x15;
        public const uint PreampTreble   = 0x16;
        public const uint PreampPresence = 0x17;
        public const uint PreampLevel    = 0x18;
        public const uint PreampBright   = 0x19;
        public const uint PreampSoloSw   = 0x1B;
        public const uint PreampSoloLevel = 0x1C;

        // Embedded EQ1 (prm_prop_patch_0 inline, relative to Patch0 block)
        public const uint Eq1Sw          = 0x30;
        public const uint Eq1Type        = 0x31;
        public const uint Eq1LowCut      = 0x32;
        public const uint Eq1LowGain     = 0x33;
        public const uint Eq1LowMidFreq  = 0x34;
        public const uint Eq1LowMidQ     = 0x35;
        public const uint Eq1LowMidGain  = 0x36;
        public const uint Eq1HiMidFreq   = 0x37;
        public const uint Eq1HiMidQ      = 0x38;
        public const uint Eq1HiMidGain   = 0x39;
        public const uint Eq1HighGain    = 0x3A;
        public const uint Eq1HighCut     = 0x3B;
        public const uint Eq1Level       = 0x3C;
        public const uint Eq1Geq31Hz     = 0x3D;
        public const uint Eq1Geq62Hz     = 0x3E;
        public const uint Eq1Geq125Hz    = 0x3F;
        public const uint Eq1Geq250Hz    = 0x40;
        public const uint Eq1Geq500Hz    = 0x41;
        public const uint Eq1Geq1kHz     = 0x42;
        public const uint Eq1Geq2kHz     = 0x43;
        public const uint Eq1Geq4kHz     = 0x44;
        public const uint Eq1Geq8kHz     = 0x45;
        public const uint Eq1Geq16kHz    = 0x46;
        public const uint Eq1GeqLevel    = 0x47;
    }

    // ── PatchStatus parameter offsets (prm_prop_patch_status) ───────────────────
    public static class PatchStatusParams
    {
        // Knob positions (physical front-panel)
        public const uint AmpType     = 0x00; // PRM_KNOB_POS_TYPE  (0-4: ACOUSTIC/CLEAN/CRUNCH/LEAD/BROWN)
        public const uint Gain        = 0x01; // PRM_KNOB_POS_GAIN
        public const uint Volume      = 0x02; // PRM_KNOB_POS_VOLUME
        public const uint Bass        = 0x03; // PRM_KNOB_POS_BASS
        public const uint Middle      = 0x04; // PRM_KNOB_POS_MIDDLE
        public const uint Treble      = 0x05; // PRM_KNOB_POS_TREBLE
        public const uint Presence    = 0x06; // PRM_KNOB_POS_PRESENCE
        public const uint BoostLevel  = 0x07; // PRM_KNOB_POS_BOOST
        public const uint ModLevel    = 0x08; // PRM_KNOB_POS_MOD
        public const uint FxLevel     = 0x09; // PRM_KNOB_POS_FX
        public const uint DelayLevel  = 0x0A; // PRM_KNOB_POS_DELAY
        public const uint ReverbLevel = 0x0B; // PRM_KNOB_POS_REVERB

        // LED / variation states
        public const uint Variation      = 0x0C; // PRM_LED_STATE_VARI
        public const uint LedBooster     = 0x0D; // PRM_LED_STATE_BOOST
        public const uint LedMod         = 0x0E; // PRM_LED_STATE_MOD
        public const uint LedFx          = 0x0F; // PRM_LED_STATE_FX
        public const uint LedDelay       = 0x10; // PRM_LED_STATE_DELAY
        public const uint LedReverb      = 0x11; // PRM_LED_STATE_REVERB
    }

    // ── Patch1 parameter offsets (prm_prop_patch_1 — Reverb + PedalFX block) ────
    public static class Patch1Params
    {
        // Reverb
        public const uint ReverbSw          = 0x00;
        public const uint ReverbType        = 0x01;
        public const uint ReverbTime        = 0x02;
        public const uint ReverbPreDelay    = 0x03; // INTEGER2x7
        public const uint ReverbLowCut      = 0x05;
        public const uint ReverbHighCut     = 0x06;
        public const uint ReverbDensity     = 0x07;
        public const uint ReverbEffectLevel = 0x08;
        public const uint ReverbDirectMix   = 0x09;
        public const uint ReverbSpringColor = 0x0B;

        // Pedal FX (starts at 0x10 within Patch1)
        public const uint PedalFxSw             = 0x10;
        public const uint PedalFxType           = 0x11;
        public const uint PedalFxWahType        = 0x12;
        public const uint PedalFxWahPedalPos    = 0x13;
        public const uint PedalFxWahMin         = 0x14;
        public const uint PedalFxWahMax         = 0x15;
        public const uint PedalFxWahEffectLevel = 0x16;
        public const uint PedalFxWahDirectMix   = 0x17;
        public const uint PedalFxBendPitch      = 0x18;
        public const uint PedalFxBendPedalPos   = 0x19;
        public const uint PedalFxBendEffectLevel = 0x1A;
        public const uint PedalFxBendDirectMix  = 0x1B;
        public const uint PedalFxEvh95Pos       = 0x1C;
        public const uint PedalFxEvh95Min       = 0x1D;
        public const uint PedalFxEvh95Max       = 0x1E;
        public const uint PedalFxEvh95EffectLevel = 0x1F;
        public const uint PedalFxEvh95DirectMix = 0x20;
        public const uint FootVolume            = 0x21;

        // Solo
        public const uint SoloSw    = 0x54;
        public const uint SoloLevel = 0x55;
    }

    // ── Patch2 parameter offsets (prm_prop_patch_2) ─────────────────────────────
    public static class Patch2Params
    {
        public const uint ChainPattern      = 0x00; // PRM_CHAIN_PTN
        public const uint PosSendReturn     = 0x01;
        public const uint PosEq             = 0x02;
        public const uint PosPedalFx        = 0x03;
        public const uint CabinetResonance  = 0x23; // PRM_CABINET_RESONANCE
    }

    // ── Delay parameter offsets (prm_prop_patch_delay) ──────────────────────────
    public static class DelayParams
    {
        public const uint Sw          = 0x00;
        public const uint Type        = 0x01;
        public const uint DelayTime   = 0x02; // INTEGER2x7 (2 bytes)
        public const uint Feedback    = 0x04;
        public const uint HighCut     = 0x05;
        public const uint EffectLevel = 0x06;
        public const uint DirectMix   = 0x07;
        public const uint TapTime     = 0x08;
        public const uint ModRate     = 0x13;
        public const uint ModDepth    = 0x14;
        public const uint Range       = 0x15;
        public const uint Filter      = 0x16;
        public const uint FeedbackPhase = 0x17;
        public const uint DelayPhase  = 0x18;
        public const uint ModSw       = 0x19;
    }

    // ── Contour parameter offsets (prm_prop_contour) ─────────────────────────────
    public static class ContourParams
    {
        public const uint Type      = 0x00;
        public const uint FreqShift = 0x01;
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
