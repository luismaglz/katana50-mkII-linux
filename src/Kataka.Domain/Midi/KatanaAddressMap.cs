namespace Kataka.Domain.Midi;

/// <summary>
///     Roland SysEx address map for the Katana MkII, mirroring the structure in address_map.js.
///     Absolute address = BaseArea + BlockOffset + ParameterOffset.
/// </summary>
public static class KatanaAddressMap
{
    /// <summary> Memory area base addresses ────────────────────────────────────────────── </summary>
    public const uint System = 0x00000000;

    /// <summary>Parameter offset constant 'Temporary' from address_map.js.</summary>
    public const uint Temporary = 0x60000000;

    /// <summary>Parameter offset constant 'HwStatus' from address_map.js.</summary>
    public const uint HwStatus = 0x7F010200;

    // User patch slots (stored patches, not live-edit)
    /// <summary>Parameter offset constant 'UserPatch1' from address_map.js.</summary>
    public const uint UserPatch1 = 0x10000000;

    /// <summary>Parameter offset constant 'UserPatch2' from address_map.js.</summary>
    public const uint UserPatch2 = 0x10010000;

    /// <summary>Parameter offset constant 'UserPatch3' from address_map.js.</summary>
    public const uint UserPatch3 = 0x10020000;

    /// <summary>Parameter offset constant 'UserPatch4' from address_map.js.</summary>
    public const uint UserPatch4 = 0x10030000;

    /// <summary>Parameter offset constant 'UserPatch5' from address_map.js.</summary>
    public const uint UserPatch5 = 0x10040000;

    /// <summary>Parameter offset constant 'UserPatch6' from address_map.js.</summary>
    public const uint UserPatch6 = 0x10050000;

    /// <summary>Parameter offset constant 'UserPatch7' from address_map.js.</summary>
    public const uint UserPatch7 = 0x10060000;

    /// <summary>Parameter offset constant 'UserPatch8' from address_map.js.</summary>
    public const uint UserPatch8 = 0x10070000;

    /// <summary>Parameter offset constant 'UserPatch9' from address_map.js.</summary>
    public const uint UserPatch9 = 0x10080000;

    /// <summary> Address computation helper ────────────────────────────────────────────── </summary>
    /// <summary>
    ///     Computes the 4-byte Roland SysEx address from the three address components.
    /// </summary>
    public static byte[] ComputeAddress(uint baseArea, uint blockOffset, uint paramOffset)
    {
        var abs = baseArea + blockOffset + paramOffset;
        return [(byte)(abs >> 24), (byte)(abs >> 16), (byte)(abs >> 8), (byte)abs];
    }

    /// <summary> System block offsets ──────────────────────────────────────────────────── </summary>
    public static class SystemBlocks
    {
        /// <summary>Parameter offset constant 'Main' from address_map.js.</summary>
        public const uint Main = 0x00000000; // prm_prop_system (USB, system EQ)

        /// <summary>Parameter offset constant 'GlobalEqSelect' from address_map.js.</summary>
        public const uint GlobalEqSelect = 0x0000002E; // prm_prop_sys_eq_sel

        /// <summary>Parameter offset constant 'GlobalEq1' from address_map.js.</summary>
        public const uint GlobalEq1 = 0x00000030; // prm_prop_sys_eq (bank 1)

        /// <summary>Parameter offset constant 'GlobalEq2' from address_map.js.</summary>
        public const uint GlobalEq2 = 0x00000050; // prm_prop_sys_eq (bank 2)

        /// <summary>Parameter offset constant 'GlobalEq3' from address_map.js.</summary>
        public const uint GlobalEq3 = 0x00000070; // prm_prop_sys_eq (bank 3)

        /// <summary>Parameter offset constant 'Info' from address_map.js.</summary>
        public const uint Info = 0x00010000; // prm_prop_info (patch num)

        /// <summary>Parameter offset constant 'Midi' from address_map.js.</summary>
        public const uint Midi = 0x00020000; // prm_prop_midi
    }

    /// <summary> Patch block offsets (within Temporary or UserPatch areas) ─────────────── </summary>
    public static class PatchBlocks
    {
        /// <summary>Parameter offset constant 'PatchName' from address_map.js.</summary>
        public const uint PatchName = 0x00000000; // prm_prop_patch_name (16-byte name)

        /// <summary>Parameter offset constant 'Patch0' from address_map.js.</summary>
        public const uint Patch0 = 0x00000010; // prm_prop_patch_0: booster + preamp stored values + EQ1

        /// <summary>Parameter offset constant 'Eq2' from address_map.js.</summary>
        public const uint Eq2 = 0x00000060; // prm_prop_patch_eq2

        /// <summary>Parameter offset constant 'Mod' from address_map.js.</summary>
        public const uint Mod = 0x00000100; // prm_prop_patch_fx: Fx(1) / Mod

        /// <summary>Parameter offset constant 'Fx' from address_map.js.</summary>
        public const uint Fx = 0x00000300; // prm_prop_patch_fx: Fx(2) / FX

        /// <summary>Parameter offset constant 'Delay1' from address_map.js.</summary>
        public const uint Delay1 = 0x00000500; // prm_prop_patch_delay

        /// <summary>Parameter offset constant 'Delay2' from address_map.js.</summary>
        public const uint Delay2 = 0x00000520;

        /// <summary>Parameter offset constant 'Patch1' from address_map.js.</summary>
        public const uint Patch1 = 0x00000540; // prm_prop_patch_1: Reverb + PedalFX + SendReturn + NS

        /// <summary>Parameter offset constant 'Patch2' from address_map.js.</summary>
        public const uint Patch2 = 0x00000620; // prm_prop_patch_2: Chain, CabinetResonance, FxBox assign

        /// <summary>Parameter offset constant 'PatchStatus' from address_map.js.</summary>
        public const uint PatchStatus = 0x00000650; // prm_prop_patch_status: knob positions, LEDs

        /// <summary>Parameter offset constant 'Mk2V2' from address_map.js.</summary>
        public const uint Mk2V2 = 0x00000F10; // prm_prop_mk2_v2: Solo EQ/Delay (Ver200+)

        /// <summary>Parameter offset constant 'Contour1' from address_map.js.</summary>
        public const uint Contour1 = 0x00000F30; // prm_prop_contour (Ver200+)

        /// <summary>Parameter offset constant 'Contour2' from address_map.js.</summary>
        public const uint Contour2 = 0x00000F38;

        /// <summary>Parameter offset constant 'Contour3' from address_map.js.</summary>
        public const uint Contour3 = 0x00000F40;
    }

    /// <summary> System Main parameter offsets (prm_prop_system at SystemBlocks.Main) ──── </summary>
    public static class SystemMainParams
    {
        // USB
        /// <summary>Parameter offset constant 'UsbInOutMode' from address_map.js.</summary>
        public const uint UsbInOutMode = 0x00;

        /// <summary>Parameter offset constant 'UsbInputLevel' from address_map.js.</summary>
        public const uint UsbInputLevel = 0x01;

        /// <summary>Parameter offset constant 'UsbOutLevel' from address_map.js.</summary>
        public const uint UsbOutLevel = 0x02;

        /// <summary>Parameter offset constant 'UsbMixLevel' from address_map.js.</summary>
        public const uint UsbMixLevel = 0x03;

        /// <summary>Parameter offset constant 'UsbLoopback' from address_map.js.</summary>
        public const uint UsbLoopback = 0x06;

        /// <summary>Parameter offset constant 'Usb2OutLevel' from address_map.js.</summary>
        public const uint Usb2OutLevel = 0x07;

        // Built-in system EQ (always-active EQ, separate from the 3 Global EQ banks)
        /// <summary>Parameter offset constant 'EqSw' from address_map.js.</summary>
        public const uint EqSw = 0x10;

        /// <summary>Parameter offset constant 'EqType' from address_map.js.</summary>
        public const uint EqType = 0x11;

        /// <summary>Parameter offset constant 'EqPosition' from address_map.js.</summary>
        public const uint EqPosition = 0x12;

        /// <summary>Parameter offset constant 'EqLowCut' from address_map.js.</summary>
        public const uint EqLowCut = 0x13;

        /// <summary>Parameter offset constant 'EqLowGain' from address_map.js.</summary>
        public const uint EqLowGain = 0x14;

        /// <summary>Parameter offset constant 'EqLowMidFreq' from address_map.js.</summary>
        public const uint EqLowMidFreq = 0x15;

        /// <summary>Parameter offset constant 'EqLowMidQ' from address_map.js.</summary>
        public const uint EqLowMidQ = 0x16;

        /// <summary>Parameter offset constant 'EqLowMidGain' from address_map.js.</summary>
        public const uint EqLowMidGain = 0x17;

        /// <summary>Parameter offset constant 'EqHiMidFreq' from address_map.js.</summary>
        public const uint EqHiMidFreq = 0x18;

        /// <summary>Parameter offset constant 'EqHiMidQ' from address_map.js.</summary>
        public const uint EqHiMidQ = 0x19;

        /// <summary>Parameter offset constant 'EqHiMidGain' from address_map.js.</summary>
        public const uint EqHiMidGain = 0x1A;

        /// <summary>Parameter offset constant 'EqHighGain' from address_map.js.</summary>
        public const uint EqHighGain = 0x1B;

        /// <summary>Parameter offset constant 'EqHighCut' from address_map.js.</summary>
        public const uint EqHighCut = 0x1C;

        /// <summary>Parameter offset constant 'EqLevel' from address_map.js.</summary>
        public const uint EqLevel = 0x1D;

        /// <summary>Parameter offset constant 'EqGeq31Hz' from address_map.js.</summary>
        public const uint EqGeq31Hz = 0x1E;

        /// <summary>Parameter offset constant 'EqGeq62Hz' from address_map.js.</summary>
        public const uint EqGeq62Hz = 0x1F;

        /// <summary>Parameter offset constant 'EqGeq125Hz' from address_map.js.</summary>
        public const uint EqGeq125Hz = 0x20;

        /// <summary>Parameter offset constant 'EqGeq250Hz' from address_map.js.</summary>
        public const uint EqGeq250Hz = 0x21;

        /// <summary>Parameter offset constant 'EqGeq500Hz' from address_map.js.</summary>
        public const uint EqGeq500Hz = 0x22;

        /// <summary>Parameter offset constant 'EqGeq1kHz' from address_map.js.</summary>
        public const uint EqGeq1kHz = 0x23;

        /// <summary>Parameter offset constant 'EqGeq2kHz' from address_map.js.</summary>
        public const uint EqGeq2kHz = 0x24;

        /// <summary>Parameter offset constant 'EqGeq4kHz' from address_map.js.</summary>
        public const uint EqGeq4kHz = 0x25;

        /// <summary>Parameter offset constant 'EqGeq8kHz' from address_map.js.</summary>
        public const uint EqGeq8kHz = 0x26;

        /// <summary>Parameter offset constant 'EqGeq16kHz' from address_map.js.</summary>
        public const uint EqGeq16kHz = 0x27;

        /// <summary>Parameter offset constant 'EqGeqLevel' from address_map.js.</summary>
        public const uint EqGeqLevel = 0x28;

        /// <summary>Parameter offset constant 'LineOutAirFeel' from address_map.js.</summary>
        public const uint LineOutAirFeel = 0x29;

        /// <summary>Parameter offset constant 'ExpandMode' from address_map.js.</summary>
        public const uint ExpandMode = 0x2A;
    }

    /// <summary> Global EQ bank parameter offsets (prm_prop_sys_eq) ───────────────────── </summary>
    // NOTE: these banks (SysEq1/2/3) have NO per-bank SW — the on/off is
    // SystemMainParams.EqSw. Each bank stores only the EQ shape settings.
    /// <summary>Parameter offsets for GlobalEqParams (from address_map.js).</summary>
    public static class GlobalEqParams
    {
        /// <summary>Parameter offset constant 'Type' from address_map.js.</summary>
        public const uint Type = 0x00; // 0=PARAMETRIC EQ, 1=GE-10

        /// <summary>Parameter offset constant 'Position' from address_map.js.</summary>
        public const uint Position = 0x01; // Ver200

        /// <summary>Parameter offset constant 'LowCut' from address_map.js.</summary>
        public const uint LowCut = 0x02;

