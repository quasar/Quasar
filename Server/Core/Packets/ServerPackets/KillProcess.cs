using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class KillProcess : IPacket
    {
        [ProtoMember(1)]
        public int PID { get; set; }

        public KillProcess()
        {
        }

        public KillProcess(int pid)
        {
            this.PID = pid;
        }

        public void Execute(Client client)
        {
            client.Send<KillProcess>(this);
        }
    }
}