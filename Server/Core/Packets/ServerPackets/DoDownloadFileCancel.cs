using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoDownloadFileCancel : IPacket
    {
        public int ID { get; set; }

        public DoDownloadFileCancel()
        {
        }

        public DoDownloadFileCancel(int id)
        {
            this.ID = id;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}