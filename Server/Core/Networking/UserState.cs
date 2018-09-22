namespace xServer.Core.Networking
{
    public class UserState
    {
        public string Version { get; set; }
        public string OperatingSystem { get; set; }
        public string AccountType { get; set; }
        public int ImageIndex { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Id { get; set; }
        public string Username { get; set; }
        public string PCName { get; set; }
        public string UserAtPc { get { return string.Format("{0}@{1}", Username, PCName); } }
        public string CountryWithCode { get { return string.Format("{0} [{1}]", Country, CountryCode); } }
        public string Tag { get; set; }
        public string DownloadDirectory { get; set; }
    }
}
