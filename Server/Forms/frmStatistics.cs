using System;
using System.Drawing;
using System.Windows.Forms;

namespace xRAT_2.Forms
{
    public partial class frmStatistics : Form
    {
        private long BytesReceived;
        private long BytesSent;
        private int ReceivedPercent;
        private int SentPercent;

        private int ConnectedClients;
        private int AllTimeConnectedClients;
        private int OfflineClients;
        private int ConnectedClientsPercent;
        private int AllTimePercent;
        private int OfflineClientsPercent;

        public frmStatistics(long received, long sent, int connected, int alltimeconnectedclients)
        {
            BytesReceived = received;
            BytesSent = sent;
            ConnectedClients = connected;
            AllTimeConnectedClients = alltimeconnectedclients;

            InitializeComponent();
        }

        private int calculate(long value, long sum)
        {
            if (sum != 0)
                return (int) (((float) value/(float) sum)*100);
            else
                return 0;
        }

        private int calculate(int value, int sum)
        {
            if (sum != 0)
                return (int) (((float) value/(float) sum)*100);
            else
                return 0;
        }

        private void frmStatistics_Load(object sender, EventArgs e)
        {
            long sum = BytesReceived + BytesSent;
            int received = calculate(BytesReceived, sum);
            int sent = calculate(BytesSent, sum);

            if (received + sent != 100)
                received += 1;

            if (received + sent != 100)
                received += 1;

            ReceivedPercent = received;
            SentPercent = sent;

            OfflineClients = AllTimeConnectedClients - ConnectedClients;
            int sumClients = ConnectedClients + AllTimeConnectedClients + OfflineClients;

            ConnectedClientsPercent = calculate(ConnectedClients, sumClients);
            AllTimePercent = calculate(AllTimeConnectedClients, sumClients);
            OfflineClientsPercent = calculate(OfflineClients, sumClients);
        }

        private void tabTraffic_Paint(object sender, PaintEventArgs e)
        {
            if (BytesReceived != 0 && BytesSent != 0)
            {
                int[] myPiePercents = {ReceivedPercent, SentPercent};

                Color[] PieColors = {Color.Green, Color.Blue};

                Size PieSize = new Size(150, 150);

                if (myPiePercents[0] + myPiePercents[1] != 100)
                    myPiePercents[0] += 2;
                else
                    myPiePercents[0] += 1;

                int sum = 0;
                foreach (int percent_loopVariable in myPiePercents)
                    sum += percent_loopVariable;

                int PiePercentTotal = 0;
                for (int PiePercents = 0; PiePercents < myPiePercents.Length; PiePercents++)
                {
                    using (SolidBrush brush = new SolidBrush(PieColors[PiePercents]))
                    {
                        e.Graphics.FillPie(brush, new Rectangle(new Point(25, 50), PieSize),
                            Convert.ToSingle(PiePercentTotal*360/100),
                            Convert.ToSingle(myPiePercents[PiePercents]*360/100));
                    }
                    PiePercentTotal += myPiePercents[PiePercents];
                }

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Green), 5), new Point(220, 130), new Point(250, 130));
                e.Graphics.DrawString(BytesReceived + " Bytes received (" + ReceivedPercent + "%)", this.Font,
                    new SolidBrush(Color.Black), new Point(260, 123));

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 5), new Point(220, 160), new Point(250, 160));
                e.Graphics.DrawString(BytesSent + " Bytes sent (" + SentPercent + "%)", this.Font,
                    new SolidBrush(Color.Black), new Point(260, 153));
            }
        }

        private void tabClients_Paint(object sender, PaintEventArgs e)
        {
            if (AllTimeConnectedClients != 0)
            {
                int[] myPiePercents = {ConnectedClientsPercent, AllTimePercent, OfflineClientsPercent};

                Color[] PieColors = {Color.Green, Color.Blue, Color.Red};

                Size PieSize = new Size(150, 150);

                if (myPiePercents[0] + myPiePercents[1] != 100)
                    myPiePercents[0] += 2;
                else
                    myPiePercents[0] += 1;

                int sum = 0;
                foreach (int percent_loopVariable in myPiePercents)
                    sum += percent_loopVariable;

                int PiePercentTotal = 0;
                for (int PiePercents = 0; PiePercents < myPiePercents.Length; PiePercents++)
                {
                    using (SolidBrush brush = new SolidBrush(PieColors[PiePercents]))
                    {
                        e.Graphics.FillPie(brush, new Rectangle(new Point(25, 50), PieSize),
                            Convert.ToSingle(PiePercentTotal*360/100),
                            Convert.ToSingle(myPiePercents[PiePercents]*360/100));
                    }
                    PiePercentTotal += myPiePercents[PiePercents];
                }

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Green), 5), new Point(220, 130), new Point(250, 130));
                e.Graphics.DrawString(ConnectedClients + " Connected Clients (" + ConnectedClientsPercent + "%)",
                    this.Font, new SolidBrush(Color.Black), new Point(260, 123));

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Blue), 5), new Point(220, 160), new Point(250, 160));
                e.Graphics.DrawString(
                    AllTimeConnectedClients + " All Time Connected Clients (" + AllTimePercent + "%)", this.Font,
                    new SolidBrush(Color.Black), new Point(260, 153));

                e.Graphics.DrawLine(new Pen(new SolidBrush(Color.Red), 5), new Point(220, 190), new Point(250, 190));
                e.Graphics.DrawString(OfflineClients + " Offline Clients (" + OfflineClientsPercent + "%)", this.Font,
                    new SolidBrush(Color.Black), new Point(260, 183));
            }
        }
    }
}