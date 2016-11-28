namespace xServer.Core.NAudio.Wave.MmeInterop
{
	/// <summary>
	/// Manufacturer codes from mmreg.h
	/// </summary>
	public enum Manufacturers 
	{
		/// <summary>Microsoft Corporation</summary>
		Microsoft					= 1,
		/// <summary>Creative Labs, Inc</summary>
		Creative					= 2,
		/// <summary>Media Vision, Inc.</summary>
		Mediavision					= 3,
		/// <summary>Fujitsu Corp.</summary>
		Fujitsu						= 4,
		/// <summary>Artisoft, Inc.</summary>
		Artisoft					= 20,
		/// <summary>Turtle Beach, Inc.</summary>
		TurtleBeach					= 21,
		/// <summary>IBM Corporation</summary>
		Ibm							= 22,
		/// <summary>Vocaltec LTD.</summary>
		Vocaltec					= 23,
		/// <summary>Roland</summary>
		Roland						= 24,
		/// <summary>DSP Solutions, Inc.</summary>
		DspSolutions				= 25,
		/// <summary>NEC</summary>
		Nec							= 26,
		/// <summary>ATI</summary>
		Ati							= 27,
		/// <summary>Wang Laboratories, Inc</summary>
		Wanglabs					= 28,
		/// <summary>Tandy Corporation</summary>
		Tandy						= 29,
		/// <summary>Voyetra</summary>
		Voyetra						= 30,
		/// <summary>Antex Electronics Corporation</summary>
		Antex						= 31,
		/// <summary>ICL Personal Systems</summary>
		IclPS						= 32,
		/// <summary>Intel Corporation</summary>
		Intel						= 33,
		/// <summary>Advanced Gravis</summary>
		Gravis						= 34,
		/// <summary>Video Associates Labs, Inc.</summary>
		Val							= 35,
		/// <summary>InterActive Inc</summary>
		Interactive					= 36,
		/// <summary>Yamaha Corporation of America</summary>
		Yamaha						= 37,
		/// <summary>Everex Systems, Inc</summary>
		Everex						= 38,
		/// <summary>Echo Speech Corporation</summary>
		Echo						= 39,
		/// <summary>Sierra Semiconductor Corp</summary>
		Sierra						= 40,
		/// <summary>Computer Aided Technologies</summary>
		Cat							= 41,
		/// <summary>APPS Software International</summary>
		Apps						= 42,
		/// <summary>DSP Group, Inc</summary>
		DspGroup					= 43,
		/// <summary>microEngineering Labs</summary>
		Melabs						= 44,
		/// <summary>Computer Friends, Inc.</summary>
		ComputerFriends				= 45,
		/// <summary>ESS Technology</summary>
		Ess							= 46,
		/// <summary>Audio, Inc.</summary>
		Audiofile					= 47,
		/// <summary>Motorola, Inc.</summary>
		Motorola					= 48,
		/// <summary>Canopus, co., Ltd.</summary>
		Canopus						= 49,
		/// <summary>Seiko Epson Corporation</summary>
		Epson						= 50,
		/// <summary>Truevision</summary>
		Truevision					= 51,
		/// <summary>Aztech Labs, Inc.</summary>
		Aztech						= 52,
		/// <summary>Videologic</summary>
		Videologic					= 53,
		/// <summary>SCALACS</summary>
		Scalacs						= 54,
		/// <summary>Korg Inc.</summary>
		Korg						= 55,
		/// <summary>Audio Processing Technology</summary>
		Apt							= 56,
		/// <summary>Integrated Circuit Systems, Inc.</summary>
		Ics							= 57,
		/// <summary>Iterated Systems, Inc.</summary>
		Iteratedsys					= 58,
		/// <summary>Metheus</summary>
		Metheus						= 59,
		/// <summary>Logitech, Inc.</summary>
		Logitech					= 60,
		/// <summary>Winnov, Inc.</summary>
		Winnov						= 61,
		/// <summary>NCR Corporation</summary>
		Ncr							= 62,
		/// <summary>EXAN</summary>
		Exan						= 63,
		/// <summary>AST Research Inc.</summary>
		Ast							= 64,
		/// <summary>Willow Pond Corporation</summary>
		Willowpond					= 65,
		/// <summary>Sonic Foundry</summary>
		Sonicfoundry				= 66,
		/// <summary>Vitec Multimedia</summary>
		Vitec						= 67,
		/// <summary>MOSCOM Corporation</summary>
		Moscom						= 68,
		/// <summary>Silicon Soft, Inc.</summary>
		Siliconsoft					= 69,
		/// <summary>Supermac</summary>
		Supermac					= 73,
		/// <summary>Audio Processing Technology</summary>
		Audiopt						= 74,
		/// <summary>Speech Compression</summary>
		Speechcomp					= 76,
		/// <summary>Ahead, Inc.</summary>
		Ahead						= 77,
		/// <summary>Dolby Laboratories</summary>
		Dolby						= 78,
		/// <summary>OKI</summary>
		Oki							= 79,
		/// <summary>AuraVision Corporation</summary>
		Auravision					= 80,
		/// <summary>Ing C. Olivetti &amp; C., S.p.A.</summary>
		Olivetti					= 81,
		/// <summary>I/O Magic Corporation</summary>
		Iomagic						= 82,
		/// <summary>Matsushita Electric Industrial Co., LTD.</summary>
		Matsushita					= 83,
		/// <summary>Control Resources Limited</summary>
		Controlres					= 84,
		/// <summary>Xebec Multimedia Solutions Limited</summary>
		Xebec						= 85,
		/// <summary>New Media Corporation</summary>
		Newmedia					= 86,
		/// <summary>Natural MicroSystems</summary>
		Nms							= 87,
		/// <summary>Lyrrus Inc.</summary>
		Lyrrus						= 88,
		/// <summary>Compusic</summary>
		Compusic					= 89,
		/// <summary>OPTi Computers Inc.</summary>
		Opti						= 90,
		/// <summary>Adlib Accessories Inc.</summary>
		Adlacc						= 91,
		/// <summary>Compaq Computer Corp.</summary>
		Compaq						= 92,
		/// <summary>Dialogic Corporation</summary>
		Dialogic					= 93,
		/// <summary>InSoft, Inc.</summary>
		Insoft						= 94,
		/// <summary>M.P. Technologies, Inc.</summary>
		Mptus						= 95,
		/// <summary>Weitek</summary>
		Weitek						= 96,
		/// <summary>Lernout &amp; Hauspie</summary>
		LernoutAndHauspie			= 97,
		/// <summary>Quanta Computer Inc.</summary>
		Qciar						= 98,
		/// <summary>Apple Computer, Inc.</summary>
		Apple						= 99,
		/// <summary>Digital Equipment Corporation</summary>
		Digital						= 100,
		/// <summary>Mark of the Unicorn</summary>
		Motu						= 101,
		/// <summary>Workbit Corporation</summary>
		Workbit						= 102,
		/// <summary>Ositech Communications Inc.</summary>
		Ositech						= 103,
		/// <summary>miro Computer Products AG</summary>
		Miro						= 104,
		/// <summary>Cirrus Logic</summary>
		Cirruslogic					= 105,
		/// <summary>ISOLUTION  B.V.</summary>
		Isolution					= 106,
		/// <summary>Horizons Technology, Inc</summary>
		Horizons					= 107,
		/// <summary>Computer Concepts Ltd</summary>
		Concepts					= 108,
		/// <summary>Voice Technologies Group, Inc.</summary>
		Vtg							= 109,
		/// <summary>Radius</summary>
		Radius						= 110,
		/// <summary>Rockwell International</summary>
		Rockwell					= 111,
		/// <summary>Co. XYZ for testing</summary>
		Xyz							= 112,
		/// <summary>Opcode Systems</summary>
		Opcode						= 113,
		/// <summary>Voxware Inc</summary>
		Voxware						= 114,
		/// <summary>Northern Telecom Limited</summary>
		NorthernTelecom				= 115,
		/// <summary>APICOM</summary>
		Apicom						= 116,
		/// <summary>Grande Software</summary>
		Grande						= 117,
		/// <summary>ADDX</summary>
		Addx						= 118,
		/// <summary>Wildcat Canyon Software</summary>
		Wildcat						= 119,
		/// <summary>Rhetorex Inc</summary>
		Rhetorex					= 120,
		/// <summary>Brooktree Corporation</summary>
		Brooktree					= 121,
		/// <summary>ENSONIQ Corporation</summary>
		Ensoniq						= 125,
		/// <summary>FAST Multimedia AG</summary>
		Fast						= 126,
		/// <summary>NVidia Corporation</summary>
		Nvidia						= 127,
		/// <summary>OKSORI Co., Ltd.</summary>
		Oksori						= 128,
		/// <summary>DiAcoustics, Inc.</summary>
		Diacoustics					= 129,
		/// <summary>Gulbransen, Inc.</summary>
		Gulbransen					= 130,
		/// <summary>Kay Elemetrics, Inc.</summary>
		KayElemetrics				= 131,
		/// <summary>Crystal Semiconductor Corporation</summary>
		Crystal						= 132,
		/// <summary>Splash Studios</summary>
		SplashStudios				= 133,
		/// <summary>Quarterdeck Corporation</summary>
		Quarterdeck					= 134,
		/// <summary>TDK Corporation</summary>
		Tdk							= 135,
		/// <summary>Digital Audio Labs, Inc.</summary>
		DigitalAudioLabs			= 136,
		/// <summary>Seer Systems, Inc.</summary>
		Seersys						= 137,
		/// <summary>PictureTel Corporation</summary>
		Picturetel					= 138,
		/// <summary>AT&amp;T Microelectronics</summary>
		AttMicroelectronics			= 139,
		/// <summary>Osprey Technologies, Inc.</summary>
		Osprey						= 140,
		/// <summary>Mediatrix Peripherals</summary>
		Mediatrix					= 141,
		/// <summary>SounDesignS M.C.S. Ltd.</summary>
		Soundesigns					= 142,
		/// <summary>A.L. Digital Ltd.</summary>
		Aldigital					= 143,
		/// <summary>Spectrum Signal Processing, Inc.</summary>
		SpectrumSignalProcessing	= 144,
		/// <summary>Electronic Courseware Systems, Inc.</summary>
		Ecs							= 145,
		/// <summary>AMD</summary>
		Amd							= 146,
		/// <summary>Core Dynamics</summary>
		Coredynamics				= 147,
		/// <summary>CANAM Computers</summary>
		Canam						= 148,
		/// <summary>Softsound, Ltd.</summary>
		Softsound					= 149,
		/// <summary>Norris Communications, Inc.</summary>
		Norris						= 150,
		/// <summary>Danka Data Devices</summary>
		Ddd							= 151,
		/// <summary>EuPhonics</summary>
		Euphonics					= 152,
		/// <summary>Precept Software, Inc.</summary>
		Precept						= 153,
		/// <summary>Crystal Net Corporation</summary>
		CrystalNet					= 154,
		/// <summary>Chromatic Research, Inc</summary>
		Chromatic					= 155,
		/// <summary>Voice Information Systems, Inc</summary>
		Voiceinfo					= 156,
		/// <summary>Vienna Systems</summary>
		Viennasys					= 157,
		/// <summary>Connectix Corporation</summary>
		Connectix					= 158,
		/// <summary>Gadget Labs LLC</summary>
		Gadgetlabs					= 159,
		/// <summary>Frontier Design Group LLC</summary>
		Frontier					= 160,
		/// <summary>Viona Development GmbH</summary>
		Viona						= 161,
		/// <summary>Casio Computer Co., LTD</summary>
		Casio						= 162,
		/// <summary>Diamond Multimedia</summary>
		Diamondmm					= 163,
		/// <summary>S3</summary>
		S3							= 164,
		/// <summary>Fraunhofer</summary>
		FraunhoferIis				= 172,
													    
		/*
		public static String GetName(int manufacturerId) {
			switch(manufacturerId) {
			case Gravis:			return "Advanced Gravis Computer Technology, Ltd.";
			case Antex:				return "Antex Electronics Corporation";
			case Apps:				return "APPS Software";
			case Artisoft:			return "Artisoft, Inc.";
			case Ast:				return "AST Research, Inc.";
			case Ati:				return "ATI Technologies, Inc.";
			case Audiofile:			return "Audio, Inc.";
			case Apt:				return "Audio Processing Technology";
			case Audiopt:			return "Audio Processing Technology";
			case Auravision:		return "Auravision Corporation";
			case Aztech:			return "Aztech Labs, Inc.";
			case Canopus:			return "Canopus, Co., Ltd.";
			case Compusic:			return "Compusic";
			case Cat:				return "Computer Aided Technology, Inc.";
			case ComputerFriends:	return "Computer Friends, Inc.";
			case Controlres:		return "Control Resources Corporation"; 
			case Creative:			return "Creative Labs, Inc.";
			case Dialogic:			return "Dialogic Corporation";
			case Dolby:				return "Dolby Laboratories, Inc.";
			case DspGroup:			return "DSP Group, Inc.";
			case DspSolutions:		return "DSP Solutions, Inc.";
			case Echo:				return "Echo Speech Corporation";
			case Ess:				return "ESS Technology, Inc.";
			case Everex:			return "Everex Systems, Inc.";
			case Exan:				return "EXAN, Ltd.";
			case Fujitsu:			return "Fujitsu, Ltd.";
			case Iomagic:			return "I/O Magic Corporation";
			case IclPS:				return "ICL Personal Systems";
			case Olivetti:			return "Ing. C. Olivetti & C., S.p.A.";
			case Ics:				return "Integrated Circuit Systems, Inc.";
			case Intel:				return "Intel Corporation";
			case Interactive:		return "InterActive, Inc.";
			case Ibm:				return "International Business Machines";
			case Iteratedsys:		return "Iterated Systems, Inc.";
			case Logitech:			return "Logitech, Inc.";
			case Lyrrus:			return "Lyrrus, Inc.";
			case Matsushita:		return "Matsushita Electric Corporation of America";
			case Mediavision:		return "Media Vision, Inc.";
			case Metheus:			return "Metheus Corporation";
			case Melabs:			return "microEngineering Labs";
			case Microsoft:			return "Microsoft Corporation";
			case Moscom:			return "MOSCOM Corporation";
			case Motorola:			return "Motorola, Inc.";
			case Nms:				return "Natural MicroSystems Corporation";
			case Ncr:				return "NCR Corporation";
			case Nec:				return "NEC Corporation";
			case Newmedia:			return "New Media Corporation";
			case Oki:				return "OKI";
			case Opti:				return "OPTi, Inc.";
			case Roland:			return "Roland Corporation";
			case Scalacs:			return "SCALACS";
			case Epson:				return "Seiko Epson Corporation, Inc.";
			case Sierra:			return "Sierra Semiconductor Corporation";
			case Siliconsoft:		return "Silicon Software, Inc.";
			case Sonicfoundry:		return "Sonic Foundry";
			case Speechcomp:		return "Speech Compression";
			case Supermac:			return "Supermac Technology, Inc.";
			case Tandy:				return "Tandy Corporation";
			case Korg:				return "Toshihiko Okuhura, Korg, Inc.";
			case Truevision:		return "Truevision, Inc.";
			case TurtleBeach:		return "Turtle Beach Systems";
			case Val:				return "Video Associates Labs, Inc.";
			case Videologic:		return "VideoLogic, Inc.";
			case Vitec:				return "Visual Information Technologies, Inc.";
			case Vocaltec:			return "VocalTec, Inc.";
			case Voyetra:			return "Voyetra Technologies";
			case Wanglabs:			return "Wang Laboratories";
			case Willowpond:		return "Willow Pond Corporation";
			case Winnov:			return "Winnov, LP";
			case Xebec:				return "Xebec Multimedia Solutions Limited";
			case Yamaha:			return "Yamaha Corporation of America";
			default:				return String.Format("Unknown Manufacturer ({0})",manufacturerId);
			}			
		}
		**/
	}
}