using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using Quasar.Client.Networking;
using Quasar.Common.Enums;
using Quasar.Common.Messages;

namespace Quasar.Client.Helper
{
    public static class WindowsAccountHelper
    {
        public static UserStatus LastUserStatus { get; set; }

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

        public static void StartUserIdleCheckThread()
        {
            new Thread(UserIdleThread) {IsBackground = true}.Start();
        }

        static void UserIdleThread()
        {
            while (!QuasarClient.Exiting)
            {
                Thread.Sleep(5000);
                if (IsUserIdle())
                {
                    if (LastUserStatus != UserStatus.Idle)
                    {
                        LastUserStatus = UserStatus.Idle;
                        Program.ConnectClient.Send(new SetUserStatus {Message = LastUserStatus});
                    }
                }
                else
                {
                    if (LastUserStatus != UserStatus.Active)
                    {
                        LastUserStatus = UserStatus.Active;
                        Program.ConnectClient.Send(new SetUserStatus { Message = LastUserStatus });
                    }
                }
            }
        }

        static bool IsUserIdle()
        {
            long ticks = Stopwatch.GetTimestamp();

            long idleTime = ticks - NativeMethodsHelper.GetLastInputInfoTickCount();

            idleTime = ((idleTime > 0) ? (idleTime / 1000) : 0);

            return (idleTime > 600); // idle for 10 minutes
        }
    }
}
