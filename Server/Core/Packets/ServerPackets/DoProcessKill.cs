using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoProcessKill : IPacket
    {
        [ProtoMember(1)]
        public int PID { get; set; }

        public DoProcessKill()
        {
        }

        public DoProcessKill(int pid)
        {
            this.PID = pid;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}