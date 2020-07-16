using Quasar.Common.Models;
using System.Collections.Generic;

namespace Quasar.Client.Recovery
{
    /// <summary>
    /// Provides a common way to read stored accounts from applications.
    /// </summary>
    public interface IAccountReader
    {
        /// <summary>
        /// Reads the stored accounts of the application.
        /// </summary>
        /// <returns>A list of recovered accounts</returns>
        IEnumerable<RecoveredAccount> ReadAccounts();

        /// <summary>
        /// The name of the application.
        /// </summary>
        string ApplicationName { get; }
    }
}
