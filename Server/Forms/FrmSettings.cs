using System;
using System.Globalization;
using System.Windows.Forms;
using xServer.Core;
using xServer.Settings;

namespace xServer.Forms
{
    public partial class FrmSettings : Form
    {
        private readonly Server _listenServer;

        public FrmSettings(Server listenServer)
        {
            this._listenServer = listenServer;

            InitializeComponent();

            if (listenServer.Listening)
            {
                btnListen.Text = "Stop listening";
                ncPort.Enabled = false;
                txtPassword.Enabled = false;
            }
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            ncPort.Value = XMLSettings.ListenPort;
            chkAutoListen.Checked = XMLSettings.AutoListen;
            chkPopup.Checked = XMLSettings.ShowPopup;
            txtPassword.Text = XMLSettings.Password;
            chkUseUpnp.Checked = XMLSettings.UseUPnP;
            chkShowTooltip.Checked = XMLSettings.ShowToolTip;
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            if (btnListen.Text == "Start listening" && !_listenServer.Listening)
            {
                try
                {
                    if (chkUseUpnp.Checked)
                        Core.Helper.UPnP.ForwardPort(ushort.Parse(ncPort.Value.ToString(CultureInfo.InvariantCulture)));
                    _listenServer.Listen(ushort.Parse(ncPort.Value.ToString(CultureInfo.InvariantCulture)));
                }
                finally
                {
                    btnListen.Text = "Stop listening";
                    ncPort.Enabled = false;
                    txtPassword.Enabled = false;
                }
            }
            else if (btnListen.Text == "Stop listening" && _listenServer.Listening)
            {
                try
                {
                    _listenServer.Disconnect();
                }
                finally
                {
                    btnListen.Text = "Start listening";
                    ncPort.Enabled = true;
                    txtPassword.Enabled = true;
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            XMLSettings.WriteValue("ListenPort", ncPort.Value.ToString(CultureInfo.InvariantCulture));
            XMLSettings.ListenPort = ushort.Parse(ncPort.Value.ToString(CultureInfo.InvariantCulture));

            XMLSettings.WriteValue("AutoListen", chkAutoListen.Checked.ToString());
            XMLSettings.AutoListen = chkAutoListen.Checked;

            XMLSettings.WriteValue("ShowPopup", chkPopup.Checked.ToString());
            XMLSettings.ShowPopup = chkPopup.Checked;

            XMLSettings.WriteValue("Password", txtPassword.Text);
            XMLSettings.Password = txtPassword.Text;

            XMLSettings.WriteValue("UseUPnP", chkUseUpnp.Checked.ToString());
            XMLSettings.UseUPnP = chkUseUpnp.Checked;

            XMLSettings.WriteValue("ShowToolTip", chkShowTooltip.Checked.ToString());
            XMLSettings.ShowToolTip = chkShowTooltip.Checked;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Discard your changes?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
                this.Close();
        }
    }
}