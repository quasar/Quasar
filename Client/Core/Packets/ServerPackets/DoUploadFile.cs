using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoUploadFile : IPacket
    {
        public int ID { get; set; }

        public string RemotePath { get; set; }

        public byte[] Block { get; set; }

        public int MaxBlocks { get; set; }

        public int CurrentBlock { get; set; }

        public DoUploadFile()
        {
        }

        public DoUploadFile(int id, string remotepath, byte[] block, int maxblocks, int currentblock)
        {
            this.ID = id;
            this.RemotePath = remotepath;
            this.Block = block;
            this.MaxBlocks = maxblocks;
            this.CurrentBlock = currentblock;
        }

        public void Execute(Client client)
        {
            client.SendBlocking(this);
        }
    }
}