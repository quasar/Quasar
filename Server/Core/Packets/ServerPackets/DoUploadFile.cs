using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoUploadFile : IPacket
    {
        [ProtoMember(1)]
        public int ID { get; set; }

        [ProtoMember(2)]
        public string RemotePath { get; set; }

        [ProtoMember(3)]
        public byte[] Block { get; set; }

        [ProtoMember(4)]
        public int MaxBlocks { get; set; }

        [ProtoMember(5)]
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