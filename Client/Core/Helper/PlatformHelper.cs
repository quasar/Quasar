using System;
using System.Management;
using System.Text.RegularExpressions;

namespace xClient.Core.Helper
{
    public static class PlatformHelper
    {
        /// <summary>
        /// Initializes the <see cref="PlatformHelper"/> class.
        /// </summary>
        static PlatformHelper()
        {
            Win32NT = Environment.OSVersion.Platform == PlatformID.Win32NT;
            XpOrHigher = Win32NT && Environment.OSVersion.Version.Major >= 5;
            VistaOrHigher = Win32NT && Environment.OSVersion.Version.Major >= 6;
            SevenOrHigher = Win32NT && (Environment.OSVersion.Version >= new Version(6, 1));
            EightOrHigher = Win32NT && (Environment.OSVersion.Version >= new Version(6, 2, 9200));
            EightPointOneOrHigher = Win32NT && (Environment.OSVersion.Version >= new Version(6, 3));
            TenOrHigher = Win32NT && (Environment.OSVersion.Version >= new Version(10, 0));
            RunningOnMono = Type.GetType("Mono.Runtime") != null;

            Name = "Unknown OS";
            using (
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject os in searcher.Get())
                {
                    Name = os["Caption"].ToString();
                    break;
                }
            }

            Name = Regex.Replace(Name, "^.*(?=Windows)", "").TrimEnd().TrimStart(); // Remove everything before first match "Windows" and trim end & start
        }

        /// <summary>
        /// Gets the name of the operating system running on this computer (including the edition).
        /// </summary>
        public static string Name { get; private set; }

        /// <summary>
        /// Determines if the current application is 32 or 64-bit.
        /// </summary>
        public static int Architecture { get { return IntPtr.Size * 8; } }

        /// <summary>
        /// Returns a indicating whether the application is running in Mono runtime.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the application is running in Mono runtime; otherwise, <c>false</c>.
        /// </value>
        public static bool RunningOnMono { get; private set; }

        /// <summary>
        /// Returns a indicating whether the Operating System is Windows 32 NT based.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Operating System is Windows 32 NT based; otherwise, <c>false</c>.
        /// </value>
        public static bool Win32NT { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the Operating System is Windows XP or higher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Operating System is Windows 8 or higher; otherwise, <c>false</c>.
        /// </value>
        public static bool XpOrHigher { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the Operating System is Windows Vista or higher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Operating System is Windows Vista or higher; otherwise, <c>false</c>.
        /// </value>
        public static bool VistaOrHigher { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the Operating System is Windows 7 or higher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Operating System is Windows 7 or higher; otherwise, <c>false</c>.
        /// </value>
        public static bool SevenOrHigher { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the Operating System is Windows 8 or higher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Operating System is Windows 8 or higher; otherwise, <c>false</c>.
        /// </value>
        public static bool EightOrHigher { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the Operating System is Windows 8.1 or higher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Operating System is Windows 8.1 or higher; otherwise, <c>false</c>.
        /// </value>
        public static bool EightPointOneOrHigher { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the Operating System is Windows 10 or higher.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the Operating System is Windows 10 or higher; otherwise, <c>false</c>.
        /// </value>
        public static bool TenOrHigher { get; private set; }
    }
}
