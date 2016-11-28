using System;

namespace xServer.Core.NAudio.Mixer
{    
    /// <summary>
    /// Mixer Interop Flags
    /// </summary>
    [Flags]
    public enum MixerFlags
    {
        #region Objects
        /// <summary>
        /// MIXER_OBJECTF_HANDLE 	= 0x80000000;
        /// </summary>
        Handle = unchecked ( (int) 0x80000000 ),
        /// <summary>
        /// MIXER_OBJECTF_MIXER 	= 0x00000000;
        /// </summary>
        Mixer = 0,
        /// <summary>
        /// MIXER_OBJECTF_HMIXER
        /// </summary>
        MixerHandle = Mixer | Handle,
        /// <summary>
        /// MIXER_OBJECTF_WAVEOUT
        /// </summary>
        WaveOut = 0x10000000,
        /// <summary>
        /// MIXER_OBJECTF_HWAVEOUT
        /// </summary>
        WaveOutHandle = WaveOut | Handle,
        /// <summary>
        /// MIXER_OBJECTF_WAVEIN
        /// </summary>
        WaveIn = 0x20000000,
        /// <summary>
        /// MIXER_OBJECTF_HWAVEIN
        /// </summary>
        WaveInHandle = WaveIn | Handle,
        /// <summary>
        /// MIXER_OBJECTF_MIDIOUT
        /// </summary>
        MidiOut = 0x30000000,
        /// <summary>
        /// MIXER_OBJECTF_HMIDIOUT
        /// </summary>
        MidiOutHandle = MidiOut | Handle,
        /// <summary>
        /// MIXER_OBJECTF_MIDIIN
        /// </summary>
        MidiIn = 0x40000000,
        /// <summary>
        /// MIXER_OBJECTF_HMIDIIN
        /// </summary>
        MidiInHandle = MidiIn | Handle,
        /// <summary>
        /// MIXER_OBJECTF_AUX
        /// </summary>
        Aux = 0x50000000,
        #endregion

        #region Get/Set control details
        /// <summary>
        /// MIXER_GETCONTROLDETAILSF_VALUE      	= 0x00000000;
        /// MIXER_SETCONTROLDETAILSF_VALUE      	= 0x00000000;
        /// </summary>
        Value = 0,
        /// <summary>
        /// MIXER_GETCONTROLDETAILSF_LISTTEXT   	= 0x00000001;
        /// MIXER_SETCONTROLDETAILSF_LISTTEXT   	= 0x00000001;
        /// </summary>
        ListText = 1,
        /// <summary>
        /// MIXER_GETCONTROLDETAILSF_QUERYMASK  	= 0x0000000F;
        /// MIXER_SETCONTROLDETAILSF_QUERYMASK  	= 0x0000000F;
        /// MIXER_GETLINECONTROLSF_QUERYMASK    	= 0x0000000F;
        /// </summary>
        QueryMask = 0xF,
        #endregion

        #region get line controls
        /// <summary>
        /// MIXER_GETLINECONTROLSF_ALL          	= 0x00000000;
        /// </summary>
        All = 0,
        /// <summary>
        /// MIXER_GETLINECONTROLSF_ONEBYID      	= 0x00000001;
        /// </summary>
        OneById = 1,
        /// <summary>
        /// MIXER_GETLINECONTROLSF_ONEBYTYPE    	= 0x00000002;
        /// </summary>
        OneByType = 2,		
        #endregion

        /// <summary>
        /// MIXER_GETLINEINFOF_DESTINATION      	= 0x00000000;
        /// </summary>
        GetLineInfoOfDestination = 0,
        /// <summary>
        /// MIXER_GETLINEINFOF_SOURCE           	= 0x00000001;
        /// </summary>
        GetLineInfoOfSource = 1,
        /// <summary>
        /// MIXER_GETLINEINFOF_LINEID           	= 0x00000002;
        /// </summary>
        GetLineInfoOfLineId = 2,
        /// <summary>
        /// MIXER_GETLINEINFOF_COMPONENTTYPE    	= 0x00000003;
        /// </summary>
        GetLineInfoOfComponentType = 3,
        /// <summary>
        /// MIXER_GETLINEINFOF_TARGETTYPE       	= 0x00000004;
        /// </summary>
        GetLineInfoOfTargetType = 4,
        /// <summary>
        /// MIXER_GETLINEINFOF_QUERYMASK        	= 0x0000000F;
        /// </summary>
        GetLineInfoOfQueryMask = 0xF,
    }
}
