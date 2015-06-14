using xServer.Core.Networking;

namespace xServer.Core.Packets
{
    public interface IPacket
    {
        void Execute(Client client);
    }
}