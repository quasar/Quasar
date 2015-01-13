using System;
using System.Drawing;
using System.Windows.Forms;

namespace xServer.Forms
{
    public partial class frmStatistics : Form
    {
        long BytesReceived;
        long BytesSent;
        int ReceivedPercent;
        int SentPercent;

        int ConnectedClients;
        int AllTimeConnectedClients;
        int OfflineClients;
        int ConnectedClientsPercent;
        int AllTimePercent;
        int OfflineClientsPercent;

        public frmStatistics(long received, long sent, int connected, int alltimeconnectedclients)
        {
            BytesReceived = received;
            BytesSent = sent;
            ConnectedClients = connected;
            AllTimeConnectedClients = alltimeconnectedclients;

            InitializeComponent();
        }

        private void frmStatistics_Load(object sender, EventArgs e)
        {
            ReceivedPercent = CalculatePercentage(BytesReceived, BytesReceived + BytesSent);
            SentPercent = CalculatePercentage(BytesSent, BytesReceived + BytesSent);

            OfflineClients = AllTimeConnectedClients - ConnectedClients;
            int sumClients = ConnectedClients + AllTimeConnectedClients + OfflineClients;

            ConnectedClientsPercent = CalculatePercentage(ConnectedClients, sumClients);
            AllTimePercent = CalculatePercentage(AllTimeConnectedClients, sumClients);
            OfflineClientsPercent = CalculatePercentage(OfflineClients, sumClients);
        }

        private void tabTraffic_Paint(object sender, PaintEventArgs e)
        {
            DrawPieChartTraffic(new float[] { BytesReceived, BytesSent });

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Green), 5), new Point(220, 130), new Point(250, 130));
            e.Graphics.DrawString(BytesReceived + " Bytes received (" + ReceivedPercent + "%)", this.Font, new SolidBrush(Color.Black), new Point(260, 123));

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 5), new Point(220, 160), new Point(250, 160));
            e.Graphics.DrawString(BytesSent + " Bytes sent (" + SentPercent + "%)", this.Font, new SolidBrush(Color.Black), new Point(260, 153));
        }

        private void tabClients_Paint(object sender, PaintEventArgs e)
        {
            DrawPieChartClients(new float[] { ConnectedClients, AllTimeConnectedClients, OfflineClients });

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Green), 5), new Point(220, 130), new Point(250, 130));
            e.Graphics.DrawString(ConnectedClients + " Connected Clients (" + ConnectedClientsPercent + "%)", this.Font, new SolidBrush(Color.Black), new Point(260, 123));

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 5), new Point(220, 160), new Point(250, 160));
            e.Graphics.DrawString(AllTimeConnectedClients + " All Time Connected Clients (" + AllTimePercent + "%)", this.Font, new SolidBrush(Color.Black), new Point(260, 153));

            e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red), 5), new Point(220, 190), new Point(250, 190));
            e.Graphics.DrawString(OfflineClients + " Offline Clients (" + OfflineClientsPercent + "%)", this.Font, new SolidBrush(Color.Black), new Point(260, 183));
        }

        private void DrawPieChartTraffic(float [] values)
        {
            tabTraffic.Invoke((MethodInvoker)delegate
            {
                var p = new Pen(Color.Black, 1);
                var g = tabTraffic.CreateGraphics();
                var rec = new Rectangle(25, 50, 150, 150);
                float total = 0;

                foreach (var value in values)
                    total += value;
                for (int i = 0; i < values.Length; i++)
                    values[i] = (values[i] / total) * 360;

                var b1 = new SolidBrush(Color.Green);
                var b2 = new SolidBrush(Color.Blue);

                g.DrawPie(p, rec, 0, values[0]);
                g.FillPie(b1, rec, 0, values[0]);
                g.DrawPie(p, rec, values[0], values[1]);
                g.FillPie(b2, rec, values[0], values[1]);

                b1.Dispose();
                b2.Dispose();
            });
        }

        private void DrawPieChartClients(float[] values)
        {
            tabClients.Invoke((MethodInvoker)delegate
            {
                var p = new Pen(Color.Black, 1);
                var g = tabClients.CreateGraphics();
                var rec = new Rectangle(25, 50, 150, 150);
                float total = 0;

                foreach (var value in values)
                    total += value;
                for (int i = 0; i < values.Length; i++)
                    values[i] = (values[i] / total) * 360;

                var b1 = new SolidBrush(Color.Green);
                var b2 = new SolidBrush(Color.Blue);
                var b3 = new SolidBrush(Color.Red);

                g.DrawPie(p, rec, 0, values[0]);
                g.FillPie(b1, rec, 0, values[0]);
                g.DrawPie(p, rec, values[0], values[1]);
                g.FillPie(b2, rec, values[0], values[1]);
                g.DrawPie(p, rec, values[1] + values[0], values[2]);
                g.FillPie(b3, rec, values[1] + values[0], values[2]);

                b1.Dispose();
                b2.Dispose();
                b3.Dispose();
            });
        }

        private int CalculatePercentage(float value, float sum)
        {
            return (sum != 0) ? (int)((value / sum) * 100) : 0;
        }
    }
}
