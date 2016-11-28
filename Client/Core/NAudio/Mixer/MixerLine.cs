// created on 10/12/2002 at 20:37

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using xServer.Core.NAudio.Wave.MmeInterop;

namespace xServer.Core.NAudio.Mixer 
{
    /// <summary>
    /// Represents a mixer line (source or destination)
    /// </summary>
    public class MixerLine 
    {
        private MixerInterop.MIXERLINE mixerLine;
        private IntPtr mixerHandle;
        private MixerFlags mixerHandleType;

        /// <summary>
        /// Creates a new mixer destination
        /// </summary>
        /// <param name="mixerHandle">Mixer Handle</param>
        /// <param name="destinationIndex">Destination Index</param>
        /// <param name="mixerHandleType">Mixer Handle Type</param>
        public MixerLine(IntPtr mixerHandle, int destinationIndex, MixerFlags mixerHandleType) 
        {
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            mixerLine = new MixerInterop.MIXERLINE();
            mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
            mixerLine.dwDestination = destinationIndex;
            MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.GetLineInfoOfDestination), "mixerGetLineInfo");
        }

        /// <summary>
        /// Creates a new Mixer Source For a Specified Source
        /// </summary>
        /// <param name="mixerHandle">Mixer Handle</param>
        /// <param name="destinationIndex">Destination Index</param>
        /// <param name="sourceIndex">Source Index</param>
        /// <param name="mixerHandleType">Flag indicating the meaning of mixerHandle</param>
        public MixerLine(IntPtr mixerHandle, int destinationIndex, int sourceIndex, MixerFlags mixerHandleType) 
        {
            this.mixerHandle = mixerHandle;
            this.mixerHandleType = mixerHandleType;
            mixerLine = new MixerInterop.MIXERLINE();
            mixerLine.cbStruct = Marshal.SizeOf(mixerLine);
            mixerLine.dwDestination = destinationIndex;
            mixerLine.dwSource = sourceIndex;
            MmException.Try(MixerInterop.mixerGetLineInfo(mixerHandle, ref mixerLine, mixerHandleType | MixerFlags.GetLineInfoOfSource), "mixerGetLineInfo");
        }

        /// <summary>
        /// Creates a new Mixer Source
        /// </summary>
        /// <param name="waveInDevice">Wave In Device</param>
        public static int GetMixerIdForWaveIn(int waveInDevice)
        {
            int mixerId = -1;
            MmException.Try(MixerInterop.mixerGetID((IntPtr)waveInDevice, out mixerId, MixerFlags.WaveIn), "mixerGetID");
            return mixerId;
        }

        /// <summary>
        /// Mixer Line Name
        /// </summary>
        public String Name 
        {
            get 
            {
                return mixerLine.szName;
            }
        }
        
        /// <summary>
        /// Mixer Line short name
        /// </summary>
        public String ShortName 
        {
            get 
            {
                return mixerLine.szShortName;
            }
        }

        /// <summary>
        /// The line ID
        /// </summary>
        public int LineId
        {
            get
            {
                return mixerLine.dwLineID;
            }
        }

        /// <summary>
        /// Component Type
        /// </summary>
        public MixerLineComponentType ComponentType
        {
            get
            {
                return mixerLine.dwComponentType;
            }
        }

        /// <summary>
        /// Mixer destination type description
        /// </summary>
        public String TypeDescription 
        {
            get 
            {
                switch (mixerLine.dwComponentType)
                {
                    // destinations
                    case MixerLineComponentType.DestinationUndefined:
                        return "Undefined Destination";
                    case MixerLineComponentType.DestinationDigital:
                        return "Digital Destination";
                    case MixerLineComponentType.DestinationLine:
                        return "Line Level Destination";
                    case MixerLineComponentType.DestinationMonitor:
                        return "Monitor Destination";
                    case MixerLineComponentType.DestinationSpeakers:
                        return "Speakers Destination";
                    case MixerLineComponentType.DestinationHeadphones:
                        return "Headphones Destination";
                    case MixerLineComponentType.DestinationTelephone:
                        return "Telephone Destination";
                    case MixerLineComponentType.DestinationWaveIn:
                        return "Wave Input Destination";
                    case MixerLineComponentType.DestinationVoiceIn:
                        return "Voice Recognition Destination";
                    // sources
                    case MixerLineComponentType.SourceUndefined:
                        return "Undefined Source";
                    case MixerLineComponentType.SourceDigital:
                        return "Digital Source";
                    case MixerLineComponentType.SourceLine:
                        return "Line Level Source";
                    case MixerLineComponentType.SourceMicrophone:
                        return "Microphone Source";
                    case MixerLineComponentType.SourceSynthesizer:
                        return "Synthesizer Source";
                    case MixerLineComponentType.SourceCompactDisc:
                        return "Compact Disk Source";
                    case MixerLineComponentType.SourceTelephone:
                        return "Telephone Source";
                    case MixerLineComponentType.SourcePcSpeaker:
                        return "PC Speaker Source";
                    case MixerLineComponentType.SourceWaveOut:
                        return "Wave Out Source";
                    case MixerLineComponentType.SourceAuxiliary:
                        return "Auxiliary Source";
                    case MixerLineComponentType.SourceAnalog:
                        return "Analog Source";
                    default:
                        return "Invalid Component Type";
                }
            }				
        }
        
        /// <summary>
        /// Number of channels
        /// </summary>
        public int Channels 
        {
            get 
            {
                return mixerLine.cChannels;
            }
        }
        
        /// <summary>
        /// Number of sources
        /// </summary>
        public int SourceCount 
        {
            get 
            {
                return mixerLine.cConnections;
            }
        }
        
        /// <summary>
        /// Number of controls
        /// </summary>
        public int ControlsCount 
        {
            get 
            {
                return mixerLine.cControls;
            }
        }

        /// <summary>
        /// Is this destination active
        /// </summary>
        public bool IsActive
        {
            get
            {
                return (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_ACTIVE) != 0;
            }
        }

        /// <summary>
        /// Is this destination disconnected
        /// </summary>
        public bool IsDisconnected
        {
            get
            {
                return (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_DISCONNECTED) != 0;
            }
        }

        /// <summary>
        /// Is this destination a source
        /// </summary>
        public bool IsSource
        {
            get
            {
                return (mixerLine.fdwLine & MixerInterop.MIXERLINE_LINEF.MIXERLINE_LINEF_SOURCE) != 0;
            }
        }

        /// <summary>
        /// Gets the specified source
        /// </summary>
        public MixerLine GetSource(int sourceIndex) 
        {
            if(sourceIndex < 0 || sourceIndex >= SourceCount) 
            {
                throw new ArgumentOutOfRangeException("sourceIndex");
            }
            return new MixerLine(mixerHandle, mixerLine.dwDestination, sourceIndex, this.mixerHandleType);			
        }

        /// <summary>
        /// Enumerator for the controls on this Mixer Limne
        /// </summary>
        public IEnumerable<MixerControl> Controls
        {
            get
            {
                return MixerControl.GetMixerControls(this.mixerHandle, this, this.mixerHandleType);
            }
        }

        /// <summary>
        /// Enumerator for the sources on this Mixer Line
        /// </summary>
        public IEnumerable<MixerLine> Sources
        {
            get
            {
                for (int source = 0; source < SourceCount; source++)
                {
                    yield return GetSource(source);
                }
            }
        }

        /// <summary>
        /// The name of the target output device
        /// </summary>
        public string TargetName
        {
            get
            {
                return mixerLine.szPname;
            }
        }

        /// <summary>
        /// Describes this Mixer Line (for diagnostic purposes)
        /// </summary>
        public override string ToString()
        {
            return String.Format("{0} {1} ({2} controls, ID={3})", 
                Name, TypeDescription, ControlsCount, mixerLine.dwLineID);
        }
    }
}