        /// <summary>Parameter offset constant 'LowGain' from address_map.js.</summary>
        public const uint LowGain = 0x03;

        /// <summary>Parameter offset constant 'LowMidFreq' from address_map.js.</summary>
        public const uint LowMidFreq = 0x04;

        /// <summary>Parameter offset constant 'LowMidQ' from address_map.js.</summary>
        public const uint LowMidQ = 0x05;

        /// <summary>Parameter offset constant 'LowMidGain' from address_map.js.</summary>
        public const uint LowMidGain = 0x06;

        /// <summary>Parameter offset constant 'HiMidFreq' from address_map.js.</summary>
        public const uint HiMidFreq = 0x07;

        /// <summary>Parameter offset constant 'HiMidQ' from address_map.js.</summary>
        public const uint HiMidQ = 0x08;

        /// <summary>Parameter offset constant 'HiMidGain' from address_map.js.</summary>
        public const uint HiMidGain = 0x09;

        /// <summary>Parameter offset constant 'HighGain' from address_map.js.</summary>
        public const uint HighGain = 0x0A;

        /// <summary>Parameter offset constant 'HighCut' from address_map.js.</summary>
        public const uint HighCut = 0x0B;

        /// <summary>Parameter offset constant 'Level' from address_map.js.</summary>
        public const uint Level = 0x0C;

        /// <summary>Parameter offset constant 'Geq31Hz' from address_map.js.</summary>
        public const uint Geq31Hz = 0x0D;

        /// <summary>Parameter offset constant 'Geq62Hz' from address_map.js.</summary>
        public const uint Geq62Hz = 0x0E;

        /// <summary>Parameter offset constant 'Geq125Hz' from address_map.js.</summary>
        public const uint Geq125Hz = 0x0F;

        /// <summary>Parameter offset constant 'Geq250Hz' from address_map.js.</summary>
        public const uint Geq250Hz = 0x10;

        /// <summary>Parameter offset constant 'Geq500Hz' from address_map.js.</summary>
        public const uint Geq500Hz = 0x11;

        /// <summary>Parameter offset constant 'Geq1kHz' from address_map.js.</summary>
        public const uint Geq1kHz = 0x12;

        /// <summary>Parameter offset constant 'Geq2kHz' from address_map.js.</summary>
        public const uint Geq2kHz = 0x13;

        /// <summary>Parameter offset constant 'Geq4kHz' from address_map.js.</summary>
        public const uint Geq4kHz = 0x14;

        /// <summary>Parameter offset constant 'Geq8kHz' from address_map.js.</summary>
        public const uint Geq8kHz = 0x15;

        /// <summary>Parameter offset constant 'Geq16kHz' from address_map.js.</summary>
        public const uint Geq16kHz = 0x16;

        /// <summary>Parameter offset constant 'GeqLevel' from address_map.js.</summary>
        public const uint GeqLevel = 0x17;
    }

    /// <summary> Patch0 parameter offsets (prm_prop_patch_0) ───────────────────────────── </summary>
    public static class Patch0Params
    {
        // Booster (OD/DS)
        /// <summary>Parameter offset constant 'BoosterSw' from address_map.js.</summary>
        public const uint BoosterSw = 0x00;

        /// <summary>Parameter offset constant 'BoosterType' from address_map.js.</summary>
        public const uint BoosterType = 0x01;

        /// <summary>Parameter offset constant 'BoosterDrive' from address_map.js.</summary>
        public const uint BoosterDrive = 0x02;

        /// <summary>Parameter offset constant 'BoosterBottom' from address_map.js.</summary>
        public const uint BoosterBottom = 0x03;

        /// <summary>Parameter offset constant 'BoosterTone' from address_map.js.</summary>
        public const uint BoosterTone = 0x04;

        /// <summary>Parameter offset constant 'BoosterSoloSw' from address_map.js.</summary>
        public const uint BoosterSoloSw = 0x05;

        /// <summary>Parameter offset constant 'BoosterSoloLevel' from address_map.js.</summary>
        public const uint BoosterSoloLevel = 0x06;

        /// <summary>Parameter offset constant 'BoosterEffectLevel' from address_map.js.</summary>
        public const uint BoosterEffectLevel = 0x07;

        /// <summary>Parameter offset constant 'BoosterDirectMix' from address_map.js.</summary>
        public const uint BoosterDirectMix = 0x08;

        // Preamp
        /// <summary>Parameter offset constant 'PreampType' from address_map.js.</summary>
        public const uint PreampType = 0x11;

        /// <summary>Parameter offset constant 'PreampGain' from address_map.js.</summary>
        public const uint PreampGain = 0x12;

        /// <summary>Parameter offset constant 'PreampBass' from address_map.js.</summary>
        public const uint PreampBass = 0x14;

        /// <summary>Parameter offset constant 'PreampMiddle' from address_map.js.</summary>
        public const uint PreampMiddle = 0x15;

        /// <summary>Parameter offset constant 'PreampTreble' from address_map.js.</summary>
        public const uint PreampTreble = 0x16;

        /// <summary>Parameter offset constant 'PreampPresence' from address_map.js.</summary>
        public const uint PreampPresence = 0x17;

        /// <summary>Parameter offset constant 'PreampLevel' from address_map.js.</summary>
        public const uint PreampLevel = 0x18;

        /// <summary>Parameter offset constant 'PreampBright' from address_map.js.</summary>
        public const uint PreampBright = 0x19;

        /// <summary>Parameter offset constant 'PreampSoloSw' from address_map.js.</summary>
        public const uint PreampSoloSw = 0x1B;

        /// <summary>Parameter offset constant 'PreampSoloLevel' from address_map.js.</summary>
        public const uint PreampSoloLevel = 0x1C;

        // Embedded EQ1 (prm_prop_patch_0 inline, relative to Patch0 block)
        /// <summary>Parameter offset constant 'Eq1Sw' from address_map.js.</summary>
        public const uint Eq1Sw = 0x30;

        /// <summary>Parameter offset constant 'Eq1Type' from address_map.js.</summary>
        public const uint Eq1Type = 0x31;

        /// <summary>Parameter offset constant 'Eq1LowCut' from address_map.js.</summary>
        public const uint Eq1LowCut = 0x32;

        /// <summary>Parameter offset constant 'Eq1LowGain' from address_map.js.</summary>
        public const uint Eq1LowGain = 0x33;

        /// <summary>Parameter offset constant 'Eq1LowMidFreq' from address_map.js.</summary>
        public const uint Eq1LowMidFreq = 0x34;

        /// <summary>Parameter offset constant 'Eq1LowMidQ' from address_map.js.</summary>
        public const uint Eq1LowMidQ = 0x35;

        /// <summary>Parameter offset constant 'Eq1LowMidGain' from address_map.js.</summary>
        public const uint Eq1LowMidGain = 0x36;

        /// <summary>Parameter offset constant 'Eq1HiMidFreq' from address_map.js.</summary>
        public const uint Eq1HiMidFreq = 0x37;

        /// <summary>Parameter offset constant 'Eq1HiMidQ' from address_map.js.</summary>
        public const uint Eq1HiMidQ = 0x38;

        /// <summary>Parameter offset constant 'Eq1HiMidGain' from address_map.js.</summary>
        public const uint Eq1HiMidGain = 0x39;

        /// <summary>Parameter offset constant 'Eq1HighGain' from address_map.js.</summary>
        public const uint Eq1HighGain = 0x3A;

        /// <summary>Parameter offset constant 'Eq1HighCut' from address_map.js.</summary>
        public const uint Eq1HighCut = 0x3B;

        /// <summary>Parameter offset constant 'Eq1Level' from address_map.js.</summary>
        public const uint Eq1Level = 0x3C;

        /// <summary>Parameter offset constant 'Eq1Geq31Hz' from address_map.js.</summary>
        public const uint Eq1Geq31Hz = 0x3D;

        /// <summary>Parameter offset constant 'Eq1Geq62Hz' from address_map.js.</summary>
        public const uint Eq1Geq62Hz = 0x3E;

        /// <summary>Parameter offset constant 'Eq1Geq125Hz' from address_map.js.</summary>
        public const uint Eq1Geq125Hz = 0x3F;

        /// <summary>Parameter offset constant 'Eq1Geq250Hz' from address_map.js.</summary>
        public const uint Eq1Geq250Hz = 0x40;

        /// <summary>Parameter offset constant 'Eq1Geq500Hz' from address_map.js.</summary>
        public const uint Eq1Geq500Hz = 0x41;

        /// <summary>Parameter offset constant 'Eq1Geq1kHz' from address_map.js.</summary>
        public const uint Eq1Geq1kHz = 0x42;

        /// <summary>Parameter offset constant 'Eq1Geq2kHz' from address_map.js.</summary>
        public const uint Eq1Geq2kHz = 0x43;

        /// <summary>Parameter offset constant 'Eq1Geq4kHz' from address_map.js.</summary>
        public const uint Eq1Geq4kHz = 0x44;

        /// <summary>Parameter offset constant 'Eq1Geq8kHz' from address_map.js.</summary>
        public const uint Eq1Geq8kHz = 0x45;

        /// <summary>Parameter offset constant 'Eq1Geq16kHz' from address_map.js.</summary>
        public const uint Eq1Geq16kHz = 0x46;

        /// <summary>Parameter offset constant 'Eq1GeqLevel' from address_map.js.</summary>
        public const uint Eq1GeqLevel = 0x47;
    }

    /// <summary> PatchStatus parameter offsets (prm_prop_patch_status) ─────────────────── </summary>
    public static class PatchStatusParams
    {
        // Knob positions (physical front-panel)
        /// <summary>Parameter offset constant 'AmpType' from address_map.js.</summary>
        public const uint AmpType = 0x00; // PRM_KNOB_POS_TYPE  (0-4: ACOUSTIC/CLEAN/CRUNCH/LEAD/BROWN)

        /// <summary>Parameter offset constant 'Gain' from address_map.js.</summary>
        public const uint Gain = 0x01; // PRM_KNOB_POS_GAIN

        /// <summary>Parameter offset constant 'Volume' from address_map.js.</summary>
        public const uint Volume = 0x02; // PRM_KNOB_POS_VOLUME

        /// <summary>Parameter offset constant 'Bass' from address_map.js.</summary>
        public const uint Bass = 0x03; // PRM_KNOB_POS_BASS

        /// <summary>Parameter offset constant 'Middle' from address_map.js.</summary>
        public const uint Middle = 0x04; // PRM_KNOB_POS_MIDDLE

        /// <summary>Parameter offset constant 'Treble' from address_map.js.</summary>
        public const uint Treble = 0x05; // PRM_KNOB_POS_TREBLE

        /// <summary>Parameter offset constant 'Presence' from address_map.js.</summary>
        public const uint Presence = 0x06; // PRM_KNOB_POS_PRESENCE

        /// <summary>Parameter offset constant 'BoostLevel' from address_map.js.</summary>
        public const uint BoostLevel = 0x07; // PRM_KNOB_POS_BOOST

        /// <summary>Parameter offset constant 'ModLevel' from address_map.js.</summary>
        public const uint ModLevel = 0x08; // PRM_KNOB_POS_MOD

        /// <summary>Parameter offset constant 'FxLevel' from address_map.js.</summary>
        public const uint FxLevel = 0x09; // PRM_KNOB_POS_FX

        /// <summary>Parameter offset constant 'DelayLevel' from address_map.js.</summary>
        public const uint DelayLevel = 0x0A; // PRM_KNOB_POS_DELAY

        /// <summary>Parameter offset constant 'ReverbLevel' from address_map.js.</summary>
        public const uint ReverbLevel = 0x0B; // PRM_KNOB_POS_REVERB

        // LED / variation states
        /// <summary>Parameter offset constant 'Variation' from address_map.js.</summary>
        public const uint Variation = 0x0C; // PRM_LED_STATE_VARI

        /// <summary>Parameter offset constant 'LedBooster' from address_map.js.</summary>
        public const uint LedBooster = 0x0D; // PRM_LED_STATE_BOOST

        /// <summary>Parameter offset constant 'LedMod' from address_map.js.</summary>
        public const uint LedMod = 0x0E; // PRM_LED_STATE_MOD

