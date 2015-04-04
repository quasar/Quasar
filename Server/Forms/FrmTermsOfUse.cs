using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using xServer.Properties;
using xServer.Settings;

namespace xServer.Forms
{
    public partial class FrmTermsOfUse : Form
    {
        private static bool _exit = true;

        public FrmTermsOfUse()
        {
            InitializeComponent();
            rtxtContent.Text = Resources.TermsOfUse;
        }

        private void FrmTermsOfUse_Load(object sender, EventArgs e)
        {
            lblToU.Left = (Width/2) - (lblToU.Width/2);
            var t = new Thread(Wait20Sec);
            t.Start();
        }

        private void FrmTermsOfUse_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _exit)
                Process.GetCurrentProcess().Kill();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            XMLSettings.WriteValue("ShowToU", (!chkDontShowAgain.Checked).ToString());
            _exit = false;
            Close();
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Wait20Sec()
        {
            for (var i = 19; i >= 0; i--)
            {
                Thread.Sleep(1000);
                try
                {
                    Invoke((MethodInvoker) delegate { btnAccept.Text = "Accept (" + i + ")"; });
                }
                catch
                {
                }
            }

            Invoke((MethodInvoker) delegate
            {
                btnAccept.Text = "Accept";
                btnAccept.Enabled = true;
            });
        }
    }
}