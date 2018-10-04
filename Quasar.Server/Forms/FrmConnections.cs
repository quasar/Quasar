using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Networking;

namespace Quasar.Server.Forms
{
    public partial class FrmConnections : Form
    {
        /// <summary>
        /// The client which can be used for the connections manager.
        /// </summary>
        private readonly Client _connectClient;

        /// <summary>
        /// The message handler for handling the communication with the client.
        /// </summary>
        private readonly TcpConnectionsHandler _connectionsHandler;

        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<string, ListViewGroup> _groups = new Dictionary<string, ListViewGroup>();

        /// <summary>
        /// Holds the opened connections manager form for each client.
        /// </summary>
        private static readonly Dictionary<Client, FrmConnections> OpenedForms = new Dictionary<Client, FrmConnections>();

        /// <summary>
        /// Creates a new connections manager form for the client or gets the current open form, if there exists one already.
        /// </summary>
        /// <param name="client">The client used for the connections manager form.</param>
        /// <returns>
        /// Returns a new connections manager form for the client if there is none currently open, otherwise creates a new one.
        /// </returns>
        public static FrmConnections CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmConnections f = new FrmConnections(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmConnections"/> class using the given client.
        /// </summary>
        /// <param name="client">The client used for the connections manager form.</param>
        public FrmConnections(Client client)
        {
            _connectClient = client;
            _connectionsHandler = new TcpConnectionsHandler(client);

            RegisterMessageHandler();
            InitializeComponent();
        }

        /// <summary>
        /// Registers the connections manager message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            _connectClient.ClientState += ClientDisconnected;
            _connectionsHandler.ProgressChanged += TcpConnectionsChanged;
            MessageHandler.Register(_connectionsHandler);
        }

        /// <summary>
        /// Unregisters the connections manager message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_connectionsHandler);
            _connectionsHandler.ProgressChanged -= TcpConnectionsChanged;
            _connectClient.ClientState -= ClientDisconnected;
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

        /// <summary>
        /// Called whenever a TCP connection changed.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="connections">The current TCP connections of the client.</param>
        private void TcpConnectionsChanged(object sender, TcpConnection[] connections)
        {
            lstConnections.Items.Clear();

            foreach (var con in connections)
            {
                string state = con.State.ToString();

                ListViewItem lvi = new ListViewItem(new[]
                {
                    con.ProcessName, con.LocalAddress, con.LocalPort.ToString(),
                    con.RemoteAddress, con.RemotePort.ToString(), state
                });

                if (!_groups.ContainsKey(state))
                {
                    // create new group if not exists already
                    ListViewGroup g = new ListViewGroup(state, state);
                    lstConnections.Groups.Add(g);
                    _groups.Add(state, g);
                }

                lvi.Group = lstConnections.Groups[state];
                lstConnections.Items.Add(lvi);
            }
        }

        private void FrmConnections_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Connections", _connectClient);
            _connectionsHandler.RefreshTcpConnections();
        }

        private void FrmConnections_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
            _connectionsHandler.Dispose();
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _connectionsHandler.RefreshTcpConnections();
        }

        private void closeConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool modified = false;

            foreach (ListViewItem lvi in lstConnections.SelectedItems)
            {
                _connectionsHandler.CloseTcpConnection(lvi.SubItems[1].Text, ushort.Parse(lvi.SubItems[2].Text),
                    lvi.SubItems[3].Text, ushort.Parse(lvi.SubItems[4].Text));
                modified = true;
            }

            if (modified)
            {
                _connectionsHandler.RefreshTcpConnections();
            }
        }
    }
}
