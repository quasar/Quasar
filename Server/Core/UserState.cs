using System.Drawing;
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

        public frmRemoteDesktop frmRDP { get; set; }
        public frmTaskManager frmTM { get; set; }
        public frmFileManager frmFM { get; set; }
        public frmSystemInformation frmSI { get; set; }
        public frmShowMessagebox frmSM { get; set; }
        public frmRemoteShell frmRS { get; set; }

        public bool isAuthenticated { get; set; }
        public bool lastDesktopSeen { get; set; }
        public bool lastDirectorySeen { get; set; }

        public Bitmap lastDesktop { get; set; }

        public UserState()
        {
            isAuthenticated = false;
            lastDesktopSeen = true;
            lastDirectorySeen = true;
        }
    }
}
