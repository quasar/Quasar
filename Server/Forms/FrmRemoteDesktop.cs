using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Packets.ClientPackets;
using xServer.Core.Utilities;
using xServer.Enums;

namespace xServer.Forms
{
    public partial class FrmRemoteDesktop : Form
    {
        private readonly Client _connectClient;
        private bool _enableMouseInput;
        private bool _started;
        private FrameCounter _frameCounter;
        private Stopwatch _sWatch;
        private int _screenWidth;
        private int _screenHeight;

        public readonly Queue<GetDesktopResponse> ProcessingScreensQueue = new Queue<GetDesktopResponse>();
        public readonly object ProcessingScreensLock = new object();
        public bool ProcessingScreens;

        public FrmRemoteDesktop(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRdp = this;
            InitializeComponent();
        }

        private void FrmRemoteDesktop_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Remote Desktop", _connectClient);

            panelTop.Left = (this.Width/2) - (panelTop.Width/2);

            btnHide.Left = (panelTop.Width/2) - (btnHide.Width/2);

            btnShow.Location = new Point(377, 0);
            btnShow.Left = (this.Width/2) - (btnShow.Width/2);

            if (_connectClient.Value != null)
                new Core.Packets.ServerPackets.GetMonitors().Execute(_connectClient);
        }

        public void ProcessScreens(object state)
        {
            while (true)
            {
                GetDesktopResponse packet;
                lock (ProcessingScreensQueue)
                {
                    if (ProcessingScreensQueue.Count == 0)
                    {
                        lock (ProcessingScreensLock)
                        {
                            ProcessingScreens = false;
                        }
                        return;
                    }

                    packet = ProcessingScreensQueue.Dequeue();
                }

                if (_connectClient.Value.StreamCodec == null)
                    _connectClient.Value.StreamCodec = new UnsafeStreamCodec(packet.Quality, packet.Monitor, packet.Resolution);

                if (_connectClient.Value.StreamCodec.ImageQuality != packet.Quality || _connectClient.Value.StreamCodec.Monitor != packet.Monitor
                    || _connectClient.Value.StreamCodec.Resolution != packet.Resolution)
                {
                    if (_connectClient.Value.StreamCodec != null)
                        _connectClient.Value.StreamCodec.Dispose();

                    _connectClient.Value.StreamCodec = new UnsafeStreamCodec(packet.Quality, packet.Monitor, packet.Resolution);
                }

                using (MemoryStream ms = new MemoryStream(packet.Image))
                {
                    if (_connectClient.Value.FrmRdp != null)
                        _connectClient.Value.FrmRdp.UpdateImage(_connectClient.Value.StreamCodec.DecodeData(ms), true);
                }

                packet.Image = null;
            }
        }