        /// <summary>Parameter offset constant 'LedFx' from address_map.js.</summary>
        public const uint LedFx = 0x0F; // PRM_LED_STATE_FX

        /// <summary>Parameter offset constant 'LedDelay' from address_map.js.</summary>
        public const uint LedDelay = 0x10; // PRM_LED_STATE_DELAY

        /// <summary>Parameter offset constant 'LedReverb' from address_map.js.</summary>
        public const uint LedReverb = 0x11; // PRM_LED_STATE_REVERB
    }

    /// <summary> Patch1 parameter offsets (prm_prop_patch_1 — Reverb + PedalFX block) ──── </summary>
    public static class Patch1Params
    {
        // Reverb
        /// <summary>Parameter offset constant 'ReverbSw' from address_map.js.</summary>
        public const uint ReverbSw = 0x00;

        /// <summary>Parameter offset constant 'ReverbType' from address_map.js.</summary>
        public const uint ReverbType = 0x01;

        /// <summary>Parameter offset constant 'ReverbTime' from address_map.js.</summary>
        public const uint ReverbTime = 0x02;

        /// <summary>Parameter offset constant 'ReverbPreDelay' from address_map.js.</summary>
        public const uint ReverbPreDelay = 0x03; // INTEGER2x7

        /// <summary>Parameter offset constant 'ReverbLowCut' from address_map.js.</summary>
        public const uint ReverbLowCut = 0x05;

        /// <summary>Parameter offset constant 'ReverbHighCut' from address_map.js.</summary>
        public const uint ReverbHighCut = 0x06;

        /// <summary>Parameter offset constant 'ReverbDensity' from address_map.js.</summary>
        public const uint ReverbDensity = 0x07;

        /// <summary>Parameter offset constant 'ReverbEffectLevel' from address_map.js.</summary>
        public const uint ReverbEffectLevel = 0x08;

        /// <summary>Parameter offset constant 'ReverbDirectMix' from address_map.js.</summary>
        public const uint ReverbDirectMix = 0x09;

        /// <summary>Parameter offset constant 'ReverbSpringColor' from address_map.js.</summary>
        public const uint ReverbSpringColor = 0x0B;

        // Pedal FX (starts at 0x10 within Patch1)
        /// <summary>Parameter offset constant 'PedalFxSw' from address_map.js.</summary>
        public const uint PedalFxSw = 0x10;

        /// <summary>Parameter offset constant 'PedalFxType' from address_map.js.</summary>
        public const uint PedalFxType = 0x11;

        /// <summary>Parameter offset constant 'PedalFxWahType' from address_map.js.</summary>
        public const uint PedalFxWahType = 0x12;

        /// <summary>Parameter offset constant 'PedalFxWahPedalPos' from address_map.js.</summary>
        public const uint PedalFxWahPedalPos = 0x13;

        /// <summary>Parameter offset constant 'PedalFxWahMin' from address_map.js.</summary>
        public const uint PedalFxWahMin = 0x14;

        /// <summary>Parameter offset constant 'PedalFxWahMax' from address_map.js.</summary>
        public const uint PedalFxWahMax = 0x15;

        /// <summary>Parameter offset constant 'PedalFxWahEffectLevel' from address_map.js.</summary>
        public const uint PedalFxWahEffectLevel = 0x16;

        /// <summary>Parameter offset constant 'PedalFxWahDirectMix' from address_map.js.</summary>
        public const uint PedalFxWahDirectMix = 0x17;

        /// <summary>Parameter offset constant 'PedalFxBendPitch' from address_map.js.</summary>
        public const uint PedalFxBendPitch = 0x18;

        /// <summary>Parameter offset constant 'PedalFxBendPedalPos' from address_map.js.</summary>
        public const uint PedalFxBendPedalPos = 0x19;

        /// <summary>Parameter offset constant 'PedalFxBendEffectLevel' from address_map.js.</summary>
        public const uint PedalFxBendEffectLevel = 0x1A;

        /// <summary>Parameter offset constant 'PedalFxBendDirectMix' from address_map.js.</summary>
        public const uint PedalFxBendDirectMix = 0x1B;

        /// <summary>Parameter offset constant 'PedalFxEvh95Pos' from address_map.js.</summary>
        public const uint PedalFxEvh95Pos = 0x1C;

        /// <summary>Parameter offset constant 'PedalFxEvh95Min' from address_map.js.</summary>
        public const uint PedalFxEvh95Min = 0x1D;

        /// <summary>Parameter offset constant 'PedalFxEvh95Max' from address_map.js.</summary>
        public const uint PedalFxEvh95Max = 0x1E;

        /// <summary>Parameter offset constant 'PedalFxEvh95EffectLevel' from address_map.js.</summary>
        public const uint PedalFxEvh95EffectLevel = 0x1F;

        /// <summary>Parameter offset constant 'PedalFxEvh95DirectMix' from address_map.js.</summary>
        public const uint PedalFxEvh95DirectMix = 0x20;

        /// <summary>Parameter offset constant 'FootVolume' from address_map.js.</summary>
        public const uint FootVolume = 0x21;

        // Solo
        /// <summary>Parameter offset constant 'SoloSw' from address_map.js.</summary>
        public const uint SoloSw = 0x54;

        /// <summary>Parameter offset constant 'SoloLevel' from address_map.js.</summary>
        public const uint SoloLevel = 0x55;
    }

    /// <summary> Patch2 parameter offsets (prm_prop_patch_2) ───────────────────────────── </summary>
    public static class Patch2Params
    {
        /// <summary>Parameter offset constant 'ChainPattern' from address_map.js.</summary>
        public const uint ChainPattern = 0x00; // PRM_CHAIN_PTN

        /// <summary>Parameter offset constant 'PosSendReturn' from address_map.js.</summary>
        public const uint PosSendReturn = 0x01;

        /// <summary>Parameter offset constant 'PosEq' from address_map.js.</summary>
        public const uint PosEq = 0x02;

        /// <summary>Parameter offset constant 'PosPedalFx' from address_map.js.</summary>
        public const uint PosPedalFx = 0x03;

        /// <summary>Parameter offset constant 'CabinetResonance' from address_map.js.</summary>
        public const uint CabinetResonance = 0x23; // PRM_CABINET_RESONANCE
    }

    /// <summary> Delay parameter offsets (prm_prop_patch_delay) ────────────────────────── </summary>
    public static class DelayParams
    {
        /// <summary>Parameter offset constant 'Sw' from address_map.js.</summary>
        public const uint Sw = 0x00;

        /// <summary>Parameter offset constant 'Type' from address_map.js.</summary>
        public const uint Type = 0x01;

        /// <summary>Parameter offset constant 'DelayTime' from address_map.js.</summary>
        public const uint DelayTime = 0x02; // INTEGER2x7 (2 bytes)

        /// <summary>Parameter offset constant 'Feedback' from address_map.js.</summary>
        public const uint Feedback = 0x04;

        /// <summary>Parameter offset constant 'HighCut' from address_map.js.</summary>
        public const uint HighCut = 0x05;

        /// <summary>Parameter offset constant 'EffectLevel' from address_map.js.</summary>
        public const uint EffectLevel = 0x06;

        /// <summary>Parameter offset constant 'DirectMix' from address_map.js.</summary>
        public const uint DirectMix = 0x07;

        /// <summary>Parameter offset constant 'TapTime' from address_map.js.</summary>
        public const uint TapTime = 0x08;

        /// <summary>Parameter offset constant 'ModRate' from address_map.js.</summary>
        public const uint ModRate = 0x13;

        /// <summary>Parameter offset constant 'ModDepth' from address_map.js.</summary>
        public const uint ModDepth = 0x14;

        /// <summary>Parameter offset constant 'Range' from address_map.js.</summary>
        public const uint Range = 0x15;

        /// <summary>Parameter offset constant 'Filter' from address_map.js.</summary>
        public const uint Filter = 0x16;

        /// <summary>Parameter offset constant 'FeedbackPhase' from address_map.js.</summary>
        public const uint FeedbackPhase = 0x17;

        /// <summary>Parameter offset constant 'DelayPhase' from address_map.js.</summary>
        public const uint DelayPhase = 0x18;

        /// <summary>Parameter offset constant 'ModSw' from address_map.js.</summary>
        public const uint ModSw = 0x19;
    }

    /// <summary> Contour parameter offsets (prm_prop_contour) ───────────────────────────── </summary>
    public static class ContourParams
    {
        /// <summary>Parameter offset constant 'Type' from address_map.js.</summary>
        public const uint Type = 0x00;

        /// <summary>Parameter offset constant 'FreqShift' from address_map.js.</summary>
        public const uint FreqShift = 0x01;
    }

    // prm_prop_sys_power_adjust
    /// <summary>Parameter offsets for SysPowerAdjustParams (from address_map.js).</summary>
    public static class SysPowerAdjustParams
    {
        /// <summary>Parameter offset constant 'SysHalfPowerAdjustEditor' from address_map.js.</summary>
        public const uint SysHalfPowerAdjustEditor = 0x00000000;
    }

    // prm_prop_sys_lineout_custom
    /// <summary>Parameter offsets for SysLineoutCustomParams (from address_map.js).</summary>
    public static class SysLineoutCustomParams
    {
        /// <summary>Parameter offset constant 'SysLineoutCustomSw' from address_map.js.</summary>
        public const uint SysLineoutCustomSw = 0x00000000;

        /// <summary>Parameter offset constant 'SysLineoutCustomSelect' from address_map.js.</summary>
        public const uint SysLineoutCustomSelect = 0x00000001;
    }

    // prm_prop_sys_lineout_custom_setting
    /// <summary>Parameter offsets for SysLineoutCustomSettingParams (from address_map.js).</summary>
    public static class SysLineoutCustomSettingParams
    {
        /// <summary>Parameter offset constant 'SysLineoutM1MicType' from address_map.js.</summary>
        public const uint SysLineoutM1MicType = 0x00000000;

        /// <summary>Parameter offset constant 'SysLineoutM1MicDistance' from address_map.js.</summary>
        public const uint SysLineoutM1MicDistance = 0x00000001;

        /// <summary>Parameter offset constant 'SysLineoutM1MicPosition' from address_map.js.</summary>
        public const uint SysLineoutM1MicPosition = 0x00000002;
    }

    // prm_prop_sys_gafc_function
    /// <summary>Parameter offsets for SysGafcFunctionParams (from address_map.js).</summary>
    public static class SysGafcFunctionParams
    {
        /// <summary>Parameter offset constant 'SysGafcFuncSw1' from address_map.js.</summary>
        public const uint SysGafcFuncSw1 = 0x00000000;

        /// <summary>Parameter offset constant 'SysGafcFuncSw2' from address_map.js.</summary>
        public const uint SysGafcFuncSw2 = 0x00000001;

        /// <summary>Parameter offset constant 'SysGafcFuncSw3' from address_map.js.</summary>
        public const uint SysGafcFuncSw3 = 0x00000002;

        /// <summary>Parameter offset constant 'SysGafcFuncSw4' from address_map.js.</summary>
        public const uint SysGafcFuncSw4 = 0x00000003;

        /// <summary>Parameter offset constant 'SysGafcFuncSw5' from address_map.js.</summary>
        public const uint SysGafcFuncSw5 = 0x00000004;
    }

    // prm_prop_patch_eq2
    /// <summary>Parameter offsets for PatchEq2Params (from address_map.js).</summary>
    public static class PatchEq2Params
    {
        /// <summary>Parameter offset constant 'EqSw' from address_map.js.</summary>
        public const uint EqSw = 0x00000000;

        /// <summary>Parameter offset constant 'EqType' from address_map.js.</summary>
        public const uint EqType = 0x00000001;

        /// <summary>Parameter offset constant 'EqLowCut' from address_map.js.</summary>
        public const uint EqLowCut = 0x00000002;

        /// <summary>Parameter offset constant 'EqLowGain' from address_map.js.</summary>
        public const uint EqLowGain = 0x00000003;

        /// <summary>Parameter offset constant 'EqLowmidFreq' from address_map.js.</summary>
        public const uint EqLowmidFreq = 0x00000004;

        /// <summary>Parameter offset constant 'EqLowmidQ' from address_map.js.</summary>
        public const uint EqLowmidQ = 0x00000005;

