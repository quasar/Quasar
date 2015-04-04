using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class StartProcess : IPacket
    {
        public StartProcess()
        {
        }

        public StartProcess(string processname)
        {
            Processname = processname;
        }

        [ProtoMember(1)]
        public string Processname { get; set; }

        public void Execute(Client client)
        {
            client.Send<StartProcess>(this);
        }
    }
}