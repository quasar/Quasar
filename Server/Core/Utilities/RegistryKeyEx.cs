using System;
using System.Linq;
using Microsoft.Win32;

namespace xServer.Core.Utilities
{
    public class RegistryKeyEx : IComparable<RegistryKeyEx>, IDisposable
    {
        #region IComparable<RegistryKeyEx> Implementation

        /// <summary>
        /// Sorts RegistryKeys based on their names.
        /// </summary>
        /// <param name="obj">The RegistryKeyEx to compare.</param>
        /// <returns>Returns </returns>
        public int CompareTo(RegistryKeyEx obj)
        {
            if (obj == null)
            {
                return 1;
            }
            else
            {
                RegistryKeyEx regKeyEx = obj as RegistryKeyEx;

                if (regKeyEx == null)
                {
                    throw new ArgumentException("Uncomparable Object: Object is not a RegistryKeyEx");
                }
                else
                {
                    return this.Name.CompareTo(regKeyEx.Name);
                }
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Key != null)
                {
                    Key.Close();
                }
            }
        }

        #endregion

        public RegistryKey Key { get; private set; }

        public string Name { get; private set; }
        public string ParentName { get; private set; }

        public RegistryKeyEx(RegistryKey key)
        {
            this.Key = key;
            this.Name = (key.Name.Contains('\\') ? key.Name.Substring(key.Name.LastIndexOf('\\')) : key.Name);
            // Determine if it is a node without a parent. If it is not, then store the parent's name.
            int index = key.Name.Length - this.Name.Length - 1;
            if (index > 0)
            {
                ParentName = key.Name.Substring(0, index);
            }
        }
    }
}