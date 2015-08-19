using System;
using System.Diagnostics;
using System.Windows.Forms;
using xServer.Core.Data;

namespace xServer.Forms
{
    public partial class FrmAbout : Form
    {
        public FrmAbout()
        {
            InitializeComponent();

            lblVersion.Text = "v" + Application.ProductVersion;
            rtxtContent.Text = Properties.Resources.TermsOfUse;

            lnkGithubPage.Links.Add(new LinkLabel.Link { LinkData = Settings.RepositoryURL });
            lnkCredits.Links.Add(new LinkLabel.Link { LinkData = Settings.RepositoryURL + "#credits" });
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