using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoDownloadFilePause : IPacket
    {
        public int ID { get; set; }

        public DoDownloadFilePause()
        {
        }

        public DoDownloadFilePause(int id)
        {
            this.ID = id;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}