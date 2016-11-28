using System;
using System.Runtime.InteropServices;

namespace xServer.Core.NAudio.Wave.MmeInterop
{
    /// <summary>
    /// WaveInCapabilities structure (based on WAVEINCAPS2 from mmsystem.h)
    /// http://msdn.microsoft.com/en-us/library/ms713726(VS.85).aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WaveInCapabilities
    {
        /// <summary>
        /// wMid
        /// </summary>
        private short manufacturerId;
        /// <summary>
        /// wPid
        /// </summary>
        private short productId;
        /// <summary>
        /// vDriverVersion
        /// </summary>
        private int driverVersion;
        /// <summary>
        /// Product Name (szPname)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxProductNameLength)]
        private string productName;
        /// <summary>
        /// Supported formats (bit flags) dwFormats 
        /// </summary>
        private SupportedWaveFormat supportedFormats;
        /// <summary>
        /// Supported channels (1 for mono 2 for stereo) (wChannels)
        /// Seems to be set to -1 on a lot of devices
        /// </summary>
        private short channels;
        /// <summary>
        /// wReserved1
        /// </summary>
        private short reserved;

        // extra WAVEINCAPS2 members
        private Guid manufacturerGuid;
        private Guid productGuid;
        private Guid nameGuid;

        private const int MaxProductNameLength = 32;

        /// <summary>
        /// Number of channels supported
        /// </summary>
        public int Channels
        {
            get
            {
                return channels;
            }
        }

        /// <summary>
        /// The product name
        /// </summary>
        public string ProductName
        {
            get
            {
                return productName;
            }
        }

        /// <summary>
        /// The device name Guid (if provided)
        /// </summary>
        public Guid NameGuid { get { return nameGuid; } }
        /// <summary>
        /// The product name Guid (if provided)
        /// </summary>
        public Guid ProductGuid { get { return productGuid; } }
        /// <summary>
        /// The manufacturer guid (if provided)
        /// </summary>
        public Guid ManufacturerGuid { get { return manufacturerGuid; } }

        /// <summary>
        /// Checks to see if a given SupportedWaveFormat is supported
        /// </summary>
        /// <param name="waveFormat">The SupportedWaveFormat</param>
        /// <returns>true if supported</returns>
        public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
        {
            return (supportedFormats & waveFormat) == waveFormat;
        }

    }

    internal static class WaveCapabilitiesHelpers
    {
        public static readonly Guid MicrosoftDefaultManufacturerId = new Guid("d5a47fa8-6d98-11d1-a21a-00a0c9223196");
        public static readonly Guid DefaultWaveOutGuid = new Guid("E36DC310-6D9A-11D1-A21A-00A0C9223196");
        public static readonly Guid DefaultWaveInGuid = new Guid("E36DC311-6D9A-11D1-A21A-00A0C9223196");

        /// <summary>
        /// The device name from the registry if supported
        /// </summary>
        public static string GetNameFromGuid(Guid guid)
        {
            // n.b it seems many audio drivers just return the default values, which won't be in the registry
            // http://www.tech-archive.net/Archive/Development/microsoft.public.win32.programmer.mmedia/2006-08/msg00102.html
            string name = null;
            using (var namesKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"System\CurrentControlSet\Control\MediaCategories"))
            using (var nameKey = namesKey.OpenSubKey(guid.ToString("B")))
            {
                if (nameKey != null) name = nameKey.GetValue("Name") as string;
            }
            return name;
        }
    }
}
