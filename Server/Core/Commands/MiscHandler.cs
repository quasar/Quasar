using Quasar.Common.Messages;
using xServer.Core.Networking;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN MISCELLANEOUS METHODS. */
    public static partial class CommandHandler
    {
        public static void HandleDoShellExecuteResponse(Client client, DoShellExecuteResponse packet)
        {
            if (client.Value == null || client.Value.FrmRs == null || string.IsNullOrEmpty(packet.Output))
                return;

            if (packet.IsError)
                client.Value.FrmRs.PrintError(packet.Output);
            else
                client.Value.FrmRs.PrintMessage(packet.Output);
        }
    }
}