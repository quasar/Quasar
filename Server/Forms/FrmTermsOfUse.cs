using System;
using System.Threading;
using System.Windows.Forms;
using xServer.Core.Data;

namespace xServer.Forms
{
    public partial class FrmTermsOfUse : Form
    {
        private static bool _exit = true;

        public FrmTermsOfUse()
        {
            InitializeComponent();
            rtxtContent.Text = Properties.Resources.TermsOfUse;
        }

        private void FrmTermsOfUse_Load(object sender, EventArgs e)
        {
            lblToU.Left = (this.Width/2) - (lblToU.Width/2);
            Thread t = new Thread(Wait20Sec) {IsBackground = true};
            t.Start();
        }

        private void FrmTermsOfUse_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _exit)
                System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            var showToU = !chkDontShowAgain.Checked;
            Settings.ShowToU = showToU;
            _exit = false;
            this.Close();
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void Wait20Sec()
        {
            for (int i = 19; i >= 0; i--)
            {
                Thread.Sleep(1000);
                try
                {
                    this.Invoke((MethodInvoker) delegate { btnAccept.Text = "Accept (" + i + ")"; });
                }
                catch
                {
                }
            }

            this.Invoke((MethodInvoker) delegate
            {
                btnAccept.Text = "Accept";
                btnAccept.Enabled = true;
            });
        }
    }
}