using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xClient.Core.Registry
{
    [Serializable]
    public class RegValueData
    {
        public string Name { get; set; }
        public RegistryValueKind Kind { get; set; }
        public object Data { get; set; }

        public RegValueData(string name, RegistryValueKind kind, object data)
        {
            Name = name;
            Kind = kind;
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("({0}:{1}:{2})", Name, Kind, Data);
        }
    }
}
