//
// address_map.js
//
// Copyright 2018 Roland Corporation. All rights reserved.
//
// 20220627_1547

function AddressMap() {

	/* parameter definitions  */

	var prm_prop_system = [	// 0x00000000
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:0   , name:'USB IN OUT MODE'        },    // PRM_SYS_USB_IN_OUT_MODE
        { addr:0x00000001, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'USB INPUT LEVEL'        },    // PRM_SYS_USB_INPUT_LEVEL
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'USB OUT LEVEL'          },    // PRM_SYS_USB_OUT_LEV
        { addr:0x00000003, size:INTEGER1x7        , ofs:0   , init:25  , min:0   , max:100 , name:'USB MIX LEVEL'          },    // PRM_SYS_USB_MIX_LEV
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:1   , min:1   , max:1   , name:'USB DIR MONITOR'        },    // PRM_SYS_USB_DIR_MONITOR
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:0   , name:'USB DIR MONITOR CMD'    },    // PRM_SYS_USB_DIR_MONITOR_CMD
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'USB LOOPBACK'           },    // PRM_SYS_USB_LOOPBACK
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'USB2 OUT LEVEL'         },    // PRM_SYS_USB_2_OUT_LEV
        { addr:0x00000008, size:(PADDING | 0x8)   , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // (padding)
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_SYS_EQ_SW
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'TYPE'                   },    // PRM_SYS_EQ_TYPE
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:3   , name:'POSITION'               },    // PRM_SYS_EQ_POSITION                 //V200
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:17  , name:'LOW CUT'                },    // PRM_SYS_EQ_PEQ_LOW_CUT
        { addr:0x00000014, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW GAIN'               },    // PRM_SYS_EQ_PEQ_LOW_GAIN
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:13  , min:0   , max:27  , name:'LOW-MID FREQ'           },    // PRM_SYS_EQ_PEQ_LOWMID_FREQ
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'LOW-MID Q'              },    // PRM_SYS_EQ_PEQ_LOWMID_Q
        { addr:0x00000017, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW-MID GAIN'           },    // PRM_SYS_EQ_PEQ_LOWMID_GAIN
        { addr:0x00000018, size:INTEGER1x7        , ofs:0   , init:23  , min:0   , max:27  , name:'HIGH-MID FREQ'          },    // PRM_SYS_EQ_PEQ_HIGHMID_FREQ
        { addr:0x00000019, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'HIGH-MID Q'             },    // PRM_SYS_EQ_PEQ_HIGHMID_Q
        { addr:0x0000001a, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH-MID GAIN'          },    // PRM_SYS_EQ_PEQ_HIGHMID_GAIN
        { addr:0x0000001b, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH GAIN'              },    // PRM_SYS_EQ_PEQ_HIGH_GAIN
        { addr:0x0000001c, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:14  , name:'HIGH CUT'               },    // PRM_SYS_EQ_PEQ_HIGH_CUT
        { addr:0x0000001d, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LEVEL'                  },    // PRM_SYS_EQ_PEQ_LEVEL
        { addr:0x0000001e, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'31Hz'                   },    // PRM_SYS_EQ_GEQ_BAND1
        { addr:0x0000001f, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'62Hz'                   },    // PRM_SYS_EQ_GEQ_BAND2
        { addr:0x00000020, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'125Hz'                  },    // PRM_SYS_EQ_GEQ_BAND3
        { addr:0x00000021, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'250Hz'                  },    // PRM_SYS_EQ_GEQ_BAND4
        { addr:0x00000022, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'500Hz'                  },    // PRM_SYS_EQ_GEQ_BAND5
        { addr:0x00000023, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'1KHz'                   },    // PRM_SYS_EQ_GEQ_BAND6
        { addr:0x00000024, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'2KHz'                   },    // PRM_SYS_EQ_GEQ_BAND7
        { addr:0x00000025, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'4KHz'                   },    // PRM_SYS_EQ_GEQ_BAND8
        { addr:0x00000026, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'8KHz'                   },    // PRM_SYS_EQ_GEQ_BAND9
        { addr:0x00000027, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'16KHz'                  },    // PRM_SYS_EQ_GEQ_BAND10
        { addr:0x00000028, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'LEVEL'                  },    // PRM_SYS_EQ_GEQ_LEVEL
        { addr:0x00000029, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'LINE OUT AIR FEEL'      },    // PRM_SYS_LINE_OUT_MODE
        { addr:0x0000002a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'EXPAND MODE'            },    // PRM_SYS_EXPAND_MODE
        { addr:0x0000002b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'TYPE'                   },    // PRM_SYS_PWR_CAB_EQ_TYPE
        { addr:0x0000002c, size:INTEGER1x7        , ofs:-1  , init:1   , min:1   , max:100 , name:'FREQ'                   },    // PRM_SYS_PWR_CAB_EQ_FREQ
        { addr:0x0000002d, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LEVEL'                  },    // PRM_SYS_PWR_CAB_EQ_LEVEL
	];
	var prm_prop_sys_eq_sel = [	// 0x0000002e
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'EQ SELECT'              },    // PRM_SYS_GLOBAL_EQ_SELECT
	];
	var prm_prop_sys_power_adjust = [	// 0x0000002f                                                                                                                   //Ver210
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:8   , name:'HALF POWER ADJUST'      },    // PRM_SYS_HALF_POWER_ADJUST_EDITOR
        ];
	var prm_prop_sys_eq = [	// 0x00000030
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'TYPE'                   },    // PRM_SYS_EQ_GRN_TYPE
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:3   , name:'POSITION'               },    // PRM_SYS_EQ_GRN_POSITION                 //V200
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:17  , name:'LOW CUT'                },    // PRM_SYS_EQ_GRN_PEQ_LOW_CUT
        { addr:0x00000003, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW GAIN'               },    // PRM_SYS_EQ_GRN_PEQ_LOW_GAIN
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:12  , min:0   , max:27  , name:'LOW-MID FREQ'           },    // PRM_SYS_EQ_GRN_PEQ_LOWMID_FREQ
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:5   , name:'LOW-MID Q'              },    // PRM_SYS_EQ_GRN_PEQ_LOWMID_Q
        { addr:0x00000006, size:INTEGER1x7        , ofs:20  , init:-2  , min:-20 , max:20  , name:'LOW-MID GAIN'           },    // PRM_SYS_EQ_GRN_PEQ_LOWMID_GAIN
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:23  , min:0   , max:27  , name:'HIGH-MID FREQ'          },    // PRM_SYS_EQ_GRN_PEQ_HIGHMID_FREQ
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'HIGH-MID Q'             },    // PRM_SYS_EQ_GRN_PEQ_HIGHMID_Q
        { addr:0x00000009, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH-MID GAIN'          },    // PRM_SYS_EQ_GRN_PEQ_HIGHMID_GAIN
        { addr:0x0000000a, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH GAIN'              },    // PRM_SYS_EQ_GRN_PEQ_HIGH_GAIN
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:14  , name:'HIGH CUT'               },    // PRM_SYS_EQ_GRN_PEQ_HIGH_CUT
        { addr:0x0000000c, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LEVEL'                  },    // PRM_SYS_EQ_GRN_PEQ_LEVEL
        { addr:0x0000000d, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'31Hz'                   },    // PRM_SYS_EQ_GRN_GEQ_BAND1
        { addr:0x0000000e, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'62Hz'                   },    // PRM_SYS_EQ_GRN_GEQ_BAND2
        { addr:0x0000000f, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'125Hz'                  },    // PRM_SYS_EQ_GRN_GEQ_BAND3
        { addr:0x00000010, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'250Hz'                  },    // PRM_SYS_EQ_GRN_GEQ_BAND4
        { addr:0x00000011, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'500Hz'                  },    // PRM_SYS_EQ_GRN_GEQ_BAND5
        { addr:0x00000012, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'1KHz'                   },    // PRM_SYS_EQ_GRN_GEQ_BAND6
        { addr:0x00000013, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'2KHz'                   },    // PRM_SYS_EQ_GRN_GEQ_BAND7
        { addr:0x00000014, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'4KHz'                   },    // PRM_SYS_EQ_GRN_GEQ_BAND8
        { addr:0x00000015, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'8KHz'                   },    // PRM_SYS_EQ_GRN_GEQ_BAND9
        { addr:0x00000016, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'16KHz'                  },    // PRM_SYS_EQ_GRN_GEQ_BAND10
        { addr:0x00000017, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'LEVEL'                  },    // PRM_SYS_EQ_GRN_GEQ_LEVEL
	];
        var prm_prop_sys_lineout_custom = [	// 0x00000110                                                                                                                   //Ver200
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'CUSTOM'                 },    // PRM_SYS_LINEOUT_CUSTOM_SW
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'CUSTOM SELECT'          },    // PRM_SYS_LINEOUT_CUSTOM_SELECT
	];
	var prm_prop_sys_lineout_custom_setting = [	// 0x00000112                                                                                                           //Ver200
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:4   , name:'MIC TYPE'               },    // PRM_SYS_LINEOUT_M1_MIC_TYPE
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:20  , name:'MIC DISTANCE'           },    // PRM_SYS_LINEOUT_M1_MIC_DISTANCE
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:10  , name:'MIC POSITION'           },    // PRM_SYS_LINEOUT_M1_MIC_POSITION
	];
	var prm_prop_sys_gafc_function = [	// 0x00000120                                                                                                                   //Ver201
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:6   , min:0   , max:9   , name:'GAFC SW1'               },    // PRM_SYS_GAFC_FUNC_SW1
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:9   , name:'GAFC SW2'               },    // PRM_SYS_GAFC_FUNC_SW2
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:9   , name:'GAFC SW3'               },    // PRM_SYS_GAFC_FUNC_SW3
        { addr:0x00000003, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:9   , name:'GAFC SW4'               },    // PRM_SYS_GAFC_FUNC_SW4
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:9   , name:'GAFC SW5'               },    // PRM_SYS_GAFC_FUNC_SW5
	];
	var prm_prop_info = [	// 0x00010000
        { addr:0x00000000, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:8   , name:'PATCH NUM'              },    // PRM_SYS_PATCH_SEL
	];
	var prm_prop_midi = [	// 0x00000000
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:15  , name:'RX CHANNEL'             },    // PRM_SYS_MIDI_RX_CH
        { addr:0x00000001, size:INTEGER1x7        , ofs:1   , init:15  , min:0   , max:62  , name:'RxCcBoosterSw'          },    // PRM_SYS_MIDI_CC_BOOST_SW
        { addr:0x00000002, size:INTEGER1x7        , ofs:1   , init:16  , min:0   , max:62  , name:'RxCcModSw'              },    // PRM_SYS_MIDI_CC_MOD_SW
        { addr:0x00000003, size:INTEGER1x7        , ofs:1   , init:17  , min:0   , max:62  , name:'RxCcFxSw'               },    // PRM_SYS_MIDI_CC_FX_SW
        { addr:0x00000004, size:INTEGER1x7        , ofs:1   , init:18  , min:0   , max:62  , name:'RxCcDelaySw'            },    // PRM_SYS_MIDI_CC_DELAY_SW
        { addr:0x00000005, size:INTEGER1x7        , ofs:1   , init:19  , min:0   , max:62  , name:'RxCcReverbSw'           },    // PRM_SYS_MIDI_CC_REVERB_SW
        { addr:0x00000006, size:INTEGER1x7        , ofs:1   , init:20  , min:0   , max:62  , name:'RxCcSendReturnSw'       },    // PRM_SYS_MIDI_CC_XLP_SW
        { addr:0x00000007, size:INTEGER1x7        , ofs:1   , init:47  , min:0   , max:62  , name:'RxCcExpPedal'           },    // PRM_SYS_MIDI_CC_EXPPEDAL
        { addr:0x00000008, size:INTEGER1x7        , ofs:1   , init:48  , min:0   , max:62  , name:'RxCcGafcExp1'           },    // PRM_SYS_MIDI_CC_GAFCEXP1
        { addr:0x00000009, size:INTEGER1x7        , ofs:1   , init:49  , min:0   , max:62  , name:'RxCcGafcExp2'           },    // PRM_SYS_MIDI_CC_GAFCEXP2
        { addr:0x0000000a, size:INTEGER1x7        , ofs:1   , init:50  , min:0   , max:62  , name:'RxCcGafcFs1'            },    // PRM_SYS_MIDI_CC_GAFCFS1
        { addr:0x0000000b, size:INTEGER1x7        , ofs:1   , init:51  , min:0   , max:62  , name:'RxCcGafcFs2'            },    // PRM_SYS_MIDI_CC_GAFCFS2
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:127 , name:'RxPcPanel'              },    // PRM_SYS_MIDI_PC_PANEL
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'RxPcACh1'               },    // PRM_SYS_MIDI_PC_A1
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:127 , name:'RxPcACh2'               },    // PRM_SYS_MIDI_PC_A2
        { addr:0x0000000f, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:127 , name:'RxPcACh3'               },    // PRM_SYS_MIDI_PC_A3
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:3   , min:0   , max:127 , name:'RxPcACh4'               },    // PRM_SYS_MIDI_PC_A4
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:127 , name:'RxPcBCh1'               },    // PRM_SYS_MIDI_PC_B1
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:6   , min:0   , max:127 , name:'RxPcBCh2'               },    // PRM_SYS_MIDI_PC_B2
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:7   , min:0   , max:127 , name:'RxPcBCh3'               },    // PRM_SYS_MIDI_PC_B3
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:8   , min:0   , max:127 , name:'RxPcBCh4'               },    // PRM_SYS_MIDI_PC_B4
        { addr:0x00000015, size:INTEGER1x7        , ofs:1   , init:52  , min:0   , max:62  , name:'RxCcGafcExp3'           },    // PRM_SYS_MIDI_CC_GAFCEXP3                    //Ver201
        { addr:0x00000016, size:INTEGER1x7        , ofs:1   , init:53  , min:0   , max:62  , name:'RxCcGafcExExp1'         },    // PRM_SYS_MIDI_CC_GAFCEXEXP1                  //Ver201
        { addr:0x00000017, size:INTEGER1x7        , ofs:1   , init:54  , min:0   , max:62  , name:'RxCcGafcExExp2'         },    // PRM_SYS_MIDI_CC_GAFCEXEXP2                  //Ver201
        { addr:0x00000018, size:INTEGER1x7        , ofs:1   , init:55  , min:0   , max:62  , name:'RxCcGafcExExp3'         },    // PRM_SYS_MIDI_CC_GAFCEXEXP3                  //Ver201
        { addr:0x00000019, size:INTEGER1x7        , ofs:1   , init:56  , min:0   , max:62  , name:'RxCcGafcFs3'            },    // PRM_SYS_MIDI_CC_GAFCFS3                     //Ver201
        { addr:0x0000001a, size:INTEGER1x7        , ofs:1   , init:57  , min:0   , max:62  , name:'RxCcGafcExFs1'          },    // PRM_SYS_MIDI_CC_GAFCEXFS1                   //Ver201
        { addr:0x0000001b, size:INTEGER1x7        , ofs:1   , init:58  , min:0   , max:62  , name:'RxCcGafcExFs2'          },    // PRM_SYS_MIDI_CC_GAFCEXFS2                   //Ver201
        { addr:0x0000001c, size:INTEGER1x7        , ofs:1   , init:59  , min:0   , max:62  , name:'RxCcGafcExFs3'          },    // PRM_SYS_MIDI_CC_GAFCEXFS3                   //Ver201
	];
	var prm_prop_patch_name = [	// 0x00000000
        { addr:0x00000000, size:16                , ofs:0   , init:'KATANA Mk2      ', min:32  , max:125 , name:'PATCH NAME'             },    // PRM_PATCH_NAME0
	];
	var prm_prop_patch_0 = [	// 0x00000010
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_ODDS_SW
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:25  , name:'TYPE'                   },    // PRM_ODDS_TYPE                      Ver200
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:120 , name:'DRIVE'                  },    // PRM_ODDS_DRIVE
        { addr:0x00000003, size:INTEGER1x7        , ofs:50  , init:10  , min:-50 , max:50  , name:'BOTTOM'                 },    // PRM_ODDS_BOTTOM
        { addr:0x00000004, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'TONE'                   },    // PRM_ODDS_TONE
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SOLO SW'                },    // PRM_ODDS_SOLO_SW
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SOLO LEVEL'             },    // PRM_ODDS_SOLO_LEVEL
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:40  , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_ODDS_EFFECT_LEVEL
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_ODDS_DIRECT_LEVEL
        { addr:0x00000009, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:8   , name:''                       },    // 
        { addr:0x0000000a, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000000f, size:(PADDING | 0x1)   , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // (padding)
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:1   , min:1   , max:1   , name:''                       },    // 
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:8   , min:0   , max:32  , name:'TYPE'                   },    // PRM_PREAMP_A_TYPE
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:60  , min:0   , max:120 , name:'GAIN'                   },    // PRM_PREAMP_A_GAIN
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:10  , min:10  , max:10  , name:''                       },    // 
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'BASS'                   },    // PRM_PREAMP_A_BASS
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'MIDDLE'                 },    // PRM_PREAMP_A_MIDDLE
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'TREBLE'                 },    // PRM_PREAMP_A_TREBLE
        { addr:0x00000017, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'PRESENCE'               },    // PRM_PREAMP_A_PRESENCE
        { addr:0x00000018, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_PREAMP_A_LEVEL
        { addr:0x00000019, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'BRIGHT'                 },    // PRM_PREAMP_A_BRIGHT
        { addr:0x0000001a, size:INTEGER1x7        , ofs:0   , init:1   , min:1   , max:1   , name:''                       },    // 
        { addr:0x0000001b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SOLO SW'                },    // PRM_PREAMP_A_SOLO_SW
        { addr:0x0000001c, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SOLO LEVEL'             },    // PRM_PREAMP_A_SOLO_LEVEL
        { addr:0x0000001d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // 
        { addr:0x0000001e, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:4   , name:''                       },    // 
        { addr:0x0000001f, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:''                       },    // 
        { addr:0x00000020, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:10  , name:''                       },    // 
        { addr:0x00000021, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000022, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000023, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:6   , name:''                       },    // 
        { addr:0x00000024, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000025, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000026, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:10  , name:''                       },    // 
        { addr:0x00000027, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:10  , name:''                       },    // 
        { addr:0x00000028, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000029, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000002a, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000002b, size:INTEGER1x7        , ofs:0   , init:7   , min:0   , max:10  , name:''                       },    // 
        { addr:0x0000002c, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:20  , name:''                       },    // 
        { addr:0x0000002d, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:20  , name:''                       },    // 
        { addr:0x0000002e, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:3   , name:''                       },    // 
        { addr:0x0000002f, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:''                       },    // 
        { addr:0x00000030, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_EQ_SW
        { addr:0x00000031, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'TYPE'                   },    // PRM_EQ_TYPE
        { addr:0x00000032, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:17  , name:'LOW CUT'                },    // PRM_EQ_LOW_CUT
        { addr:0x00000033, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW GAIN'               },    // PRM_EQ_LOW_GAIN
        { addr:0x00000034, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:27  , name:'LOW-MID FREQ'           },    // PRM_EQ_LOWMID_FREQ
        { addr:0x00000035, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'LOW-MID Q'              },    // PRM_EQ_LOWMID_Q
        { addr:0x00000036, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW-MID GAIN'           },    // PRM_EQ_LOWMID_GAIN
        { addr:0x00000037, size:INTEGER1x7        , ofs:0   , init:23  , min:0   , max:27  , name:'HIGH-MID FREQ'          },    // PRM_EQ_HIGHMID_FREQ
        { addr:0x00000038, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'HIGH-MID Q'             },    // PRM_EQ_HIGHMID_Q
        { addr:0x00000039, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH-MID GAIN'          },    // PRM_EQ_HIGHMID_GAIN
        { addr:0x0000003a, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH GAIN'              },    // PRM_EQ_HIGH_GAIN
        { addr:0x0000003b, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:14  , name:'HIGH CUT'               },    // PRM_EQ_HIGH_CUT
        { addr:0x0000003c, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LEVEL'                  },    // PRM_EQ_LEVEL
        { addr:0x0000003d, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'31Hz'                   },    // PRM_EQ_GEQ_BAND1
        { addr:0x0000003e, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'62Hz'                   },    // PRM_EQ_GEQ_BAND2
        { addr:0x0000003f, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'125Hz'                  },    // PRM_EQ_GEQ_BAND3
        { addr:0x00000040, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'250Hz'                  },    // PRM_EQ_GEQ_BAND4
        { addr:0x00000041, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'500Hz'                  },    // PRM_EQ_GEQ_BAND5
        { addr:0x00000042, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'1KHz'                   },    // PRM_EQ_GEQ_BAND6
        { addr:0x00000043, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'2KHz'                   },    // PRM_EQ_GEQ_BAND7
        { addr:0x00000044, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'4KHz'                   },    // PRM_EQ_GEQ_BAND8
        { addr:0x00000045, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'8KHz'                   },    // PRM_EQ_GEQ_BAND9
        { addr:0x00000046, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'16KHz'                  },    // PRM_EQ_GEQ_BAND10
        { addr:0x00000047, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'LEVEL'                  },    // PRM_EQ_GEQ_LEVEL
	];
	var prm_prop_patch_eq2 = [	// 0x00000060
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_EQ_SW
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'TYPE'                   },    // PRM_EQ_TYPE
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:17  , name:'LOW CUT'                },    // PRM_EQ_LOW_CUT
        { addr:0x00000003, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW GAIN'               },    // PRM_EQ_LOW_GAIN
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:27  , name:'LOW-MID FREQ'           },    // PRM_EQ_LOWMID_FREQ
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'LOW-MID Q'              },    // PRM_EQ_LOWMID_Q
        { addr:0x00000006, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW-MID GAIN'           },    // PRM_EQ_LOWMID_GAIN
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:23  , min:0   , max:27  , name:'HIGH-MID FREQ'          },    // PRM_EQ_HIGHMID_FREQ
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'HIGH-MID Q'             },    // PRM_EQ_HIGHMID_Q
        { addr:0x00000009, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH-MID GAIN'          },    // PRM_EQ_HIGHMID_GAIN
        { addr:0x0000000a, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH GAIN'              },    // PRM_EQ_HIGH_GAIN
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:14  , name:'HIGH CUT'               },    // PRM_EQ_HIGH_CUT
        { addr:0x0000000c, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LEVEL'                  },    // PRM_EQ_LEVEL
        { addr:0x0000000d, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'31Hz'                   },    // PRM_EQ_GEQ_BAND1
        { addr:0x0000000e, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'62Hz'                   },    // PRM_EQ_GEQ_BAND2
        { addr:0x0000000f, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'125Hz'                  },    // PRM_EQ_GEQ_BAND3
        { addr:0x00000010, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'250Hz'                  },    // PRM_EQ_GEQ_BAND4
        { addr:0x00000011, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'500Hz'                  },    // PRM_EQ_GEQ_BAND5
        { addr:0x00000012, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'1KHz'                   },    // PRM_EQ_GEQ_BAND6
        { addr:0x00000013, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'2KHz'                   },    // PRM_EQ_GEQ_BAND7
        { addr:0x00000014, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'4KHz'                   },    // PRM_EQ_GEQ_BAND8
        { addr:0x00000015, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'8KHz'                   },    // PRM_EQ_GEQ_BAND9
        { addr:0x00000016, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'16KHz'                  },    // PRM_EQ_GEQ_BAND10
        { addr:0x00000017, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'LEVEL'                  },    // PRM_EQ_GEQ_LEVEL
	];
	var prm_prop_patch_fx = [	// 0x00000100
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_FX1_SW
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:29  , min:0   , max:40  , name:'TYPE'                   },    // PRM_FX1_FXTYPE                      Ver200
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'MODE'                   },    // PRM_FX1_TWAH_MODE
        { addr:0x00000003, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'POLARITY'               },    // PRM_FX1_TWAH_POLARITY
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SENS'                   },    // PRM_FX1_TWAH_SENS
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:35  , min:0   , max:100 , name:'FREQ'                   },    // PRM_FX1_TWAH_FREQ
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:35  , min:0   , max:100 , name:'PEAK'                   },    // PRM_FX1_TWAH_PEAK
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_TWAH_D_LEVEL
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_TWAH_LEVEL
        { addr:0x00000009, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'MODE'                   },    // PRM_FX1_AWAH_MODE
        { addr:0x0000000a, size:INTEGER1x7        , ofs:0   , init:35  , min:0   , max:100 , name:'FREQ'                   },    // PRM_FX1_AWAH_FREQ
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'PEAK'                   },    // PRM_FX1_AWAH_PEAK
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_AWAH_RATE
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:60  , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_AWAH_DEPTH
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRET MIX'              },    // PRM_FX1_AWAH_D_LEVEL
        { addr:0x0000000f, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_AWAH_LEVEL
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:5   , name:'TYPE'                   },    // PRM_FX1_SUBWAH_TYPE
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL POS'              },    // PRM_FX1_SUBWAH_PDLPOS
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'PEDAL MIN'              },    // PRM_FX1_SUBWAH_MIN
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL MAX'              },    // PRM_FX1_SUBWAH_MAX
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_SUBWAH_E_LEVEL
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_SUBWAH_D_LEVEL
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:6   , name:'TYPE'                   },    // PRM_FX1_ADCOMP_TYPE
        { addr:0x00000017, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SUSTAIN'                },    // PRM_FX1_ADCOMP_SUSTAIN
        { addr:0x00000018, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'ATTACK'                 },    // PRM_FX1_ADCOMP_ATTACK
        { addr:0x00000019, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'TONE'                   },    // PRM_FX1_ADCOMP_TONE
        { addr:0x0000001a, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_ADCOMP_LEVEL
        { addr:0x0000001b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'TYPE'                   },    // PRM_FX1_LIMITER_TYPE
        { addr:0x0000001c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'ATTACK'                 },    // PRM_FX1_LIMITER_ATTACK
        { addr:0x0000001d, size:INTEGER1x7        , ofs:0   , init:30  , min:0   , max:100 , name:'THRESHOLD'              },    // PRM_FX1_LIMITER_THRESHOLD
        { addr:0x0000001e, size:INTEGER1x7        , ofs:0   , init:11  , min:0   , max:17  , name:'RATIO'                  },    // PRM_FX1_LIMITER_RATIO
        { addr:0x0000001f, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:100 , name:'RELEASE'                },    // PRM_FX1_LIMITER_RELEASE
        { addr:0x00000020, size:INTEGER1x7        , ofs:0   , init:30  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_LIMITER_LEVEL
        { addr:0x00000021, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'31Hz'                   },    // PRM_FX1_GEQ_BAND1
        { addr:0x00000022, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'62Hz'                   },    // PRM_FX1_GEQ_BAND2
        { addr:0x00000023, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'125Hz'                  },    // PRM_FX1_GEQ_BAND3
        { addr:0x00000024, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'250Hz'                  },    // PRM_FX1_GEQ_BAND4
        { addr:0x00000025, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'500Hz'                  },    // PRM_FX1_GEQ_BAND5
        { addr:0x00000026, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'1kHz'                   },    // PRM_FX1_GEQ_BAND6
        { addr:0x00000027, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'2kHz'                   },    // PRM_FX1_GEQ_BAND7
        { addr:0x00000028, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'4kHz'                   },    // PRM_FX1_GEQ_BAND8
        { addr:0x00000029, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'8kHz'                   },    // PRM_FX1_GEQ_BAND9
        { addr:0x0000002a, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'16kHz'                  },    // PRM_FX1_GEQ_BAND10
        { addr:0x0000002b, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LEVEL'                  },    // PRM_FX1_GEQ_LEVEL
        { addr:0x0000002c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:17  , name:'LOW CUT'                },    // PRM_FX1_PEQ_LOW_CUT
        { addr:0x0000002d, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW GAIN'               },    // PRM_FX1_PEQ_LOW_GAIN
        { addr:0x0000002e, size:INTEGER1x7        , ofs:0   , init:13  , min:0   , max:27  , name:'LOW-MID FREQ'           },    // PRM_FX1_PEQ_LOWMID_FREQ
        { addr:0x0000002f, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'LOW-MID Q'              },    // PRM_FX1_PEQ_LOWMID_Q
        { addr:0x00000030, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LOW-MID GAIN'           },    // PRM_FX1_PEQ_LOWMID_GAIN
        { addr:0x00000031, size:INTEGER1x7        , ofs:0   , init:23  , min:0   , max:27  , name:'HIGH-MID FREQ'          },    // PRM_FX1_PEQ_HIGHMID_FREQ
        { addr:0x00000032, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'HIGH-MID Q'             },    // PRM_FX1_PEQ_HIGHMID_Q
        { addr:0x00000033, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH-MID GAIN'          },    // PRM_FX1_PEQ_HIGHMID_GAIN
        { addr:0x00000034, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'HIGH GAIN'              },    // PRM_FX1_PEQ_HIGH_GAIN
        { addr:0x00000035, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:14  , name:'HIGH CUT'               },    // PRM_FX1_PEQ_HIGH_CUT
        { addr:0x00000036, size:INTEGER1x7        , ofs:20  , init:0   , min:-20 , max:20  , name:'LEVEL'                  },    // PRM_FX1_PEQ_LEVEL
        { addr:0x00000037, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:7   , name:'TYPE'                   },    // PRM_FX1_GTRSIM_TYPE
        { addr:0x00000038, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'LOW'                    },    // PRM_FX1_GTRSIM_LOW
        { addr:0x00000039, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'HIGH'                   },    // PRM_FX1_GTRSIM_HIGH
        { addr:0x0000003a, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_GTRSIM_LEVEL
        { addr:0x0000003b, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'BODY'                   },    // PRM_FX1_GTRSIM_BODY
        { addr:0x0000003c, size:INTEGER1x7        , ofs:0   , init:45  , min:0   , max:100 , name:'SENS'                   },    // PRM_FX1_SLOWGEAR_SENS
        { addr:0x0000003d, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'RISE TIME'              },    // PRM_FX1_SLOWGEAR_RISETIME
        { addr:0x0000003e, size:INTEGER1x7        , ofs:0   , init:60  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_SLOWGEAR_LEVEL
        { addr:0x0000003f, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'WAVE'                   },    // PRM_FX1_WAVESYN_WAVE
        { addr:0x00000040, size:INTEGER1x7        , ofs:0   , init:40  , min:0   , max:100 , name:'CUTOFF'                 },    // PRM_FX1_WAVESYN_CUTOFF
        { addr:0x00000041, size:INTEGER1x7        , ofs:0   , init:30  , min:0   , max:100 , name:'RESONANCE'              },    // PRM_FX1_WAVESYN_RESONANCE
        { addr:0x00000042, size:INTEGER1x7        , ofs:0   , init:40  , min:0   , max:100 , name:'FILTER SENS'            },    // PRM_FX1_WAVESYN_FLT_SENS
        { addr:0x00000043, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'FILTER DECAY'           },    // PRM_FX1_WAVESYN_FLT_DECAY
        { addr:0x00000044, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'FILTER DEPTH'           },    // PRM_FX1_WAVESYN_FLT_DEPTH
        { addr:0x00000045, size:INTEGER1x7        , ofs:0   , init:25  , min:0   , max:100 , name:'SYNTH LEVEL'            },    // PRM_FX1_WAVESYN_SYN_LEVEL
        { addr:0x00000046, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_WAVESYN_D_LEVEL
        { addr:0x00000047, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'RANGE'                  },    // PRM_FX1_OCTAVE_RANGE
        { addr:0x00000048, size:INTEGER1x7        , ofs:0   , init:62  , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_OCTAVE_LEVEL
        { addr:0x00000049, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_OCTAVE_D_LEVEL
        { addr:0x0000004a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'VOICE'                  },    // PRM_FX1_PITCHSHIFT_VOICE
        { addr:0x0000004b, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:3   , name:'PS1:MODE'               },    // PRM_FX1_PITCHSHIFT_MODE1
        { addr:0x0000004c, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'PS1:PITCH'              },    // PRM_FX1_PITCHSHIFT_PITCH1
        { addr:0x0000004d, size:INTEGER1x7        , ofs:50  , init:10  , min:-50 , max:50  , name:'PS1:FINE'               },    // PRM_FX1_PITCHSHIFT_FINE1
        { addr:0x0000004e, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:300 , name:'PS1:PRE DELAY'          },    // PRM_FX1_PITCHSHIFT_PREDELAY1
        { addr:0x00000050, size:INTEGER1x7        , ofs:0   , init:70  , min:0   , max:100 , name:'PS1:LEVEL'              },    // PRM_FX1_PITCHSHIFT_LEVEL1
        { addr:0x00000051, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:3   , name:'PS2:MODE'               },    // PRM_FX1_PITCHSHIFT_MODE2
        { addr:0x00000052, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'PS2:PITCH'              },    // PRM_FX1_PITCHSHIFT_PITCH2
        { addr:0x00000053, size:INTEGER1x7        , ofs:50  , init:-10 , min:-50 , max:50  , name:'PS2:FINE'               },    // PRM_FX1_PITCHSHIFT_FINE2
        { addr:0x00000054, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:300 , name:'PS2:PRE DELAY'          },    // PRM_FX1_PITCHSHIFT_PREDELAY2
        { addr:0x00000056, size:INTEGER1x7        , ofs:0   , init:70  , min:0   , max:100 , name:'PS2:LEVEL'              },    // PRM_FX1_PITCHSHIFT_LEVEL2
        { addr:0x00000057, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'PS1:FEEDBACK'           },    // PRM_FX1_PITCHSHIFT_FEEDBACK
        { addr:0x00000058, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_PITCHSHIFT_D_LEVEL
        { addr:0x00000059, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'VOICE'                  },    // PRM_FX1_HARMONIST_VOICE
        { addr:0x0000005a, size:INTEGER1x7        , ofs:0   , init:12  , min:0   , max:29  , name:'HR1:HARMONY'            },    // PRM_FX1_HARMONIST_HARMONY1
        { addr:0x0000005b, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:300 , name:'HR1:PRE DELAY'          },    // PRM_FX1_HARMONIST_PREDELAY1
        { addr:0x0000005d, size:INTEGER1x7        , ofs:0   , init:70  , min:0   , max:100 , name:'HR1:LEVEL'              },    // PRM_FX1_HARMONIST_LEVEL1
        { addr:0x0000005e, size:INTEGER1x7        , ofs:0   , init:7   , min:0   , max:29  , name:'HR2:HARMONY'            },    // PRM_FX1_HARMONIST_HARMONY2
        { addr:0x0000005f, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:300 , name:'HR2:PREDELAY'           },    // PRM_FX1_HARMONIST_PREDELAY2
        { addr:0x00000061, size:INTEGER1x7        , ofs:0   , init:80  , min:0   , max:100 , name:'HR2:LEVEL'              },    // PRM_FX1_HARMONIST_LEVEL2
        { addr:0x00000062, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'HR1:FEEDBACK'           },    // PRM_FX1_HARMONIST_FEEDBACK
        { addr:0x00000063, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_HARMONIST_D_LEVEL
        { addr:0x00000064, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:C'                  },    // PRM_FX1_HARMONIST_V1_HARM_1
        { addr:0x00000065, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:Db'                 },    // PRM_FX1_HARMONIST_V1_HARM_2
        { addr:0x00000066, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:D'                  },    // PRM_FX1_HARMONIST_V1_HARM_3
        { addr:0x00000067, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:Eb'                 },    // PRM_FX1_HARMONIST_V1_HARM_4
        { addr:0x00000068, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:E'                  },    // PRM_FX1_HARMONIST_V1_HARM_5
        { addr:0x00000069, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:F'                  },    // PRM_FX1_HARMONIST_V1_HARM_6
        { addr:0x0000006a, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:F#'                 },    // PRM_FX1_HARMONIST_V1_HARM_7
        { addr:0x0000006b, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:G'                  },    // PRM_FX1_HARMONIST_V1_HARM_8
        { addr:0x0000006c, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:Ab'                 },    // PRM_FX1_HARMONIST_V1_HARM_9
        { addr:0x0000006d, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:A'                  },    // PRM_FX1_HARMONIST_V1_HARM_10
        { addr:0x0000006e, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:Bb'                 },    // PRM_FX1_HARMONIST_V1_HARM_11
        { addr:0x0000006f, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR1:B'                  },    // PRM_FX1_HARMONIST_V1_HARM_12
        { addr:0x00000070, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:C'                  },    // PRM_FX1_HARMONIST_V2_HARM_1
        { addr:0x00000071, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:Db'                 },    // PRM_FX1_HARMONIST_V2_HARM_2
        { addr:0x00000072, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:D'                  },    // PRM_FX1_HARMONIST_V2_HARM_3
        { addr:0x00000073, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:Eb'                 },    // PRM_FX1_HARMONIST_V2_HARM_4
        { addr:0x00000074, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:E'                  },    // PRM_FX1_HARMONIST_V2_HARM_5
        { addr:0x00000075, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:F'                  },    // PRM_FX1_HARMONIST_V2_HARM_6
        { addr:0x00000076, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:F#'                 },    // PRM_FX1_HARMONIST_V2_HARM_7
        { addr:0x00000077, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:G'                  },    // PRM_FX1_HARMONIST_V2_HARM_8
        { addr:0x00000078, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:Ab'                 },    // PRM_FX1_HARMONIST_V2_HARM_9
        { addr:0x00000079, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:A'                  },    // PRM_FX1_HARMONIST_V2_HARM_10
        { addr:0x0000007a, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:Bb'                 },    // PRM_FX1_HARMONIST_V2_HARM_11
        { addr:0x0000007b, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'HR2:B'                  },    // PRM_FX1_HARMONIST_V2_HARM_12
        { addr:0x0000007c, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:3   , name:'TYPE'                   },    // PRM_FX1_ACPROCESS_TYPE
        { addr:0x0000007d, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'BASS'                   },    // PRM_FX1_ACPROCESS_BASS
        { addr:0x0000007e, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'MIDDLE'                 },    // PRM_FX1_ACPROCESS_MID
        { addr:0x0000007f, size:INTEGER1x7        , ofs:0   , init:16  , min:0   , max:27  , name:'MIDDLE FREQ'            },    // PRM_FX1_ACPROCESS_MID_F
        { addr:0x00000100, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'TREBLE'                 },    // PRM_FX1_ACPROCESS_TREBLE
        { addr:0x00000101, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'PRESENCE'               },    // PRM_FX1_ACPROCESS_PRESENCE
        { addr:0x00000102, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_ACPROCESS_LEVEL
        { addr:0x00000103, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'TYPE'                   },    // PRM_FX1_PHASER_TYPE
        { addr:0x00000104, size:INTEGER1x7        , ofs:0   , init:70  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_PHASER_RATE
        { addr:0x00000105, size:INTEGER1x7        , ofs:0   , init:40  , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_PHASER_DEPTH
        { addr:0x00000106, size:INTEGER1x7        , ofs:0   , init:55  , min:0   , max:100 , name:'MANUAL'                 },    // PRM_FX1_PHASER_MANUAL
        { addr:0x00000107, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'RESONANCE'              },    // PRM_FX1_PHASER_RESONANCE
        { addr:0x00000108, size:INTEGER1x7        , ofs:1   , init:-1  , min:-1  , max:100 , name:'STEP RATE'              },    // PRM_FX1_PHASER_STEPRATE
        { addr:0x00000109, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_PHASER_E_LEVEL
        { addr:0x0000010a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_PHASER_D_LEVEL
        { addr:0x0000010b, size:INTEGER1x7        , ofs:0   , init:31  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_FLANGER_RATE
        { addr:0x0000010c, size:INTEGER1x7        , ofs:0   , init:40  , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_FLANGER_DEPTH
        { addr:0x0000010d, size:INTEGER1x7        , ofs:0   , init:82  , min:0   , max:100 , name:'MANUAL'                 },    // PRM_FX1_FLANGER_MANUAL
        { addr:0x0000010e, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'RESONANCE'              },    // PRM_FX1_FLANGER_RESONANCE
        { addr:0x0000010f, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // 
        { addr:0x00000110, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:10  , name:'LOW CUT'                },    // PRM_FX1_FLANGER_LOWCUT
        { addr:0x00000111, size:INTEGER1x7        , ofs:0   , init:60  , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_FLANGER_E_LEVEL
        { addr:0x00000112, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_FLANGER_D_LEVEL
        { addr:0x00000113, size:INTEGER1x7        , ofs:0   , init:70  , min:0   , max:100 , name:'WAVE SHAPE'             },    // PRM_FX1_TREMOLO_WAVESHAPE
        { addr:0x00000114, size:INTEGER1x7        , ofs:0   , init:85  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_TREMOLO_RATE
        { addr:0x00000115, size:INTEGER1x7        , ofs:0   , init:65  , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_TREMOLO_DEPTH
        { addr:0x00000116, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_TREMOLO_LEVEL
        { addr:0x00000117, size:INTEGER1x7        , ofs:0   , init:1   , min:1   , max:1   , name:''                       },    // 
        { addr:0x00000118, size:INTEGER1x7        , ofs:0   , init:30  , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000119, size:INTEGER1x7        , ofs:0   , init:85  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_ROTARY_RATE_FAST
        { addr:0x0000011a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // 
        { addr:0x0000011b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // 
        { addr:0x0000011c, size:INTEGER1x7        , ofs:0   , init:60  , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_ROTARY_DEPTH
        { addr:0x0000011d, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_ROTARY_LEVEL
        { addr:0x0000011e, size:INTEGER1x7        , ofs:0   , init:70  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_UNI_V_RATE
        { addr:0x0000011f, size:INTEGER1x7        , ofs:0   , init:60  , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_UNI_V_DEPTH
        { addr:0x00000120, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_UNI_V_LEVEL
        { addr:0x00000121, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:19  , name:'PATTERN'                },    // PRM_FX1_SLICER_PATTERN
        { addr:0x00000122, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_SLICER_RATE
        { addr:0x00000123, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'TRIGGER SENS'           },    // PRM_FX1_SLICER_TRIGSENS
        { addr:0x00000124, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_SLICER_EFFECT_LEVEL
        { addr:0x00000125, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_SLICER_DIRECT_LEVEL
        { addr:0x00000126, size:INTEGER1x7        , ofs:0   , init:80  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_VIBRATO_RATE
        { addr:0x00000127, size:INTEGER1x7        , ofs:0   , init:45  , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_VIBRATO_DEPTH
        { addr:0x00000128, size:INTEGER1x7        , ofs:0   , init:1   , min:1   , max:1   , name:''                       },    // 
        { addr:0x00000129, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // 
        { addr:0x0000012a, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_VIBRATO_LEVEL
        { addr:0x0000012b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'MODE'                   },    // PRM_FX1_RINGMOD_MODE
        { addr:0x0000012c, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'FREQUENCY'              },    // PRM_FX1_RINGMOD_FREQ
        { addr:0x0000012d, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_RINGMOD_E_LEVEL
        { addr:0x0000012e, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_RINGMOD_D_LEVEL
        { addr:0x0000012f, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'MODE'                   },    // PRM_FX1_HUMANIZER_MODE
        { addr:0x00000130, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'VOWEL1'                 },    // PRM_FX1_HUMANIZER_VOWEL1
        { addr:0x00000131, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:4   , name:'VOWEL2'                 },    // PRM_FX1_HUMANIZER_VOWEL2
        { addr:0x00000132, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SENS'                   },    // PRM_FX1_HUMANIZER_SENS
        { addr:0x00000133, size:INTEGER1x7        , ofs:0   , init:80  , min:0   , max:100 , name:'RATE'                   },    // PRM_FX1_HUMANIZER_RATE
        { addr:0x00000134, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DEPTH'                  },    // PRM_FX1_HUMANIZER_DEPTH
        { addr:0x00000135, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'MANUAL'                 },    // PRM_FX1_HUMANIZER_MANUAL
        { addr:0x00000136, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_HUMANIZER_LEVEL
        { addr:0x00000137, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:16  , name:'XOVER FREQUENCY'        },    // PRM_FX1_2x2CHORUS_XOVERF
        { addr:0x00000138, size:INTEGER1x7        , ofs:0   , init:43  , min:0   , max:100 , name:'LOW RATE'               },    // PRM_FX1_2x2CHORUS_LOW_RATE
        { addr:0x00000139, size:INTEGER1x7        , ofs:0   , init:46  , min:0   , max:100 , name:'LOW DEPTH'              },    // PRM_FX1_2x2CHORUS_LOW_DEPTH
        { addr:0x0000013a, size:INTEGER1x7        , ofs:0   , init:3   , min:0   , max:80  , name:'LOW PRE DELAY'          },    // PRM_FX1_2x2CHORUS_LOW_PREDELAY
        { addr:0x0000013b, size:INTEGER1x7        , ofs:0   , init:75  , min:0   , max:100 , name:'LOW LEVEL'              },    // PRM_FX1_2x2CHORUS_LOW_LEVEL
        { addr:0x0000013c, size:INTEGER1x7        , ofs:0   , init:33  , min:0   , max:100 , name:'HIGH RATE'              },    // PRM_FX1_2x2CHORUS_HIGH_RATE
        { addr:0x0000013d, size:INTEGER1x7        , ofs:0   , init:48  , min:0   , max:100 , name:'HIGH DEPTH'             },    // PRM_FX1_2x2CHORUS_HIGH_DEPTH
        { addr:0x0000013e, size:INTEGER1x7        , ofs:0   , init:3   , min:0   , max:80  , name:'HIGH PRE DELAY'         },    // PRM_FX1_2x2CHORUS_HIGH_PREDELAY
        { addr:0x0000013f, size:INTEGER1x7        , ofs:0   , init:65  , min:0   , max:100 , name:'HIGH LEVEL'             },    // PRM_FX1_2x2CHORUS_HIGH_LEVEL
        { addr:0x00000140, size:INTEGER1x7        , ofs:0   , init:80  , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_2x2CHORUS_DIRECT_LEVEL
        { addr:0x00000141, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'HIGH'                   },    // PRM_FX1_ACSIM_TOP
        { addr:0x00000142, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'BODY'                   },    // PRM_FX1_ACSIM_BODY
        { addr:0x00000143, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'LOW'                    },    // PRM_FX1_ACSIM_LOW
        { addr:0x00000144, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:''                       },    // 
        { addr:0x00000145, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_FX1_ACSIM_LEVEL
        { addr:0x00000146, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'SCRIPT'                 },    // PRM_FX1_EVH_PHASER_SCRIPT
        { addr:0x00000147, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SPEED'                  },    // PRM_FX1_EVH_PHASER_SPEED
        { addr:0x00000148, size:INTEGER1x7        , ofs:0   , init:45  , min:0   , max:100 , name:'MANUAL'                 },    // PRM_FX1_EVH_FLANGER_MANUAL
        { addr:0x00000149, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'WIDTH'                  },    // PRM_FX1_EVH_FLANGER_WIDTH
        { addr:0x0000014a, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SPEED'                  },    // PRM_FX1_EVH_FLANGER_SPEED
        { addr:0x0000014b, size:INTEGER1x7        , ofs:0   , init:80  , min:0   , max:100 , name:'REGEN.'                 },    // PRM_FX1_EVH_FLANGER_REGEN
        { addr:0x0000014c, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL POS'              },    // PRM_FX1_EVH_WAH_PEDAL_POS
        { addr:0x0000014d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'PEDAL MIN'              },    // PRM_FX1_EVH_WAH_PEDAL_MIN
        { addr:0x0000014e, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL MAX'              },    // PRM_FX1_EVH_WAH_PEDAL_MAX
        { addr:0x0000014f, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_EVH_WAH_EFFECT_LEVEL
        { addr:0x00000150, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_EVH_WAH_DIRECT_MIX
        { addr:0x00000151, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SELECT'                 },    // PRM_FX1_DC30_SELECTOR
        { addr:0x00000152, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'INPUT VOLUME'           },    // PRM_FX1_DC30_INPUT_VOLUME
        { addr:0x00000153, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'CHORUS INTENSITY'       },    // PRM_FX1_DC30_CHORUS_INTENSITY
        { addr:0x00000154, size:INTEGER2x7        , ofs:0   , init:400 , min:40  , max:600 , name:'ECHO REPEAT RATE'       },    // PRM_FX1_DC30_ECHO_REPEAT_RATE
        { addr:0x00000156, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'ECHO INTENSISTY'        },    // PRM_FX1_DC30_ECHO_INTENSITY
        { addr:0x00000157, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'ECHO VOLUME'            },    // PRM_FX1_DC30_ECHO_VOLUME
        { addr:0x00000158, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'TONE'                   },    // PRM_FX1_DC30_TONE
        { addr:0x00000159, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'OUTPUT'                 },    // PRM_FX1_DC30_OUTPUT
        { addr:0x0000015a, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'1OCT LEVEL'             },    // PRM_FX1_HEAVY_OCTAVE_1OCT_LEVEL
        { addr:0x0000015b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'2OCT LEVEL'             },    // PRM_FX1_HEAVY_OCTAVE_2OCT_LEVEL
        { addr:0x0000015c, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_HEAVY_OCTAVE_DIRECT_LEVEL
        { addr:0x0000015d, size:INTEGER1x7        , ofs:24  , init:12  , min:-24 , max:24  , name:'PITCH'                  },    // PRM_FX1_PEDAL_BEND_PITCH_MAX                //Ver200
        { addr:0x0000015e, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'PEDAL POS'              },    // PRM_FX1_PEDAL_BEND_PEDAL_POSITION           //Ver200
        { addr:0x0000015f, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_FX1_PEDAL_BEND_EFFECT_LEVEL             //Ver200
        { addr:0x00000160, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_FX1_PEDAL_BEND_DIRECT_MIX               //Ver200
	];
	var prm_prop_patch_delay = [	// 0x00000500
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_DLY_SW
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:10  , name:'TYPE'                   },    // PRM_DLY_TYPE
        { addr:0x00000002, size:INTEGER2x7        , ofs:0   , init:400 , min:1   , max:2000, name:'DELAY TIME'             },    // PRM_DLY_COMMON_DLY_TIME
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:22  , min:0   , max:100 , name:'FEEDBACK'               },    // PRM_DLY_COMMON_FEEDBACK
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:14  , name:'HIGH CUT'               },    // PRM_DLY_COMMON_HICUT
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:120 , name:'EFFECT LEVEL'           },    // PRM_DLY_COMMON_EFFECT_LEVEL
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_DLY_COMMON_DIRECT_LEVEL
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'TAP TIME'               },    // PRM_DLY_PAN_TAPTIME
        { addr:0x00000009, size:INTEGER2x7        , ofs:0   , init:100 , min:1   , max:1000, name:''                       },    // 
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:22  , min:0   , max:100 , name:''                       },    // 
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:14  , name:''                       },    // 
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:120 , name:''                       },    // 
        { addr:0x0000000e, size:INTEGER2x7        , ofs:0   , init:400 , min:1   , max:1000, name:''                       },    // 
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:22  , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:14  , name:''                       },    // 
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:120 , name:''                       },    // 
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:40  , min:0   , max:100 , name:'MOD RATE'               },    // PRM_DLY_MOD_RATE
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:55  , min:0   , max:100 , name:'MOD DEPTH'              },    // PRM_DLY_MOD_DEPTH
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'RANGE'                  },    // PRM_DLY_VINTAGE_LPF
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'FILTER'                 },    // PRM_DLY_VINTAGE_FILTER
        { addr:0x00000017, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'FEEDBACK PHASE'         },    // PRM_DLY_VINTAGE_FEEDBACK_PHASE
        { addr:0x00000018, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'DELAY PHASE'            },    // PRM_DLY_VINTAGE_EFFECT_PHASE
        { addr:0x00000019, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'MOD SW'                 },    // PRM_DLY_VINTAGE_MOD_SW
	];
	var prm_prop_patch_1 = [	// 0x00000540
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_REVERB_SW
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:6   , name:'TYPE'                   },    // PRM_REVERB_TYPE
        { addr:0x00000002, size:INTEGER1x7        , ofs:-1  , init:30  , min:1   , max:100 , name:'REVERB TIME'            },    // PRM_REVERB_TIME
        { addr:0x00000003, size:INTEGER2x7        , ofs:0   , init:10  , min:0   , max:500 , name:'PRE DELAY'              },    // PRM_REVERB_PREDELAY
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:17  , name:'LOW CUT'                },    // PRM_REVERB_LOWCUT
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:8   , min:0   , max:14  , name:'HIGH CUT'               },    // PRM_REVERB_HICUT
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:8   , min:0   , max:10  , name:'DENSITY'                },    // PRM_REVERB_DENSITY
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:35  , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_REVERB_EFFECT_LEVEL
        { addr:0x00000009, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_REVERB_DIRECT_LEVEL
        { addr:0x0000000a, size:(PADDING | 0x1)   , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // (padding)
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'SPRING COLOR'           },    // PRM_REVERB_SPRING_COLOR
        { addr:0x0000000c, size:(PADDING | 0x4)   , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // (padding)
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_PEDAL_FX_SW
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'TYPE'                   },    // PRM_PEDAL_FX_TYPE
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:5   , name:'TYPE'                   },    // PRM_PEDAL_FX_WAH_TYPE
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL POS'              },    // PRM_PEDAL_FX_WAH_PEDAL_POSITION
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'PEDAL MIN'              },    // PRM_PEDAL_FX_WAH_PEDAL_MIN
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL MAX'              },    // PRM_PEDAL_FX_WAH_PEDAL_MAX
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_PEDAL_FX_WAH_EFFECT_LEVEL
        { addr:0x00000017, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_PEDAL_FX_WAH_DIRECT_MIX
        { addr:0x00000018, size:INTEGER1x7        , ofs:24  , init:12  , min:-24 , max:24  , name:'PITCH'                  },    // PRM_PEDAL_FX_PEDAL_BEND_PITCH_MAX
        { addr:0x00000019, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'PEDAL POS'              },    // PRM_PEDAL_FX_PEDAL_BEND_PEDAL_POSITION
        { addr:0x0000001a, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_PEDAL_FX_PEDAL_BEND_EFFECT_LEVEL
        { addr:0x0000001b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_PEDAL_FX_PEDAL_BEND_DIRECT_MIX
        { addr:0x0000001c, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL POS'              },    // PRM_PEDAL_FX_EVH95_POS
        { addr:0x0000001d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'PEDAL MIN'              },    // PRM_PEDAL_FX_EVH95_MIN
        { addr:0x0000001e, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'PEDAL MAX'              },    // PRM_PEDAL_FX_EVH95_MAX
        { addr:0x0000001f, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'EFFECT LEVEL'           },    // PRM_PEDAL_FX_EVH95_E_LEVEL
        { addr:0x00000020, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:100 , name:'DIRECT MIX'             },    // PRM_PEDAL_FX_EVH95_D_LEVEL
        { addr:0x00000021, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'FOOT VOLUME'            },    // PRM_FOOT_VOLUME_VOL_LEVEL
        { addr:0x00000022, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'SW'                     },    // PRM_SEND_RETURN_SW
        { addr:0x00000023, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'MODE'                   },    // PRM_SEND_RETURN_MODE
        { addr:0x00000024, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'SEND LEVEL'             },    // PRM_SEND_RETURN_SEND_LEVEL
        { addr:0x00000025, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'RETURN LEVEL'           },    // PRM_SEND_RETURN_RETURN_LEVEL
        { addr:0x00000026, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_NS1_SW
        { addr:0x00000027, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:100 , name:'THRESHOLD'              },    // PRM_NS1_THRESHOLD
        { addr:0x00000028, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'RELEASE'                },    // PRM_NS1_RELEASE
        { addr:0x00000029, size:(PADDING | 0x7)   , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // (padding)
        { addr:0x00000030, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:''                       },    // 
        { addr:0x00000031, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:11  , name:'MASTER KEY'             },    // PRM_MASTER_KEY
        { addr:0x00000032, size:(PADDING | 0x22)  , ofs:0   , init:0   , min:0   , max:0   , name:''                       },    // (padding)
        { addr:0x00000054, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SW'                     },    // PRM_SOLO_SW
        { addr:0x00000055, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'LEVEL'                  },    // PRM_SOLO_LEVEL
        { addr:0x00000056, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'CONTOUR SW'             },    // PRM_CONTOUR_SW
        { addr:0x00000057, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'CONTOUR SELECT'         },    // PRM_CONTOUR_SELECT                  //Ver200
        { addr:0x00000058, size:INTEGER1x7        , ofs:0   , init:7   , min:7   , max:7   , name:'FS2 FUNCTION'           },    // PRM_FS_FUNCTION_FS2
        { addr:0x00000059, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'POSITION'               },    // PRM_POSITION_EQ2
        { addr:0x0000005a, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'FREQ SHIFT'             },    // PRM_CONTOUR_FREQ_SHIFT
    ];
    var prm_prop_patch_2 = [	// 0x00000620
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:6   , name:'CHAIN'                  },    // PRM_CHAIN_PTN                       //Ver200
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'POSITION'               },    // PRM_POSITION_SEND_RETURN
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'POSITION'               },    // PRM_POSITION_EQ
        { addr:0x00000003, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'POSITION'               },    // PRM_POSITION_PEDAL_FX
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:25  , name:'BOOSTER GRN'            },    // PRM_FXBOX_ASGN_BOOSTER_G            //Ver200
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:11  , min:0   , max:25  , name:'BOOSTER RED'            },    // PRM_FXBOX_ASGN_BOOSTER_R            //Ver200
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:25  , name:'BOOSTER YLW'            },    // PRM_FXBOX_ASGN_BOOSTER_Y            //Ver200
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:29  , min:0   , max:40  , name:'MOD GRN'                },    // PRM_FXBOX_ASGN_MOD_G                //Ver200
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:35  , min:0   , max:40  , name:'MOD RED'                },    // PRM_FXBOX_ASGN_MOD_R                //Ver200
        { addr:0x00000009, size:INTEGER1x7        , ofs:0   , init:36  , min:0   , max:40  , name:'MOD YLW'                },    // PRM_FXBOX_ASGN_MOD_Y                //Ver200
        { addr:0x0000000a, size:INTEGER1x7        , ofs:0   , init:21  , min:0   , max:40  , name:'FX GRN'                 },    // PRM_FXBOX_ASGN_FX_G                 //Ver200
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:40  , name:'FX RED'                 },    // PRM_FXBOX_ASGN_FX_R                 //Ver200
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:39  , min:0   , max:40  , name:'FX YLW'                 },    // PRM_FXBOX_ASGN_FX_Y                 //Ver200
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:10  , name:'DELAY GRN'              },    // PRM_FXBOX_ASGN_DELAY_G  
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:7   , min:0   , max:10  , name:'DELAY RED'              },    // PRM_FXBOX_ASGN_DELAY_R  
        { addr:0x0000000f, size:INTEGER1x7        , ofs:0   , init:8   , min:0   , max:10  , name:'DELAY YLW'              },    // PRM_FXBOX_ASGN_DELAY_Y  
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:6   , name:'REVERB GRN'             },    // PRM_FXBOX_ASGN_REVERB_G 
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:6   , name:'REVERB RED'             },    // PRM_FXBOX_ASGN_REVERB_R 
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:3   , min:0   , max:6   , name:'REVERB YLW'             },    // PRM_FXBOX_ASGN_REVERB_Y 
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:10  , name:'DELAY2 GRN'             },    // PRM_FXBOX_ASGN_DELAY2_G 
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:7   , min:0   , max:10  , name:'DELAY2 RED'             },    // PRM_FXBOX_ASGN_DELAY2_R 
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:8   , min:0   , max:10  , name:'DELAY2 YLW'             },    // PRM_FXBOX_ASGN_DELAY2_Y 
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:2   , name:'LAYER MODE GRN'         },    // PRM_FXBOX_DLYREV_LAYER_G
        { addr:0x00000017, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:2   , name:'LAYER MODE RED'         },    // PRM_FXBOX_DLYREV_LAYER_R
        { addr:0x00000018, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:2   , name:'LAYER MODE YLW'         },    // PRM_FXBOX_DLYREV_LAYER_Y
        { addr:0x00000019, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:''                       },    // PRM_FXBOX_SEL_BOOST
        { addr:0x0000001a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:''                       },    // PRM_FXBOX_SEL_MOD
        { addr:0x0000001b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:''                       },    // PRM_FXBOX_SEL_FX
        { addr:0x0000001c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:''                       },    // PRM_FXBOX_SEL_DELAY
        { addr:0x0000001d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:''                       },    // PRM_FXBOX_SEL_REVERB
        { addr:0x0000001e, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:9   , name:'EXP PEDAL FUNCTION'     },    // PRM_PEDAL_FUNCTION_EXP_PEDAL
        { addr:0x0000001f, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:9   , name:'GAFC EXP1FUNCTION'      },    // PRM_PEDAL_FUNCTION_GAFC_EXP1
        { addr:0x00000020, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:9   , name:'GAFC EXP2FUNCTION'      },    // PRM_PEDAL_FUNCTION_GAFC_EXP2
        { addr:0x00000021, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:9   , name:'GAFC FS1FUNCTION'       },    // PRM_FS_FUNCTION_GAFC_FS1
        { addr:0x00000022, size:INTEGER1x7        , ofs:0   , init:4   , min:0   , max:9   , name:'GAFC FS2FUNCTION'       },    // PRM_FS_FUNCTION_GAFC_FS2
        { addr:0x00000023, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:2   , name:'CABINET RESONANCE'      },    // PRM_CABINET_RESONANCE
    ];
    var prm_prop_patch_status = [	// 0x00000650
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:4   , name:'TYPE'                   },    // PRM_KNOB_POS_TYPE
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'GAIN'                   },    // PRM_KNOB_POS_GAIN
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'VOLUME'                 },    // PRM_KNOB_POS_VOLUME
        { addr:0x00000003, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'BASS'                   },    // PRM_KNOB_POS_BASS
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'MIDDLE'                 },    // PRM_KNOB_POS_MIDDLE
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'TREBLE'                 },    // PRM_KNOB_POS_TREBLE
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:100 , name:'PRESENCE'               },    // PRM_KNOB_POS_PRESENCE
        { addr:0x00000007, size:INTEGER1x7        , ofs:1   , init:-1  , min:-1  , max:100 , name:'BOOSTER'                },    // PRM_KNOB_POS_BOOST
        { addr:0x00000008, size:INTEGER1x7        , ofs:1   , init:-1  , min:-1  , max:100 , name:'MOD'                    },    // PRM_KNOB_POS_MOD
        { addr:0x00000009, size:INTEGER1x7        , ofs:1   , init:-1  , min:-1  , max:100 , name:'FX'                     },    // PRM_KNOB_POS_FX
        { addr:0x0000000a, size:INTEGER1x7        , ofs:1   , init:-1  , min:-1  , max:100 , name:'DELAY'                  },    // PRM_KNOB_POS_DELAY
        { addr:0x0000000b, size:INTEGER1x7        , ofs:1   , init:-1  , min:-1  , max:100 , name:'REVERB'                 },    // PRM_KNOB_POS_REVERB
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'VARIATION'              },    // PRM_LED_STATE_VARI
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'BOOSTER'                },    // PRM_LED_STATE_BOOST
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'MOD'                    },    // PRM_LED_STATE_MOD
        { addr:0x0000000f, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'FX'                     },    // PRM_LED_STATE_FX
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'DELAY'                  },    // PRM_LED_STATE_DELAY
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'REVERB'                 },    // PRM_LED_STATE_REVERB
	];
	var prm_prop_patch_assign = [	// 0x00000700
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:7   , name:'BOOSTER'                },    // PRM_KNOB_ASSIGN_BOOSTER
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:8   , name:'DELAY'                  },    // PRM_KNOB_ASSIGN_DELAY
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:7   , name:'REVERB'                 },    // PRM_KNOB_ASSIGN_REVERB
        { addr:0x00000003, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:10  , name:'CHORUS'                 },    // PRM_KNOB_ASSIGN_CHORUS
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:7   , name:'FLANGER'                },    // PRM_KNOB_ASSIGN_FLANGER
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:7   , name:'PHASER'                 },    // PRM_KNOB_ASSIGN_PHASER
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'UNI-V'                  },    // PRM_KNOB_ASSIGN_UNI_V
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'TREMOLO'                },    // PRM_KNOB_ASSIGN_TREMOLO
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'VIBRATO'                },    // PRM_KNOB_ASSIGN_VIBRATO
        { addr:0x00000009, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'ROTARY'                 },    // PRM_KNOB_ASSIGN_ROTARY
        { addr:0x0000000a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'RING MOD'               },    // PRM_KNOB_ASSIGN_RING_MOD
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'SLOW GEAR'              },    // PRM_KNOB_ASSIGN_SLOW_GEAR
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'SLICER'                 },    // PRM_KNOB_ASSIGN_SLICER
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'COMP'                   },    // PRM_KNOB_ASSIGN_COMP
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:5   , name:'LIMITER'                },    // PRM_KNOB_ASSIGN_LIMITER
        { addr:0x0000000f, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:5   , name:'T.WAH'                  },    // PRM_KNOB_ASSIGN_T_WAH
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:6   , name:'AUTO WAH'               },    // PRM_KNOB_ASSIGN_AUTO_WAH
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:5   , name:'PEDAL WAH'              },    // PRM_KNOB_ASSIGN_PEDAL_WAH
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:11  , name:'GEQ'                    },    // PRM_KNOB_ASSIGN_GEQ
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:11  , name:'PEQ'                    },    // PRM_KNOB_ASSIGN_PEQ
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'GUITAR SIM'             },    // PRM_KNOB_ASSIGN_GUITAR_SIM
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'AC.GUITAR SIM'          },    // PRM_KNOB_ASSIGN_AC_GUITAR_SIM
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:6   , name:'AC.PROCESSOR'           },    // PRM_KNOB_ASSIGN_AC_PROCESSOR
        { addr:0x00000017, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:7   , name:'WAVE SYNTH'             },    // PRM_KNOB_ASSIGN_WAVE_SYNTH
        { addr:0x00000018, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'OCTAVE'                 },    // PRM_KNOB_ASSIGN_OCTAVE
        { addr:0x00000019, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:10  , name:'PITCH SHIFTER'          },    // PRM_KNOB_ASSIGN_PITCH_SHIFTER
        { addr:0x0000001a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:9   , name:'HARMONIST'              },    // PRM_KNOB_ASSIGN_HARMONIST
        { addr:0x0000001b, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:5   , name:'HUMANIZER'              },    // PRM_KNOB_ASSIGN_HUMANIZER
        { addr:0x0000001c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'PHASE 90E'              },    // PRM_KNOB_ASSIGN_EVH_PHASER
        { addr:0x0000001d, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'FLANGER 117E'           },    // PRM_KNOB_ASSIGN_EVH_FLANGER
        { addr:0x0000001e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:5   , name:'WAH 95E'                },    // PRM_KNOB_ASSIGN_EVH_WAH
        { addr:0x0000001f, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:6   , name:'DC-30'                  },    // PRM_KNOB_ASSIGN_DC30
        { addr:0x00000020, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'HEAVY OCT'              },    // PRM_KNOB_ASSIGN_HEAVY_OCT
        { addr:0x00000021, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:4   , name:'PEDAL BEND'             },    // PRM_KNOB_ASSIGN_PEDAL_BEND          //Ver200
	];
	var prm_prop_patch_assign_minmax = [	// 0x00000830
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'BOOSTER MIN'            },    // PRM_EXP_PEDAL_ASSIGN_BOOSTER_MIN
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'BOOSTER MAX'            },    // PRM_EXP_PEDAL_ASSIGN_BOOSTER_MAX
        { addr:0x00000002, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:2000, name:'DELAY MIN'              },    // PRM_EXP_PEDAL_ASSIGN_DELAY_MIN
        { addr:0x00000004, size:INTEGER2x7        , ofs:0   , init:100 , min:0   , max:2000, name:'DELAY MAX'              },    // PRM_EXP_PEDAL_ASSIGN_DELAY_MAX
        { addr:0x00000006, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:500 , name:'REVERB MIN'             },    // PRM_EXP_PEDAL_ASSIGN_REVERB_MIN
        { addr:0x00000008, size:INTEGER2x7        , ofs:0   , init:100 , min:0   , max:500 , name:'REVERB MAX'             },    // PRM_EXP_PEDAL_ASSIGN_REVERB_MAX
        { addr:0x0000000a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'CHORUS MIN'             },    // PRM_EXP_PEDAL_ASSIGN_CHORUS_MIN
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'CHORUS MAX'             },    // PRM_EXP_PEDAL_ASSIGN_CHORUS_MAX
        { addr:0x0000000c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'FLANGER MIN'            },    // PRM_EXP_PEDAL_ASSIGN_FLANGER_MIN
        { addr:0x0000000d, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'FLANGER MAX'            },    // PRM_EXP_PEDAL_ASSIGN_FLANGER_MAX
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'PHASER MIN'             },    // PRM_EXP_PEDAL_ASSIGN_PHASER_MIN
        { addr:0x0000000f, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'PHASER MAX'             },    // PRM_EXP_PEDAL_ASSIGN_PHASER_MAX
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'UNI-V MIN'              },    // PRM_EXP_PEDAL_ASSIGN_UNI_V_MIN
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'UNI-V MAX'              },    // PRM_EXP_PEDAL_ASSIGN_UNI_V_MAX
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'TREMOLO MIN'            },    // PRM_EXP_PEDAL_ASSIGN_TREMOLO_MIN
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'TREMOLO MAX'            },    // PRM_EXP_PEDAL_ASSIGN_TREMOLO_MAX
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'VIBRATO MIN'            },    // PRM_EXP_PEDAL_ASSIGN_VIBRATO_MIN
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'VIBRATO MAX'            },    // PRM_EXP_PEDAL_ASSIGN_VIBRATO_MAX
        { addr:0x00000016, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'ROTARY MIN'             },    // PRM_EXP_PEDAL_ASSIGN_ROTARY_MIN
        { addr:0x00000017, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'ROTARY MAX'             },    // PRM_EXP_PEDAL_ASSIGN_ROTARY_MAX
        { addr:0x00000018, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'RING MOD MIN'           },    // PRM_EXP_PEDAL_ASSIGN_RING_MOD_MIN
        { addr:0x00000019, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'RING MOD MAX'           },    // PRM_EXP_PEDAL_ASSIGN_RING_MOD_MAX
        { addr:0x0000001a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'SLOW GEAR MIN'          },    // PRM_EXP_PEDAL_ASSIGN_SLOW_GEAR_MIN
        { addr:0x0000001b, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'SLOW GEAR MAX'          },    // PRM_EXP_PEDAL_ASSIGN_SLOW_GEAR_MAX
        { addr:0x0000001c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'SLICER MIN'             },    // PRM_EXP_PEDAL_ASSIGN_SLICER_MIN
        { addr:0x0000001d, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'SLICER MAX'             },    // PRM_EXP_PEDAL_ASSIGN_SLICER_MAX
        { addr:0x0000001e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'COMP MIN'               },    // PRM_EXP_PEDAL_ASSIGN_COMP_MIN
        { addr:0x0000001f, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'COMP MAX'               },    // PRM_EXP_PEDAL_ASSIGN_COMP_MAX
        { addr:0x00000020, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'LIMITER MIN'            },    // PRM_EXP_PEDAL_ASSIGN_LIMITER_MIN
        { addr:0x00000021, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'LIMITER MAX'            },    // PRM_EXP_PEDAL_ASSIGN_LIMITER_MAX
        { addr:0x00000022, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'T.WAH MIN'              },    // PRM_EXP_PEDAL_ASSIGN_T_WAH_MIN
        { addr:0x00000023, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'T.WAH MAX'              },    // PRM_EXP_PEDAL_ASSIGN_T_WAH_MAX
        { addr:0x00000024, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'AUTO WAH MIN'           },    // PRM_EXP_PEDAL_ASSIGN_AUTO_WAH_MIN
        { addr:0x00000025, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'AUTO WAH MAX'           },    // PRM_EXP_PEDAL_ASSIGN_AUTO_WAH_MAX
        { addr:0x00000026, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'PEDAL WAH MIN'          },    // PRM_EXP_PEDAL_ASSIGN_PEDAL_WAH_MIN
        { addr:0x00000027, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'PEDAL WAH MAX'          },    // PRM_EXP_PEDAL_ASSIGN_PEDAL_WAH_MAX
        { addr:0x00000028, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'GEQ MIN'                },    // PRM_EXP_PEDAL_ASSIGN_GEQ_MIN
        { addr:0x00000029, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'GEQ MAX'                },    // PRM_EXP_PEDAL_ASSIGN_GEQ_MAX
        { addr:0x0000002a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'PEQ MIN'                },    // PRM_EXP_PEDAL_ASSIGN_PEQ_MIN
        { addr:0x0000002b, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'PEQ MAX'                },    // PRM_EXP_PEDAL_ASSIGN_PEQ_MAX
        { addr:0x0000002c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'GUITAR SIM MIN'         },    // PRM_EXP_PEDAL_ASSIGN_GUITAR_SIM_MIN
        { addr:0x0000002d, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'GUITAR SIM MAX'         },    // PRM_EXP_PEDAL_ASSIGN_GUITAR_SIM_MAX
        { addr:0x0000002e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'AC.GUITAR SIM MIN'      },    // PRM_EXP_PEDAL_ASSIGN_AC_GUITAR_SIM_MIN
        { addr:0x0000002f, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'AC.GUITAR SIM MAX'      },    // PRM_EXP_PEDAL_ASSIGN_AC_GUITAR_SIM_MAX
        { addr:0x00000030, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'AC.PROCESSOR MIN'       },    // PRM_EXP_PEDAL_ASSIGN_AC_PROCESSOR_MIN
        { addr:0x00000031, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'AC.PROCESSOR MAX'       },    // PRM_EXP_PEDAL_ASSIGN_AC_PROCESSOR_MAX
        { addr:0x00000032, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'WAVE SYNTH MIN'         },    // PRM_EXP_PEDAL_ASSIGN_WAVE_SYNTH_MIN
        { addr:0x00000033, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'WAVE SYNTH MAX'         },    // PRM_EXP_PEDAL_ASSIGN_WAVE_SYNTH_MAX
        { addr:0x00000034, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'OCTAVE MIN'             },    // PRM_EXP_PEDAL_ASSIGN_OCTAVE_MIN
        { addr:0x00000035, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'OCTAVE MAX'             },    // PRM_EXP_PEDAL_ASSIGN_OCTAVE_MAX
        { addr:0x00000036, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:300 , name:'PITCH SHIFTER MIN'      },    // PRM_EXP_PEDAL_ASSIGN_PITCH_SHIFTER_MIN
        { addr:0x00000038, size:INTEGER2x7        , ofs:0   , init:100 , min:0   , max:300 , name:'PITCH SHIFTER MAX'      },    // PRM_EXP_PEDAL_ASSIGN_PITCH_SHIFTER_MAX
        { addr:0x0000003a, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:300 , name:'HARMONIST MIN'          },    // PRM_EXP_PEDAL_ASSIGN_HARMONIST_MIN
        { addr:0x0000003c, size:INTEGER2x7        , ofs:0   , init:100 , min:0   , max:300 , name:'HARMONIST MAX'          },    // PRM_EXP_PEDAL_ASSIGN_HARMONIST_MAX
        { addr:0x0000003e, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'HUMANIZER MIN'          },    // PRM_EXP_PEDAL_ASSIGN_HUMANIZER_MIN
        { addr:0x0000003f, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'HUMANIZER MAX'          },    // PRM_EXP_PEDAL_ASSIGN_HUMANIZER_MAX
        { addr:0x00000040, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'PHASER 90E MIN'         },    // PRM_EXP_PEDAL_ASSIGN_EVH_PHASER_MIN
        { addr:0x00000041, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'PHASER 90E MAX'         },    // PRM_EXP_PEDAL_ASSIGN_EVH_PHASER_MAX
        { addr:0x00000042, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'FLANGER 117E MIN'       },    // PRM_EXP_PEDAL_ASSIGN_EVH_FLANGER_MIN
        { addr:0x00000043, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'FLANGER 117E MAX'       },    // PRM_EXP_PEDAL_ASSIGN_EVH_FLANGER_MAX
        { addr:0x00000044, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'WAH 95E MIN'            },    // PRM_EXP_PEDAL_ASSIGN_EVH_WAH_MIN
        { addr:0x00000045, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'WAH 95E MAX'            },    // PRM_EXP_PEDAL_ASSIGN_EVH_WAH_MAX
        { addr:0x00000046, size:INTEGER2x7        , ofs:0   , init:0   , min:0   , max:600 , name:'DC-30 MIN'              },    // PRM_EXP_PEDAL_ASSIGN_DC30_MIN
        { addr:0x00000048, size:INTEGER2x7        , ofs:0   , init:100 , min:0   , max:600 , name:'DC-30 MAX'              },    // PRM_EXP_PEDAL_ASSIGN_DC30_MAX
        { addr:0x0000004a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'HEAVY OCTAVE MIN'       },    // PRM_EXP_PEDAL_ASSIGN_HEAVY_OCT_MIN
        { addr:0x0000004b, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'HEAVY OCTAVE MAX'       },    // PRM_EXP_PEDAL_ASSIGN_HEAVY_OCT_MAX
        { addr:0x0000004c, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:127 , name:'MIN'                    },    // PRM_EXP_PEDAL_ASSIGN_PEDAL_BEND_MIN         //Ver200
        { addr:0x0000004d, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:127 , name:'MAX'                    },    // PRM_EXP_PEDAL_ASSIGN_PEDAL_BEND_MAX         //Ver200
	];
	var prm_prop_patch_ctrl_assign = [	// 0x00000f00                                                                                                                   //Ver201
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:9   , name:'GAFC EXP3 FUNCTION'     },    // PRM_PEDAL_FUNCTION_GAFC_EXP3
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:9   , name:'GAFC EX EXP1 FUNCTION'  },    // PRM_PEDAL_FUNCTION_GAFC_EX_EXP1
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:9   , name:'GAFC EX EXP2 FUNCTION'  },    // PRM_PEDAL_FUNCTION_GAFC_EX_EXP2
        { addr:0x00000003, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:9   , name:'GAFC EX EXP3 FUNCTION'  },    // PRM_PEDAL_FUNCTION_GAFC_EX_EXP3
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:9   , name:'GAFC FS3 FUNCTION'      },    // PRM_FS_FUNCTION_GAFC_FS3
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:2   , min:0   , max:9   , name:'GAFC EX FS1 FUNCTION'   },    // PRM_FS_FUNCTION_GAFC_EX_FS1
        { addr:0x00000006, size:INTEGER1x7        , ofs:0   , init:6   , min:0   , max:9   , name:'GAFC EX FS2 FUNCTION'   },    // PRM_FS_FUNCTION_GAFC_EX_FS2
        { addr:0x00000007, size:INTEGER1x7        , ofs:0   , init:5   , min:0   , max:9   , name:'GAFC EX FS3FUNCTION'    },    // PRM_FS_FUNCTION_GAFC_EX_FS3
	];
	var prm_prop_patch_fs_assign = [	// 0x00000f08                                                                                                           //Ver200
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'FS1 TIP FUNCTION'       },    // PRM_FS_FUNCTION_FS1_TIP
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:2   , name:'FS1 RING FUNCTION'      },    // PRM_FS_FUNCTION_FS1_RING
	];
	var prm_prop_mk2_v2 = [	// 0x00000f10
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SOLO_EQ_POSITION'       },    // PRM_POSITION_SOLO_EQ                //Ver200
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SOLO_EQ_SW'             },    // PRM_SOLO_EQ_SW                      //Ver200
        { addr:0x00000002, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:17  , name:'SOLO_EQ_LOW_CUT'        },    // PRM_SOLO_EQ_LOW_CUT                 //Ver200
        { addr:0x00000003, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'SOLO_EQ_LOW_GAIN'       },    // PRM_SOLO_EQ_LOW_GAIN                //Ver200
        { addr:0x00000004, size:INTEGER1x7        , ofs:0   , init:13  , min:0   , max:27  , name:'SOLO_EQ_MID_FREQ'       },    // PRM_SOLO_EQ_MID_FREQ                //Ver200
        { addr:0x00000005, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:5   , name:'SOLO_EQ_MID_Q'          },    // PRM_SOLO_EQ_MID_Q                   //Ver200
        { addr:0x00000006, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'SOLO_EQ_MID_GAIN'       },    // PRM_SOLO_EQ_MID_GAIN                //Ver200
        { addr:0x00000007, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'SOLO_EQ_HIGH_GAIN'      },    // PRM_SOLO_EQ_HIGH_GAIN               //Ver200
        { addr:0x00000008, size:INTEGER1x7        , ofs:0   , init:14  , min:0   , max:14  , name:'SOLO_EQ_HIGH_CUT'       },    // PRM_SOLO_EQ_HIGH_CUT                //Ver200
        { addr:0x00000009, size:INTEGER1x7        , ofs:24  , init:0   , min:-24 , max:24  , name:'SOLO_EQ_LEVEL'          },    // PRM_SOLO_EQ_LEVEL                   //Ver200
        { addr:0x0000000a, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SOLO_DELAY_SW'          },    // PRM_SOLO_DELAY_SW                   //Ver210
        { addr:0x0000000b, size:INTEGER1x7        , ofs:0   , init:1   , min:0   , max:1   , name:'SOLO_DELAY_CARRYOVER'   },    // PRM_SOLO_DELAY_CARRYOVER            //Ver210
        { addr:0x0000000c, size:INTEGER2x7        , ofs:0   , init:400 , min:1   , max:2000, name:'SOLO_DELAY_TIME'        },    // PRM_SOLO_DELAY_TIME                 //Ver210
        { addr:0x0000000e, size:INTEGER1x7        , ofs:0   , init:22  , min:0   , max:100 , name:'SOLO_DELAY_FEEDBACK'    },    // PRM_SOLO_DELAY_FEEDBACK             //Ver210
        { addr:0x0000000f, size:INTEGER1x7        , ofs:0   , init:50  , min:0   , max:120 , name:'SOLO_DELAY_EFFECT_LEVEL' },    // PRM_SOLO_DELAY_EFFECT_LEVEL        //Ver210
        { addr:0x00000010, size:INTEGER1x7        , ofs:0   , init:100 , min:0   , max:100 , name:'SOLO_DELAY_DIRECT_LEVEL' },    // PRM_SOLO_DELAY_DIRECT_LEVEL        //Ver210
        { addr:0x00000011, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:2   , name:'SOLO_DELAY_FILTER'      },    // PRM_SOLO_DELAY_FILTER               //Ver210
        { addr:0x00000012, size:INTEGER1x7        , ofs:0   , init:10  , min:0   , max:14  , name:'SOLO_DELAY_HIGH_CUT'    },    // PRM_SOLO_DELAY_HIGH_CUT             //Ver210
        { addr:0x00000013, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'SOLO_DELAY_MOD_SW'      },    // PRM_SOLO_DELAY_MOD_SW               //Ver210
        { addr:0x00000014, size:INTEGER1x7        , ofs:0   , init:40  , min:0   , max:100 , name:'SOLO_DELAY_MOD_RATE'    },    // PRM_SOLO_DELAY_MOD_RATE             //Ver210
        { addr:0x00000015, size:INTEGER1x7        , ofs:0   , init:55  , min:0   , max:100 , name:'SOLO_DELAY_MOD_DEPTH'   },    // PRM_SOLO_DELAY_MOD_DEPTH            //Ver210
	];
	var prm_prop_contour = [	// 0x00000f30 //Ver200
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:3   , name:'CONTOUR TYPE'           },    // PRM_CONTOUR1_TYPE
        { addr:0x00000001, size:INTEGER1x7        , ofs:50  , init:0   , min:-50 , max:50  , name:'FREQ SHIFT'             },    // PRM_CONTOUR1_FREQ_SHIFT
	];

	var prm_prop_status = [	// 0x00000001
        { addr:0x00000000, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:1   , name:'AMP IN SLAVE MODE'      },    // PRM_EXP_PEDAL_ASSIGN_BOOSTER_MIN
        { addr:0x00000001, size:INTEGER1x7        , ofs:0   , init:0   , min:0   , max:15  , name:'GA_FC_TYPE'             },                                           //Ver201 
	];

	/* block definitions  */
	var System = [
		{ addr: 0x00000000, size: 0, child: prm_prop_system,	name: 'System' },
		{ addr: 0x0000002E, size: 0, child: prm_prop_sys_eq_sel,name: 'SysEqSel'  },
                { addr: 0x0000002F, size: 0, child: prm_prop_sys_power_adjust,name: 'SysPwrAdj'  },                                                                     //Ver210
		{ addr: 0x00000030, size: 0, child: prm_prop_sys_eq,	name: 'SysEq(1)'  },
		{ addr: 0x00000050, size: 0, child: prm_prop_sys_eq,	name: 'SysEq(2)'  },
		{ addr: 0x00000070, size: 0, child: prm_prop_sys_eq,	name: 'SysEq(3)'  },
		{ addr: 0x00000110, size: 0, child: prm_prop_sys_lineout_custom,			name: 'LineoutCustom' },                                        //Ver200
		{ addr: 0x00000112, size: 0, child: prm_prop_sys_lineout_custom_setting,	name: 'LineoutCustomSetting(1)' },                                      //Ver200
		{ addr: 0x00000118, size: 0, child: prm_prop_sys_lineout_custom_setting,	name: 'LineoutCustomSetting(2)' },                                      //Ver200
		{ addr: 0x00000120, size: 0, child: prm_prop_sys_gafc_function,				name: 'GafcSetting'  },                                         //Ver201
		{ addr: 0x00010000, size: 0, child: prm_prop_info,		name: 'Info'   },
		{ addr: 0x00020000, size: 0, child: prm_prop_midi,		name: 'Midi'   },
	];

	var Patch = [
		{ addr: 0x00000000, size: 0, child: prm_prop_patch_name,			name: 'PatchName' },
		{ addr: 0x00000010, size: 0, child: prm_prop_patch_0,				name: 'Patch_0'   },
		{ addr: 0x00000060, size: 0, child: prm_prop_patch_eq2,				name: 'Eq(2)'     },
		{ addr: 0x00000100, size: 0, child: prm_prop_patch_fx,				name: 'Fx(1)'     },
		{ addr: 0x00000300, size: 0, child: prm_prop_patch_fx,				name: 'Fx(2)'     },
		{ addr: 0x00000500, size: 0, child: prm_prop_patch_delay,			name: 'Delay(1)'  },
		{ addr: 0x00000520, size: 0, child: prm_prop_patch_delay,			name: 'Delay(2)'  },
		{ addr: 0x00000540, size: 0, child: prm_prop_patch_1,				name: 'Patch_1'   },
		{ addr: 0x00000620, size: 0, child: prm_prop_patch_2,				name: 'Patch_2'   },
		{ addr: 0x00000650, size: 0, child: prm_prop_patch_status,			name: 'Status'    },
		{ addr: 0x00000700, size: 0, child: prm_prop_patch_assign,			name: 'KnobAsgn'},
		{ addr: 0x00000800, size: 0, child: prm_prop_patch_assign,			name: 'ExpPedalAsgn'},
		{ addr: 0x00000830, size: 0, child: prm_prop_patch_assign_minmax,	name: 'ExpPedalAsgnMinMax'},
		{ addr: 0x00000900, size: 0, child: prm_prop_patch_assign,			name: 'GafcExp1Asgn'},
		{ addr: 0x00000930, size: 0, child: prm_prop_patch_assign_minmax,	name: 'GafcExp1AsgnMinMax'},
		{ addr: 0x00000A00, size: 0, child: prm_prop_patch_assign,			name: 'GafcExp2Asgn'},
		{ addr: 0x00000A30, size: 0, child: prm_prop_patch_assign_minmax,	name: 'GafcExp2AsgnMinMax'},
		{ addr: 0x00000B00, size: 0, child: prm_prop_patch_assign,			name: 'GafcExp3Asgn'},          //Ver201
		{ addr: 0x00000B30, size: 0, child: prm_prop_patch_assign_minmax,	name: 'GafcExp3AsgnMinMax'},            //Ver201
		{ addr: 0x00000C00, size: 0, child: prm_prop_patch_assign,			name: 'GafcExExp1Asgn'},        //Ver201
		{ addr: 0x00000C30, size: 0, child: prm_prop_patch_assign_minmax,	name: 'GafcExExp1AsgnMinMax'},          //Ver201
		{ addr: 0x00000D00, size: 0, child: prm_prop_patch_assign,			name: 'GafcExExp2Asgn'},        //Ver201
		{ addr: 0x00000D30, size: 0, child: prm_prop_patch_assign_minmax,	name: 'GafcExExp2AsgnMinMax'},          //Ver201
		{ addr: 0x00000E00, size: 0, child: prm_prop_patch_assign,			name: 'GafcExExp3Asgn'},        //Ver201
		{ addr: 0x00000E30, size: 0, child: prm_prop_patch_assign_minmax,	name: 'GafcExExp3AsgnMinMax'},          //Ver201
		{ addr: 0x00000F00, size: 0, child: prm_prop_patch_ctrl_assign,		name: 'CtrlAsgn'},                      //Ver201
		{ addr: 0x00000F08, size: 0, child: prm_prop_patch_fs_assign,		name: 'FsAsgn'},                        //Ver200
		{ addr: 0x00000F10, size: 0, child: prm_prop_mk2_v2,				name: 'Patch_Mk2V2'},           //Ver200
		{ addr: 0x00000F30, size: 0, child: prm_prop_contour,				name: 'Contour(1)'},            //Ver200
		{ addr: 0x00000F38, size: 0, child: prm_prop_contour,				name: 'Contour(2)'},            //Ver200
		{ addr: 0x00000F40, size: 0, child: prm_prop_contour,				name: 'Contour(3)'},            //Ver200

	];

	var Status = [
		{ addr: 0x00000001, size: 0, child: prm_prop_status,			name: 'Status' },
	];

	this.root = [
		{ addr: 0x00000000, size: 0, child: System, name: 'System' },
		{ addr: 0x10000000, size: 0, child: Patch,  name: 'UserPatch(1)' },
		{ addr: 0x10010000, size: 0, child: Patch,  name: 'UserPatch(2)' },
		{ addr: 0x10020000, size: 0, child: Patch,  name: 'UserPatch(3)' },
		{ addr: 0x10030000, size: 0, child: Patch,  name: 'UserPatch(4)' },
		{ addr: 0x10040000, size: 0, child: Patch,  name: 'UserPatch(5)' },
		{ addr: 0x10050000, size: 0, child: Patch,  name: 'UserPatch(6)' },
		{ addr: 0x10060000, size: 0, child: Patch,  name: 'UserPatch(7)' },
		{ addr: 0x10070000, size: 0, child: Patch,  name: 'UserPatch(8)' },
		{ addr: 0x10080000, size: 0, child: Patch,  name: 'UserPatch(9)' },
		{ addr: 0x60000000, size: 0, child: Patch,  name: 'Temporary' },
		{ addr: 0x7F010200, size: 0, child: Status, name: 'Status' },
	];

	/* construction */
	(function(root){

		/* calucrate size */
		(function(a) {
			for (var n = a.length - 1; n >= 0; n--) {
				var b = a[n];
				if ('child' in b) {
					b.size = arguments.callee(b.child);
				} else {
					var bytes = 0;
					switch (b.size) {
						case INTEGER1x1: case INTEGER1x2: case INTEGER1x3: case INTEGER1x4:
						case INTEGER1x5: case INTEGER1x6: case INTEGER1x7:
							bytes = 1; break;
						case INTEGER2x4:
						case INTEGER2x7:
							bytes = 2; break;
						case INTEGER4x4:
							bytes = 4; break;
						default:
							bytes = b.size; break;
					}
					return b.addr + bytes;
				}
			}
			return 0;
		})(root);

		/* all addr and size is nibbled. */
		(function(a) {
			for (var n = 0, max = a.length; n < max; n++) {
				var b = a[n];
				if (!('nibbled' in b)) {
					b.nibbled = true;
					b.addr = nibble(b.addr);
					b.size = (0 < b.size && b.size < INTEGER1x1) ? nibble(b.size) : b.size;
					if ('child' in b) arguments.callee(b.child);
				}
			}
		})(root);

//		for (var i = 0; i < 6; i++) {
//			var b = {};
//			b.name  = 'User(' + (i + 1) + ')';
//			b.addr  = nibble(0x10010000) + (i * nibble(0x10000));
//			b.size  = 0;
//			b.child = Patch;
//			root.push(b);
//		}

	})(this.root);

	this.layout = [ /* for layout tool */
		{ addr: 0x00000000, size: 0, child: System, name: 'System' },
		{ addr: 0x60000000, size: 0, child: Patch,  name: 'Temporary' },
		{ addr: 0x7F010200, size: 0, child: Status, name: 'Status' },
	];
}