        /// <summary>Parameter offset constant 'EqLowmidGain' from address_map.js.</summary>
        public const uint EqLowmidGain = 0x00000006;

        /// <summary>Parameter offset constant 'EqHighmidFreq' from address_map.js.</summary>
        public const uint EqHighmidFreq = 0x00000007;

        /// <summary>Parameter offset constant 'EqHighmidQ' from address_map.js.</summary>
        public const uint EqHighmidQ = 0x00000008;

        /// <summary>Parameter offset constant 'EqHighmidGain' from address_map.js.</summary>
        public const uint EqHighmidGain = 0x00000009;

        /// <summary>Parameter offset constant 'EqHighGain' from address_map.js.</summary>
        public const uint EqHighGain = 0x0000000a;

        /// <summary>Parameter offset constant 'EqHighCut' from address_map.js.</summary>
        public const uint EqHighCut = 0x0000000b;

        /// <summary>Parameter offset constant 'EqLevel' from address_map.js.</summary>
        public const uint EqLevel = 0x0000000c;

        /// <summary>Parameter offset constant 'EqGeqBand1' from address_map.js.</summary>
        public const uint EqGeqBand1 = 0x0000000d;

        /// <summary>Parameter offset constant 'EqGeqBand2' from address_map.js.</summary>
        public const uint EqGeqBand2 = 0x0000000e;

        /// <summary>Parameter offset constant 'EqGeqBand3' from address_map.js.</summary>
        public const uint EqGeqBand3 = 0x0000000f;

        /// <summary>Parameter offset constant 'EqGeqBand4' from address_map.js.</summary>
        public const uint EqGeqBand4 = 0x00000010;

        /// <summary>Parameter offset constant 'EqGeqBand5' from address_map.js.</summary>
        public const uint EqGeqBand5 = 0x00000011;

        /// <summary>Parameter offset constant 'EqGeqBand6' from address_map.js.</summary>
        public const uint EqGeqBand6 = 0x00000012;

        /// <summary>Parameter offset constant 'EqGeqBand7' from address_map.js.</summary>
        public const uint EqGeqBand7 = 0x00000013;

        /// <summary>Parameter offset constant 'EqGeqBand8' from address_map.js.</summary>
        public const uint EqGeqBand8 = 0x00000014;

        /// <summary>Parameter offset constant 'EqGeqBand9' from address_map.js.</summary>
        public const uint EqGeqBand9 = 0x00000015;

        /// <summary>Parameter offset constant 'EqGeqBand10' from address_map.js.</summary>
        public const uint EqGeqBand10 = 0x00000016;

        /// <summary>Parameter offset constant 'EqGeqLevel' from address_map.js.</summary>
        public const uint EqGeqLevel = 0x00000017;
    }

    // prm_prop_patch_fx
    /// <summary>Parameter offsets for PatchFxParams (from address_map.js).</summary>
    public static class PatchFxParams
    {
        /// <summary>Parameter offset constant 'Fx1Sw' from address_map.js.</summary>
        public const uint Fx1Sw = 0x00000000;

        /// <summary>Parameter offset constant 'Fx1Fxtype' from address_map.js.</summary>
        public const uint Fx1Fxtype = 0x00000001;

        /// <summary>Parameter offset constant 'Fx1TwahMode' from address_map.js.</summary>
        public const uint Fx1TwahMode = 0x00000002;

        /// <summary>Parameter offset constant 'Fx1TwahPolarity' from address_map.js.</summary>
        public const uint Fx1TwahPolarity = 0x00000003;

        /// <summary>Parameter offset constant 'Fx1TwahSens' from address_map.js.</summary>
        public const uint Fx1TwahSens = 0x00000004;

        /// <summary>Parameter offset constant 'Fx1TwahFreq' from address_map.js.</summary>
        public const uint Fx1TwahFreq = 0x00000005;

        /// <summary>Parameter offset constant 'Fx1TwahPeak' from address_map.js.</summary>
        public const uint Fx1TwahPeak = 0x00000006;

        /// <summary>Parameter offset constant 'Fx1TwahDLevel' from address_map.js.</summary>
        public const uint Fx1TwahDLevel = 0x00000007;

        /// <summary>Parameter offset constant 'Fx1TwahLevel' from address_map.js.</summary>
        public const uint Fx1TwahLevel = 0x00000008;

        /// <summary>Parameter offset constant 'Fx1AwahMode' from address_map.js.</summary>
        public const uint Fx1AwahMode = 0x00000009;

        /// <summary>Parameter offset constant 'Fx1AwahFreq' from address_map.js.</summary>
        public const uint Fx1AwahFreq = 0x0000000a;

        /// <summary>Parameter offset constant 'Fx1AwahPeak' from address_map.js.</summary>
        public const uint Fx1AwahPeak = 0x0000000b;

        /// <summary>Parameter offset constant 'Fx1AwahRate' from address_map.js.</summary>
        public const uint Fx1AwahRate = 0x0000000c;

        /// <summary>Parameter offset constant 'Fx1AwahDepth' from address_map.js.</summary>
        public const uint Fx1AwahDepth = 0x0000000d;

        /// <summary>Parameter offset constant 'Fx1AwahDLevel' from address_map.js.</summary>
        public const uint Fx1AwahDLevel = 0x0000000e;

        /// <summary>Parameter offset constant 'Fx1AwahLevel' from address_map.js.</summary>
        public const uint Fx1AwahLevel = 0x0000000f;

        /// <summary>Parameter offset constant 'Fx1SubwahType' from address_map.js.</summary>
        public const uint Fx1SubwahType = 0x00000010;

        /// <summary>Parameter offset constant 'Fx1SubwahPdlpos' from address_map.js.</summary>
        public const uint Fx1SubwahPdlpos = 0x00000011;

        /// <summary>Parameter offset constant 'Fx1SubwahMin' from address_map.js.</summary>
        public const uint Fx1SubwahMin = 0x00000012;

        /// <summary>Parameter offset constant 'Fx1SubwahMax' from address_map.js.</summary>
        public const uint Fx1SubwahMax = 0x00000013;

        /// <summary>Parameter offset constant 'Fx1SubwahELevel' from address_map.js.</summary>
        public const uint Fx1SubwahELevel = 0x00000014;

        /// <summary>Parameter offset constant 'Fx1SubwahDLevel' from address_map.js.</summary>
        public const uint Fx1SubwahDLevel = 0x00000015;

        /// <summary>Parameter offset constant 'Fx1AdcompType' from address_map.js.</summary>
        public const uint Fx1AdcompType = 0x00000016;

        /// <summary>Parameter offset constant 'Fx1AdcompSustain' from address_map.js.</summary>
        public const uint Fx1AdcompSustain = 0x00000017;

        /// <summary>Parameter offset constant 'Fx1AdcompAttack' from address_map.js.</summary>
        public const uint Fx1AdcompAttack = 0x00000018;

        /// <summary>Parameter offset constant 'Fx1AdcompTone' from address_map.js.</summary>
        public const uint Fx1AdcompTone = 0x00000019;

        /// <summary>Parameter offset constant 'Fx1AdcompLevel' from address_map.js.</summary>
        public const uint Fx1AdcompLevel = 0x0000001a;

        /// <summary>Parameter offset constant 'Fx1LimiterType' from address_map.js.</summary>
        public const uint Fx1LimiterType = 0x0000001b;

        /// <summary>Parameter offset constant 'Fx1LimiterAttack' from address_map.js.</summary>
        public const uint Fx1LimiterAttack = 0x0000001c;

        /// <summary>Parameter offset constant 'Fx1LimiterThreshold' from address_map.js.</summary>
        public const uint Fx1LimiterThreshold = 0x0000001d;

        /// <summary>Parameter offset constant 'Fx1LimiterRatio' from address_map.js.</summary>
        public const uint Fx1LimiterRatio = 0x0000001e;

        /// <summary>Parameter offset constant 'Fx1LimiterRelease' from address_map.js.</summary>
        public const uint Fx1LimiterRelease = 0x0000001f;

        /// <summary>Parameter offset constant 'Fx1LimiterLevel' from address_map.js.</summary>
        public const uint Fx1LimiterLevel = 0x00000020;

        /// <summary>Parameter offset constant 'Fx1GeqBand1' from address_map.js.</summary>
        public const uint Fx1GeqBand1 = 0x00000021;

        /// <summary>Parameter offset constant 'Fx1GeqBand2' from address_map.js.</summary>
        public const uint Fx1GeqBand2 = 0x00000022;

        /// <summary>Parameter offset constant 'Fx1GeqBand3' from address_map.js.</summary>
        public const uint Fx1GeqBand3 = 0x00000023;

        /// <summary>Parameter offset constant 'Fx1GeqBand4' from address_map.js.</summary>
        public const uint Fx1GeqBand4 = 0x00000024;

        /// <summary>Parameter offset constant 'Fx1GeqBand5' from address_map.js.</summary>
        public const uint Fx1GeqBand5 = 0x00000025;

        /// <summary>Parameter offset constant 'Fx1GeqBand6' from address_map.js.</summary>
        public const uint Fx1GeqBand6 = 0x00000026;

        /// <summary>Parameter offset constant 'Fx1GeqBand7' from address_map.js.</summary>
        public const uint Fx1GeqBand7 = 0x00000027;

        /// <summary>Parameter offset constant 'Fx1GeqBand8' from address_map.js.</summary>
        public const uint Fx1GeqBand8 = 0x00000028;

        /// <summary>Parameter offset constant 'Fx1GeqBand9' from address_map.js.</summary>
        public const uint Fx1GeqBand9 = 0x00000029;

        /// <summary>Parameter offset constant 'Fx1GeqBand10' from address_map.js.</summary>
        public const uint Fx1GeqBand10 = 0x0000002a;

        /// <summary>Parameter offset constant 'Fx1GeqLevel' from address_map.js.</summary>
        public const uint Fx1GeqLevel = 0x0000002b;

        /// <summary>Parameter offset constant 'Fx1PeqLowCut' from address_map.js.</summary>
        public const uint Fx1PeqLowCut = 0x0000002c;

        /// <summary>Parameter offset constant 'Fx1PeqLowGain' from address_map.js.</summary>
        public const uint Fx1PeqLowGain = 0x0000002d;

        /// <summary>Parameter offset constant 'Fx1PeqLowmidFreq' from address_map.js.</summary>
        public const uint Fx1PeqLowmidFreq = 0x0000002e;

        /// <summary>Parameter offset constant 'Fx1PeqLowmidQ' from address_map.js.</summary>
        public const uint Fx1PeqLowmidQ = 0x0000002f;

        /// <summary>Parameter offset constant 'Fx1PeqLowmidGain' from address_map.js.</summary>
        public const uint Fx1PeqLowmidGain = 0x00000030;

        /// <summary>Parameter offset constant 'Fx1PeqHighmidFreq' from address_map.js.</summary>
        public const uint Fx1PeqHighmidFreq = 0x00000031;

        /// <summary>Parameter offset constant 'Fx1PeqHighmidQ' from address_map.js.</summary>
        public const uint Fx1PeqHighmidQ = 0x00000032;

        /// <summary>Parameter offset constant 'Fx1PeqHighmidGain' from address_map.js.</summary>
        public const uint Fx1PeqHighmidGain = 0x00000033;

        /// <summary>Parameter offset constant 'Fx1PeqHighGain' from address_map.js.</summary>
        public const uint Fx1PeqHighGain = 0x00000034;

        /// <summary>Parameter offset constant 'Fx1PeqHighCut' from address_map.js.</summary>
        public const uint Fx1PeqHighCut = 0x00000035;

        /// <summary>Parameter offset constant 'Fx1PeqLevel' from address_map.js.</summary>
        public const uint Fx1PeqLevel = 0x00000036;

        /// <summary>Parameter offset constant 'Fx1GtrsimType' from address_map.js.</summary>
        public const uint Fx1GtrsimType = 0x00000037;

        /// <summary>Parameter offset constant 'Fx1GtrsimLow' from address_map.js.</summary>
        public const uint Fx1GtrsimLow = 0x00000038;

        /// <summary>Parameter offset constant 'Fx1GtrsimHigh' from address_map.js.</summary>
        public const uint Fx1GtrsimHigh = 0x00000039;

