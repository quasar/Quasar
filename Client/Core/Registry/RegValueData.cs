using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xClient.Core.Registry
{
    [Serializable]
    public class RegValueData
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string Data { get; private set; }

        public RegValueData(string name, string type, string data)
        {
            Name = name;
            Type = type;
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("({0}:{1}:{2})", Name, Type, Data);
        }
    }
}
