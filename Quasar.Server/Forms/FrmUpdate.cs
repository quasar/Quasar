using System;
using System.IO;
using System.Windows.Forms;
using Quasar.Server.Helper;

namespace Quasar.Server.Forms
{
    public partial class FrmUpdate : Form
    {
        public bool UseDownload { get; set; }
        public string UploadPath { get; set; }
        public string DownloadUrl { get; set; }

        private readonly int _selectedClients;

        public FrmUpdate(int selected)
        {
            _selectedClients = selected;
            InitializeComponent();
        }

        private void FrmUpdate_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Update Clients", _selectedClients);
            btnUpdate.Text = "Update Client" + ((_selectedClients > 1) ? "s" : string.Empty);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            UseDownload = radioURL.Checked;
            UploadPath = txtPath.Text;
            DownloadUrl = txtURL.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.Filter = "Executable (*.exe)|*.exe";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = Path.Combine(ofd.InitialDirectory, ofd.FileName);
                }
            }
        }

        private void radioLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            groupLocalFile.Enabled = radioLocalFile.Checked;
            groupURL.Enabled = !radioLocalFile.Checked;
        }

        private void radioURL_CheckedChanged(object sender, EventArgs e)
        {
            groupLocalFile.Enabled = !radioURL.Checked;
            groupURL.Enabled = radioURL.Checked;
        }

    }
}