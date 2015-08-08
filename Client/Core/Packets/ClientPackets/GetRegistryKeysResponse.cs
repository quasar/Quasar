using ProtoBuf;
using xClient.Core.Networking;
using xClient.Core.Utilities;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetRegistryKeysResponse : IPacket
    {
        [ProtoMember(1)]
        public object Identifier { get; set; }

        [ProtoMember(2)]
        public RegistryKeyEx[] RegKeys { get; set; }

        public GetRegistryKeysResponse()
            : this(new RegistryKeyEx[0])
        { }

        public GetRegistryKeysResponse(RegistryKeyEx[] regKeys, object identifier = null)
        {
            Identifier = identifier;
            RegKeys = regKeys;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}