using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetProcessesResponse : IPacket
    {
        [ProtoMember(1)]
        public string[] Processes { get; set; }

        [ProtoMember(2)]
        public int[] IDs { get; set; }

        [ProtoMember(3)]
        public string[] Titles { get; set; }

        public GetProcessesResponse()
        {
        }

        public GetProcessesResponse(string[] processes, int[] ids, string[] titles)
        {
            this.Processes = processes;
            this.IDs = ids;
            this.Titles = titles;
        }

        public void Execute(Client client)
        {
            client.Send<GetProcessesResponse>(this);
        }
    }
}