        /// <summary>Parameter offset constant 'Fx1GtrsimLevel' from address_map.js.</summary>
        public const uint Fx1GtrsimLevel = 0x0000003a;

        /// <summary>Parameter offset constant 'Fx1GtrsimBody' from address_map.js.</summary>
        public const uint Fx1GtrsimBody = 0x0000003b;

        /// <summary>Parameter offset constant 'Fx1SlowgearSens' from address_map.js.</summary>
        public const uint Fx1SlowgearSens = 0x0000003c;

        /// <summary>Parameter offset constant 'Fx1SlowgearRisetime' from address_map.js.</summary>
        public const uint Fx1SlowgearRisetime = 0x0000003d;

        /// <summary>Parameter offset constant 'Fx1SlowgearLevel' from address_map.js.</summary>
        public const uint Fx1SlowgearLevel = 0x0000003e;

        /// <summary>Parameter offset constant 'Fx1WavesynWave' from address_map.js.</summary>
        public const uint Fx1WavesynWave = 0x0000003f;

        /// <summary>Parameter offset constant 'Fx1WavesynCutoff' from address_map.js.</summary>
        public const uint Fx1WavesynCutoff = 0x00000040;

        /// <summary>Parameter offset constant 'Fx1WavesynResonance' from address_map.js.</summary>
        public const uint Fx1WavesynResonance = 0x00000041;

        /// <summary>Parameter offset constant 'Fx1WavesynFltSens' from address_map.js.</summary>
        public const uint Fx1WavesynFltSens = 0x00000042;

        /// <summary>Parameter offset constant 'Fx1WavesynFltDecay' from address_map.js.</summary>
        public const uint Fx1WavesynFltDecay = 0x00000043;

        /// <summary>Parameter offset constant 'Fx1WavesynFltDepth' from address_map.js.</summary>
        public const uint Fx1WavesynFltDepth = 0x00000044;

        /// <summary>Parameter offset constant 'Fx1WavesynSynLevel' from address_map.js.</summary>
        public const uint Fx1WavesynSynLevel = 0x00000045;

        /// <summary>Parameter offset constant 'Fx1WavesynDLevel' from address_map.js.</summary>
        public const uint Fx1WavesynDLevel = 0x00000046;

        /// <summary>Parameter offset constant 'Fx1OctaveRange' from address_map.js.</summary>
        public const uint Fx1OctaveRange = 0x00000047;

        /// <summary>Parameter offset constant 'Fx1OctaveLevel' from address_map.js.</summary>
        public const uint Fx1OctaveLevel = 0x00000048;

        /// <summary>Parameter offset constant 'Fx1OctaveDLevel' from address_map.js.</summary>
        public const uint Fx1OctaveDLevel = 0x00000049;

        /// <summary>Parameter offset constant 'Fx1PitchshiftVoice' from address_map.js.</summary>
        public const uint Fx1PitchshiftVoice = 0x0000004a;

        /// <summary>Parameter offset constant 'Fx1PitchshiftMode1' from address_map.js.</summary>
        public const uint Fx1PitchshiftMode1 = 0x0000004b;

        /// <summary>Parameter offset constant 'Fx1PitchshiftPitch1' from address_map.js.</summary>
        public const uint Fx1PitchshiftPitch1 = 0x0000004c;

        /// <summary>Parameter offset constant 'Fx1PitchshiftFine1' from address_map.js.</summary>
        public const uint Fx1PitchshiftFine1 = 0x0000004d;

        /// <summary>Parameter offset constant 'Fx1PitchshiftPredelay1' from address_map.js.</summary>
        public const uint Fx1PitchshiftPredelay1 = 0x0000004e;

        /// <summary>Parameter offset constant 'Fx1PitchshiftLevel1' from address_map.js.</summary>
        public const uint Fx1PitchshiftLevel1 = 0x00000050;

        /// <summary>Parameter offset constant 'Fx1PitchshiftMode2' from address_map.js.</summary>
        public const uint Fx1PitchshiftMode2 = 0x00000051;

        /// <summary>Parameter offset constant 'Fx1PitchshiftPitch2' from address_map.js.</summary>
        public const uint Fx1PitchshiftPitch2 = 0x00000052;

        /// <summary>Parameter offset constant 'Fx1PitchshiftFine2' from address_map.js.</summary>
        public const uint Fx1PitchshiftFine2 = 0x00000053;

        /// <summary>Parameter offset constant 'Fx1PitchshiftPredelay2' from address_map.js.</summary>
        public const uint Fx1PitchshiftPredelay2 = 0x00000054;

        /// <summary>Parameter offset constant 'Fx1PitchshiftLevel2' from address_map.js.</summary>
        public const uint Fx1PitchshiftLevel2 = 0x00000056;

        /// <summary>Parameter offset constant 'Fx1PitchshiftFeedback' from address_map.js.</summary>
        public const uint Fx1PitchshiftFeedback = 0x00000057;

        /// <summary>Parameter offset constant 'Fx1PitchshiftDLevel' from address_map.js.</summary>
        public const uint Fx1PitchshiftDLevel = 0x00000058;

        /// <summary>Parameter offset constant 'Fx1HarmonistVoice' from address_map.js.</summary>
        public const uint Fx1HarmonistVoice = 0x00000059;

        /// <summary>Parameter offset constant 'Fx1HarmonistHarmony1' from address_map.js.</summary>
        public const uint Fx1HarmonistHarmony1 = 0x0000005a;

        /// <summary>Parameter offset constant 'Fx1HarmonistPredelay1' from address_map.js.</summary>
        public const uint Fx1HarmonistPredelay1 = 0x0000005b;

        /// <summary>Parameter offset constant 'Fx1HarmonistLevel1' from address_map.js.</summary>
        public const uint Fx1HarmonistLevel1 = 0x0000005d;

        /// <summary>Parameter offset constant 'Fx1HarmonistHarmony2' from address_map.js.</summary>
        public const uint Fx1HarmonistHarmony2 = 0x0000005e;

        /// <summary>Parameter offset constant 'Fx1HarmonistPredelay2' from address_map.js.</summary>
        public const uint Fx1HarmonistPredelay2 = 0x0000005f;

        /// <summary>Parameter offset constant 'Fx1HarmonistLevel2' from address_map.js.</summary>
        public const uint Fx1HarmonistLevel2 = 0x00000061;

        /// <summary>Parameter offset constant 'Fx1HarmonistFeedback' from address_map.js.</summary>
        public const uint Fx1HarmonistFeedback = 0x00000062;

        /// <summary>Parameter offset constant 'Fx1HarmonistDLevel' from address_map.js.</summary>
        public const uint Fx1HarmonistDLevel = 0x00000063;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm1' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm1 = 0x00000064;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm2' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm2 = 0x00000065;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm3' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm3 = 0x00000066;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm4' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm4 = 0x00000067;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm5' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm5 = 0x00000068;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm6' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm6 = 0x00000069;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm7' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm7 = 0x0000006a;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm8' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm8 = 0x0000006b;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm9' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm9 = 0x0000006c;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm10' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm10 = 0x0000006d;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm11' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm11 = 0x0000006e;

        /// <summary>Parameter offset constant 'Fx1HarmonistV1Harm12' from address_map.js.</summary>
        public const uint Fx1HarmonistV1Harm12 = 0x0000006f;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm1' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm1 = 0x00000070;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm2' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm2 = 0x00000071;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm3' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm3 = 0x00000072;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm4' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm4 = 0x00000073;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm5' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm5 = 0x00000074;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm6' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm6 = 0x00000075;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm7' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm7 = 0x00000076;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm8' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm8 = 0x00000077;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm9' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm9 = 0x00000078;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm10' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm10 = 0x00000079;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm11' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm11 = 0x0000007a;

        /// <summary>Parameter offset constant 'Fx1HarmonistV2Harm12' from address_map.js.</summary>
        public const uint Fx1HarmonistV2Harm12 = 0x0000007b;

        /// <summary>Parameter offset constant 'Fx1AcprocessType' from address_map.js.</summary>
        public const uint Fx1AcprocessType = 0x0000007c;

        /// <summary>Parameter offset constant 'Fx1AcprocessBass' from address_map.js.</summary>
        public const uint Fx1AcprocessBass = 0x0000007d;

        /// <summary>Parameter offset constant 'Fx1AcprocessMid' from address_map.js.</summary>
        public const uint Fx1AcprocessMid = 0x0000007e;

        /// <summary>Parameter offset constant 'Fx1AcprocessMidF' from address_map.js.</summary>
        public const uint Fx1AcprocessMidF = 0x0000007f;

        /// <summary>Parameter offset constant 'Fx1AcprocessTreble' from address_map.js.</summary>
        public const uint Fx1AcprocessTreble = 0x00000100;

        /// <summary>Parameter offset constant 'Fx1AcprocessPresence' from address_map.js.</summary>
        public const uint Fx1AcprocessPresence = 0x00000101;

        /// <summary>Parameter offset constant 'Fx1AcprocessLevel' from address_map.js.</summary>
        public const uint Fx1AcprocessLevel = 0x00000102;

        /// <summary>Parameter offset constant 'Fx1PhaserType' from address_map.js.</summary>
        public const uint Fx1PhaserType = 0x00000103;

        /// <summary>Parameter offset constant 'Fx1PhaserRate' from address_map.js.</summary>
        public const uint Fx1PhaserRate = 0x00000104;

        /// <summary>Parameter offset constant 'Fx1PhaserDepth' from address_map.js.</summary>
        public const uint Fx1PhaserDepth = 0x00000105;

        /// <summary>Parameter offset constant 'Fx1PhaserManual' from address_map.js.</summary>
        public const uint Fx1PhaserManual = 0x00000106;

        /// <summary>Parameter offset constant 'Fx1PhaserResonance' from address_map.js.</summary>
        public const uint Fx1PhaserResonance = 0x00000107;

        /// <summary>Parameter offset constant 'Fx1PhaserSteprate' from address_map.js.</summary>
        public const uint Fx1PhaserSteprate = 0x00000108;

        /// <summary>Parameter offset constant 'Fx1PhaserELevel' from address_map.js.</summary>
        public const uint Fx1PhaserELevel = 0x00000109;

        /// <summary>Parameter offset constant 'Fx1PhaserDLevel' from address_map.js.</summary>
        public const uint Fx1PhaserDLevel = 0x0000010a;

        /// <summary>Parameter offset constant 'Fx1FlangerRate' from address_map.js.</summary>
        public const uint Fx1FlangerRate = 0x0000010b;

        /// <summary>Parameter offset constant 'Fx1FlangerDepth' from address_map.js.</summary>
        public const uint Fx1FlangerDepth = 0x0000010c;

        /// <summary>Parameter offset constant 'Fx1FlangerManual' from address_map.js.</summary>
        public const uint Fx1FlangerManual = 0x0000010d;

        /// <summary>Parameter offset constant 'Fx1FlangerResonance' from address_map.js.</summary>
        public const uint Fx1FlangerResonance = 0x0000010e;

        /// <summary>Parameter offset constant 'Addr_0000010f' from address_map.js.</summary>
        public const uint Addr_0000010f = 0x0000010f;

        /// <summary>Parameter offset constant 'Fx1FlangerLowcut' from address_map.js.</summary>
        public const uint Fx1FlangerLowcut = 0x00000110;

        /// <summary>Parameter offset constant 'Fx1FlangerELevel' from address_map.js.</summary>
        public const uint Fx1FlangerELevel = 0x00000111;

        /// <summary>Parameter offset constant 'Fx1FlangerDLevel' from address_map.js.</summary>
        public const uint Fx1FlangerDLevel = 0x00000112;

        /// <summary>Parameter offset constant 'Fx1TremoloWaveshape' from address_map.js.</summary>
        public const uint Fx1TremoloWaveshape = 0x00000113;

        /// <summary>Parameter offset constant 'Fx1TremoloRate' from address_map.js.</summary>
        public const uint Fx1TremoloRate = 0x00000114;

        /// <summary>Parameter offset constant 'Fx1TremoloDepth' from address_map.js.</summary>
        public const uint Fx1TremoloDepth = 0x00000115;

        /// <summary>Parameter offset constant 'Fx1TremoloLevel' from address_map.js.</summary>
        public const uint Fx1TremoloLevel = 0x00000116;

