using System;
using System.Windows.Forms;

namespace xRAT_2.Forms
{
    public partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();

            lblVersion.Text = Settings.XMLSettings.VERSION;
            rtxtContent.Text = Properties.Resources.TermsOfUse;

            lblCredits.Text =
                "Credits: Banksy\n" +
                "              protobuf (Copyright 2008 Google Inc.)\n\n" +
                "Elevation Form Translators: Xenocode, Increment, DeadLine, Perfectionist,\n" +
                "                                               Qmz_, GameFire, navaro21";
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
