using System.Windows.Forms;
using Quasar.Common.Models;
using Quasar.Server.Extensions;
using Quasar.Server.Registry;

namespace Quasar.Server.Controls
{
    public class RegistryValueLstItem : ListViewItem
    {
        private string _type { get; set; }
        private string _data { get; set; }

        public string RegName {
            get { return this.Name; }
            set 
            { 
                this.Name = value;
                this.Text = RegValueHelper.GetName(value);
            }
        }
        public string Type {
            get { return _type; }
            set
            {
                _type = value;

                if (this.SubItems.Count < 2)
                    this.SubItems.Add(_type);
                else
                    this.SubItems[1].Text = _type;

                this.ImageIndex = GetRegistryValueImgIndex(_type);
            }
        }

        public string Data {
            get { return _data; }
            set
            {
                _data = value;

                if (this.SubItems.Count < 3)
                    this.SubItems.Add(_data);
                else 
                    this.SubItems[2].Text = _data;
            }
        }

        public RegistryValueLstItem(RegValueData value)
        {
            RegName = value.Name;
            Type = value.Kind.RegistryTypeToString();
            Data = RegValueHelper.RegistryValueToString(value);
        }

        private int GetRegistryValueImgIndex(string type)
        {
            switch (type)
            {
                case "REG_MULTI_SZ":
                case "REG_SZ":
                case "REG_EXPAND_SZ":
                    return 0;
                case "REG_BINARY":
                case "REG_DWORD":
                case "REG_QWORD":
                default:
                    return 1;
            }
        }
    }
}
