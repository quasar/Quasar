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
    public partial class FrmRegValueEditWord : Form
    {
        private readonly Client _connectClient;

        private readonly RegValueData _value;

        private readonly string _keyPath;

        private int valueBase;

        #region CONSTANT

        private const int HEXA_32BIT_MAX_LENGTH = 8;
        private const int HEXA_64BIT_MAX_LENGTH = 16;
        private const int DEC_32BIT_MAX_LENGTH = 10;
        private const int DEC_64BIT_MAX_LENGTH = 20;
        private const int HEXA_BASE = 16;
        private const int DEC_BASE = 10;

        private const string DWORD_WARNING = "The decimal value entered is greater than the maximum value of a DWORD (32-bit number). Should the value be truncated in order to continue?";
        private const string QWORD_WARNING = "The decimal value entered is greater than the maximum value of a QWORD (64-bit number). Should the value be truncated in order to continue?"; 
        #endregion

        public FrmRegValueEditWord(string keyPath, RegValueData value, Client c)
        {
            _connectClient = c;
            _keyPath = keyPath;
            _value = value;

            InitializeComponent();

            this.valueNameTxtBox.Text = value.Name;

            if (value.Kind == RegistryValueKind.DWord) {
                this.Text = "Edit DWORD (32-bit) Value";
                this.valueDataTxtBox.Text = ((uint)(int)value.Data).ToString("X");
                this.valueDataTxtBox.MaxLength = HEXA_32BIT_MAX_LENGTH;
            }
            else if (value.Kind == RegistryValueKind.QWord) {
                this.Text = "Edit QWORD (64-bit) Value";
                this.valueDataTxtBox.Text = ((ulong)(long)value.Data).ToString("X");
                this.valueDataTxtBox.MaxLength = HEXA_64BIT_MAX_LENGTH;
            }
            valueBase = HEXA_BASE;
        }

        private void FrmRegValueEditWord_Load(object sender, EventArgs e)
        {
            this.valueDataTxtBox.Select();
            this.valueDataTxtBox.Focus();
        }

        #region Helpfunctions

        private string GetDataAsString(int type)
        {
            if (!String.IsNullOrEmpty(valueDataTxtBox.Text))
            {
                string text = valueDataTxtBox.Text;
                string returnType = (type == HEXA_BASE ? "X" : "D");
                try
                {
                    if (_value.Kind == RegistryValueKind.DWord)
                        return Convert.ToUInt32(text, valueBase).ToString(returnType);
                    else
                        return Convert.ToUInt64(text, valueBase).ToString(returnType);
                }
                catch
                {
                    string message = _value.Kind == RegistryValueKind.DWord ? DWORD_WARNING : QWORD_WARNING;
                    if (ShowWarning(message, "Overflow") == DialogResult.Yes) //Yes from popup
                    {
                        if (_value.Kind == RegistryValueKind.DWord)
                            return UInt32.MaxValue.ToString(returnType);
                        else
                            return UInt64.MaxValue.ToString(returnType);
                    }
                }
            }
            else
            {
                return "";
            }
            //No convertion made
            return null;
        }
        
        private DialogResult ShowWarning(string msg, string caption)
        {
            return MessageBox.Show(msg, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        }

        #endregion

        #region RadioButton Actions

        private void radioHexa_Click(object sender, EventArgs e)
        {
            if (radioHexa.Checked)
            {
                string text = GetDataAsString(HEXA_BASE);
                if (text != null)
                {
                    this.valueDataTxtBox.MaxLength = HEXA_64BIT_MAX_LENGTH;

                    if (_value.Kind == RegistryValueKind.DWord)
                        this.valueDataTxtBox.MaxLength = HEXA_32BIT_MAX_LENGTH;

                    valueDataTxtBox.Text = text;
                    valueBase = HEXA_BASE;
                }
                else if(valueBase == DEC_BASE)
                {
                    //Re-check
                    radioDecimal.Checked = true;
                }
            }
        }

        private void radioDecimal_Click(object sender, EventArgs e)
        {
            if (radioDecimal.Checked)
            {
                string text = GetDataAsString(DEC_BASE);
                if (text != null)
                {
                    this.valueDataTxtBox.MaxLength = DEC_64BIT_MAX_LENGTH;

                    if (_value.Kind == RegistryValueKind.DWord)
                        this.valueDataTxtBox.MaxLength = DEC_32BIT_MAX_LENGTH;

                    valueDataTxtBox.Text = text;
                    valueBase = DEC_BASE;
                }
                else if(valueBase == HEXA_BASE)
                {
                    //Re-check
                    radioHexa.Checked = true;
                }
            }
        }

        #endregion

        #region OK and Cancel Buttons

        private void okButton_Click(object sender, EventArgs e)
        {
            //Try to convert string
            string text = GetDataAsString(DEC_BASE);
            if (text != null)
            {
                if (_value.Kind == RegistryValueKind.DWord)
                {
                    if (text != ((uint)(int)_value.Data).ToString())
                    {
                        uint unsignedValue = Convert.ToUInt32(text);
                        object valueData = (int)(unsignedValue);

                        new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);
                    }
                }
                else if (_value.Kind == RegistryValueKind.QWord)
                {
                    if (text != ((ulong)(long)_value.Data).ToString())
                    {
                        ulong unsignedValue = Convert.ToUInt64(text);
                        object valueData = (long)(unsignedValue);

                        new xServer.Core.Packets.ServerPackets.DoChangeRegistryValue(_keyPath, new RegValueData(_value.Name, _value.Kind, valueData)).Execute(_connectClient);
                    }
                }
                this.Close();
            }
        }
        
        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void valueDataTxtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Control keys are ok
            if(!Char.IsControl(e.KeyChar)) {
                if (radioHexa.Checked)
                {
                    e.Handled = !(IsHexa(e.KeyChar));
                }
                else
                {
                    e.Handled = !(Char.IsDigit(e.KeyChar));
                }
            }
        }

        #endregion

        private static bool IsHexa(char c)
        {
            return (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F') || Char.IsDigit(c);
        }
    }
}
