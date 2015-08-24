using System.Runtime.Serialization;

namespace xClient.Core.Data
{
    [DataContract]
    public class GeoInformation
    {
        [DataMember]
        public double longitude { get; set; }

        [DataMember]
        public double latitude { get; set; }

        [DataMember]
        public string asn { get; set; }

        [DataMember]
        public string offset { get; set; }

        [DataMember]
        public string ip { get; set; }

        [DataMember]
        public string area_code { get; set; }

        [DataMember]
        public string continent_code { get; set; }

        [DataMember]
        public string dma_code { get; set; }

        [DataMember]
        public string city { get; set; }

        [DataMember]
        public string timezone { get; set; }

        [DataMember]
        public string region { get; set; }

        [DataMember]
        public string country_code { get; set; }

        [DataMember]
        public string isp { get; set; }

        [DataMember]
        public string postal_code { get; set; }

        [DataMember]
        public string country { get; set; }

        [DataMember]
        public string country_code3 { get; set; }

        [DataMember]
        public string region_code { get; set; }
    }
}
