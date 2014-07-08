using ProtoBuf;

namespace Core.Packets.ClientPackets
{
    [ProtoContract]
    public class GetSystemInfoResponse : IPacket
    {
        [ProtoMember(1)]
        public string CPU { get; set; }

        [ProtoMember(2)]
        public int RAM { get; set; }

        [ProtoMember(3)]
        public string GPU { get; set; }

        [ProtoMember(4)]
        public string Username { get; set; }

        [ProtoMember(5)]
        public string PCName { get; set; }

        [ProtoMember(6)]
        public string Uptime { get; set; }

        [ProtoMember(7)]
        public string LAN { get; set; }

        [ProtoMember(8)]
        public string WAN { get; set; }

        public GetSystemInfoResponse() { }
        public GetSystemInfoResponse(string cpu, int ram, string gpu, string username, string pcname, string uptime, string lan, string wan)
        {
            this.CPU = cpu;
            this.RAM = ram;
            this.GPU = gpu;
            this.Username = username;
            this.PCName = pcname;
            this.Uptime = uptime;
            this.LAN = lan;
            this.WAN = wan;
        }

        public void Execute(Client client)
        {
            client.Send<GetSystemInfoResponse>(this);
        }
    }
}