using System.Windows.Forms;
using xServer.Core.Networking;

namespace xServer.Forms
{
    public partial class FrmRegistryEditor : Form
    {
        private readonly Client _connectClient;

        public FrmRegistryEditor(Client c)
        {
            _connectClient = c;
            _connectClient.Value.FrmRe = this;

            InitializeComponent();
        }
    }
}