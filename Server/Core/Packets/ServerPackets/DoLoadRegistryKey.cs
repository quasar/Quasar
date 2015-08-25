using System;
using Microsoft.Win32;
using xServer.Core.Networking;
using xServer.Core.Utilities;

namespace xServer.Core.Packets.ServerPackets
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