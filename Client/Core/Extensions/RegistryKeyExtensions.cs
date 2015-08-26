using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xClient.Core.Extensions
{
    public static class RegistryKeyExtensions
    {
        /// <summary>
        /// Determines if the registry key by the name provided is null or has the value of null.
        /// </summary>
        /// <param name="keyName">The name associated with the registry key.</param>
        /// <param name="key">The actual registry key.</param>
        /// <returns>True if the provided name is null or empty, or the key is null; False if otherwise.</returns>
        private static bool IsNameOrValueNull(this string keyName, RegistryKey key)
        {
            return (string.IsNullOrEmpty(keyName) || (key == null));
        }

        /// <summary>
        /// Attempts to get the string value of the key using the specified key name. This method assumes
        /// correct input.
        /// </summary>
        /// <param name="key">The key of which we obtain the value of.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <param name="defaultValue">The default value if value can not be determinated.</param>
        /// <returns>Returns the value of the key using the specified key name. If unable to do so,
        /// defaultValue will be returned instead.</returns>
        public static string GetValueSafe(this RegistryKey key, string keyName, string defaultValue = "")
        {
            try
            {
                return key.GetValue(keyName, defaultValue).ToString();
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Attempts to obtain a readonly (non-writable) sub key from the key provided using the
        /// specified name. Exceptions thrown will be caught and will only return a null key.
        /// This method assumes the caller will dispose of the key when done using it.
        /// </summary>
        /// <param name="key">The key of which the sub key is obtained from.</param>
        /// <param name="name">The name of the sub-key.</param>
        /// <returns>Returns the sub-key obtained from the key and name provided; Returns null if
        /// unable to obtain a sub-key.</returns>
        public static RegistryKey OpenReadonlySubKeySafe(this RegistryKey key, string name)
        {
            try
            {
                return key.OpenSubKey(name, false);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Attempts to obtain a writable sub key from the key provided using the specified
        /// name. This method assumes the caller will dispose of the key when done using it.
        /// </summary>
        /// <param name="key">The key of which the sub key is obtained from.</param>
        /// <param name="name">The name of the sub-key.</param>
        /// <returns>Returns the sub-key obtained from the key and name provided; Returns null if
        /// unable to obtain a sub-key.</returns>
        public static RegistryKey OpenWritableSubKeySafe(this RegistryKey key, string name)
        {
            try
            {
                return key.OpenSubKey(name, true);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all of the value names associated with the registry key and returns
        /// formatted strings of the filtered values.
        /// </summary>
        /// <param name="key">The registry key of which the values are obtained.</param>
        /// <returns>Yield returns formatted strings of the key and the key value.</returns>
        public static IEnumerable<string> GetFormattedKeyValues(this RegistryKey key)
        {
            if (key == null) yield break;

            foreach (var k in key.GetValueNames().Where(keyVal => !keyVal.IsNameOrValueNull(key)).Where(k => !string.IsNullOrEmpty(k)))
            {
                yield return string.Format("{0}||{1}", k, key.GetValueSafe(k));
            }
        }

        public static string RegistryTypeToString(this RegistryValueKind valueKind, object valueData)
        {
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    return Encoding.ASCII.GetString((byte[])valueData);
                case RegistryValueKind.MultiString:
                    return string.Join(" ", (string[])valueData);
                case RegistryValueKind.DWord:
                    return ((uint)((int)valueData)).ToString();
                case RegistryValueKind.QWord:
                    return ((ulong)((long)valueData)).ToString();
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    return valueData.ToString();
                case RegistryValueKind.Unknown:
                default:
                    return string.Empty;
            }
        }
    }
}