using System;
using xServer.Core.Networking;
using xServer.Core.Packets.ServerPackets;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetDirectoryResponse : IPacket
    {
        public string[] Files { get; set; }

        public string[] Folders { get; set; }

        public long[] FilesSize { get; set; }
  
        public DateTime[] LastModificationDates { get; set; }

        public DateTime[] CreationDates { get; set; }

        public InformationDetail Detail { get; set; }

        public GetDirectoryResponse()
        {
        }
        public GetDirectoryResponse(string[] files, string[] folders, long[] filessize, DateTime[] lastModificationDates, DateTime[] creationDates, InformationDetail detail)
        {
            this.Files = files;
            this.Folders = folders;
            this.FilesSize = filessize;
            this.LastModificationDates = lastModificationDates;
            this.CreationDates = creationDates;
            this.Detail = detail;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}