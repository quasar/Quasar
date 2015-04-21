using ProtoBuf;

namespace xServer.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class InitializeCommand : IPacket
    {
        public InitializeCommand()
        {
        }

        public void Execute(Client client)
        {
            client.Send<InitializeCommand>(this);
        }
    }
}