        public void AddMonitors(int montiors)
        {
            try
            {
                cbMonitors.Invoke((MethodInvoker) delegate
                {
                    for (int i = 0; i < montiors; i++)
                        cbMonitors.Items.Add(string.Format("Monitor {0}", i + 1));
                    cbMonitors.SelectedIndex = 0;
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CountFps()
        {
            var deltaTime = (float)_sWatch.Elapsed.TotalSeconds;
            _sWatch = Stopwatch.StartNew();

            _frameCounter.Update(deltaTime);

            UpdateFps(_frameCounter.AverageFramesPerSecond);
        }

        private void UpdateFps(float fps)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Text = string.Format("{0} - FPS: {1}", WindowHelper.GetWindowTitle("Remote Desktop", _connectClient), fps.ToString("0.00"));
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void UpdateScreenResolution(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }

        public void UpdateImage(Bitmap bmp, bool cloneBitmap = false)
        {
            try
            {
                CountFps();
                UpdateScreenResolution(bmp.Width, bmp.Height);
                picDesktop.Invoke((MethodInvoker) delegate
                {
                    // get old image to dispose it correctly
                    var oldImage = picDesktop.Image;

                    picDesktop.SuspendLayout();
                    picDesktop.Image = cloneBitmap ? new Bitmap(bmp, picDesktop.Width, picDesktop.Height) /*resize bitmap*/ : bmp;
                    picDesktop.ResumeLayout();

                    if (oldImage != null)
                        oldImage.Dispose();
                });
            }
            catch (InvalidOperationException)
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format(
                        "An unexpected error occurred: {0}\n\nPlease report this as fast as possible here:\\https://github.com/MaxXor/xRAT/issues",
                        ex.Message), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ToggleControls(bool t)
        {
            _started = !t;
            try
            {
                this.Invoke((MethodInvoker) delegate
                {
                    btnStart.Enabled = t;
                    btnStop.Enabled = !t;
                    barQuality.Enabled = t;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void FrmRemoteDesktop_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_started)
                new Core.Packets.ServerPackets.GetDesktop(0, 0, RemoteDesktopAction.Stop).Execute(_connectClient);
            if (_sWatch != null)
                _sWatch.Stop();
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRdp = null;
        }

        private void FrmRemoteDesktop_Resize(object sender, EventArgs e)
        {
            panelTop.Left = (this.Width/2) - (panelTop.Width/2);
            btnShow.Left = (this.Width/2) - (btnShow.Width/2);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cbMonitors.Items.Count == 0)
            {
                MessageBox.Show("No monitor detected.\nPlease wait till the client sends a list with available monitors.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _frameCounter = new FrameCounter();
            _sWatch = Stopwatch.StartNew();

            ToggleControls(false);

            new Core.Packets.ServerPackets.GetDesktop(barQuality.Value, cbMonitors.SelectedIndex, RemoteDesktopAction.Start).Execute(_connectClient);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            new Core.Packets.ServerPackets.GetDesktop(0, 0, RemoteDesktopAction.Stop).Execute(_connectClient);
            ToggleControls(true);
            _sWatch.Stop();
        }

        private void barQuality_Scroll(object sender, EventArgs e)
        {
            int value = barQuality.Value;
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
                this.picDesktop.Cursor = Cursors.Default;
                btnMouse.Image = Properties.Resources.mouse_delete;
                _enableMouseInput = false;
            }
            else
            {
                this.picDesktop.Cursor = Cursors.Hand;
                btnMouse.Image = Properties.Resources.mouse_add;
                _enableMouseInput = true;
            }
        }

        private int GetRemoteWidth(int localX)
        {
            return localX * _screenWidth / picDesktop.Width;
        }

        private int GetRemoteHeight(int localY)
        {
            return localY * _screenHeight / picDesktop.Height;
        }

        private void picDesktop_MouseDown(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && _enableMouseInput && !btnStart.Enabled)
            {
                int local_x = e.X;
                int local_y = e.Y;

                int remote_x = GetRemoteWidth(local_x);
                int remote_y = GetRemoteHeight(local_y);

                MouseAction action = MouseAction.None;

                if (e.Button == MouseButtons.Left)
                    action = MouseAction.LeftDown;
                if (e.Button == MouseButtons.Right)
                    action = MouseAction.RightDown;

                int selectedMonitorIndex = cbMonitors.SelectedIndex;

                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoMouseEvent(action, true, remote_x, remote_y, selectedMonitorIndex).Execute(_connectClient);
            }
        }

        private void picDesktop_MouseUp(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && _enableMouseInput && !btnStart.Enabled)
            {
                int local_x = e.X;
                int local_y = e.Y;

                int remote_x = GetRemoteWidth(local_x);
                int remote_y = GetRemoteHeight(local_y);

                MouseAction action = MouseAction.None;

                if (e.Button == MouseButtons.Left)
                    action = MouseAction.LeftDown;
                if (e.Button == MouseButtons.Right)
                    action = MouseAction.RightDown;

                int selectedMonitorIndex = cbMonitors.SelectedIndex;

                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoMouseEvent(action, false, remote_x, remote_y, selectedMonitorIndex).Execute(_connectClient);
            }
        }

        private void picDesktop_MouseMove(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && _enableMouseInput && !btnStart.Enabled)
            {
                int local_x = e.X;
                int local_y = e.Y;

                int remote_x = GetRemoteWidth(local_x);
                int remote_y = GetRemoteHeight(local_y);

                int selectedMonitorIndex = cbMonitors.SelectedIndex;

                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoMouseEvent(MouseAction.MoveCursor, false, remote_x, remote_y, selectedMonitorIndex).Execute(_connectClient);
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