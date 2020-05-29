using Microsoft.Win32;
using Quasar.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.Client.Extensions
{
    /// <summary>
    /// Provides extensions for registry key and value operations.
    /// </summary>
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
        /// <param name="defaultValue">The default value if value can not be determined.</param>
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
        /// Attempts to create a sub key from the key provided using the specified
        /// name. This method assumes the caller will dispose of the key when done using it.
        /// </summary>
        /// <param name="key">The key of which the sub key is to be created from.</param>
        /// <param name="name">The name of the sub-key.</param>
        /// <returns>Returns the sub-key that was created for the key and name provided; Returns null if
        /// unable to create a sub-key.</returns>
        public static RegistryKey CreateSubKeySafe(this RegistryKey key, string name)
        {
            try
            {
                return key.CreateSubKey(name);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Attempts to delete a sub-key and its children from the key provided using the specified
        /// name.
        /// </summary>
        /// <param name="key">The key of which the sub-key is to be deleted from.</param>
        /// <param name="name">The name of the sub-key.</param>
        /// <returns>Returns <c>true</c> if the action succeeded, otherwise <c>false</c>.</returns>
        public static bool DeleteSubKeyTreeSafe(this RegistryKey key, string name)
        {
            try
            {
                key.DeleteSubKeyTree(name, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /*
        * Derived and Adapted from drdandle's article, 
        * Copy and Rename Registry Keys at Code project.
        * Copy and Rename Registry Keys (Post Date: November 11, 2006)
        * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        * This is a work that is not of the original. It
        * has been modified to suit the needs of another
        * application.
        * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        * First Modified by StingRaptor on January 21, 2016
        * ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        * Original Source:
        * http://www.codeproject.com/Articles/16343/Copy-and-Rename-Registry-Keys
        */

        /// <summary>
        /// Attempts to rename a sub-key to the key provided using the specified old
        /// name and new name.
        /// </summary>
        /// <param name="key">The key of which the subkey is to be renamed from.</param>
        /// <param name="oldName">The old name of the sub-key.</param>
        /// <param name="newName">The new name of the sub-key.</param>
        /// <returns>Returns <c>true</c> if the action succeeded, otherwise <c>false</c>.</returns>
        public static bool RenameSubKeySafe(this RegistryKey key, string oldName, string newName)
        {
            try
            {
                //Copy from old to new
                key.CopyKey(oldName, newName);
                //Dispose of the old key
                key.DeleteSubKeyTree(oldName);
                return true;
            }
            catch
            {
                //Try to dispose of the newKey (The rename failed)
                key.DeleteSubKeyTreeSafe(newName);
                return false;
            }
        }

        /// <summary>
        /// Attempts to copy a old subkey to a new subkey for the key 
        /// provided using the specified old name and new name. (throws exceptions)
        /// </summary>
        /// <param name="key">The key of which the subkey is to be deleted from.</param>
        /// <param name="oldName">The old name of the sub-key.</param>
        /// <param name="newName">The new name of the sub-key.</param>
        public static void CopyKey(this RegistryKey key, string oldName, string newName)
        {
            //Create a new key
            using (RegistryKey newKey = key.CreateSubKey(newName))
            {
                //Open old key
                using (RegistryKey oldKey = key.OpenSubKey(oldName, true))
                {
                    //Copy from old to new
                    RecursiveCopyKey(oldKey, newKey);
                }
            }
        }

        /// <summary>
        /// Attempts to rename a sub-key to the key provided using the specified old
        /// name and new name.
        /// </summary>
        /// <param name="sourceKey">The source key to copy from.</param>
        /// <param name="destKey">The destination key to copy to.</param>
        private static void RecursiveCopyKey(RegistryKey sourceKey, RegistryKey destKey)
        {

            //Copy all of the registry values
            foreach (string valueName in sourceKey.GetValueNames())
            {
                object valueObj = sourceKey.GetValue(valueName);
                RegistryValueKind valueKind = sourceKey.GetValueKind(valueName);
                destKey.SetValue(valueName, valueObj, valueKind);
            }

            //Copy all of the subkeys
            foreach (string subKeyName in sourceKey.GetSubKeyNames())
            {
                using (RegistryKey sourceSubkey = sourceKey.OpenSubKey(subKeyName))
                {
                    using (RegistryKey destSubKey = destKey.CreateSubKey(subKeyName))
                    {
                        //Recursive call to copy the sub key data
                        RecursiveCopyKey(sourceSubkey, destSubKey);
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to set a registry value for the key provided using the specified
        /// name, data and kind. If the registry value does not exist it will be created
        /// </summary>
        /// <param name="key">The key of which the value is to be set for.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="data">The data of the value</param>
        /// <param name="kind">The value kind of the value</param>
        /// <returns>Returns <c>true</c> if the action succeeded, otherwise <c>false</c>.</returns>
        public static bool SetValueSafe(this RegistryKey key, string name, object data, RegistryValueKind kind)
        {
            try
            {
                // handle type conversion
                if (kind != RegistryValueKind.Binary && data.GetType() == typeof(byte[]))
                {
                    switch (kind)
                    {
                        case RegistryValueKind.String:
                        case RegistryValueKind.ExpandString:
                            data = ByteConverter.ToString((byte[]) data);
                            break;
                        case RegistryValueKind.DWord:
                            data = ByteConverter.ToUInt32((byte[]) data);
                            break;
                        case RegistryValueKind.QWord:
                            data = ByteConverter.ToUInt64((byte[]) data);
                            break;
                        case RegistryValueKind.MultiString:
                            data = ByteConverter.ToStringArray((byte[]) data);
                            break;
                    }
                }
                key.SetValue(name, data, kind);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to delete a registry value for the key provided using the specified
        /// name.
        /// </summary>
        /// <param name="key">The key of which the value is to be delete from.</param>
        /// <param name="name">The name of the value.</param>
        /// <returns>Returns <c>true</c> if the action succeeded, otherwise <c>false</c>.</returns>
        public static bool DeleteValueSafe(this RegistryKey key, string name)
        {
            try
            {
                key.DeleteValue(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to rename a registry value to the key provided using the specified old
        /// name and new name.
        /// </summary>
        /// <param name="key">The key of which the registry value is to be renamed from.</param>
        /// <param name="oldName">The old name of the registry value.</param>
        /// <param name="newName">The new name of the registry value.</param>
        /// <returns>Returns <c>true</c> if the action succeeded, otherwise <c>false</c>.</returns>
        public static bool RenameValueSafe(this RegistryKey key, string oldName, string newName)
        {
            try
            {
                //Copy from old to new
                key.CopyValue(oldName, newName);
                //Dispose of the old value
                key.DeleteValue(oldName);
                return true;
            }
            catch
            {
                //Try to dispose of the newKey (The rename failed)
                key.DeleteValueSafe(newName);
                return false;
            }
        }

        /// <summary>
        /// Attempts to copy a old registry value to a new registry value for the key 
        /// provided using the specified old name and new name. (throws exceptions)
        /// </summary>
        /// <param name="key">The key of which the registry value is to be copied.</param>
        /// <param name="oldName">The old name of the registry value.</param>
        /// <param name="newName">The new name of the registry value.</param>
        public static void CopyValue(this RegistryKey key, string oldName, string newName)
        {
            RegistryValueKind valueKind = key.GetValueKind(oldName);
            object valueData = key.GetValue(oldName);

            key.SetValue(newName, valueData, valueKind);
        }

        /// <summary>
        /// Checks if the specified subkey exists in the key
        /// </summary>
        /// <param name="key">The key of which to search.</param>
        /// <param name="name">The name of the sub-key to find.</param>
        /// <returns>Returns <c>true</c> if the action succeeded, otherwise <c>false</c>.</returns>
        public static bool ContainsSubKey(this RegistryKey key, string name)
        {
            foreach (string subkey in key.GetSubKeyNames())
            {
                if (subkey == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if the specified registry value exists in the key
        /// </summary>
        /// <param name="key">The key of which to search.</param>
        /// <param name="name">The name of the registry value to find.</param>
        /// <returns>Returns <c>true</c> if the action succeeded, otherwise <c>false</c>.</returns>
        public static bool ContainsValue(this RegistryKey key, string name)
        {
            foreach (string value in key.GetValueNames())
            {
                if (value == name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets all of the value names associated with the registry key and returns
        /// formatted strings of the filtered values.
        /// </summary>
        /// <param name="key">The registry key of which the values are obtained.</param>
        /// <returns>Yield returns formatted strings of the key and the key value.</returns>
        public static IEnumerable<Tuple<string, string>> GetKeyValues(this RegistryKey key)
        {
            if (key == null) yield break;

            foreach (var k in key.GetValueNames().Where(keyVal => !keyVal.IsNameOrValueNull(key)).Where(k => !string.IsNullOrEmpty(k)))
            {
                yield return new Tuple<string, string>(k, key.GetValueSafe(k));
            }
        }

        /// <summary>
        /// Gets the default value for a given data type of a registry value.
        /// </summary>
        /// <param name="valueKind">The data type of the registry value.</param>
        /// <returns>The default value for the given <see cref="valueKind"/>.</returns>
        public static object GetDefault(this RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.Binary:
                    return new byte[] {};
                case RegistryValueKind.MultiString:
                    return new string[] {};
                case RegistryValueKind.DWord:
                    return 0;
                case RegistryValueKind.QWord:
                    return (long)0;
                case RegistryValueKind.String:
                case RegistryValueKind.ExpandString:
                    return "";
                default:
                    return null;
            }
        }
    }
}
