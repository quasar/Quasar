using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace xServer.Core.Helper
{
    public static class Helper
    {
        private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private static readonly Random _rnd = new Random(Environment.TickCount);

        public static string GetRandomFilename(int length, string extension)
        {
            StringBuilder randomName = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                randomName.Append(CHARS[_rnd.Next(CHARS.Length)]);

            return string.Concat(randomName.ToString(), extension);
        }

        public static string GetRandomName(int length)
        {
            StringBuilder randomName = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                randomName.Append(CHARS[_rnd.Next(CHARS.Length)]);

            return randomName.ToString();
        }

        public static string GetFileSize(long size)
        {
            string[] sizes = {"B", "KB", "MB", "GB"};
            double len = size;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len/1024;
            }
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }

        public static int GetFileIcon(string extension)
        {
            if (string.IsNullOrEmpty(extension))
                return 2;

            switch (extension.ToLower())
            {
                default:
                    return 2;
                case ".exe":
                    return 3;
                case ".txt":
                    return 4;
                case ".rar":
                case ".zip":
                case ".zipx":
                case ".tar":
                case ".tgz":
                case ".s7z":
                case ".7z":
                case ".bz2":
                case ".cab":
                case ".zz":
                    return 5;
                case ".doc":
                case ".docx":
                case ".odt":
                    return 6;
                case ".pdf":
                    return 7;
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".gif":
                    return 8;
                case ".mp4":
                case ".mov":
                case ".avi":
                case ".wmv":
                    return 9;
                case ".mp3":
                case ".wav":
                    return 10;
            }
        }
    }
}