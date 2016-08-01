using System.Runtime.Serialization;

namespace xClient.Core.Data
{
    [DataContract]
    public class GeoInformation
    {
        [DataMember(Name = "as")]
        public string As { get; set; }
        [DataMember(Name = "city")]
        public string City { get; set; }
        [DataMember(Name = "country")]
        public string Country { get; set; }
        [DataMember(Name = "countryCode")]
        public string CountryCode { get; set; }
        [DataMember(Name = "isp")]
        public string Isp { get; set; }
        [DataMember(Name = "lat")]
        public double Lat { get; set; }
        [DataMember(Name = "lon")]
        public double Lon { get; set; }
        [DataMember(Name = "org")]
        public string Org { get; set; }
        [DataMember(Name = "query")]
        public string Ip { get; set; }
        [DataMember(Name = "region")]
        public string Region { get; set; }
        [DataMember(Name = "regionName")]
        public string RegionName { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }
        [DataMember(Name = "zip")]
        public string Zip { get; set; }
    }
}
