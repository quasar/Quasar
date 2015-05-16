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

        [ProtoMember(4)]
        public long[] MemoryUsed { get; set; }

        public GetProcessesResponse()
        {
        }

        public GetProcessesResponse(string[] processes, int[] ids, string[] titles, long[] memoryUsed)
        {
            this.Processes = processes;
            this.IDs = ids;
            this.Titles = titles;
            this.MemoryUsed = memoryUsed;
        }

        public void Execute(Client client)
        {
            client.Send<GetProcessesResponse>(this);
        }
    }
}