        /// <summary>Parameter offset constant 'Addr_00000117' from address_map.js.</summary>
        public const uint Addr_00000117 = 0x00000117;

        /// <summary>Parameter offset constant 'Addr_00000118' from address_map.js.</summary>
        public const uint Addr_00000118 = 0x00000118;

        /// <summary>Parameter offset constant 'Fx1RotaryRateFast' from address_map.js.</summary>
        public const uint Fx1RotaryRateFast = 0x00000119;

        /// <summary>Parameter offset constant 'Addr_0000011a' from address_map.js.</summary>
        public const uint Addr_0000011a = 0x0000011a;

        /// <summary>Parameter offset constant 'Addr_0000011b' from address_map.js.</summary>
        public const uint Addr_0000011b = 0x0000011b;

        /// <summary>Parameter offset constant 'Fx1RotaryDepth' from address_map.js.</summary>
        public const uint Fx1RotaryDepth = 0x0000011c;

        /// <summary>Parameter offset constant 'Fx1RotaryLevel' from address_map.js.</summary>
        public const uint Fx1RotaryLevel = 0x0000011d;

        /// <summary>Parameter offset constant 'Fx1UniVRate' from address_map.js.</summary>
        public const uint Fx1UniVRate = 0x0000011e;

        /// <summary>Parameter offset constant 'Fx1UniVDepth' from address_map.js.</summary>
        public const uint Fx1UniVDepth = 0x0000011f;

        /// <summary>Parameter offset constant 'Fx1UniVLevel' from address_map.js.</summary>
        public const uint Fx1UniVLevel = 0x00000120;

        /// <summary>Parameter offset constant 'Fx1SlicerPattern' from address_map.js.</summary>
        public const uint Fx1SlicerPattern = 0x00000121;

        /// <summary>Parameter offset constant 'Fx1SlicerRate' from address_map.js.</summary>
        public const uint Fx1SlicerRate = 0x00000122;

        /// <summary>Parameter offset constant 'Fx1SlicerTrigsens' from address_map.js.</summary>
        public const uint Fx1SlicerTrigsens = 0x00000123;

        /// <summary>Parameter offset constant 'Fx1SlicerEffectLevel' from address_map.js.</summary>
        public const uint Fx1SlicerEffectLevel = 0x00000124;

        /// <summary>Parameter offset constant 'Fx1SlicerDirectLevel' from address_map.js.</summary>
        public const uint Fx1SlicerDirectLevel = 0x00000125;

        /// <summary>Parameter offset constant 'Fx1VibratoRate' from address_map.js.</summary>
        public const uint Fx1VibratoRate = 0x00000126;

        /// <summary>Parameter offset constant 'Fx1VibratoDepth' from address_map.js.</summary>
        public const uint Fx1VibratoDepth = 0x00000127;

        /// <summary>Parameter offset constant 'Addr_00000128' from address_map.js.</summary>
        public const uint Addr_00000128 = 0x00000128;

        /// <summary>Parameter offset constant 'Addr_00000129' from address_map.js.</summary>
        public const uint Addr_00000129 = 0x00000129;

        /// <summary>Parameter offset constant 'Fx1VibratoLevel' from address_map.js.</summary>
        public const uint Fx1VibratoLevel = 0x0000012a;

        /// <summary>Parameter offset constant 'Fx1RingmodMode' from address_map.js.</summary>
        public const uint Fx1RingmodMode = 0x0000012b;

        /// <summary>Parameter offset constant 'Fx1RingmodFreq' from address_map.js.</summary>
        public const uint Fx1RingmodFreq = 0x0000012c;

        /// <summary>Parameter offset constant 'Fx1RingmodELevel' from address_map.js.</summary>
        public const uint Fx1RingmodELevel = 0x0000012d;

        /// <summary>Parameter offset constant 'Fx1RingmodDLevel' from address_map.js.</summary>
        public const uint Fx1RingmodDLevel = 0x0000012e;

        /// <summary>Parameter offset constant 'Fx1HumanizerMode' from address_map.js.</summary>
        public const uint Fx1HumanizerMode = 0x0000012f;

        /// <summary>Parameter offset constant 'Fx1HumanizerVowel1' from address_map.js.</summary>
        public const uint Fx1HumanizerVowel1 = 0x00000130;

        /// <summary>Parameter offset constant 'Fx1HumanizerVowel2' from address_map.js.</summary>
        public const uint Fx1HumanizerVowel2 = 0x00000131;

        /// <summary>Parameter offset constant 'Fx1HumanizerSens' from address_map.js.</summary>
        public const uint Fx1HumanizerSens = 0x00000132;

        /// <summary>Parameter offset constant 'Fx1HumanizerRate' from address_map.js.</summary>
        public const uint Fx1HumanizerRate = 0x00000133;

        /// <summary>Parameter offset constant 'Fx1HumanizerDepth' from address_map.js.</summary>
        public const uint Fx1HumanizerDepth = 0x00000134;

        /// <summary>Parameter offset constant 'Fx1HumanizerManual' from address_map.js.</summary>
        public const uint Fx1HumanizerManual = 0x00000135;

        /// <summary>Parameter offset constant 'Fx1HumanizerLevel' from address_map.js.</summary>
        public const uint Fx1HumanizerLevel = 0x00000136;

        /// <summary>Parameter offset constant 'Fx12' from address_map.js.</summary>
        public const uint Fx12 = 0x00000137;

        /// <summary>Parameter offset constant 'Fx12_00000138_1' from address_map.js.</summary>
        public const uint Fx12_00000138_1 = 0x00000138;

        /// <summary>Parameter offset constant 'Fx12_00000139_1' from address_map.js.</summary>
        public const uint Fx12_00000139_1 = 0x00000139;

        /// <summary>Parameter offset constant 'Fx12_0000013a_1' from address_map.js.</summary>
        public const uint Fx12_0000013a_1 = 0x0000013a;

        /// <summary>Parameter offset constant 'Fx12_0000013b_1' from address_map.js.</summary>
        public const uint Fx12_0000013b_1 = 0x0000013b;

        /// <summary>Parameter offset constant 'Fx12_0000013c_1' from address_map.js.</summary>
        public const uint Fx12_0000013c_1 = 0x0000013c;

        /// <summary>Parameter offset constant 'Fx12_0000013d_1' from address_map.js.</summary>
        public const uint Fx12_0000013d_1 = 0x0000013d;

        /// <summary>Parameter offset constant 'Fx12_0000013e_1' from address_map.js.</summary>
        public const uint Fx12_0000013e_1 = 0x0000013e;

        /// <summary>Parameter offset constant 'Fx12_0000013f_1' from address_map.js.</summary>
        public const uint Fx12_0000013f_1 = 0x0000013f;

        /// <summary>Parameter offset constant 'Fx12_00000140_1' from address_map.js.</summary>
        public const uint Fx12_00000140_1 = 0x00000140;

        /// <summary>Parameter offset constant 'Fx1AcsimTop' from address_map.js.</summary>
        public const uint Fx1AcsimTop = 0x00000141;

        /// <summary>Parameter offset constant 'Fx1AcsimBody' from address_map.js.</summary>
        public const uint Fx1AcsimBody = 0x00000142;

        /// <summary>Parameter offset constant 'Fx1AcsimLow' from address_map.js.</summary>
        public const uint Fx1AcsimLow = 0x00000143;

        /// <summary>Parameter offset constant 'Addr_00000144' from address_map.js.</summary>
        public const uint Addr_00000144 = 0x00000144;

        /// <summary>Parameter offset constant 'Fx1AcsimLevel' from address_map.js.</summary>
        public const uint Fx1AcsimLevel = 0x00000145;

        /// <summary>Parameter offset constant 'Fx1EvhPhaserScript' from address_map.js.</summary>
        public const uint Fx1EvhPhaserScript = 0x00000146;

        /// <summary>Parameter offset constant 'Fx1EvhPhaserSpeed' from address_map.js.</summary>
        public const uint Fx1EvhPhaserSpeed = 0x00000147;

        /// <summary>Parameter offset constant 'Fx1EvhFlangerManual' from address_map.js.</summary>
        public const uint Fx1EvhFlangerManual = 0x00000148;

        /// <summary>Parameter offset constant 'Fx1EvhFlangerWidth' from address_map.js.</summary>
        public const uint Fx1EvhFlangerWidth = 0x00000149;

        /// <summary>Parameter offset constant 'Fx1EvhFlangerSpeed' from address_map.js.</summary>
        public const uint Fx1EvhFlangerSpeed = 0x0000014a;

        /// <summary>Parameter offset constant 'Fx1EvhFlangerRegen' from address_map.js.</summary>
        public const uint Fx1EvhFlangerRegen = 0x0000014b;

        /// <summary>Parameter offset constant 'Fx1EvhWahPedalPos' from address_map.js.</summary>
        public const uint Fx1EvhWahPedalPos = 0x0000014c;

        /// <summary>Parameter offset constant 'Fx1EvhWahPedalMin' from address_map.js.</summary>
        public const uint Fx1EvhWahPedalMin = 0x0000014d;

        /// <summary>Parameter offset constant 'Fx1EvhWahPedalMax' from address_map.js.</summary>
        public const uint Fx1EvhWahPedalMax = 0x0000014e;

        /// <summary>Parameter offset constant 'Fx1EvhWahEffectLevel' from address_map.js.</summary>
        public const uint Fx1EvhWahEffectLevel = 0x0000014f;

        /// <summary>Parameter offset constant 'Fx1EvhWahDirectMix' from address_map.js.</summary>
        public const uint Fx1EvhWahDirectMix = 0x00000150;

        /// <summary>Parameter offset constant 'Fx1Dc30Selector' from address_map.js.</summary>
        public const uint Fx1Dc30Selector = 0x00000151;

        /// <summary>Parameter offset constant 'Fx1Dc30InputVolume' from address_map.js.</summary>
        public const uint Fx1Dc30InputVolume = 0x00000152;

        /// <summary>Parameter offset constant 'Fx1Dc30ChorusIntensity' from address_map.js.</summary>
        public const uint Fx1Dc30ChorusIntensity = 0x00000153;

        /// <summary>Parameter offset constant 'Fx1Dc30EchoRepeatRate' from address_map.js.</summary>
        public const uint Fx1Dc30EchoRepeatRate = 0x00000154;

        /// <summary>Parameter offset constant 'Fx1Dc30EchoIntensity' from address_map.js.</summary>
        public const uint Fx1Dc30EchoIntensity = 0x00000156;

        /// <summary>Parameter offset constant 'Fx1Dc30EchoVolume' from address_map.js.</summary>
        public const uint Fx1Dc30EchoVolume = 0x00000157;

        /// <summary>Parameter offset constant 'Fx1Dc30Tone' from address_map.js.</summary>
        public const uint Fx1Dc30Tone = 0x00000158;

        /// <summary>Parameter offset constant 'Fx1Dc30Output' from address_map.js.</summary>
        public const uint Fx1Dc30Output = 0x00000159;

        /// <summary>Parameter offset constant 'Fx1HeavyOctave1octLevel' from address_map.js.</summary>
        public const uint Fx1HeavyOctave1octLevel = 0x0000015a;

        /// <summary>Parameter offset constant 'Fx1HeavyOctave2octLevel' from address_map.js.</summary>
        public const uint Fx1HeavyOctave2octLevel = 0x0000015b;

        /// <summary>Parameter offset constant 'Fx1HeavyOctaveDirectLevel' from address_map.js.</summary>
        public const uint Fx1HeavyOctaveDirectLevel = 0x0000015c;

        /// <summary>Parameter offset constant 'Fx1PedalBendPitchMax' from address_map.js.</summary>
        public const uint Fx1PedalBendPitchMax = 0x0000015d;

        /// <summary>Parameter offset constant 'Fx1PedalBendPedalPosition' from address_map.js.</summary>
        public const uint Fx1PedalBendPedalPosition = 0x0000015e;

        /// <summary>Parameter offset constant 'Fx1PedalBendEffectLevel' from address_map.js.</summary>
        public const uint Fx1PedalBendEffectLevel = 0x0000015f;

        /// <summary>Parameter offset constant 'Fx1PedalBendDirectMix' from address_map.js.</summary>
        public const uint Fx1PedalBendDirectMix = 0x00000160;
    }

