using Microsoft.Win32;
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
    public partial class FrmRegValueEditString : Form
    {
        private readonly Client _connectClient;

        private readonly RegValueData _value;

        private readonly string _keyPath;

        public FrmRegValueEditString(string keyPath, RegValueData value, Client c)
        {
            _connectClient = c;
            _keyPath = keyPath;
            _value = value;

            InitializeComponent();

            this.valueNameTxtBox.Text = RegValueHelper.GetName(value.Name);
            this.valueDataTxtBox.Text = value.Data == null ? "" : value.Data.ToString();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (_value.Data == null || valueDataTxtBox.Text != _value.Data.ToString())
            {
                object valueData = valueDataTxtBox.Text;
                new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);
            }
        }
    }
}
