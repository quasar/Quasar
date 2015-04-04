using ProtoBuf;

namespace xClient.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetProcessesResponse : IPacket
    {
        public GetProcessesResponse()
        {
        }

        public GetProcessesResponse(string[] processes, int[] ids, string[] titles)
        {
            Processes = processes;
            IDs = ids;
            Titles = titles;
        }

        [ProtoMember(1)]
        public string[] Processes { get; set; }

        [ProtoMember(2)]
        public int[] IDs { get; set; }

        [ProtoMember(3)]
        public string[] Titles { get; set; }

        public void Execute(Client client)
        {
            client.Send<GetProcessesResponse>(this);
        }
    }
}