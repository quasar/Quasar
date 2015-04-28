using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class StartProcess : IPacket
    {
        [ProtoMember(1)]
        public string Processname { get; set; }

        public StartProcess()
        {
        }

        public StartProcess(string processname)
        {
            this.Processname = processname;
        }

        public void Execute(Client client)
        {
            client.Send<StartProcess>(this);
        }
    }
}