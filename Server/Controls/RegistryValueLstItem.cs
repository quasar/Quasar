using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace xServer.Controls
{
    //Comparer for comparing registry values (listview)
    public class RegistryValueListItemComparer : IComparer
    {
        public RegistryValueListItemComparer() { }

        public int Compare(object x, object y)
        {
            if (x.GetType() == typeof(RegistryValueLstItem) && y.GetType() == typeof(RegistryValueLstItem))
            {
                return String.Compare(((RegistryValueLstItem)x).RegName, ((RegistryValueLstItem)y).RegName);
            }
            return -1;
        }
    }

    internal class RegistryValueLstItem : ListViewItem
    {

        public string RegName { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }

        public RegistryValueLstItem(string name, string type, string data) :
            base(name)
        {
            RegName = name;
            this.Name = name;
            this.SubItems.Add(type);
            Type = type;
            this.SubItems.Add(data);
            Data = data;
        }
    }
}
