using System;
using System.Collections.Generic;
using System.Text;

namespace xClient.Core.Keylogger
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public class KeyloggerKey : Attribute
    {
        public string KeyName { get; private set; }
        public bool IsSpecialKey { get; private set; }

        /// <summary>
        /// Constructs the attribute used by the keylogger
        /// keys to hold data for them.
        /// </summary>
        /// <param name="PrintedName">The printed value of the key when converting
        /// the specific key to its string value.</param>
        /// <param name="IsSpecialKey">Determines if the key is a special key.</param>
        internal KeyloggerKey(string PrintedName, bool _IsSpecialKey = false)
        {
            KeyName = PrintedName;
            IsSpecialKey = _IsSpecialKey;
        }
    }
}