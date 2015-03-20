using System.Security.Cryptography;
using System.Text;

namespace xClient.Core.Encryption
{
    public static class SHA256
    {
        public static string ComputeHash(string input)
        {
            string hash = string.Empty;
            byte[] data = Encoding.UTF8.GetBytes(input);

            using (SHA256Managed sha = new SHA256Managed())
            {
                data = sha.ComputeHash(data, 0, data.Length);
            }

            foreach (byte _byte in data)
                hash += _byte.ToString("X2");

            return hash.ToUpper();
        }
    }
}
