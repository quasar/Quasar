using System;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();

            lblVersion.Text = Settings.XMLSettings.VERSION;
            rtxtContent.Text = Properties.Resources.TermsOfUse;

            lblCredits.Text =
                "Credits: Banksy\n" +
                "              ResourceLib (Copyright (c) Daniel Doubrovkine, Vestris Inc., 2008-2013)\n" +
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