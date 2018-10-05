using System;
using System.Diagnostics;
using System.Windows.Forms;
using Quasar.Server.Data;

namespace Quasar.Server.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();

            lblVersion.Text = $"v{Application.ProductVersion}";
            rtxtContent.Text = Properties.Resources.License;

            lnkGithubPage.Links.Add(new LinkLabel.Link {LinkData = Settings.RepositoryURL});
            lnkCredits.Links.Add(new LinkLabel.Link {LinkData = Settings.RepositoryURL + "/tree/master/Licenses"});
        }
        
        private void lnkGithubPage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkGithubPage.LinkVisited = true;
            Process.Start(e.Link.LinkData.ToString());
        }

        private void lnkCredits_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            lnkCredits.LinkVisited = true;
            Process.Start(e.Link.LinkData.ToString());
        }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}