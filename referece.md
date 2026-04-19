#04D939

#DBF227

#F2CB05

#3C3F40

Ah, seeing the source code completely changes the context! Now I see exactly what you are doing. You are working with Roland/BOSS MIDI System Exclusive (SysEx) addressing for the Katana amplifier.

With this context, the two addresses from your previous question **are related, but they were actually pointing to two entirely different "Gain" knobs.**

Here is the exact formula for how `address_map.js` translates into the byte arrays found in `KatanaMkIIParameterCatalog.cs`.

---

### The Roland SysEx Mapping Formula

To get the 4-byte array used in the C# catalog, you must add three hex values together:
**Base Address + Block Offset + Parameter Offset = Absolute Address**

Once you have the Absolute Address, you simply split it into 4 bytes.

Let's break down exactly how this works using your files so you can map the missing values.

#### 1. The Base Address (The Memory Area)
Look at the very bottom of `address_map.js` at `this.root`. The amp has different areas of memory. The one used for live, real-time tweaking is the **Temporary** patch area.
* **Temporary Base Address = `0x60000000`**

#### 2. The Block Offset
Look at the `var Patch = [...]` array. The Temporary patch is broken up into functional blocks (EQ, FX, Delay, etc.).
* `Patch_0` (Preamp settings) is at `0x00000010`
* `Status` (Front panel physical knobs) is at `0x00000650`

#### 3. The Parameter Offset
Look at the individual parameter lists, like `prm_prop_patch_0` or `prm_prop_patch_status`. This is the specific byte where the setting lives inside its block.

---

### Solving the Mystery of Your First Question

In your first question, you combined the parameter offset for **Preamp Gain** with the final byte array for **Panel Amp Gain**. Here is how the math actually works out for both.

**Example A: Preamp Gain (The stored patch value)**
If you look inside `prm_prop_patch_0`, you will find your `0x12` address:
`{ addr:0x00000012 ... name:'GAIN' }` // PRM_PREAMP_A_GAIN

Let's calculate the absolute address:
* Base (Temporary): `0x60000000`
* Block (Patch_0): `0x00000010`
* Parameter (Gain): `0x00000012`
* **Total Sum = `0x60000022`**

If you look in your C# file, you will see this matches perfectly:
```csharp
public static KatanaParameterDefinition PreampGain { get; } =
    new("preamp-gain", "Preamp Gain", [0x60, 0x00, 0x00, 0x22], ...);
```

**Example B: Panel Amp Gain (The physical knob)**
If you look inside `prm_prop_patch_status`, you will find a different Gain:
`{ addr:0x00000001 ... name:'GAIN' }` // PRM_KNOB_POS_GAIN

Let's calculate the absolute address:
* Base (Temporary): `0x60000000`
* Block (Status): `0x00000650`
* Parameter (Gain): `0x00000001`
* **Total Sum = `0x60000651`**

And checking your C# file, this perfectly matches the byte array from your original question:
```csharp
public static KatanaParameterDefinition AmpGain { get; } =
    new("amp-gain", "Gain", [0x60, 0x00, 0x06, 0x51], ...);
```

---

### How to Add Missing Values

When you want to add a new parameter to your C# catalog, follow these steps:
1.  Find the parameter you want in `address_map.js` (e.g., `TREMOLO DEPTH` is `0x00000115` inside `prm_prop_patch_fx`).
2.  Find the block offset for `prm_prop_patch_fx`. In the `Patch` list, `Fx(1)` is at `0x00000100`.
3.  Add the Temporary Base (`0x60000000`), the Block (`0x00000100`), and the Parameter (`0x00000115`).
4.  Your final hex is `0x60000215`.
5.  Split it into your C# byte array: `[0x60, 0x00, 0x02, 0x15]`.

*(Note: Roland SysEx uses 7-bit math, so addresses jump from `0x7F` to `0x100`. The script handles this with the `nibbled` function, but standard hexadecimal addition works perfectly for constructing these 4-byte arrays).*

Which specific effects block or parameters are you looking to map next? I'd be happy to help you calculate the first few to make sure the formula is working for you.

