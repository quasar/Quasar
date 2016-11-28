// created on 13/12/2002 at 22:04

using System;
using System.Runtime.InteropServices;
using xClient.Core.NAudio.Wave.MmeInterop;

namespace xClient.Core.NAudio.Mixer
{
	/// <summary>
	/// Represents an unsigned mixer control
	/// </summary>
	public class UnsignedMixerControl : MixerControl 
	{
		private MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[] unsignedDetails;
		
		internal UnsignedMixerControl(MixerInterop.MIXERCONTROL mixerControl,IntPtr mixerHandle, MixerFlags mixerHandleType, int nChannels) 
		{
			this.mixerControl = mixerControl;
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
			this.nChannels = nChannels;
			this.mixerControlDetails = new MixerInterop.MIXERCONTROLDETAILS();
			GetControlDetails();
		}

		/// <summary>
		/// Gets the details for this control
		/// </summary>
        protected override void GetDetails(IntPtr pDetails)
        {
            unsignedDetails = new MixerInterop.MIXERCONTROLDETAILS_UNSIGNED[nChannels];
            for (int channel = 0; channel < nChannels; channel++)
            {
                unsignedDetails[channel] = (MixerInterop.MIXERCONTROLDETAILS_UNSIGNED)Marshal.PtrToStructure(mixerControlDetails.paDetails, typeof(MixerInterop.MIXERCONTROLDETAILS_UNSIGNED));
            }
        }

		/// <summary>
		/// The control value
		/// </summary>
		public uint Value 
		{
			get 
			{
				GetControlDetails();
				return unsignedDetails[0].dwValue;
			}
			set 
			{
                int structSize = Marshal.SizeOf(unsignedDetails[0]);

                mixerControlDetails.paDetails = Marshal.AllocHGlobal(structSize * nChannels);
                for (int channel = 0; channel < nChannels; channel++)
                {
                    unsignedDetails[channel].dwValue = value;
                    long pointer = mixerControlDetails.paDetails.ToInt64() + (structSize * channel);                    
                    Marshal.StructureToPtr(unsignedDetails[channel], (IntPtr)pointer, false);
                }
				MmException.Try(MixerInterop.mixerSetControlDetails(mixerHandle, ref mixerControlDetails, MixerFlags.Value | mixerHandleType), "mixerSetControlDetails");
                Marshal.FreeHGlobal(mixerControlDetails.paDetails);
			}
		}
		
		/// <summary>
		/// The control's minimum value
		/// </summary>
		public UInt32 MinValue 
		{
			get 
			{
				return (uint) mixerControl.Bounds.minimum;
			}
		}

		/// <summary>
		/// The control's maximum value
		/// </summary>
		public UInt32 MaxValue 
		{
			get 
			{
				return (uint) mixerControl.Bounds.maximum;
			}
		}

        /// <summary>
        /// Value of the control represented as a percentage
        /// </summary>
        public double Percent
        {
            get
            {
                return 100.0 * (Value - MinValue) / (double)(MaxValue - MinValue);
            }
            set
            {
                Value = (uint)(MinValue + (value / 100.0) * (MaxValue - MinValue));
            }
        }

        /// <summary>
        /// String Representation for debugging purposes
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} {1}%", base.ToString(), Percent);
        }
	}
}
