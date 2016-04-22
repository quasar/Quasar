using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xServer.Core.Registry
{
    public class RegValueHelper
    {
        private static string DEFAULT_REG_VALUE = "(Default)";

        public static bool IsDefaultValue(string valueName)
        {
            return String.IsNullOrEmpty(valueName);
        }

        public static string GetName(string valueName)
        {
            return IsDefaultValue(valueName) ? DEFAULT_REG_VALUE : valueName;
        }
    }
}
