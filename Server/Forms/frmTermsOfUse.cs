using System;
using System.Threading;
using System.Windows.Forms;
using xRAT_2.Settings;

namespace xRAT_2.Forms
{
    public partial class frmTermsOfUse : Form
    {
        public frmTermsOfUse()
        {
            InitializeComponent();
            rtxtContent.Text = Properties.Resources.TermsOfUse;
        }

        private static bool exit = true;

        private void btnAccept_Click(object sender, EventArgs e)
        {
            XMLSettings.WriteValue("ShowToU", (!chkDontShowAgain.Checked).ToString());
            exit = false;
            this.Close();
        }

        private void frmTermsOfUse_Load(object sender, EventArgs e)
        {
            lblToU.Left = (this.Width / 2) - (lblToU.Width / 2);
            Thread t = new Thread(Wait20Sec);
            t.Start();
        }

        private void Wait20Sec()
        {
            for (int i = 19; i >= 0; i--)
            {
                System.Threading.Thread.Sleep(1000);
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnAccept.Text = "Accept (" + i + ")";
                    });
                }
                catch
                { }
            }

            this.Invoke((MethodInvoker)delegate
            {
                btnAccept.Text = "Accept";
                btnAccept.Enabled = true;
            });
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        private void frmTermsOfUse_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && exit)
                System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}
