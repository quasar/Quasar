using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Quasar.Common.Messages;
using Quasar.Server.Extensions;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Networking;

namespace Quasar.Server.Forms
{
    public partial class FrmSystemInformation : Form
    {
        /// <summary>
        /// The client which can be used for the system information.
        /// </summary>
        private readonly Client _connectClient;

        /// <summary>
        /// The message handler for handling the communication with the client.
        /// </summary>
        private readonly SystemInformationHandler _sysInfoHandler;

        /// <summary>
        /// Holds the opened system information form for each client.
        /// </summary>
        private static readonly Dictionary<Client, FrmSystemInformation> OpenedForms = new Dictionary<Client, FrmSystemInformation>();

        /// <summary>
        /// Creates a new system information form for the client or gets the current open form, if there exists one already.
        /// </summary>
        /// <param name="client">The client used for the system information form.</param>
        /// <returns>
        /// Returns a new system information form for the client if there is none currently open, otherwise creates a new one.
        /// </returns>
        public static FrmSystemInformation CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmSystemInformation f = new FrmSystemInformation(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmSystemInformation"/> class using the given client.
        /// </summary>
        /// <param name="client">The client used for the remote desktop form.</param>
        public FrmSystemInformation(Client client)
        {
            _connectClient = client;
            _sysInfoHandler = new SystemInformationHandler(client);

            RegisterMessageHandler();
            InitializeComponent();
        }

        /// <summary>
        /// Registers the system information message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            _connectClient.ClientState += ClientDisconnected;
            _sysInfoHandler.ProgressChanged += SystemInformationChanged;
            MessageHandler.Register(_sysInfoHandler);
        }

        /// <summary>
        /// Unregisters the system information message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_sysInfoHandler);
            _sysInfoHandler.ProgressChanged -= SystemInformationChanged;
            _connectClient.ClientState -= ClientDisconnected;
        }

        /// <summary>
        /// Called whenever a client disconnects.
        /// </summary>
        /// <param name="client">The client which disconnected.</param>
        /// <param name="connected">True if the client connected, false if disconnected</param>
        private void ClientDisconnected(Client client, bool connected)
        {
            if (!connected)
            {
                this.Invoke((MethodInvoker)this.Close);
            }
        }

        private void FrmSystemInformation_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("System Information", _connectClient);
            _sysInfoHandler.RefreshSystemInformation();
            AddBasicSystemInformation();
        }

        private void FrmSystemInformation_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
            _sysInfoHandler.Dispose();
        }

        private void SystemInformationChanged(object sender, List<Tuple<string, string>> infos)
        {
            // remove "Loading..." information
            lstSystem.Items.RemoveAt(2);

            foreach (var info in infos)
            {
                var lvi = new ListViewItem(new[] {info.Item1, info.Item2});
                lstSystem.Items.Add(lvi);
            }

            lstSystem.AutosizeColumns();
        }

        private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSystem.Items.Count == 0) return;

            string output = string.Empty;

            foreach (ListViewItem lvi in lstSystem.Items)
            {
                output = lvi.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate(output, (current, lvs) => current + (lvs.Text + " : "));
                output = output.Remove(output.Length - 3);
                output = output + "\r\n";
            }

            ClipboardHelper.SetClipboardTextSafe(output);
        }

        private void copySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lstSystem.SelectedItems.Count == 0) return;

            string output = string.Empty;

            foreach (ListViewItem lvi in lstSystem.SelectedItems)
            {
                output = lvi.SubItems.Cast<ListViewItem.ListViewSubItem>().Aggregate(output, (current, lvs) => current + (lvs.Text + " : "));
                output = output.Remove(output.Length - 3);
                output = output + "\r\n";
            }

            ClipboardHelper.SetClipboardTextSafe(output);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstSystem.Items.Clear();
            _sysInfoHandler.RefreshSystemInformation();
            AddBasicSystemInformation();
        }

        /// <summary>
        /// Adds basic system information which is already available to the ListView.
        /// </summary>
        private void AddBasicSystemInformation()
        {
            ListViewItem lvi =
                new ListViewItem(new[] {"Operating System", _connectClient.Value.OperatingSystem});
            lstSystem.Items.Add(lvi);
            lvi =
                new ListViewItem(new[]
                {
                    "Architecture",
                    (_connectClient.Value.OperatingSystem.Contains("32 Bit")) ? "x86 (32 Bit)" : "x64 (64 Bit)"
                });
            lstSystem.Items.Add(lvi);
            lvi = new ListViewItem(new[] {"", "Getting more information..."});
            lstSystem.Items.Add(lvi);
        }
    }
}