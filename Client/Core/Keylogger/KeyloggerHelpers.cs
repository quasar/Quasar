using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using xClient.Core.Keylogger;

namespace xClient.Core.Keylogger
{
    public static class KeyloggerHelpers
    {
        public static byte GetKeyloggerKeyValue<T>(this Enum sender) where T : KeyloggerKey
        {
            return Convert.ToByte(sender);
        }
    }
}