using System.Security.Cryptography;
using System.Text;

namespace xClient.Core.Encryption
{
    public static class SHA256
    {
        public static string ComputeHash(string input)
        {
            var data = Encoding.UTF8.GetBytes(input);

            using (var sha = new SHA256Managed())
            {
                data = sha.ComputeHash(data, 0, data.Length);
            }

            var hash = new StringBuilder();

            foreach (var _byte in data)
                hash.Append(_byte.ToString("X2"));

            return hash.ToString().ToUpper();
        }
    }
}