using xClient.Core.Networking;

namespace xClient.Core.Packets
{
    public interface IPacket
    {
        void Execute(Client client);
    }
}