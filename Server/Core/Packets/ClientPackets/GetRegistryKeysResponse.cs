using System;
using xServer.Core.Networking;
using xServer.Core.Utilities;

namespace xServer.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetRegistryKeysResponse : IPacket
    {
        public RegSeekerMatch[] Matches { get; set; }

        public bool IsRootKey { get; set; }

        public GetRegistryKeysResponse()
        { }

        public GetRegistryKeysResponse(RegSeekerMatch match, bool isRootKey = false)
            : this(new RegSeekerMatch[] { match }, isRootKey)
        { }

        public GetRegistryKeysResponse(RegSeekerMatch[] matches, bool isRootKey = false)
        {
            Matches = matches;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}