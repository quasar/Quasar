using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Controls
{
    internal class RegistryValueLstItem : ListViewItem
    {

        public string RegName { get; private set; }
        public string Type { get; private set; }
        public string Data { get; private set; }

        public RegistryValueLstItem(string name, string type, string data) :
            base(name)
        {
            RegName = name;
            this.SubItems.Add(type);
            Type = type;
            this.SubItems.Add(data);
            Data = data;
        }
    }
}
