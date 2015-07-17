using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class GetDirectory : IPacket
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }

        public GetDirectory()
        {
        }

        public GetDirectory(string remotepath)
        {
            this.RemotePath = remotepath;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}