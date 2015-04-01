using System;
using System.Management;

namespace xClient.Core.Information
{
    static public class OSInfo
    {
        #region BITS
        /// <summary>
        /// Determines if the current application is 32 or 64-bit.
        /// </summary>
        static public int Bits
        {
            get
            {
                return IntPtr.Size * 8;
            }
        }
        #endregion BITS

        #region NAME
        static private string _osName;
        /// <summary>
        /// Gets the name of the operating system running on this computer (including the edition).
        /// </summary>
        static public string Name
        {
            get
            {
                if (_osName != null)
                    return _osName;

                string name = "Uknown OS";
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem"))
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
        #endregion NAME

    }
}
