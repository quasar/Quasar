using Microsoft.Win32;
using Quasar.Client.Config;
using Quasar.Client.Helper;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Quasar.Client.Setup
{
    public static class Startup
    {
        public static bool AddToStartup()
        {
            if (WindowsAccountHelper.GetAccountType() == "Admin")
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("schtasks")
                    {
                        Arguments = "/create /tn \"" + Settings.STARTUPKEY + "\" /sc ONLOGON /tr \"" + Application.ExecutablePath + "\" /rl HIGHEST /f",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    Process p = Process.Start(startInfo);
                    p.WaitForExit(1000);
                    if (p.ExitCode == 0) return true;
                }
                catch (Exception)
                {
                }

                return RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY, Application.ExecutablePath,
                    true);
            }
            else
            {
                return RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY, Application.ExecutablePath,
                    true);
            }
        }

        public static bool RemoveFromStartup()
        {
            if (WindowsAccountHelper.GetAccountType() == "Admin")
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo("schtasks")
                    {
                        Arguments = "/delete /tn \"" + Settings.STARTUPKEY + "\" /f",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    Process p = Process.Start(startInfo);
                    p.WaitForExit(1000);
                    if (p.ExitCode == 0) return true;
                }
                catch (Exception)
                {
                }

                return RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY);
            }
            else
            {
                return RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.CurrentUser,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY);
            }
        }
    }
}
