using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
﻿using Microsoft.Win32;
using xServer.Core.Networking;
using xServer.Core.Registry;
using xServer.Core.Utilities;

namespace xServer.Forms
{
    public partial class FrmRegValueEditBinary : Form
    {
        private readonly Client _connectClient;

        private readonly RegValueData _value;

        private readonly string _keyPath;

        #region Constant

        private const string INVALID_BINARY_ERROR = "The binary value was invalid and could not be converted correctly.";

        #endregion

        public FrmRegValueEditBinary(string keyPath, RegValueData value, Client c)
        {
            _connectClient = c;
            _keyPath = keyPath;
            _value = value;

            InitializeComponent();

            this.valueNameTxtBox.Text = RegValueHelper.GetName(value.Name);

            if(value.Data == null)
            {
                hexEditor.HexTable = new byte[] { };
            }
            else {
                switch(value.Kind)
                {
                    case RegistryValueKind.Binary:
                        hexEditor.HexTable = (byte[])value.Data;
                        break;
                    case RegistryValueKind.DWord:
                        hexEditor.HexTable = ByteConverter.GetBytes((uint)(int)value.Data);
                        break;
                    case RegistryValueKind.QWord:
                        hexEditor.HexTable = ByteConverter.GetBytes((ulong)(long)value.Data);
                        break;
                    case RegistryValueKind.MultiString:
                        hexEditor.HexTable = ByteConverter.GetBytes((string[])value.Data);
                        break;
                    case RegistryValueKind.String:
                    case RegistryValueKind.ExpandString:
                        hexEditor.HexTable = ByteConverter.GetBytes(value.Data.ToString());
                        break;
                }
            }
        }

        private object GetData()
        {
            byte[] bytes = hexEditor.HexTable;
            if (bytes != null)
            {
                try
                {
                    switch(_value.Kind)
                    {
                        case RegistryValueKind.Binary:
                            return bytes;
                        case RegistryValueKind.DWord:
                            return (int)ByteConverter.ToUInt32(bytes);
                        case RegistryValueKind.QWord:
                            return (long)ByteConverter.ToUInt64(bytes);
                        case RegistryValueKind.MultiString:
                            return ByteConverter.ToStringArray(bytes);
                        case RegistryValueKind.String:
                        case RegistryValueKind.ExpandString:
                            return ByteConverter.ToString(bytes);
                    }
                }
                catch
                {
                    ShowWarning(INVALID_BINARY_ERROR, "Warning");
                }
            }
            return null;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            object valueData = GetData();
            if (valueData != null)
                new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);
            else
                DialogResult = DialogResult.None;
        }

        private void ShowWarning(string msg, string caption)
        {
            MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
