using System;

namespace Quasar.Client.IpGeoLocation
{
    /// <summary>
    /// Factory to retrieve and cache the last IP geolocation information for <see cref="MINIMUM_VALID_TIME"/> minutes.
    /// </summary>
    public static class GeoInformationFactory
    {
        /// <summary>
        /// Retriever used to get geolocation information about the WAN IP address.
        /// </summary>
        private static readonly GeoInformationRetriever Retriever = new GeoInformationRetriever();

        /// <summary>
        /// Used to cache the latest IP geolocation information.
        /// </summary>
        private static GeoInformation _geoInformation;

        /// <summary>
        /// Time of the last successful location retrieval.
        /// </summary>
        private static DateTime _lastSuccessfulLocation = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The minimum amount of minutes a successful IP geolocation retrieval is valid.
        /// </summary>
        private const int MINIMUM_VALID_TIME = 60 * 12;

        /// <summary>
        /// Gets the IP geolocation information, either cached or freshly retrieved if more than <see cref="MINIMUM_VALID_TIME"/> minutes have passed.
        /// </summary>
        /// <returns>The latest IP geolocation information.</returns>
        public static GeoInformation GetGeoInformation()
        {
            var passedTime = new TimeSpan(DateTime.UtcNow.Ticks - _lastSuccessfulLocation.Ticks);

            if (_geoInformation == null || passedTime.TotalMinutes > MINIMUM_VALID_TIME)
            {
                _geoInformation = Retriever.Retrieve();
                _lastSuccessfulLocation = DateTime.UtcNow;
            }

            return _geoInformation;
        }
    }
}
