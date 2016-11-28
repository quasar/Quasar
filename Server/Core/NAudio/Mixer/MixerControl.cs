// created on 10/12/2002 at 21:11

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using xServer.Core.NAudio.Wave.MmeInterop;

namespace xServer.Core.NAudio.Mixer
{
    /// <summary>
    /// Represents a mixer control
    /// </summary>
    public abstract class MixerControl
    {
        internal MixerInterop.MIXERCONTROL mixerControl;
        internal MixerInterop.MIXERCONTROLDETAILS mixerControlDetails;

        /// <summary>
        /// Mixer Handle
        /// </summary>
        protected IntPtr mixerHandle;

        /// <summary>
        /// Number of Channels
        /// </summary>
        protected int nChannels;

        /// <summary>
        /// Mixer Handle Type
        /// </summary>
        protected MixerFlags mixerHandleType;

        /// <summary>
        /// Gets all the mixer controls
        /// </summary>
        /// <param name="mixerHandle">Mixer Handle</param>
        /// <param name="mixerLine">Mixer Line</param>
        /// <param name="mixerHandleType">Mixer Handle Type</param>
        /// <returns></returns>
        public static IList<MixerControl> GetMixerControls(IntPtr mixerHandle, MixerLine mixerLine,
                                                           MixerFlags mixerHandleType)
        {
            var controls = new List<MixerControl>();
            if (mixerLine.ControlsCount > 0)
            {
                int mixerControlSize = Marshal.SizeOf(typeof (MixerInterop.MIXERCONTROL));
                var mlc = new MixerInterop.MIXERLINECONTROLS();
                IntPtr pmc = Marshal.AllocHGlobal(mixerControlSize * mixerLine.ControlsCount);
                mlc.cbStruct = Marshal.SizeOf(mlc);
                mlc.dwLineID = mixerLine.LineId;
                mlc.cControls = mixerLine.ControlsCount;
                mlc.pamxctrl = pmc;
                mlc.cbmxctrl = Marshal.SizeOf(typeof(MixerInterop.MIXERCONTROL));
                try
                {
                    MmResult err = MixerInterop.mixerGetLineControls(mixerHandle, ref mlc, MixerFlags.All | mixerHandleType);
                    if (err != MmResult.NoError)
                    {
                        throw new MmException(err, "mixerGetLineControls");
                    }
                    for (int i = 0; i < mlc.cControls; i++)
                    {
                        Int64 address = pmc.ToInt64() + mixerControlSize * i;

                        var mc = (MixerInterop.MIXERCONTROL)
                            Marshal.PtrToStructure((IntPtr)address, typeof(MixerInterop.MIXERCONTROL));
                        var mixerControl = GetMixerControl(mixerHandle, mixerLine.LineId, mc.dwControlID, mixerLine.Channels,
                                                                                 mixerHandleType);

                        controls.Add(mixerControl);
                    }
                }
                finally 
                {
                    Marshal.FreeHGlobal(pmc);
                }

            }
            return controls;
        }

