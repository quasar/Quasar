using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class Action : IPacket
    {
        public Action()
        {
        }

        public Action(int mode)
        {
            Mode = mode;
        }

        [ProtoMember(1)]
        public int Mode { get; set; }

        public void Execute(Client client)
        {
            client.Send<Action>(this);
        }
    }
}