using Quasar.Common.Messages;
using System;
using System.Windows.Forms;
using xServer.Core.Networking;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE SYSTEM (drives, directories, files, etc.). */
    public static partial class CommandHandler
    {
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