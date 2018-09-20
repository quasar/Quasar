using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Quasar.Common.Messages;
using Quasar.Common.Video;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmRemoteWebcam : Form
    {
        public bool IsStarted { get; private set; }
        private readonly Client _connectClient;
        private Dictionary<string, List<Resolution>> _webcams;

        public FrmRemoteWebcam(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmWebcam = this;

            InitializeComponent();
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            panelTop.Visible = true;
            btnShow.Visible = false;
            btnHide.Visible = true;
            this.ActiveControl = picWebcam;
        }

        private void btnHide_Click(object sender, EventArgs e)
        {
            panelTop.Visible = false;
            btnShow.Visible = true;
            btnHide.Visible = false;
            this.ActiveControl = picWebcam;
        }

        private void FrmRemoteWebcam_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Remote Webcam", _connectClient);

            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);

            btnHide.Left = (panelTop.Width / 2) - (btnHide.Width / 2);

            btnShow.Location = new Point(377, 0);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);

            if (_connectClient.Value != null)
                _connectClient.Send(new GetWebcams());
        }
        public void AddWebcams(Dictionary<string, List<Resolution>> webcams)
        {
            this._webcams = webcams;
            try
            {
                cbWebcams.Invoke((MethodInvoker)delegate
                {
                    foreach (var webcam in webcams.Keys)
                    {
                        cbWebcams.Items.Add(webcam);
                    }
                    cbWebcams.SelectedIndex = 0;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }
        public void UpdateImage(Bitmap bmp, bool cloneBitmap = false)
        {
            picWebcam.Image = new Bitmap(bmp, picWebcam.Width, picWebcam.Height);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cbWebcams.Items.Count == 0)
            {
                MessageBox.Show("No webcam detected.\nPlease wait till the client sends a list with available webcams.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ToggleControls(false);

            this.ActiveControl = picWebcam;

            _connectClient.Send(new GetWebcam
            {
                Webcam = cbWebcams.SelectedIndex,
                Resolution = cbResolutions.SelectedIndex
            });
        }

        public void ToggleControls(bool state)
        {
            IsStarted = !state;

            cbWebcams.Enabled = cbResolutions.Enabled = btnStart.Enabled = state;
            btnStop.Enabled = !state;
        }

        private void FrmRemoteWebcam_FormClosing(object sender, FormClosingEventArgs e)
        {
            _connectClient.Send(new DoWebcamStop());

            if (_connectClient.Value != null)
                _connectClient.Value.FrmWebcam = null;
        }

        private void FrmRemoteWebcam_Resize(object sender, EventArgs e)
        {
            panelTop.Left = (this.Width / 2) - (panelTop.Width / 2);
            btnShow.Left = (this.Width / 2) - (btnShow.Width / 2);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ToggleControls(true);
            this.ActiveControl = picWebcam;

            _connectClient.Send(new DoWebcamStop());
        }

        private void cbWebcams_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbResolutions.Invoke((MethodInvoker)delegate
            {
                cbResolutions.Items.Clear();
                foreach (var resolution in this._webcams.ElementAt(cbWebcams.SelectedIndex).Value)
                {
                    cbResolutions.Items.Add(resolution.ToString());
                }
                cbResolutions.SelectedIndex = 0;
            });
        }
    }
}
