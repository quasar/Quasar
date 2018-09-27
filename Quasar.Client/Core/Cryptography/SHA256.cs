using System.Security.Cryptography;
using System.Text;

namespace xClient.Core.Cryptography
{
    public static class SHA256
    {
        public static string ComputeHash(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);

            using (SHA256Managed sha = new SHA256Managed())
            {
                data = sha.ComputeHash(data, 0, data.Length);
            }

            StringBuilder hash = new StringBuilder();

            foreach (byte _byte in data)
                hash.Append(_byte.ToString("X2"));

            return hash.ToString().ToUpper();
        }
    }
}