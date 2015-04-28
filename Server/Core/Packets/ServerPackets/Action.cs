using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Action : IPacket
    {
        [ProtoMember(1)]
        public int Mode { get; set; }

        public Action()
        {
        }

        public Action(int mode)
        {
            this.Mode = mode;
        }

        public void Execute(Client client)
        {
            client.Send<Action>(this);
        }
    }
}