using System;
using System.Security.Principal;

namespace Quasar.Client.Helper
{
    public static class WindowsAccountHelper
    {
        public static string GetName()
        {
            return Environment.UserName;
        }

        public static string GetAccountType()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                if (identity != null)
                {
                    WindowsPrincipal principal = new WindowsPrincipal(identity);

                    if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                        return "Admin";
                    if (principal.IsInRole(WindowsBuiltInRole.User))
                        return "User";
                    if (principal.IsInRole(WindowsBuiltInRole.Guest))
                        return "Guest";
                }
            }

            return "Unknown";
        }
    }
}
