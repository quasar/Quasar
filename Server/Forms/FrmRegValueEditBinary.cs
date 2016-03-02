using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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

        #region Constant

        private const string INVALID_BINARY_ERROR = "The binary value was invalid and could not be converted correctly.";

        #endregion

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
            else if (value.Kind == Microsoft.Win32.RegistryValueKind.DWord)
            {
                hexEditor.HexTable = BitConverter.GetBytes((uint)(int)value.Data);
            }
            else if (value.Kind == Microsoft.Win32.RegistryValueKind.QWord)
            {
                hexEditor.HexTable = BitConverter.GetBytes((ulong)(long)value.Data);
            }
            else if (value.Kind == Microsoft.Win32.RegistryValueKind.String || value.Kind == Microsoft.Win32.RegistryValueKind.ExpandString)
            {
                //Convert string to bytes
                byte[] bytes = new byte[value.Data.ToString().Length * sizeof(char)];
                Buffer.BlockCopy(value.Data.ToString().ToCharArray(), 0, bytes, 0, bytes.Length);
                hexEditor.HexTable = bytes;
            }
            else if (value.Kind == Microsoft.Win32.RegistryValueKind.MultiString)
            {
                byte[] bytes = MultiStringToBytes((string[])value.Data);
                hexEditor.HexTable = bytes;
            }
        }

        private void FrmRegValueEditBinary_Load(object sender, EventArgs e)
        {
            hexEditor.Select();
            hexEditor.Focus();
        }

        #region Help function

        private object GetData()
        {
            byte[] bytes = hexEditor.HexTable;
            if (bytes != null && bytes.Length > 0)
            {
                try
                {
                    if (_value.Kind == Microsoft.Win32.RegistryValueKind.Binary)
                    {
                        return bytes;
                    }
                    else if (_value.Kind == Microsoft.Win32.RegistryValueKind.DWord)
                    {
                        uint unsignedValue = BitConverter.ToUInt32(hexEditor.HexTable, 0);
                        return (int)unsignedValue;
                    }
                    else if (_value.Kind == Microsoft.Win32.RegistryValueKind.QWord)
                    {
                        ulong unsignedValue = BitConverter.ToUInt64(hexEditor.HexTable, 0);
                        return (long)unsignedValue;
                    }
                    else if (_value.Kind == Microsoft.Win32.RegistryValueKind.String || _value.Kind == Microsoft.Win32.RegistryValueKind.ExpandString)
                    {
                        //Use ceiling function to make sure that the bytes fit in the char
                        int nrChars = (int)Math.Ceiling((float)bytes.Length / (float)sizeof(char));
                        char[] chars = new char[nrChars];
                        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
                        return new string(chars);
                    }
                    else if (_value.Kind == Microsoft.Win32.RegistryValueKind.MultiString)
                    {
                        string[] strings = BytesToMultiString(hexEditor.HexTable);
                        return strings;
                    }
                }
                catch
                {
                    string msg = INVALID_BINARY_ERROR;
                    ShowWarning(msg, "Warning");
                    return null;
                }
            }
            return null;
        }

        #endregion

        #region Ok and Cancel button

        private void okButton_Click(object sender, EventArgs e)
        {
            object valueData = GetData();
            if (valueData != null)
            {
                new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);

                this.Close();
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Misc

        private void ShowWarning(string msg, string caption)
        {
            MessageBox.Show(msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Converts the given string array to
        /// a byte array.
        /// </summary>
        /// <param name="strings"> string array</param>
        /// <returns></returns>
        private byte[] MultiStringToBytes(string[] strings)
        {
            List<byte> ret = new List<byte>();

            foreach (string str in strings)
            {
                byte[] bytes = new byte[str.Length * sizeof(char)];
                Buffer.BlockCopy(str.ToString().ToCharArray(), 0, bytes, 0, bytes.Length);
                ret.AddRange(bytes);
                //Add nullbytes
                ret.Add(byte.MinValue);
                ret.Add(byte.MinValue);
            }
            return ret.ToArray();
        }

        /// <summary>
        /// Converts a array of bytes to
        /// a string array
        /// </summary>
        /// <param name="bytes"> array of bytes</param>
        /// <returns></returns>
        private string[] BytesToMultiString(byte[] bytes)
        {
            List<string> ret = new List<string>();

            int i = 0;
            while (i < bytes.Length)
            {
                //Holds the number of nulls (3 nulls indicated end of a string)
                int nullcount = 0;
                string str = "";
                while (i < bytes.Length && nullcount < 3)
                {
                    //Null byte
                    if (bytes[i] == byte.MinValue)
                    {
                        nullcount++;
                    }
                    else
                    {
                        str += Convert.ToChar(bytes[i]);
                        nullcount = 0;
                    }
                    i++;
                }
                ret.Add(str);
            }

            return ret.ToArray();
        }

        #endregion
    }
}
