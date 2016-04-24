using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using xServer.Enums;

namespace xServer.Controls
{
    public partial class WordTextBox : TextBox
    {
        private bool isHexNumber;
        private WordType type;

        public override int MaxLength
        {
            get
            {
                return base.MaxLength;
            }
            set { }
        }

        public bool IsHexNumber
        {
            get { return isHexNumber; }
            set
            {
                if (isHexNumber == value)
                    return;

                if(value)
                {
                    if (Type == WordType.DWORD)
                        Text = UIntValue.ToString("x");
                    else
                        Text = ULongValue.ToString("x");
                }
                else
                {
                    if (Type == WordType.DWORD)
                        Text = UIntValue.ToString();
                    else
                        Text = ULongValue.ToString();
                }

                isHexNumber = value;

                UpdateMaxLength();
            }
        }

        public WordType Type
        {
            get { return type; }
            set
            {
                if (type == value)
                    return;

                type = value;

                UpdateMaxLength();
            }
        }

        public uint UIntValue
        {
            get
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                        return 0;
                    else if (IsHexNumber)
                        return UInt32.Parse(Text, NumberStyles.HexNumber);
                    else
                        return UInt32.Parse(Text);
                }
                catch (Exception)
                {
                    return UInt32.MaxValue;
                }
            }
        }

        public ulong ULongValue
        {
            get
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                        return 0;
                    else if (IsHexNumber)
                        return UInt64.Parse(Text, NumberStyles.HexNumber);
                    else
                        return UInt64.Parse(Text);
                }
                catch (Exception)
                {
                    return UInt64.MaxValue;
                }
            }
        }

        public bool IsConversionValid()
        {
            if (String.IsNullOrEmpty(Text))
                return true;

            if (!IsHexNumber)
            {
                return ConvertToHex();
            }
            return true;
        }

        public WordTextBox()
        {
            InitializeComponent();
            base.MaxLength = 8;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            e.Handled = !IsValidChar(e.KeyChar);
        }

        private bool IsValidChar(char ch)
        {
            return (Char.IsControl(ch) ||
                    Char.IsDigit(ch) ||
                    (IsHexNumber && Char.IsLetter(ch) && Char.ToLower(ch) <= 'f'));
        }

        private void UpdateMaxLength()
        {
            if(Type == WordType.DWORD)
            {
                if (IsHexNumber)
                    base.MaxLength = 8;
                else
                    base.MaxLength = 10;
            }
            else
            {
                if (IsHexNumber)
                    base.MaxLength = 16;
                else
                    base.MaxLength = 20;
            }
        }


        private bool ConvertToHex()
        {
            try
            {
                if (Type == WordType.DWORD)
                    UInt32.Parse(Text);
                else
                    UInt64.Parse(Text);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
