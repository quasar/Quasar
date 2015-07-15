using ProtoBuf;
using xServer.Core.Networking;
using PathType = xServer.Core.Commands.CommandHandler.PathType;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoPathDelete : IPacket
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public PathType PathType { get; set; }

        public DoPathDelete()
        {
        }

        public DoPathDelete(string path, PathType pathtype)
        {
            this.Path = path;
            this.PathType = pathtype;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}