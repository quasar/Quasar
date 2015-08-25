using System;
using Microsoft.Win32;
using xClient.Core.Networking;
using xClient.Core.Registry;

namespace xClient.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoLoadRegistryKey : IPacket
    {
        public string[] RootKeyNames { get; set; }

        public DoLoadRegistryKey(string[] rootKeyNames)
        {
            RootKeyNames = rootKeyNames;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}