    // prm_prop_patch_assign
    /// <summary>Parameter offsets for PatchAssignParams (from address_map.js).</summary>
    public static class PatchAssignParams
    {
        /// <summary>Parameter offset constant 'KnobAssignBooster' from address_map.js.</summary>
        public const uint KnobAssignBooster = 0x00000000;

        /// <summary>Parameter offset constant 'KnobAssignDelay' from address_map.js.</summary>
        public const uint KnobAssignDelay = 0x00000001;

        /// <summary>Parameter offset constant 'KnobAssignReverb' from address_map.js.</summary>
        public const uint KnobAssignReverb = 0x00000002;

        /// <summary>Parameter offset constant 'KnobAssignChorus' from address_map.js.</summary>
        public const uint KnobAssignChorus = 0x00000003;

        /// <summary>Parameter offset constant 'KnobAssignFlanger' from address_map.js.</summary>
        public const uint KnobAssignFlanger = 0x00000004;

        /// <summary>Parameter offset constant 'KnobAssignPhaser' from address_map.js.</summary>
        public const uint KnobAssignPhaser = 0x00000005;

        /// <summary>Parameter offset constant 'KnobAssignUniV' from address_map.js.</summary>
        public const uint KnobAssignUniV = 0x00000006;

        /// <summary>Parameter offset constant 'KnobAssignTremolo' from address_map.js.</summary>
        public const uint KnobAssignTremolo = 0x00000007;

        /// <summary>Parameter offset constant 'KnobAssignVibrato' from address_map.js.</summary>
        public const uint KnobAssignVibrato = 0x00000008;

        /// <summary>Parameter offset constant 'KnobAssignRotary' from address_map.js.</summary>
        public const uint KnobAssignRotary = 0x00000009;

        /// <summary>Parameter offset constant 'KnobAssignRingMod' from address_map.js.</summary>
        public const uint KnobAssignRingMod = 0x0000000a;

        /// <summary>Parameter offset constant 'KnobAssignSlowGear' from address_map.js.</summary>
        public const uint KnobAssignSlowGear = 0x0000000b;

        /// <summary>Parameter offset constant 'KnobAssignSlicer' from address_map.js.</summary>
        public const uint KnobAssignSlicer = 0x0000000c;

        /// <summary>Parameter offset constant 'KnobAssignComp' from address_map.js.</summary>
        public const uint KnobAssignComp = 0x0000000d;

        /// <summary>Parameter offset constant 'KnobAssignLimiter' from address_map.js.</summary>
        public const uint KnobAssignLimiter = 0x0000000e;

        /// <summary>Parameter offset constant 'KnobAssignTWah' from address_map.js.</summary>
        public const uint KnobAssignTWah = 0x0000000f;

        /// <summary>Parameter offset constant 'KnobAssignAutoWah' from address_map.js.</summary>
        public const uint KnobAssignAutoWah = 0x00000010;

        /// <summary>Parameter offset constant 'KnobAssignPedalWah' from address_map.js.</summary>
        public const uint KnobAssignPedalWah = 0x00000011;

        /// <summary>Parameter offset constant 'KnobAssignGeq' from address_map.js.</summary>
        public const uint KnobAssignGeq = 0x00000012;

        /// <summary>Parameter offset constant 'KnobAssignPeq' from address_map.js.</summary>
        public const uint KnobAssignPeq = 0x00000013;

        /// <summary>Parameter offset constant 'KnobAssignGuitarSim' from address_map.js.</summary>
        public const uint KnobAssignGuitarSim = 0x00000014;

        /// <summary>Parameter offset constant 'KnobAssignAcGuitarSim' from address_map.js.</summary>
        public const uint KnobAssignAcGuitarSim = 0x00000015;

        /// <summary>Parameter offset constant 'KnobAssignAcProcessor' from address_map.js.</summary>
        public const uint KnobAssignAcProcessor = 0x00000016;

        /// <summary>Parameter offset constant 'KnobAssignWaveSynth' from address_map.js.</summary>
        public const uint KnobAssignWaveSynth = 0x00000017;

        /// <summary>Parameter offset constant 'KnobAssignOctave' from address_map.js.</summary>
        public const uint KnobAssignOctave = 0x00000018;

        /// <summary>Parameter offset constant 'KnobAssignPitchShifter' from address_map.js.</summary>
        public const uint KnobAssignPitchShifter = 0x00000019;

        /// <summary>Parameter offset constant 'KnobAssignHarmonist' from address_map.js.</summary>
        public const uint KnobAssignHarmonist = 0x0000001a;

        /// <summary>Parameter offset constant 'KnobAssignHumanizer' from address_map.js.</summary>
        public const uint KnobAssignHumanizer = 0x0000001b;

        /// <summary>Parameter offset constant 'KnobAssignEvhPhaser' from address_map.js.</summary>
        public const uint KnobAssignEvhPhaser = 0x0000001c;

        /// <summary>Parameter offset constant 'KnobAssignEvhFlanger' from address_map.js.</summary>
        public const uint KnobAssignEvhFlanger = 0x0000001d;

        /// <summary>Parameter offset constant 'KnobAssignEvhWah' from address_map.js.</summary>
        public const uint KnobAssignEvhWah = 0x0000001e;

        /// <summary>Parameter offset constant 'KnobAssignDc30' from address_map.js.</summary>
        public const uint KnobAssignDc30 = 0x0000001f;

        /// <summary>Parameter offset constant 'KnobAssignHeavyOct' from address_map.js.</summary>
        public const uint KnobAssignHeavyOct = 0x00000020;

        /// <summary>Parameter offset constant 'KnobAssignPedalBend' from address_map.js.</summary>
        public const uint KnobAssignPedalBend = 0x00000021;
    }

    // prm_prop_patch_assign_minmax
    /// <summary>Parameter offsets for PatchAssignMinmaxParams (from address_map.js).</summary>
    public static class PatchAssignMinmaxParams
    {
        /// <summary>Parameter offset constant 'ExpPedalAssignBoosterMin' from address_map.js.</summary>
        public const uint ExpPedalAssignBoosterMin = 0x00000000;

        /// <summary>Parameter offset constant 'ExpPedalAssignBoosterMax' from address_map.js.</summary>
        public const uint ExpPedalAssignBoosterMax = 0x00000001;

        /// <summary>Parameter offset constant 'ExpPedalAssignDelayMin' from address_map.js.</summary>
        public const uint ExpPedalAssignDelayMin = 0x00000002;

        /// <summary>Parameter offset constant 'ExpPedalAssignDelayMax' from address_map.js.</summary>
        public const uint ExpPedalAssignDelayMax = 0x00000004;

        /// <summary>Parameter offset constant 'ExpPedalAssignReverbMin' from address_map.js.</summary>
        public const uint ExpPedalAssignReverbMin = 0x00000006;

        /// <summary>Parameter offset constant 'ExpPedalAssignReverbMax' from address_map.js.</summary>
        public const uint ExpPedalAssignReverbMax = 0x00000008;

        /// <summary>Parameter offset constant 'ExpPedalAssignChorusMin' from address_map.js.</summary>
        public const uint ExpPedalAssignChorusMin = 0x0000000a;

        /// <summary>Parameter offset constant 'ExpPedalAssignChorusMax' from address_map.js.</summary>
        public const uint ExpPedalAssignChorusMax = 0x0000000b;

        /// <summary>Parameter offset constant 'ExpPedalAssignFlangerMin' from address_map.js.</summary>
        public const uint ExpPedalAssignFlangerMin = 0x0000000c;

        /// <summary>Parameter offset constant 'ExpPedalAssignFlangerMax' from address_map.js.</summary>
        public const uint ExpPedalAssignFlangerMax = 0x0000000d;

        /// <summary>Parameter offset constant 'ExpPedalAssignPhaserMin' from address_map.js.</summary>
        public const uint ExpPedalAssignPhaserMin = 0x0000000e;

        /// <summary>Parameter offset constant 'ExpPedalAssignPhaserMax' from address_map.js.</summary>
        public const uint ExpPedalAssignPhaserMax = 0x0000000f;

        /// <summary>Parameter offset constant 'ExpPedalAssignUniVMin' from address_map.js.</summary>
        public const uint ExpPedalAssignUniVMin = 0x00000010;

        /// <summary>Parameter offset constant 'ExpPedalAssignUniVMax' from address_map.js.</summary>
        public const uint ExpPedalAssignUniVMax = 0x00000011;

        /// <summary>Parameter offset constant 'ExpPedalAssignTremoloMin' from address_map.js.</summary>
        public const uint ExpPedalAssignTremoloMin = 0x00000012;

        /// <summary>Parameter offset constant 'ExpPedalAssignTremoloMax' from address_map.js.</summary>
        public const uint ExpPedalAssignTremoloMax = 0x00000013;

        /// <summary>Parameter offset constant 'ExpPedalAssignVibratoMin' from address_map.js.</summary>
        public const uint ExpPedalAssignVibratoMin = 0x00000014;

        /// <summary>Parameter offset constant 'ExpPedalAssignVibratoMax' from address_map.js.</summary>
        public const uint ExpPedalAssignVibratoMax = 0x00000015;

        /// <summary>Parameter offset constant 'ExpPedalAssignRotaryMin' from address_map.js.</summary>
        public const uint ExpPedalAssignRotaryMin = 0x00000016;

        /// <summary>Parameter offset constant 'ExpPedalAssignRotaryMax' from address_map.js.</summary>
        public const uint ExpPedalAssignRotaryMax = 0x00000017;

        /// <summary>Parameter offset constant 'ExpPedalAssignRingModMin' from address_map.js.</summary>
        public const uint ExpPedalAssignRingModMin = 0x00000018;

        /// <summary>Parameter offset constant 'ExpPedalAssignRingModMax' from address_map.js.</summary>
        public const uint ExpPedalAssignRingModMax = 0x00000019;

        /// <summary>Parameter offset constant 'ExpPedalAssignSlowGearMin' from address_map.js.</summary>
        public const uint ExpPedalAssignSlowGearMin = 0x0000001a;

        /// <summary>Parameter offset constant 'ExpPedalAssignSlowGearMax' from address_map.js.</summary>
        public const uint ExpPedalAssignSlowGearMax = 0x0000001b;

        /// <summary>Parameter offset constant 'ExpPedalAssignSlicerMin' from address_map.js.</summary>
        public const uint ExpPedalAssignSlicerMin = 0x0000001c;

        /// <summary>Parameter offset constant 'ExpPedalAssignSlicerMax' from address_map.js.</summary>
        public const uint ExpPedalAssignSlicerMax = 0x0000001d;

        /// <summary>Parameter offset constant 'ExpPedalAssignCompMin' from address_map.js.</summary>
        public const uint ExpPedalAssignCompMin = 0x0000001e;

        /// <summary>Parameter offset constant 'ExpPedalAssignCompMax' from address_map.js.</summary>
        public const uint ExpPedalAssignCompMax = 0x0000001f;

        /// <summary>Parameter offset constant 'ExpPedalAssignLimiterMin' from address_map.js.</summary>
        public const uint ExpPedalAssignLimiterMin = 0x00000020;

        /// <summary>Parameter offset constant 'ExpPedalAssignLimiterMax' from address_map.js.</summary>
        public const uint ExpPedalAssignLimiterMax = 0x00000021;

        /// <summary>Parameter offset constant 'ExpPedalAssignTWahMin' from address_map.js.</summary>
        public const uint ExpPedalAssignTWahMin = 0x00000022;

        /// <summary>Parameter offset constant 'ExpPedalAssignTWahMax' from address_map.js.</summary>
        public const uint ExpPedalAssignTWahMax = 0x00000023;

        /// <summary>Parameter offset constant 'ExpPedalAssignAutoWahMin' from address_map.js.</summary>
        public const uint ExpPedalAssignAutoWahMin = 0x00000024;

        /// <summary>Parameter offset constant 'ExpPedalAssignAutoWahMax' from address_map.js.</summary>
        public const uint ExpPedalAssignAutoWahMax = 0x00000025;

        /// <summary>Parameter offset constant 'ExpPedalAssignPedalWahMin' from address_map.js.</summary>
        public const uint ExpPedalAssignPedalWahMin = 0x00000026;

