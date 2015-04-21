using System;
using System.Windows.Forms;
using xServer.Core;

namespace xServer.Forms
{
    public partial class FrmShowMessagebox : Form
    {
        private readonly Client _connectClient;

        public FrmShowMessagebox(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmSm = this;

            InitializeComponent();
        }

        private void FrmShowMessagebox_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
                this.Text = string.Format("xRAT 2.0 - Show Messagebox [{0}:{1}]",
                    _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());

            cmbMsgButtons.Items.AddRange(new string[]
            {"AbortRetryIgnore", "OK", "OKCancel", "RetryCancel", "YesNo", "YesNoCancel"});
            cmbMsgButtons.SelectedIndex = 0;
            cmbMsgIcon.Items.AddRange(new string[]
            {"None", "Error", "Hand", "Question", "Exclamation", "Warning", "Information", "Asterisk"});
            cmbMsgIcon.SelectedIndex = 0;
        }

        private void FrmShowMessagebox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmSm = null;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show(null, txtText.Text, txtCaption.Text,
                (MessageBoxButtons)
                    Enum.Parse(typeof (MessageBoxButtons), GetMessageBoxButton(cmbMsgButtons.SelectedIndex)),
                (MessageBoxIcon) Enum.Parse(typeof (MessageBoxIcon), GetMessageBoxIcon(cmbMsgIcon.SelectedIndex)));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            new Core.Packets.ServerPackets.ShowMessageBox(txtCaption.Text, txtText.Text,
                GetMessageBoxButton(cmbMsgButtons.SelectedIndex), GetMessageBoxIcon(cmbMsgIcon.SelectedIndex)).Execute(
                    _connectClient);
            this.Close();
        }

        private string GetMessageBoxButton(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    return "AbortRetryIgnore";
                case 1:
                    return "OK";
                case 2:
                    return "OKCancel";
                case 3:
                    return "RetryCancel";
                case 4:
                    return "YesNo";
                case 5:
                    return "YesNoCancel";
                default:
                    return "OK";
            }
        }

        private string GetMessageBoxIcon(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0:
                    return "None";
                case 1:
                    return "Error";
                case 2:
                    return "Hand";
                case 3:
                    return "Question";
                case 4:
                    return "Exclamation";
                case 5:
                    return "Warning";
                case 6:
                    return "Information";
                case 7:
                    return "Asterisk";
                default:
                    return "None";
            }
        }
    }
}