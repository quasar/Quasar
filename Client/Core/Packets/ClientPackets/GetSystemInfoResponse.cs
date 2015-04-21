using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetSystemInfoResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] SystemInfos { get; set; }

        public GetSystemInfoResponse()
        {
        }

        public GetSystemInfoResponse(string[] systeminfos)
        {
            this.SystemInfos = systeminfos;
        }

        public void Execute(Client client)
        {
            client.Send<GetSystemInfoResponse>(this);
        }
    }
}