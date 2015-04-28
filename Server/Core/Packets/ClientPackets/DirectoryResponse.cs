using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DirectoryResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] Files { get; set; }

        [ProtoMember(2)]
        public string[] Folders { get; set; }

        [ProtoMember(3)]
        public long[] FilesSize { get; set; }

        public DirectoryResponse()
        {
        }

        public DirectoryResponse(string[] files, string[] folders, long[] filessize)
        {
            this.Files = files;
            this.Folders = folders;
            this.FilesSize = filessize;
        }

        public void Execute(Client client)
        {
            client.Send<DirectoryResponse>(this);
        }
    }
}