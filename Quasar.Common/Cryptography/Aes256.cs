using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Quasar.Common.Cryptography
{
    public class Aes256
    {
        private const int KeyLength = 32;
        private const int AuthKeyLength = 64;
        private const int IvLength = 16;
        private const int HmacSha256Length = 32;
        private readonly byte[] _key;
        private readonly byte[] _authKey;

        private static readonly byte[] Salt =
        {
            0xBF, 0xEB, 0x1E, 0x56, 0xFB, 0xCD, 0x97, 0x3B, 0xB2, 0x19, 0x2, 0x24, 0x30, 0xA5, 0x78, 0x43, 0x0, 0x3D, 0x56,
            0x44, 0xD2, 0x1E, 0x62, 0xB9, 0xD4, 0xF1, 0x80, 0xE7, 0xE6, 0xC3, 0x39, 0x41
        };

        public Aes256(string masterKey)
        {
            if (string.IsNullOrEmpty(masterKey))
                throw new ArgumentException($"{nameof(masterKey)} can not be null or empty.");

            using (Rfc2898DeriveBytes derive = new Rfc2898DeriveBytes(masterKey, Salt, 50000))
            {
                _key = derive.GetBytes(KeyLength);
                _authKey = derive.GetBytes(AuthKeyLength);
            }
        }

        public string Encrypt(string input)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(input)));
        }

        /* FORMAT
         * ----------------------------------------
         * |     HMAC     |   IV   |  CIPHERTEXT  |
         * ----------------------------------------
         *     32 bytes    16 bytes
         */
        public byte[] Encrypt(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException($"{nameof(input)} can not be null.");

            using (var ms = new MemoryStream())
            {
                ms.Position = HmacSha256Length; // reserve first 32 bytes for HMAC
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.KeySize = 256;
                    aesProvider.BlockSize = 128;
                    aesProvider.Mode = CipherMode.CBC;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    aesProvider.Key = _key;
                    aesProvider.GenerateIV();

                    using (var cs = new CryptoStream(ms, aesProvider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        ms.Write(aesProvider.IV, 0, aesProvider.IV.Length); // write next 16 bytes the IV, followed by ciphertext
                        cs.Write(input, 0, input.Length);
                        cs.FlushFinalBlock();

                        using (var hmac = new HMACSHA256(_authKey))
                        {
                            byte[] hash = hmac.ComputeHash(ms.ToArray(), HmacSha256Length, ms.ToArray().Length - HmacSha256Length); // compute the HMAC of IV and ciphertext
                            ms.Position = 0; // write hash at beginning
                            ms.Write(hash, 0, hash.Length);
                        }
                    }
                }

                return ms.ToArray();
            }
        }

        public string Decrypt(string input)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(input)));
        }

        public byte[] Decrypt(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException($"{nameof(input)} can not be null.");

            using (var ms = new MemoryStream(input))
            {
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.KeySize = 256;
                    aesProvider.BlockSize = 128;
                    aesProvider.Mode = CipherMode.CBC;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    aesProvider.Key = _key;

                    // read first 32 bytes for HMAC
                    using (var hmac = new HMACSHA256(_authKey))
                    {
                        var hash = hmac.ComputeHash(ms.ToArray(), HmacSha256Length, ms.ToArray().Length - HmacSha256Length);
                        byte[] receivedHash = new byte[HmacSha256Length];
                        ms.Read(receivedHash, 0, receivedHash.Length);

                        if (!SafeComparison.AreEqual(hash, receivedHash))
                            throw new CryptographicException("Invalid message authentication code (MAC).");
                    }

                    byte[] iv = new byte[IvLength];
                    ms.Read(iv, 0, IvLength); // read next 16 bytes for IV, followed by ciphertext
                    aesProvider.IV = iv;

                    using (var cs = new CryptoStream(ms, aesProvider.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] temp = new byte[ms.Length - IvLength + 1];
                        byte[] data = new byte[cs.Read(temp, 0, temp.Length)];
                        Buffer.BlockCopy(temp, 0, data, 0, data.Length);
                        return data;
                    }
                }
            }
        }
    }
}
