using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xClient.Core.Registry
{
    [Serializable]
    public class RegValueData : IEquatable<RegValueData>
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }

        public RegValueData(string name, string type, string data)
        {
            Name = name;
            Type = type;
            Data = data;
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(RegValueData))
            {
                return this.Equals((RegValueData)obj);
            }

            return false;
        }

        public override string ToString()
        {
            return string.Format("({0}:{1}:{2})", Name, Type, Data);
        }

        public bool Equals(RegValueData value)
        {
            return this.Name == value.Name;
        }
    }
}
