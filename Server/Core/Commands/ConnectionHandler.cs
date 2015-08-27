using System.IO;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Packets.ClientPackets;
using xServer.Forms;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT HANDLE CONNECTION COMMANDS. */
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
                client.Value.Country = packet.Country;
                client.Value.CountryCode = packet.CountryCode;
                client.Value.Region = packet.Region;
                client.Value.City = packet.City;
                client.Value.Id = packet.Id;
                client.Value.Username = packet.Username;
                client.Value.PCName = packet.PCName;
                client.Value.Tag = packet.Tag;
                client.Value.ImageIndex = packet.ImageIndex;

                client.Value.DownloadDirectory = (!FileHelper.CheckPathForIllegalChars(client.Value.UserAtPc)) ?
                    Path.Combine(Application.StartupPath, string.Format("Clients\\{0}_{1}\\", client.Value.UserAtPc, client.Value.Id.Substring(0, 7))) :
                    Path.Combine(Application.StartupPath, string.Format("Clients\\{0}_{1}\\", client.EndPoint.Address, client.Value.Id.Substring(0, 7)));

                if (Settings.ShowToolTip)
                    new Packets.ServerPackets.GetSystemInfo().Execute(client);
            }
            catch
            {
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