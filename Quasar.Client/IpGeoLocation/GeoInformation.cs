namespace Quasar.Client.IpGeoLocation
{
    /// <summary>
    /// Stores the IP geolocation information.
    /// </summary>
    public class GeoInformation
    {
        public string IpAddress { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Timezone { get; set; }
        public string Asn { get; set; }
        public string Isp { get; set; }
        public int ImageIndex { get; set; }
    }
}
