using System.Security.Cryptography;
using System.Text;

namespace Quasar.Common.Cryptography
{
    public static class Sha256
    {
        public static string ComputeHash(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);

            using (SHA256Managed sha = new SHA256Managed())
            {
                data = sha.ComputeHash(data);
            }

            StringBuilder hash = new StringBuilder();

            foreach (byte _byte in data)
                hash.Append(_byte.ToString("X2"));

            return hash.ToString().ToUpper();
        }

        public static byte[] ComputeHash(byte[] input)
        {
            using (SHA256Managed sha = new SHA256Managed())
            {
                return sha.ComputeHash(input);
            }
        }
    }
}
