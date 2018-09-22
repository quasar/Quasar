using Quasar.Common.Messages;
using xServer.Core.Commands;

namespace xServer.Core.Networking
{
    public static class PacketHandler
    {
        public static void HandlePacket(Client client, IMessage packet)
        {
            if (client == null || client.Value == null)
                return;

            var type = packet.GetType();

            if (type == typeof(SetStatus))
            {
                CommandHandler.HandleSetStatus(client, (SetStatus)packet);
            }
            else if (type == typeof(SetUserStatus))
            {
                CommandHandler.HandleSetUserStatus(client, (SetUserStatus)packet);
            }
        }
    }
}
