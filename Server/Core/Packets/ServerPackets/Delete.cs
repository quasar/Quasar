using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Delete : IPacket
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public bool IsDir { get; set; }

        public Delete()
        {
        }

        public Delete(string path, bool isdir)
        {
            this.Path = path;
            this.IsDir = isdir;
        }

        public void Execute(Client client)
        {
            client.Send<Delete>(this);
        }
    }
}