        /// <summary>
        /// Gets a specified Mixer Control
        /// </summary>
        /// <param name="mixerHandle">Mixer Handle</param>
        /// <param name="nLineID">Line ID</param>
        /// <param name="controlId">Control ID</param>
        /// <param name="nChannels">Number of Channels</param>
        /// <param name="mixerFlags">Flags to use (indicates the meaning of mixerHandle)</param>
        /// <returns></returns>
        public static MixerControl GetMixerControl(IntPtr mixerHandle, int nLineID, int controlId, int nChannels,
                                                   MixerFlags mixerFlags)
        {
            MixerInterop.MIXERLINECONTROLS mlc = new MixerInterop.MIXERLINECONTROLS();
            MixerInterop.MIXERCONTROL mc = new MixerInterop.MIXERCONTROL();

            // set up the pointer to a structure
            IntPtr pMixerControl = Marshal.AllocCoTaskMem(Marshal.SizeOf(mc));
            //Marshal.StructureToPtr(mc, pMixerControl, false);      

            mlc.cbStruct = Marshal.SizeOf(mlc);
            mlc.cControls = 1;
            mlc.dwControlID = controlId;
            mlc.cbmxctrl = Marshal.SizeOf(mc);
            mlc.pamxctrl = pMixerControl;
            mlc.dwLineID = nLineID;
            MmResult err = MixerInterop.mixerGetLineControls(mixerHandle, ref mlc, MixerFlags.OneById | mixerFlags);
            if (err != MmResult.NoError)
            {
                Marshal.FreeCoTaskMem(pMixerControl);
                throw new MmException(err, "mixerGetLineControls");
            }

            // retrieve the structure from the pointer
            mc = (MixerInterop.MIXERCONTROL) Marshal.PtrToStructure(mlc.pamxctrl, typeof (MixerInterop.MIXERCONTROL));
            Marshal.FreeCoTaskMem(pMixerControl);

            if (MixerControl.IsControlBoolean(mc.dwControlType))
            {
                return new BooleanMixerControl(mc, mixerHandle, mixerFlags, nChannels);
            }
            else if (MixerControl.IsControlSigned(mc.dwControlType))
            {
                return new SignedMixerControl(mc, mixerHandle, mixerFlags, nChannels);
            }
            else if (MixerControl.IsControlUnsigned(mc.dwControlType))
            {
                return new UnsignedMixerControl(mc, mixerHandle, mixerFlags, nChannels);
            }
            else if (MixerControl.IsControlListText(mc.dwControlType))
            {
                return new ListTextMixerControl(mc, mixerHandle, mixerFlags, nChannels);
            }
            else if (MixerControl.IsControlCustom(mc.dwControlType))
            {
                return new CustomMixerControl(mc, mixerHandle, mixerFlags, nChannels);
            }
            else
            {
                throw new InvalidOperationException(String.Format("Unknown mixer control type {0}", mc.dwControlType));
            }
        }

        /// <summary>
        /// Gets the control details
        /// </summary>
        protected void GetControlDetails()
        {
            mixerControlDetails.cbStruct = Marshal.SizeOf(mixerControlDetails);
            mixerControlDetails.dwControlID = mixerControl.dwControlID;
            if (IsCustom)
            {
                mixerControlDetails.cChannels = 0;
            }
            else if ((mixerControl.fdwControl & MixerInterop.MIXERCONTROL_CONTROLF_UNIFORM) != 0)
            {
                mixerControlDetails.cChannels = 1;
            }
            else
            {
                mixerControlDetails.cChannels = nChannels;
            }


            if ((mixerControl.fdwControl & MixerInterop.MIXERCONTROL_CONTROLF_MULTIPLE) != 0)
            {
                mixerControlDetails.hwndOwner = (IntPtr) mixerControl.cMultipleItems;
            }
            else if (IsCustom)
            {
                mixerControlDetails.hwndOwner = IntPtr.Zero; // TODO: special cases
            }
            else
            {
                mixerControlDetails.hwndOwner = IntPtr.Zero;
            }

            if (IsBoolean)
            {
                mixerControlDetails.cbDetails = Marshal.SizeOf(new MixerInterop.MIXERCONTROLDETAILS_BOOLEAN());
            }
            else if (IsListText)
            {
                mixerControlDetails.cbDetails = Marshal.SizeOf(new MixerInterop.MIXERCONTROLDETAILS_LISTTEXT());
            }
            else if (IsSigned)
            {
                mixerControlDetails.cbDetails = Marshal.SizeOf(new MixerInterop.MIXERCONTROLDETAILS_SIGNED());
            }
            else if (IsUnsigned)
            {
                mixerControlDetails.cbDetails = Marshal.SizeOf(new MixerInterop.MIXERCONTROLDETAILS_UNSIGNED());
            }
            else
            {
                // must be custom
                mixerControlDetails.cbDetails = mixerControl.Metrics.customData;
            }
            var detailsSize = mixerControlDetails.cbDetails*mixerControlDetails.cChannels;
            if ((mixerControl.fdwControl & MixerInterop.MIXERCONTROL_CONTROLF_MULTIPLE) != 0)
            {
                // fixing issue 16390 - calculating size correctly for multiple items
                detailsSize *= (int) mixerControl.cMultipleItems;
            }
            IntPtr buffer = Marshal.AllocCoTaskMem(detailsSize);
            // To copy stuff in:
            // Marshal.StructureToPtr( theStruct, buffer, false );
            mixerControlDetails.paDetails = buffer;
            MmResult err = MixerInterop.mixerGetControlDetails(mixerHandle, ref mixerControlDetails,
                                                               MixerFlags.Value | mixerHandleType);
            // let the derived classes get the details before we free the handle			
            if (err == MmResult.NoError)
            {
                GetDetails(mixerControlDetails.paDetails);
            }
            Marshal.FreeCoTaskMem(buffer);
            if (err != MmResult.NoError)
            {
                throw new MmException(err, "mixerGetControlDetails");
            }
        }

