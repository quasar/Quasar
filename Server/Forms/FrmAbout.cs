using System;
using System.Windows.Forms;
using xServer.Properties;
using xServer.Settings;

namespace xServer.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();

            lblVersion.Text = XMLSettings.VERSION;
            rtxtContent.Text = Resources.TermsOfUse;

            lblCredits.Text =
                "Credits: Banksy\n" +
                "              ResourceLib (Copyright (c) Daniel Doubrovkine, Vestris Inc., 2008-2013)\n" +
                "              protobuf (Copyright 2008 Google Inc.)\n\n" +
                "Elevation Form Translators: Xenocode, Increment, DeadLine, Perfectionist,\n" +
                "                                               Qmz_, GameFire, navaro21";
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}