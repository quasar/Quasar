using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Quasar.Server.Forms
{
    public partial class FrmAbout : Form
    {
        private readonly string _repositoryUrl = @"https://github.com/quasar/QuasarRAT";

        public FrmAbout()
        {
            InitializeComponent();

            lblVersion.Text = $"v{Application.ProductVersion}";
            rtxtContent.Text = Properties.Resources.License;

            lnkGithubPage.Links.Add(new LinkLabel.Link {LinkData = _repositoryUrl});
            lnkCredits.Links.Add(new LinkLabel.Link {LinkData = _repositoryUrl + "/tree/master/Licenses"});
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
