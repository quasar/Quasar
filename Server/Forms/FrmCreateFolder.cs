using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class FrmCreateFolder : Form
    {

        public string FolderName { get; set; }
        public FrmCreateFolder()
        {
            InitializeComponent();
        }

        private void FrmCreateFolder_Load(object sender, EventArgs e)
        {

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            FolderName = txtFolderName.Text;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
