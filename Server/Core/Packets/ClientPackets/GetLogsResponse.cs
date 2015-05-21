using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetLogsResponse : IPacket
    {
        [ProtoMember(1)]
        public string Filename { get; set; }

        [ProtoMember(2)]
        public byte[] Block { get; set; }

        [ProtoMember(3)]
        public int MaxBlocks { get; set; }

        [ProtoMember(4)]
        public int CurrentBlock { get; set; }

        [ProtoMember(5)]
        public string CustomMessage { get; set; }

        [ProtoMember(6)]
        public int Index { get; set; }

        [ProtoMember(7)]
        public int FileCount { get; set; }

        public GetLogsResponse() { }
        public GetLogsResponse(string filename, byte[] block, int maxblocks, int currentblock, string custommessage, int index, int fileCount)
        {
            this.Filename = filename;
            this.Block = block;
            this.MaxBlocks = maxblocks;
            this.CurrentBlock = currentblock;
            this.CustomMessage = custommessage;
            this.Index = index;
            this.FileCount = fileCount;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}