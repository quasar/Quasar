// created on 15/12/2002 at 02:55

using System;

namespace xClient.Core.NAudio.Mixer
{
	[Flags]
	internal enum MixerControlClass 
	{
		Custom	= 0x00000000,
		Meter	= 0x10000000,
		Switch	= 0x20000000,
		Number	= 0x30000000,
		Slider	= 0x40000000,
		Fader	= 0x50000000,
		Time	= 0x60000000,
		List	= 0x70000000,
		Mask 	= Custom | Meter | Switch | Number | Slider | Fader | Time | List
	}

	[Flags]	
	internal enum MixerControlSubclass 
	{
		SwitchBoolean 	= 0x00000000,
		SwitchButton 	= 0x01000000,
		MeterPolled 	= 0x00000000,
		TimeMicrosecs	= 0x00000000,
		TimeMillisecs	= 0x01000000,
		ListSingle		= 0x00000000,
		ListMultiple 	= 0x01000000,
		Mask			= 0x0F000000
	}
	
	[Flags]
	internal enum MixerControlUnits 
	{
		Custom		= 0x00000000,
		Boolean		= 0x00010000,
		Signed		= 0x00020000,
		Unsigned	= 0x00030000,
		Decibels	= 0x00040000, // in 10ths
		Percent		= 0x00050000, // in 10ths
		Mask		= 0x00FF0000
	}
	
	/// <summary>
	/// Mixer control types
	/// </summary>
	public enum MixerControlType 
	{
		/// <summary>Custom</summary>
		Custom = (MixerControlClass.Custom | MixerControlUnits.Custom),
		/// <summary>Boolean meter</summary>
		BooleanMeter = (MixerControlClass.Meter | MixerControlSubclass.MeterPolled | MixerControlUnits.Boolean),
		/// <summary>Signed meter</summary>
		SignedMeter = (MixerControlClass.Meter | MixerControlSubclass.MeterPolled | MixerControlUnits.Signed),
		/// <summary>Peak meter</summary>
		PeakMeter = (SignedMeter + 1),
		/// <summary>Unsigned meter</summary>
		UnsignedMeter = (MixerControlClass.Meter | MixerControlSubclass.MeterPolled | MixerControlUnits.Unsigned),
		/// <summary>Boolean</summary>
		Boolean = (MixerControlClass.Switch | MixerControlSubclass.SwitchBoolean | MixerControlUnits.Boolean),
		/// <summary>On Off</summary>
		OnOff = (Boolean + 1),
		/// <summary>Mute</summary>
		Mute = (Boolean + 2),
		/// <summary>Mono</summary>
		Mono = (Boolean + 3),
		/// <summary>Loudness</summary>
		Loudness = (Boolean + 4),
		/// <summary>Stereo Enhance</summary>
		StereoEnhance = (Boolean + 5),
		/// <summary>Button</summary>
		Button = (MixerControlClass.Switch | MixerControlSubclass.SwitchButton | MixerControlUnits.Boolean),
		/// <summary>Decibels</summary>
		Decibels = (MixerControlClass.Number | MixerControlUnits.Decibels),
		/// <summary>Signed</summary>
		Signed = (MixerControlClass.Number | MixerControlUnits.Signed),
		/// <summary>Unsigned</summary>
		Unsigned = (MixerControlClass.Number | MixerControlUnits.Unsigned),
		/// <summary>Percent</summary>
		Percent = (MixerControlClass.Number | MixerControlUnits.Percent),
		/// <summary>Slider</summary>
		Slider = (MixerControlClass.Slider | MixerControlUnits.Signed),
		/// <summary>Pan</summary>
		Pan = (Slider + 1),
		/// <summary>Q-sound pan</summary>
		QSoundPan = (Slider + 2),
		/// <summary>Fader</summary>
		Fader = (MixerControlClass.Fader | MixerControlUnits.Unsigned),
		/// <summary>Volume</summary>
		Volume = (Fader + 1),
		/// <summary>Bass</summary>
		Bass = (Fader + 2),
		/// <summary>Treble</summary>
		Treble = (Fader + 3),
		/// <summary>Equaliser</summary>
		Equalizer = (Fader + 4),
		/// <summary>Single Select</summary>
		SingleSelect = (MixerControlClass.List | MixerControlSubclass.ListSingle | MixerControlUnits.Boolean),
		/// <summary>Mux</summary>
		Mux = (SingleSelect + 1),
		/// <summary>Multiple select</summary>
		MultipleSelect = (MixerControlClass.List | MixerControlSubclass.ListMultiple | MixerControlUnits.Boolean),
		/// <summary>Mixer</summary>
		Mixer = (MultipleSelect + 1),
		/// <summary>Micro time</summary>
		MicroTime = (MixerControlClass.Time | MixerControlSubclass.TimeMicrosecs | MixerControlUnits.Unsigned),
		/// <summary>Milli time</summary>
		MilliTime = (MixerControlClass.Time | MixerControlSubclass.TimeMillisecs | MixerControlUnits.Unsigned),
	}
}
