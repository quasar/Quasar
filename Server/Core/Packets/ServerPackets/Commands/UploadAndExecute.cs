using ProtoBuf;

namespace Core.Packets.ServerPackets
{
    [ProtoContract]
    public class UploadAndExecute : IPacket
    {
        [ProtoMember(1)]
        public byte[] FileBytes { get; set; }

        [ProtoMember(2)]
        public string FileName { get; set; }

        public UploadAndExecute() { }
        public UploadAndExecute(byte[] bytes, string fileName)
        {
            FileBytes = bytes;
            FileName = fileName;
        }

        public void Execute(Client client)
        {
            client.Send<UploadAndExecute>(this);
        }
    }
}
