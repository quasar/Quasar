using Quasar.Common.Helpers;
using System.IO;
using System.Windows.Forms;

namespace Quasar.Server.Networking
{
    public class UserState
    {
        private string _downloadDirectory;

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
        public string PcName { get; set; }
        public string UserAtPc => $"{Username}@{PcName}";
        public string CountryWithCode => $"{Country} [{CountryCode}]";
        public string Tag { get; set; }
        public string EncryptionKey { get; set; }

        public string DownloadDirectory => _downloadDirectory ?? (_downloadDirectory = (!FileHelper.HasIllegalCharacters(UserAtPc))
                                               ? Path.Combine(Application.StartupPath, $"Clients\\{UserAtPc}_{Id.Substring(0, 7)}\\")
                                               : Path.Combine(Application.StartupPath, $"Clients\\{Id}\\"));
    }
}
