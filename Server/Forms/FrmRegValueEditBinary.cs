using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using xServer.Core.Networking;
using xServer.Core.Registry;

namespace xServer.Forms
{
    public partial class FrmRegValueEditBinary : Form
    {
        private readonly Client _connectClient;

        private readonly RegValueData _value;

        private readonly string _keyPath;

        public FrmRegValueEditBinary(string keyPath, RegValueData value, Client c)
        {
            _connectClient = c;
            _keyPath = keyPath;
            _value = value;

            InitializeComponent();

            this.valueNameTxtBox.Text = value.Name;


            if (value.Kind == Microsoft.Win32.RegistryValueKind.Binary)
            {
                //TODO Adding code for displaying binary data
            }

        }

        private void FrmRegValueEditBinary_Load(object sender, EventArgs e)
        {

        }
    }
}
