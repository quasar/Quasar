using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoRenameRegistryKey : IPacket
    {
        public string ParentPath { get; set; }
        public string OldKeyName { get; set; }
        public string NewKeyName { get; set; }

        public DoRenameRegistryKey(string parentPath, string oldKeyName, string newKeyName)
        {
            ParentPath = parentPath;
            OldKeyName = oldKeyName;
            NewKeyName = newKeyName;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
