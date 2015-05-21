using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace xClient.Core.Helper
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

        public static Bitmap GetDesktop(int screenNumber)
        {
            var bounds = Screen.AllScreens[screenNumber].Bounds;
            var screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            using (Graphics graph = Graphics.FromImage(screenshot))
            {
                graph.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
                return screenshot;
            }
        }

        public static bool IsWindowsXP()
        {
            var osVersion = Environment.OSVersion.Version;
            return osVersion.Major == 5 && osVersion.Minor >= 1;
        }

        public static string FormatMacAddress(string macAddress)
        {
            return (macAddress.Length != 12)
                ? "00:00:00:00:00:00"
                : Regex.Replace(macAddress, "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})", "$1:$2:$3:$4:$5:$6");
        }
    }
}