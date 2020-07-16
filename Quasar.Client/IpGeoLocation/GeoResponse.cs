using System.Runtime.Serialization;

namespace Quasar.Client.IpGeoLocation
{
    [DataContract]
    public class GeoResponse
    {
        [DataMember(Name ="status")]
        public string Status { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "data")]
        public DataObject Data { get; set; }
    }

    [DataContract]
    public class DataObject
    {
        [DataMember(Name = "geo")]
        public LocationData Geo { get; set; }
    }

    [DataContract]
    public class LocationData
    {
        [DataMember(Name = "host")]
        public string Host;

        [DataMember(Name = "ip")]
        public string Ip;

        [DataMember(Name = "rdns")]
        public string Rdns;

        [DataMember(Name = "asn")]
        public int Asn;

        [DataMember(Name = "isp")]
        public string Isp;

        [DataMember(Name = "country_name")]
        public string CountryName;

        [DataMember(Name = "country_code")]
        public string CountryCode;

        [DataMember(Name = "region_name")]
        public string RegionName;

        [DataMember(Name = "region_code")]
        public string RegionCode;

        [DataMember(Name = "city")]
        public string City;

        [DataMember(Name = "postal_code")]
        public string PostalCode;

        [DataMember(Name = "continent_name")]
        public string ContinentName;

        [DataMember(Name = "continent_code")]
        public string ContinentCode;

        [DataMember(Name = "latitude")]
        public double Latitude;

        [DataMember(Name = "longitude")]
        public double Longitude;

        [DataMember(Name = "metro_code")]
        public object MetroCode;

        [DataMember(Name = "timezone")]
        public string Timezone;

        [DataMember(Name = "datetime")]
        public string Datetime;
    }
}
