using System.Drawing;
using xServer.Core.Helper;
using xServer.Forms;

namespace xServer.Core
{
    public class UserState
    {
        public string Version { get; set; }
        public string OperatingSystem { get; set; }
        public string AccountType { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Id { get; set; }

        public FrmRemoteDesktop FrmRdp { get; set; }
        public FrmTaskManager FrmTm { get; set; }
        public FrmFileManager FrmFm { get; set; }
        public FrmSystemInformation FrmSi { get; set; }
        public FrmShowMessagebox FrmSm { get; set; }
        public FrmRemoteShell FrmRs { get; set; }
        public FrmStartupManager FrmStm { get; set; }

        public bool IsAuthenticated { get; set; }
        public bool LastDesktopSeen { get; set; }
        public bool LastDirectorySeen { get; set; }

        public Bitmap LastDesktop { get; set; }

        public UnsafeStreamCodec StreamCodec { get; set; }

        public UserState()
        {
            IsAuthenticated = false;
            LastDesktopSeen = true;
            LastDirectorySeen = true;
        }
    }
}
