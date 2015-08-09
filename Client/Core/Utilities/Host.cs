namespace xClient.Core.Utilities
{
    public class Host
    {
        public string Hostname { get; set; } // Can be IP or Hostname
        public ushort Port { get; set; }

        public override string ToString()
        {
            return Hostname + ":" + Port;
        }
    }
}