```js
/**
 * Calculates the Roland SysEx 4-byte address array for a given parameter.
 * * @param {number} baseAddress - The root memory area (e.g., 0x60000000 for Temporary patch)
 * @param {number} blockOffset - The offset of the functional block (e.g., 0x00000100 for Fx1)
 * @param {number} parameterOffset - The specific parameter address from the map (e.g., 0x00000115)
 * @returns {string} The formatted byte array string for C#
 */
function getKatanaAddress(baseAddress, blockOffset, parameterOffset) {
    // 1. Calculate the absolute address
    const absoluteAddress = baseAddress + blockOffset + parameterOffset;

    // 2. Extract the 4 individual bytes using bitwise shifts
    const byte1 = (absoluteAddress >>> 24) & 0xFF;
    const byte2 = (absoluteAddress >>> 16) & 0xFF;
    const byte3 = (absoluteAddress >>> 8) & 0xFF;
    const byte4 = absoluteAddress & 0xFF;

    // 3. Helper to format a single byte as "0xXX"
    const toHex = (byte) => `0x${byte.toString(16).padStart(2, '0').toUpperCase()}`;

    // 4. Return the formatted C# array string
    return `[${toHex(byte1)}, ${toHex(byte2)}, ${toHex(byte3)}, ${toHex(byte4)}]`;
}

// ==========================================
// TEST CASES (Based on your files)
// ==========================================

const BASE_TEMPORARY = 0x60000000;

// Example 1: Preamp Gain (Block: Patch_0 0x10, Param: 0x12)
console.log("Preamp Gain:  ", getKatanaAddress(BASE_TEMPORARY, 0x00000010, 0x00000012));
// Expected output: [0x60, 0x00, 0x00, 0x22]

// Example 2: Panel Amp Gain (Block: Status 0x650, Param: 0x01)
console.log("Panel Amp Gain:", getKatanaAddress(BASE_TEMPORARY, 0x00000650, 0x00000001));
// Expected output: [0x60, 0x00, 0x06, 0x51]

// Example 3: Tremolo Depth (Block: Fx(1) 0x100, Param: 0x115)
console.log("Tremolo Depth: ", getKatanaAddress(BASE_TEMPORARY, 0x00000100, 0x00000115));
// Expected output: [0x60, 0x00, 0x02, 0x15]
```

These three addresses represent the "levels" of memory inside the amp. To understand them, think of the Katana like a computer: **System** is the OS settings, **Temporary** is the RAM (what you are currently hearing), and **Status** is the hardware monitor.

Here is the breakdown of what each one specifically does on your amp:

---

### 1. Temporary (`0x60000000`) - "The Live Sound"
This is the most important one. It is the **Active Edit Buffer**.
* **What it is:** Whatever sound is coming out of your speaker right now is defined by the values in this memory block.
* **When you change a knob:** The amp updates the value in this `0x60000000` range immediately.
* **Banks & Panel:** It doesn't matter if you are on CH1, CH2, or Panel mode—the amp always uses the "Temporary" buffer to make sound.
    * When you click **CH1**, the amp *copies* the saved data from the CH1 storage into this Temporary buffer.
    * When you click **Panel**, the amp *copies* the current physical positions of the knobs into this Temporary buffer.



### 2. Status (`0x7F010200`) - "The Physical State"
This address doesn't control the sound directly; it reports what the physical hardware is doing.
* **The Hardware Monitor:** This block tells you which button is currently lit up (CH1, CH2, Panel) and the actual physical rotation of the pots on the top of the amp.
* **Why it exists:** In your C# code, you saw `AmpGain` at `0x60000651`. This is in the Status block. This tells your software, "The user just physically turned the Gain knob to 50." Your software can then use that info to update a UI slider.
* **Bank Switching:** If you want to know *which* button the user just pressed on the amp, you watch the `Status` block.

### 3. System (`0x00000000`) - "The Global Brain"
These settings are "Set and Forget." They do not change when you switch patches or banks.
* **Global EQ:** If you want to cut the bass for every single patch at once because the room is boomy, you change the Global EQ in the **System** block.
* **USB/Audio:** USB Input/Output levels and "Line Out Air Feel" (Rec/Live/Blend) live here.
* **Hardware Config:** MIDI channels, Footswitch settings, and the Knob Mode (Immediate vs. Catch-up).

---

### Summary Comparison Table

| Address Root | Purpose | Does it change with Patches? |
| :--- | :--- | :--- |
| **System** (`0x00`) | Global hardware settings (USB, Global EQ) | **No.** Stays the same for all patches. |
| **Temporary** (`0x60`) | The sound you are hearing right now. | **Yes.** It is overwritten every time you switch. |
| **Status** (`0x7F`) | Hardware feedback (Lights, Button presses). | **N/A.** It reports the current hardware state. |

### How this helps you with your C# Catalog:
* If you want to **edit the current sound** (change the Gain, turn on a Booster): Use **Temporary** (`0x60...`).
* If you want to **monitor the user** (see if they switched to CH2): Use **Status** (`0x7F...`).
* If you want to **set the Global EQ**: Use **System** (`0x00...`).

**Note on Saved Banks:** The actual saved "CH1" or "CH2" patches live in the `0x10000000` range (User Patches). You generally don't edit those directly while playing. You edit the **Temporary** buffer, and then send a "Write" command to save that buffer into one of the `0x10...` slots.