        /// <summary>
        /// Gets the control details
        /// </summary>
        /// <param name="pDetails"></param>
        protected abstract void GetDetails(IntPtr pDetails);

        /// <summary>
        /// Mixer control name
        /// </summary>
        public String Name
        {
            get { return mixerControl.szName; }
        }

        /// <summary>
        /// Mixer control type
        /// </summary>
        public MixerControlType ControlType
        {
            get { return mixerControl.dwControlType; }
        }

        /// <summary>
        /// Returns true if this is a boolean control
        /// </summary>
        /// <param name="controlType">Control type</param>
        private static bool IsControlBoolean(MixerControlType controlType)
        {
            switch (controlType)
            {
                case MixerControlType.BooleanMeter:
                case MixerControlType.Boolean:
                case MixerControlType.Button:
                case MixerControlType.Loudness:
                case MixerControlType.Mono:
                case MixerControlType.Mute:
                case MixerControlType.OnOff:
                case MixerControlType.StereoEnhance:
                case MixerControlType.Mixer:
                case MixerControlType.MultipleSelect:
                case MixerControlType.Mux:
                case MixerControlType.SingleSelect:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Is this a boolean control
        /// </summary>
        public bool IsBoolean
        {
            get { return MixerControl.IsControlBoolean(mixerControl.dwControlType); }
        }

        /// <summary>
        /// Determines whether a specified mixer control type is a list text control
        /// </summary>
        private static bool IsControlListText(MixerControlType controlType)
        {
            switch (controlType)
            {
                case MixerControlType.Equalizer:
                case MixerControlType.Mixer:
                case MixerControlType.MultipleSelect:
                case MixerControlType.Mux:
                case MixerControlType.SingleSelect:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// True if this is a list text control
        /// </summary>
        public bool IsListText
        {
            get { return MixerControl.IsControlListText(mixerControl.dwControlType); }
        }

        private static bool IsControlSigned(MixerControlType controlType)
        {
            switch (controlType)
            {
                case MixerControlType.PeakMeter:
                case MixerControlType.SignedMeter:
                case MixerControlType.Signed:
                case MixerControlType.Decibels:
                case MixerControlType.Pan:
                case MixerControlType.QSoundPan:
                case MixerControlType.Slider:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// True if this is a signed control
        /// </summary>
        public bool IsSigned
        {
            get { return MixerControl.IsControlSigned(mixerControl.dwControlType); }
        }

        private static bool IsControlUnsigned(MixerControlType controlType)
        {
            switch (controlType)
            {
                case MixerControlType.UnsignedMeter:
                case MixerControlType.Unsigned:
                case MixerControlType.Bass:
                case MixerControlType.Equalizer:
                case MixerControlType.Fader:
                case MixerControlType.Treble:
                case MixerControlType.Volume:
                case MixerControlType.MicroTime:
                case MixerControlType.MilliTime:
                case MixerControlType.Percent:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// True if this is an unsigned control
        /// </summary>
        public bool IsUnsigned
        {
            get { return MixerControl.IsControlUnsigned(mixerControl.dwControlType); }
        }

        private static bool IsControlCustom(MixerControlType controlType)
        {
            return (controlType == MixerControlType.Custom);
        }

        /// <summary>
        /// True if this is a custom control
        /// </summary>
        public bool IsCustom
        {
            get { return MixerControl.IsControlCustom(mixerControl.dwControlType); }
        }

        /// <summary>
        /// String representation for debug purposes
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} {1}", Name, ControlType);
        }
    }
}