        /// <summary>Parameter offset constant 'ExpPedalAssignPedalWahMax' from address_map.js.</summary>
        public const uint ExpPedalAssignPedalWahMax = 0x00000027;

        /// <summary>Parameter offset constant 'ExpPedalAssignGeqMin' from address_map.js.</summary>
        public const uint ExpPedalAssignGeqMin = 0x00000028;

        /// <summary>Parameter offset constant 'ExpPedalAssignGeqMax' from address_map.js.</summary>
        public const uint ExpPedalAssignGeqMax = 0x00000029;

        /// <summary>Parameter offset constant 'ExpPedalAssignPeqMin' from address_map.js.</summary>
        public const uint ExpPedalAssignPeqMin = 0x0000002a;

        /// <summary>Parameter offset constant 'ExpPedalAssignPeqMax' from address_map.js.</summary>
        public const uint ExpPedalAssignPeqMax = 0x0000002b;

        /// <summary>Parameter offset constant 'ExpPedalAssignGuitarSimMin' from address_map.js.</summary>
        public const uint ExpPedalAssignGuitarSimMin = 0x0000002c;

        /// <summary>Parameter offset constant 'ExpPedalAssignGuitarSimMax' from address_map.js.</summary>
        public const uint ExpPedalAssignGuitarSimMax = 0x0000002d;

        /// <summary>Parameter offset constant 'ExpPedalAssignAcGuitarSimMin' from address_map.js.</summary>
        public const uint ExpPedalAssignAcGuitarSimMin = 0x0000002e;

        /// <summary>Parameter offset constant 'ExpPedalAssignAcGuitarSimMax' from address_map.js.</summary>
        public const uint ExpPedalAssignAcGuitarSimMax = 0x0000002f;

        /// <summary>Parameter offset constant 'ExpPedalAssignAcProcessorMin' from address_map.js.</summary>
        public const uint ExpPedalAssignAcProcessorMin = 0x00000030;

        /// <summary>Parameter offset constant 'ExpPedalAssignAcProcessorMax' from address_map.js.</summary>
        public const uint ExpPedalAssignAcProcessorMax = 0x00000031;

        /// <summary>Parameter offset constant 'ExpPedalAssignWaveSynthMin' from address_map.js.</summary>
        public const uint ExpPedalAssignWaveSynthMin = 0x00000032;

        /// <summary>Parameter offset constant 'ExpPedalAssignWaveSynthMax' from address_map.js.</summary>
        public const uint ExpPedalAssignWaveSynthMax = 0x00000033;

        /// <summary>Parameter offset constant 'ExpPedalAssignOctaveMin' from address_map.js.</summary>
        public const uint ExpPedalAssignOctaveMin = 0x00000034;

        /// <summary>Parameter offset constant 'ExpPedalAssignOctaveMax' from address_map.js.</summary>
        public const uint ExpPedalAssignOctaveMax = 0x00000035;

        /// <summary>Parameter offset constant 'ExpPedalAssignPitchShifterMin' from address_map.js.</summary>
        public const uint ExpPedalAssignPitchShifterMin = 0x00000036;

        /// <summary>Parameter offset constant 'ExpPedalAssignPitchShifterMax' from address_map.js.</summary>
        public const uint ExpPedalAssignPitchShifterMax = 0x00000038;

        /// <summary>Parameter offset constant 'ExpPedalAssignHarmonistMin' from address_map.js.</summary>
        public const uint ExpPedalAssignHarmonistMin = 0x0000003a;

        /// <summary>Parameter offset constant 'ExpPedalAssignHarmonistMax' from address_map.js.</summary>
        public const uint ExpPedalAssignHarmonistMax = 0x0000003c;

        /// <summary>Parameter offset constant 'ExpPedalAssignHumanizerMin' from address_map.js.</summary>
        public const uint ExpPedalAssignHumanizerMin = 0x0000003e;

        /// <summary>Parameter offset constant 'ExpPedalAssignHumanizerMax' from address_map.js.</summary>
        public const uint ExpPedalAssignHumanizerMax = 0x0000003f;

        /// <summary>Parameter offset constant 'ExpPedalAssignEvhPhaserMin' from address_map.js.</summary>
        public const uint ExpPedalAssignEvhPhaserMin = 0x00000040;

        /// <summary>Parameter offset constant 'ExpPedalAssignEvhPhaserMax' from address_map.js.</summary>
        public const uint ExpPedalAssignEvhPhaserMax = 0x00000041;

        /// <summary>Parameter offset constant 'ExpPedalAssignEvhFlangerMin' from address_map.js.</summary>
        public const uint ExpPedalAssignEvhFlangerMin = 0x00000042;

        /// <summary>Parameter offset constant 'ExpPedalAssignEvhFlangerMax' from address_map.js.</summary>
        public const uint ExpPedalAssignEvhFlangerMax = 0x00000043;

        /// <summary>Parameter offset constant 'ExpPedalAssignEvhWahMin' from address_map.js.</summary>
        public const uint ExpPedalAssignEvhWahMin = 0x00000044;

        /// <summary>Parameter offset constant 'ExpPedalAssignEvhWahMax' from address_map.js.</summary>
        public const uint ExpPedalAssignEvhWahMax = 0x00000045;

        /// <summary>Parameter offset constant 'ExpPedalAssignDc30Min' from address_map.js.</summary>
        public const uint ExpPedalAssignDc30Min = 0x00000046;

        /// <summary>Parameter offset constant 'ExpPedalAssignDc30Max' from address_map.js.</summary>
        public const uint ExpPedalAssignDc30Max = 0x00000048;

        /// <summary>Parameter offset constant 'ExpPedalAssignHeavyOctMin' from address_map.js.</summary>
        public const uint ExpPedalAssignHeavyOctMin = 0x0000004a;

        /// <summary>Parameter offset constant 'ExpPedalAssignHeavyOctMax' from address_map.js.</summary>
        public const uint ExpPedalAssignHeavyOctMax = 0x0000004b;

        /// <summary>Parameter offset constant 'ExpPedalAssignPedalBendMin' from address_map.js.</summary>
        public const uint ExpPedalAssignPedalBendMin = 0x0000004c;

        /// <summary>Parameter offset constant 'ExpPedalAssignPedalBendMax' from address_map.js.</summary>
        public const uint ExpPedalAssignPedalBendMax = 0x0000004d;
    }

    // prm_prop_patch_ctrl_assign
    /// <summary>Parameter offsets for PatchCtrlAssignParams (from address_map.js).</summary>
    public static class PatchCtrlAssignParams
    {
        /// <summary>Parameter offset constant 'PedalFunctionGafcExp3' from address_map.js.</summary>
        public const uint PedalFunctionGafcExp3 = 0x00000000;

        /// <summary>Parameter offset constant 'PedalFunctionGafcExExp1' from address_map.js.</summary>
        public const uint PedalFunctionGafcExExp1 = 0x00000001;

        /// <summary>Parameter offset constant 'PedalFunctionGafcExExp2' from address_map.js.</summary>
        public const uint PedalFunctionGafcExExp2 = 0x00000002;

        /// <summary>Parameter offset constant 'PedalFunctionGafcExExp3' from address_map.js.</summary>
        public const uint PedalFunctionGafcExExp3 = 0x00000003;

        /// <summary>Parameter offset constant 'FsFunctionGafcFs3' from address_map.js.</summary>
        public const uint FsFunctionGafcFs3 = 0x00000004;

        /// <summary>Parameter offset constant 'FsFunctionGafcExFs1' from address_map.js.</summary>
        public const uint FsFunctionGafcExFs1 = 0x00000005;

        /// <summary>Parameter offset constant 'FsFunctionGafcExFs2' from address_map.js.</summary>
        public const uint FsFunctionGafcExFs2 = 0x00000006;

        /// <summary>Parameter offset constant 'FsFunctionGafcExFs3' from address_map.js.</summary>
        public const uint FsFunctionGafcExFs3 = 0x00000007;
    }

    // prm_prop_patch_fs_assign
    /// <summary>Parameter offsets for PatchFsAssignParams (from address_map.js).</summary>
    public static class PatchFsAssignParams
    {
        /// <summary>Parameter offset constant 'FsFunctionFs1Tip' from address_map.js.</summary>
        public const uint FsFunctionFs1Tip = 0x00000000;

        /// <summary>Parameter offset constant 'FsFunctionFs1Ring' from address_map.js.</summary>
        public const uint FsFunctionFs1Ring = 0x00000001;
    }

    // prm_prop_mk2_v2
    /// <summary>Parameter offsets for Mk2V2Params (from address_map.js).</summary>
    public static class Mk2V2Params
    {
        /// <summary>Parameter offset constant 'PositionSoloEq' from address_map.js.</summary>
        public const uint PositionSoloEq = 0x00000000;

        /// <summary>Parameter offset constant 'SoloEqSw' from address_map.js.</summary>
        public const uint SoloEqSw = 0x00000001;

        /// <summary>Parameter offset constant 'SoloEqLowCut' from address_map.js.</summary>
        public const uint SoloEqLowCut = 0x00000002;

        /// <summary>Parameter offset constant 'SoloEqLowGain' from address_map.js.</summary>
        public const uint SoloEqLowGain = 0x00000003;

        /// <summary>Parameter offset constant 'SoloEqMidFreq' from address_map.js.</summary>
        public const uint SoloEqMidFreq = 0x00000004;

        /// <summary>Parameter offset constant 'SoloEqMidQ' from address_map.js.</summary>
        public const uint SoloEqMidQ = 0x00000005;

        /// <summary>Parameter offset constant 'SoloEqMidGain' from address_map.js.</summary>
        public const uint SoloEqMidGain = 0x00000006;

        /// <summary>Parameter offset constant 'SoloEqHighGain' from address_map.js.</summary>
        public const uint SoloEqHighGain = 0x00000007;

        /// <summary>Parameter offset constant 'SoloEqHighCut' from address_map.js.</summary>
        public const uint SoloEqHighCut = 0x00000008;

        /// <summary>Parameter offset constant 'SoloEqLevel' from address_map.js.</summary>
        public const uint SoloEqLevel = 0x00000009;

        /// <summary>Parameter offset constant 'SoloDelaySw' from address_map.js.</summary>
        public const uint SoloDelaySw = 0x0000000a;

        /// <summary>Parameter offset constant 'SoloDelayCarryover' from address_map.js.</summary>
        public const uint SoloDelayCarryover = 0x0000000b;

        /// <summary>Parameter offset constant 'SoloDelayTime' from address_map.js.</summary>
        public const uint SoloDelayTime = 0x0000000c;

        /// <summary>Parameter offset constant 'SoloDelayFeedback' from address_map.js.</summary>
        public const uint SoloDelayFeedback = 0x0000000e;

        /// <summary>Parameter offset constant 'SoloDelayEffectLevel' from address_map.js.</summary>
        public const uint SoloDelayEffectLevel = 0x0000000f;

        /// <summary>Parameter offset constant 'SoloDelayDirectLevel' from address_map.js.</summary>
        public const uint SoloDelayDirectLevel = 0x00000010;

        /// <summary>Parameter offset constant 'SoloDelayFilter' from address_map.js.</summary>
        public const uint SoloDelayFilter = 0x00000011;

        /// <summary>Parameter offset constant 'SoloDelayHighCut' from address_map.js.</summary>
        public const uint SoloDelayHighCut = 0x00000012;

        /// <summary>Parameter offset constant 'SoloDelayModSw' from address_map.js.</summary>
        public const uint SoloDelayModSw = 0x00000013;

        /// <summary>Parameter offset constant 'SoloDelayModRate' from address_map.js.</summary>
        public const uint SoloDelayModRate = 0x00000014;

        /// <summary>Parameter offset constant 'SoloDelayModDepth' from address_map.js.</summary>
        public const uint SoloDelayModDepth = 0x00000015;
    }

    // prm_prop_status
    /// <summary>Parameter offsets for StatusParams (from address_map.js).</summary>
    public static class StatusParams
    {
        /// <summary>Parameter offset constant 'ExpPedalAssignBoosterMin' from address_map.js.</summary>
        public const uint ExpPedalAssignBoosterMin = 0x00000000;

        /// <summary>Parameter offset constant 'V' from address_map.js.</summary>
        public const uint V = 0x00000001;
    }
}
