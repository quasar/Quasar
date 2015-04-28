using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using xServer.Core.Helper;

namespace xServer.Forms
{
    public partial class FrmStatistics : Form
    {
        private readonly long _bytesReceived;
        private readonly long _bytesSent;
        private int _receivedPercent;
        private int _sentPercent;

        private readonly int _connectedClients;
        private readonly int _allTimeConnectedClients;
        private int _offlineClients;
        private int _connectedClientsPercent;
        private int _allTimePercent;
        private int _offlineClientsPercent;

        public FrmStatistics(long received, long sent, int connected, int alltimeconnectedclients)
        {
            _bytesReceived = received;
            _bytesSent = sent;
            _connectedClients = connected;
            _allTimeConnectedClients = alltimeconnectedclients;

            InitializeComponent();
        }

        private void FrmStatistics_Load(object sender, EventArgs e)
        {
            _receivedPercent = CalculatePercentage(_bytesReceived, _bytesReceived + _bytesSent);
            _sentPercent = CalculatePercentage(_bytesSent, _bytesReceived + _bytesSent);

            _offlineClients = _allTimeConnectedClients - _connectedClients;
            int sumClients = _connectedClients + _allTimeConnectedClients + _offlineClients;

            _connectedClientsPercent = CalculatePercentage(_connectedClients, sumClients);
            _allTimePercent = CalculatePercentage(_allTimeConnectedClients, sumClients);
            _offlineClientsPercent = CalculatePercentage(_offlineClients, sumClients);
        }

        private void tabTraffic_Paint(object sender, PaintEventArgs e)
        {
            DrawPieChartTraffic(new float[] {_bytesReceived, _bytesSent});

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Green), 5), new Point(220, 130), new Point(250, 130));
            e.Graphics.DrawString(
                string.Format("{0} received ({1}%)", Helper.GetFileSize(_bytesReceived), _receivedPercent),
                this.Font, new SolidBrush(Color.Black), new Point(260, 123));

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 5), new Point(220, 160), new Point(250, 160));
            e.Graphics.DrawString(string.Format("{0} sent ({1}%)", Helper.GetFileSize(_bytesSent), _sentPercent),
                this.Font, new SolidBrush(Color.Black), new Point(260, 153));
        }

        private void tabClients_Paint(object sender, PaintEventArgs e)
        {
            DrawPieChartClients(new float[] {_connectedClients, _allTimeConnectedClients, _offlineClients});

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Green), 5), new Point(220, 130), new Point(250, 130));
            e.Graphics.DrawString(
                string.Format("{0} Connected Clients ({1}%)", _connectedClients, _connectedClientsPercent),
                this.Font, new SolidBrush(Color.Black), new Point(260, 123));

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 5), new Point(220, 160), new Point(250, 160));
            e.Graphics.DrawString(
                string.Format("{0} All Time Connected Clients ({1}%)", _allTimeConnectedClients, _allTimePercent),
                this.Font, new SolidBrush(Color.Black), new Point(260, 153));

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red), 5), new Point(220, 190), new Point(250, 190));
            e.Graphics.DrawString(string.Format("{0} Offline Clients ({1}%)", _offlineClients, _offlineClientsPercent),
                this.Font, new SolidBrush(Color.Black), new Point(260, 183));
        }

        private void DrawPieChartTraffic(float[] values)
        {
            tabTraffic.Invoke((MethodInvoker) delegate
            {
                using (var g = tabTraffic.CreateGraphics())
                using (var p = new Pen(Color.Black, 1))
                {
                    var rec = new Rectangle(25, 50, 150, 150);
                    float total = 0;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    foreach (var value in values)
                        total += value;
                    for (int i = 0; i < values.Length; i++)
                        values[i] = (values[i]/total)*360;

                    using (SolidBrush b1 = new SolidBrush(Color.Green), b2 = new SolidBrush(Color.Blue))
                    {
                        g.DrawPie(p, rec, 0, values[0]);
                        g.FillPie(b1, rec, 0, values[0]);
                        g.DrawPie(p, rec, values[0], values[1]);
                        g.FillPie(b2, rec, values[0], values[1]);
                    }
                }
            });
        }

        private void DrawPieChartClients(float[] values)
        {
            tabClients.Invoke((MethodInvoker) delegate
            {
                using (var g = tabClients.CreateGraphics())
                using (var p = new Pen(Color.Black, 1))
                {
                    var rec = new Rectangle(25, 50, 150, 150);
                    float total = 0;

                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.CompositingQuality = CompositingQuality.HighQuality;

                    foreach (var value in values)
                        total += value;
                    for (int i = 0; i < values.Length; i++)
                        values[i] = (values[i]/total)*360;

                    using (
                        SolidBrush b1 = new SolidBrush(Color.Green),
                            b2 = new SolidBrush(Color.Blue),
                            b3 = new SolidBrush(Color.Red))
                    {
                        g.DrawPie(p, rec, 0, values[0]);
                        g.FillPie(b1, rec, 0, values[0]);
                        g.DrawPie(p, rec, values[0], values[1]);
                        g.FillPie(b2, rec, values[0], values[1]);
                        g.DrawPie(p, rec, values[1] + values[0], values[2]);
                        g.FillPie(b3, rec, values[1] + values[0], values[2]);
                    }
                }
            });
        }

        private int CalculatePercentage(float value, float sum)
        {
            return (sum != 0) ? (int) ((value/sum)*100) : 0;
        }
    }
}