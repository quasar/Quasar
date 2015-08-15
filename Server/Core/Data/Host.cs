namespace xServer.Core.Data
{
    public class Host
    {
        /// <summary>
        /// Stores the hostname of the Host.
        /// </summary>
        /// <remarks>
        /// Can be an IPv4 address or hostname. IPv6 support not tested.
        /// </remarks>
        public string Hostname { get; set; }

        /// <summary>
        /// Stores the port of the Host.
        /// </summary>
        public ushort Port { get; set; }

        public override string ToString()
        {
            return Hostname + ":" + Port;
        }
    }
}
