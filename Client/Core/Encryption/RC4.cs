using System.Text;

namespace Core.Encryption
{
    class RC4
    {
        public static byte[] Encrypt(byte[] input, string key)
        {
            byte[] bKey = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] s = new byte[256];
            byte[] k = new byte[256];
            byte temp;
            int i, j;

            for (i = 0; i < 256; i++)
            {
                s[i] = (byte)i;
                k[i] = bKey[i % bKey.GetLength(0)];
            }

            j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + s[i] + k[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
            }

            i = j = 0;
            for (int x = 0; x < input.GetLength(0); x++)
            {
                i = (i + 1) % 256;
                j = (j + s[i]) % 256;
                temp = s[i];
                s[i] = s[j];
                s[j] = temp;
                int t = (s[i] + s[j]) % 256;
                input[x] ^= s[t];
            }
            return input;
        }

        public static byte[] Decrypt(byte[] input, string key)
        {
            return Encrypt(input, key);
        }
    }
}
