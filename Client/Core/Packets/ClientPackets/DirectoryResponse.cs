using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DirectoryResponse : IPacket
    {
        public DirectoryResponse()
        {
        }

        public DirectoryResponse(string[] files, string[] folders, long[] filessize)
        {
            Files = files;
            Folders = folders;
            FilesSize = filessize;
        }

        [ProtoMember(1)]
        public string[] Files { get; set; }

        [ProtoMember(2)]
        public string[] Folders { get; set; }

        [ProtoMember(3)]
        public long[] FilesSize { get; set; }

        public void Execute(Client client)
        {
            client.Send<DirectoryResponse>(this);
        }
    }
}