using Quasar.Common.Enums;
using System;
using System.Security.Principal;

namespace Quasar.Client.User
{
    public class UserAccount
    {
        public string UserName { get; }

        public AccountType Type { get; }

        public UserAccount()
        {
            UserName = Environment.UserName;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);

                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    Type = AccountType.Admin;
                }
                else if (principal.IsInRole(WindowsBuiltInRole.User))
                {
                    Type = AccountType.User;
                }
                else if (principal.IsInRole(WindowsBuiltInRole.Guest))
                {
                    Type = AccountType.Guest;
                }
                else
                {
                    Type = AccountType.Unknown;
                }
            }
        }
    }
}
