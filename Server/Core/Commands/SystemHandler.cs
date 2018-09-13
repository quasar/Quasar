using Quasar.Common.Messages;
using System;
using System.Text;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Networking;
using xServer.Forms;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
        public static void HandleGetSystemInfoResponse(Client client, GetSystemInfoResponse packet)
        {
            if (packet.SystemInfos == null)
                return;

            if (Settings.ShowToolTip)
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

            if (client.Value == null || client.Value.FrmSi == null)
                return;

            ListViewItem[] lviCollection = new ListViewItem[packet.SystemInfos.Length / 2];
            for (int i = 0, j = 0; i < packet.SystemInfos.Length; i += 2, j++)
            {
                if (packet.SystemInfos[i] != null && packet.SystemInfos[i + 1] != null)
                {
                    lviCollection[j] = new ListViewItem(new string[] { packet.SystemInfos[i], packet.SystemInfos[i + 1] });
                }
            }

            if (client.Value != null && client.Value.FrmSi != null)
                client.Value.FrmSi.AddItems(lviCollection);
        }

        public static void HandleGetStartupItemsResponse(Client client, GetStartupItemsResponse packet)
        {
            if (client.Value == null || client.Value.FrmStm == null || packet.StartupItems == null)
                return;

            foreach (var item in packet.StartupItems)
            {
                if (client.Value == null || client.Value.FrmStm == null) return;

                int type;
                if (!int.TryParse(item.Substring(0, 1), out type)) continue;

                string preparedItem = item.Remove(0, 1);
                var temp = preparedItem.Split(new string[] { "||" }, StringSplitOptions.None);
                var l = new ListViewItem(temp)
                {
                    Group = client.Value.FrmStm.GetGroup(type),
                    Tag = type
                };

                if (l.Group == null)
                    return;

                client.Value.FrmStm.AddAutostartItemToListview(l);
            }
        }
    }
}