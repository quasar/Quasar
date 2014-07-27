using System;
using System.Windows.Forms;
using xRAT_2.Settings;

namespace xRAT_2.Forms
{
    public partial class frmSettings : Form
    {
        private Core.Server listenServer;

        public frmSettings(Core.Server listenServer)
        {
            this.listenServer = listenServer;

            InitializeComponent();

            if (listenServer.Listening)
            {
                btnListen.Text = "Stop listening";
                ncPort.Enabled = false;
                txtPassword.Enabled = false;
            }
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            ncPort.Value = XMLSettings.ListenPort;
            chkAutoListen.Checked = XMLSettings.AutoListen;
            chkPopup.Checked = XMLSettings.ShowPopup;
            txtPassword.Text = XMLSettings.Password;
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            if (btnListen.Text == "Start listening" && !listenServer.Listening)
            {
                listenServer.Listen(ushort.Parse(ncPort.Value.ToString()));
                btnListen.Text = "Stop listening";
                ncPort.Enabled = false;
                txtPassword.Enabled = false;
            }
            else if (btnListen.Text == "Stop listening" && listenServer.Listening)
            {
                listenServer.Disconnect();
                btnListen.Text = "Start listening";
                ncPort.Enabled = true;
                txtPassword.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            XMLSettings.WriteValue("ListenPort", ncPort.Value.ToString());
            XMLSettings.ListenPort = ushort.Parse(ncPort.Value.ToString());

            XMLSettings.WriteValue("AutoListen", chkAutoListen.Checked.ToString());
            XMLSettings.AutoListen = chkAutoListen.Checked;

            XMLSettings.WriteValue("ShowPopup", chkPopup.Checked.ToString());
            XMLSettings.ShowPopup = chkPopup.Checked;

            XMLSettings.WriteValue("Password", txtPassword.Text);
            XMLSettings.Password = txtPassword.Text;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Discard your changes?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                System.Windows.Forms.DialogResult.Yes)
                this.Close();
        }
    }
}