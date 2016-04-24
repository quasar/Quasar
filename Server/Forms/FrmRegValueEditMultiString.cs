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
    public partial class FrmRegValueEditMultiString : Form
    {
        private readonly Client _connectClient;

        private readonly RegValueData _value;

        private readonly string _keyPath;

        public FrmRegValueEditMultiString(string keyPath, RegValueData value, Client c)
        {
            _connectClient = c;
            _keyPath = keyPath;
            _value = value;

            InitializeComponent();

            this.valueNameTxtBox.Text = value.Name;
            this.valueDataTxtBox.Text = String.Join("\r\n",((string[])value.Data));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string[] valueData = valueDataTxtBox.Text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);
        }

    }
}
