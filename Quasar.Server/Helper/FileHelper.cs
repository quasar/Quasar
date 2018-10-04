using System.IO;
using System.Linq;
using System.Text;
using Quasar.Common.Cryptography;
using Quasar.Common.Helpers;
using Quasar.Common.Utilities;

namespace Quasar.Server.Helper
{
    public static class FileHelper
    {
        private static readonly SafeRandom Random = new SafeRandom();
        private static readonly string[] Sizes = {"B", "KB", "MB", "GB", "TB", "PB"};
        private static readonly char[] IllegalChars = Path.GetInvalidPathChars().Union(Path.GetInvalidFileNameChars()).ToArray();

        public static bool CheckPathForIllegalChars(string path)
        {
            return path.Any(c => IllegalChars.Contains(c));
        }

        public static string GetRandomFilename(int length, string extension = "")
        {
            return string.Concat(StringHelper.GetRandomString(length), extension);
        }

        public static int GetNewTransferId(int o = 0)
        {
            return Random.Next(0, int.MaxValue) + o;
        }

        public static string GetDataSize(long size)
        {
            double len = size;
            int order = 0;
            while (len >= 1024 && order + 1 < Sizes.Length)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {Sizes[order]}";
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
                byte[] data = Aes128.Encrypt(Encoding.UTF8.GetBytes(appendText));
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
            return File.Exists(filename) ? Encoding.UTF8.GetString(Aes128.Decrypt(File.ReadAllBytes(filename))) : string.Empty;
        }
    }
}
