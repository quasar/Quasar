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

        #region Constants

        private const string WARNING_MSG = "Data of type REG_MULTI_SZ cannot contain empty strings. Registry Editor will remove the empty strings found.";

        #endregion

        public FrmRegValueEditMultiString(string keyPath, RegValueData value, Client c)
        {
            _connectClient = c;
            _keyPath = keyPath;
            _value = value;

            InitializeComponent();

            this.valueNameTxtBox.Text = value.Name;
            this.valueDataTxtBox.Lines = (string[])value.Data;
        }

        private void FrmRegValueEditMultiString_Load(object sender, EventArgs e)
        {
            this.valueDataTxtBox.Select();
            this.valueDataTxtBox.Focus();
        }

        #region Ok and Cancel button

        private void okButton_Click(object sender, EventArgs e)
        {
            string[] lines = valueDataTxtBox.Lines;
            if (lines.Length > 0)
            {
                string[] valueData = GetSanitizedStrings(lines);
                if (valueData.Length != lines.Length)
                {
                    ShowWarning();
                }
                new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);
            }
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private string[] GetSanitizedStrings(string[] strs)
        {
            List<string> sanitized = new List<string>();
            foreach (string str in strs)
            {
                if (!String.IsNullOrWhiteSpace(str) && !String.IsNullOrEmpty(str))
                {
                    sanitized.Add(str);
                }
            }
            return sanitized.ToArray();
        }

        private void ShowWarning()
        {
            MessageBox.Show(WARNING_MSG, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
