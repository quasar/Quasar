using System;
using System.Net;
using System.Management;

namespace xClient.Core.Information
{
    public static class OSInfo
    {
        #region BITS

        /// <summary>
        /// Determines if the current application is 32 or 64-bit.
        /// </summary>
        public static int Bits
        {
            get { return IntPtr.Size*8; }
        }

        #endregion BITS

        #region NAME

        private static string _osName;

        /// <summary>
        /// Gets the name of the operating system running on this computer (including the edition).
        /// </summary>
        public static string Name
        {
            get
            {
                if (_osName != null)
                    return _osName;

                string name = "Uknown OS";
                using (
                    ManagementObjectSearcher searcher =
                        new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
                {
                    foreach (ManagementObject os in searcher.Get())
                    {
                        name = os["Caption"].ToString();
                        break;
                    }
                }

                if (name.StartsWith("Microsoft"))
                    name = name.Substring(name.IndexOf(" ", StringComparison.Ordinal) + 1);

                _osName = name;
                return _osName;
            }
        }
        public static string LocalName
        {
            get{
                return Dns.GetHostName().ToString();
            }
        }
        public static string Localip
        {
            get
            {
                string hostinfo = "";
                string hostName = Dns.GetHostName();
                System.Net.IPAddress[] addressList = Dns.GetHostAddresses(hostName);
                foreach (IPAddress ip in addressList)
                {
                   hostinfo+=ip.ToString();
                }
                return hostinfo;
            }
        }

        #endregion NAME
    }
}