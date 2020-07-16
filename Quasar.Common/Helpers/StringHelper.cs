using Quasar.Common.Utilities;
using System.Text;
using System.Text.RegularExpressions;

namespace Quasar.Common.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Available alphabet for generation of random strings.
        /// </summary>
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// Abbreviations of file sizes.
        /// </summary>
        private static readonly string[] Sizes = { "B", "KB", "MB", "GB", "TB", "PB" };

        /// <summary>
        /// Random number generator.
        /// </summary>
        private static readonly SafeRandom Random = new SafeRandom();

        /// <summary>
        /// Gets a random string with given length.
        /// </summary>
        /// <param name="length">The length of the random string.</param>
        /// <returns>A random string.</returns>
        public static string GetRandomString(int length)
        {
            StringBuilder randomName = new StringBuilder(length);
            for (int i = 0; i < length; i++)
                randomName.Append(Alphabet[Random.Next(Alphabet.Length)]);

            return randomName.ToString();
        }

        /// <summary>
        /// Gets the human readable file size for a given size.
        /// </summary>
        /// <param name="size">The file size in bytes.</param>
        /// <returns>The human readable file size.</returns>
        public static string GetHumanReadableFileSize(long size)
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
        /// Gets the formatted MAC address.
        /// </summary>
        /// <param name="macAddress">The unformatted MAC address.</param>
        /// <returns>The formatted MAC address.</returns>
        public static string GetFormattedMacAddress(string macAddress)
        {
            return (macAddress.Length != 12)
                ? "00:00:00:00:00:00"
                : Regex.Replace(macAddress, "(.{2})(.{2})(.{2})(.{2})(.{2})(.{2})", "$1:$2:$3:$4:$5:$6");
        }

        /// <summary>
        /// Safely removes the last N chars from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="amount">The amount of last chars to remove (=N).</param>
        /// <returns>The input string with N removed chars.</returns>
        public static string RemoveLastChars(string input, int amount = 2)
        {
            if (input.Length > amount)
                input = input.Remove(input.Length - amount);
            return input;
        }
    }
}
