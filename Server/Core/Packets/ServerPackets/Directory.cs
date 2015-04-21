using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Directory : IPacket
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }

        public Directory()
        {
        }

        public Directory(string remotepath)
        {
            this.RemotePath = remotepath;
        }

        public void Execute(Client client)
        {
            client.Send<Directory>(this);
        }
    }
}