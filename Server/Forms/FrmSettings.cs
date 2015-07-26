using System;
using System.Globalization;
using System.Windows.Forms;
using xServer.Core.Misc;
using xServer.Core.Networking;
using xServer.Core.Networking.Utilities;
using xServer.Settings;

namespace xServer.Forms
{
    public partial class FrmSettings : Form
    {
        private readonly ConnectionHandler _listenServer;

        public FrmSettings(ConnectionHandler listenServer)
        {
            this._listenServer = listenServer;

            InitializeComponent();

            if (listenServer.Listening)
            {
                btnListen.Text = "Stop listening";
                ncPort.Enabled = false;
                txtPassword.Enabled = false;
            }

            ShowPassword(false);
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            ncPort.Value = XMLSettings.ListenPort;
            chkAutoListen.Checked = XMLSettings.AutoListen;
            chkPopup.Checked = XMLSettings.ShowPopup;
            txtPassword.Text = XMLSettings.Password;
            chkUseUpnp.Checked = XMLSettings.UseUPnP;
            chkShowTooltip.Checked = XMLSettings.ShowToolTip;
            chkNoIPIntegration.Checked = XMLSettings.IntegrateNoIP;
            txtNoIPHost.Text = XMLSettings.NoIPHost;
            txtNoIPUser.Text = XMLSettings.NoIPUsername;
            txtNoIPPass.Text = XMLSettings.NoIPPassword;
        }

        private ushort GetPortSafe()
        {
            var portValue = ncPort.Value.ToString(CultureInfo.InvariantCulture);
            ushort port;
            return (!ushort.TryParse(portValue, out port)) ? (ushort)0 : port;
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            ushort port = GetPortSafe();

            if (port == 0)
            {
                MessageBox.Show("Please enter a valid port > 0.", "Please enter a valid port", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (btnListen.Text == "Start listening" && !_listenServer.Listening)
            {
                try
                {
                    if (chkUseUpnp.Checked)
                    {
                        if (!UPnP.IsDeviceFound)
                        {
                            MessageBox.Show("No available UPnP device found!", "No UPnP device", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            if (!UPnP.CreatePortMap(port))
                            {
                                MessageBox.Show("Creating a port map with the UPnP device failed!\nPlease check if your device allows to create new port maps.", "Creating port map failed", MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                            }
                        }
                    }
                    if(chkNoIPIntegration.Checked)
                        NoIpUpdater.Start();
                    _listenServer.Listen(port);
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
                    UPnP.DeletePortMap(port);
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
            ushort port = GetPortSafe();

            if (port == 0)
            {
                MessageBox.Show("Please enter a valid port > 0.", "Please enter a valid port", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            XMLSettings.WriteValue("ListenPort", port.ToString());
            XMLSettings.ListenPort = port;

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

            XMLSettings.WriteValue("EnableNoIPUpdater", chkNoIPIntegration.Checked.ToString());
            XMLSettings.IntegrateNoIP = chkNoIPIntegration.Checked;

            XMLSettings.WriteValue("NoIPHost", txtNoIPHost.Text);
            XMLSettings.NoIPHost = txtNoIPHost.Text;

            XMLSettings.WriteValue("NoIPUsername", txtNoIPUser.Text);
            XMLSettings.NoIPUsername = txtNoIPUser.Text;

            XMLSettings.WriteValue("NoIPPassword", txtNoIPPass.Text);
            XMLSettings.NoIPPassword = txtNoIPPass.Text;

            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Discard your changes?", "Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) ==
                DialogResult.Yes)
                this.Close();
        }

        private void chkNoIPIntegration_CheckedChanged(object sender, EventArgs e)
        {
            NoIPControlHandler(chkNoIPIntegration.Checked);
        }

        private void NoIPControlHandler(bool enable)
        {
            lblHost.Enabled = enable;
            lblUser.Enabled = enable;
            lblPass.Enabled = enable;
            txtNoIPHost.Enabled = enable;
            txtNoIPUser.Enabled = enable;
            txtNoIPPass.Enabled = enable;
            chkShowPassword.Enabled = enable;
        }

        private void ShowPassword(bool show = true)
        {
            txtNoIPPass.PasswordChar = (show) ? (char)0 : (char)'●';
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            ShowPassword(chkShowPassword.Checked);
        }
    }
}