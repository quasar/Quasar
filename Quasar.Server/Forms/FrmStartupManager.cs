using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Quasar.Common.Enums;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using Quasar.Server.Helper;
using Quasar.Server.Messages;
using Quasar.Server.Networking;

namespace Quasar.Server.Forms
{
    public partial class FrmStartupManager : Form
    {
        /// <summary>
        /// The client which can be used for the startup manager.
        /// </summary>
        private readonly Client _connectClient;

        /// <summary>
        /// The message handler for handling the communication with the client.
        /// </summary>
        private readonly StartupManagerHandler _startupManagerHandler;

        /// <summary>
        /// Holds the opened startup manager form for each client.
        /// </summary>
        private static readonly Dictionary<Client, FrmStartupManager> OpenedForms = new Dictionary<Client, FrmStartupManager>();

        /// <summary>
        /// Creates a new startup manager form for the client or gets the current open form, if there exists one already.
        /// </summary>
        /// <param name="client">The client used for the startup manager form.</param>
        /// <returns>
        /// Returns a new startup manager form for the client if there is none currently open, otherwise creates a new one.
        /// </returns>
        public static FrmStartupManager CreateNewOrGetExisting(Client client)
        {
            if (OpenedForms.ContainsKey(client))
            {
                return OpenedForms[client];
            }
            FrmStartupManager f = new FrmStartupManager(client);
            f.Disposed += (sender, args) => OpenedForms.Remove(client);
            OpenedForms.Add(client, f);
            return f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrmStartupManager"/> class using the given client.
        /// </summary>
        /// <param name="client">The client used for the remote desktop form.</param>
        public FrmStartupManager(Client client)
        {
            _connectClient = client;
            _startupManagerHandler = new StartupManagerHandler(client);

            RegisterMessageHandler();
            InitializeComponent();
        }

        /// <summary>
        /// Registers the startup manager message handler for client communication.
        /// </summary>
        private void RegisterMessageHandler()
        {
            _connectClient.ClientState += ClientDisconnected;
            _startupManagerHandler.ProgressChanged += StartupItemsChanged;
            MessageHandler.Register(_startupManagerHandler);
        }

        /// <summary>
        /// Unregisters the startup manager message handler.
        /// </summary>
        private void UnregisterMessageHandler()
        {
            MessageHandler.Unregister(_startupManagerHandler);
            _startupManagerHandler.ProgressChanged -= StartupItemsChanged;
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

        private void FrmStartupManager_Load(object sender, EventArgs e)
        {
            this.Text = WindowHelper.GetWindowTitle("Startup Manager", _connectClient);

            AddGroups();
            _startupManagerHandler.RefreshStartupItems();
        }

        private void FrmStartupManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterMessageHandler();
            _startupManagerHandler.Dispose();
        }

        /// <summary>
        /// Adds all supported startup types as ListView groups.
        /// </summary>
        private void AddGroups()
        {
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")
                    {Tag = StartupType.LocalMachineRun});
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce")
                    {Tag = StartupType.LocalMachineRunOnce});
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")
                    {Tag = StartupType.CurrentUserRun});
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce")
                    {Tag = StartupType.CurrentUserRunOnce});
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Run")
                    {Tag = StartupType.LocalMachineWoW64Run});
            lstStartupItems.Groups.Add(
                new ListViewGroup(
                        "HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\RunOnce")
                    {Tag = StartupType.LocalMachineWoW64RunOnce});
            lstStartupItems.Groups.Add(new ListViewGroup("%APPDATA%\\Microsoft\\Windows\\Start Menu\\Programs\\Startup")
                {Tag = StartupType.StartMenu});
        }

        /// <summary>
        /// Called whenever a startup item changed.
        /// </summary>
        /// <param name="sender">The message handler which raised the event.</param>
        /// <param name="startupItems">The current startup items of the client.</param>
        private void StartupItemsChanged(object sender, List<StartupItem> startupItems)
        {
            lstStartupItems.Items.Clear();

            foreach (var item in startupItems)
            {
                var i = lstStartupItems.Groups.Cast<ListViewGroup>().First(x => (StartupType)x.Tag == item.Type);
                ListViewItem lvi = new ListViewItem(new[] {item.Name, item.Path}) {Group = i, Tag = item};
                lstStartupItems.Items.Add(lvi);
            }
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var frm = new FrmStartupAdd())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    _startupManagerHandler.AddStartupItem(frm.StartupItem);
                    _startupManagerHandler.RefreshStartupItems();
                }
            }
        }

        private void removeEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool modified = false;

            foreach (ListViewItem item in lstStartupItems.SelectedItems)
            {
                _startupManagerHandler.RemoveStartupItem((StartupItem)item.Tag);
                modified = true;
            }

            if (modified)
            {
                _startupManagerHandler.RefreshStartupItems();
            }
        }
    }
}
