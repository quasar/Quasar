using System.Collections.Generic;
using Microsoft.Win32;
using System.Linq;

namespace xClient.Core.Extensions
{
    public static class RegistryKeyExtensions
    {
        /// <summary>
        /// Determines if the registry key by the name provided is null or has the value of null.
        /// </summary>
        /// <param name="keyName">The name associated with the registry key.</param>
        /// <param name="key">The actual registry key.</param>
        /// <param name="keyValue">The string value of the registry key determined by the key's name.</param>
        /// <returns>True if the provided name is null or empty, or the key is null; False if otherwise.</returns>
        public static bool IsNameOrValueNull(this string keyName, RegistryKey key)
        {
            return (string.IsNullOrEmpty(keyName) || (key == null));
        }

        /// <summary>
        /// Attempts to get the value of the key using the specified key name. This method assumes
        /// correct input.
        /// </summary>
        /// <param name="key">The key of which we obtain the value of.</param>
        /// <param name="keyName">The name of the key.</param>
        /// <returns>Returns the value of the key using the specified key name. If unable to do so,
        /// string.Empty will be returned instead.</returns>
        public static string GetValueSafe(this RegistryKey key, string keyName)
        {
            // Before calling this, use something such as "IsNameOrValueNull" to make sure
            // that the input used for this method is usable. The responsibility for this
            // method is to take these valid parameters and try to get the value of them,
            // allowing exceptions if any are generated.
            try
            {
                return key.GetValue(keyName).ToString();
            }
            catch
            {
                return string.Empty;
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
                return Registry.LocalMachine.OpenSubKey(name, false);
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<string> GetFormattedKeyValues(this RegistryKey key)
        {
            if (key != null)
            {
                foreach (var k in key.GetValueNames().Where(keyVal => !keyVal.IsNameOrValueNull(key))
                                                     .Select(keyVal => key.GetValueSafe(keyVal)))
                {
                    // Less-likely, but this will ensure no empty items if an exception was thrown
                    // when obtaining the value.
                    if (string.IsNullOrEmpty(k))
                    {
                        yield return string.Format("{0}||{1}", k, key.GetValue(k));
                    }
                }
            }
            else
            {
                yield break;
            }
        }
    }
}