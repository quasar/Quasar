// created on 13/12/2002 at 22:06

using System;

namespace xClient.Core.NAudio.Mixer
{
	/// <summary>
	/// List text mixer control
	/// </summary>
	public class ListTextMixerControl : MixerControl 
	{
        internal ListTextMixerControl(MixerInterop.MIXERCONTROL mixerControl, IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels) 
		{
			this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
			this.nChannels = nChannels;
			this.mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();
			
			GetControlDetails();

		}

		/// <summary>
		/// Get the details for this control
		/// </summary>
		/// <param name="pDetails">Memory location to read to</param>
		protected override void GetDetails(IntPtr pDetails) 
		{
		}

		// TODO: provide a way of getting / setting data
	}
}
