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
                hexEditor.HexTable = (byte[])value.Data;
            }
        }

        private void FrmRegValueEditBinary_Load(object sender, EventArgs e)
        {
            hexEditor.Select();
            hexEditor.Focus();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (hexEditor.HexTable != null)
                {
                    if (_value.Kind == Microsoft.Win32.RegistryValueKind.Binary)
                    {
                        byte[] binaryValue = (hexEditor.HexTable);
                        object valueData = binaryValue;

                        new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);
                    }

                    this.Close();
                }
            }
            catch { }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }
    }
}
