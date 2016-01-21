using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoCreateRegistryKey : IPacket
    {
        public string ParentPath { get; set; }

        public DoCreateRegistryKey(string parentPath)
        {
            ParentPath = parentPath;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
