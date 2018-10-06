using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Quasar.Common.Cryptography
{
    public static class Aes128
    {
        private const int KeyLength = 16;
        private const int AuthKeyLength = 64;
        private const int IvLength = 16;
        private const int HmacSha256Length = 32;
        private static byte[] _defaultKey;
        private static byte[] _defaultAuthKey;

        public static readonly byte[] Salt =
        {
            0xBF, 0xEB, 0x1E, 0x56, 0xFB, 0xCD, 0x97, 0x3B, 0xB2, 0x19, 0x2, 0x24, 0x30, 0xA5, 0x78, 0x43, 0x0, 0x3D, 0x56,
            0x44, 0xD2, 0x1E, 0x62, 0xB9, 0xD4, 0xF1, 0x80, 0xE7, 0xE6, 0xC3, 0x39, 0x41
        };

        public static void SetDefaultKey(string password)
        {
            var keys = DeriveKeys(password);
            _defaultKey = keys.Item1;
            if (_defaultKey == null || _defaultKey.Length != KeyLength) throw new ArgumentException($"Key must be {KeyLength} bytes.");
            _defaultAuthKey = keys.Item2;
            if (_defaultAuthKey == null || _defaultAuthKey.Length != AuthKeyLength) throw new ArgumentException($"Auth key must be {AuthKeyLength} bytes.");
        }

        public static void SetDefaultKey(string key, string authKey)
        {
            _defaultKey = Convert.FromBase64String(key);
            if (_defaultKey == null || _defaultKey.Length != KeyLength) throw new ArgumentException($"Key must be {KeyLength} bytes.");
            _defaultAuthKey = Convert.FromBase64String(authKey);
            if (_defaultAuthKey == null || _defaultAuthKey.Length != AuthKeyLength) throw new ArgumentException($"Auth key must be {AuthKeyLength} bytes.");
        }

        public static Tuple<byte[], byte[]> DeriveKeys(string password)
        {
            using (Rfc2898DeriveBytes derive = new Rfc2898DeriveBytes(password, Salt, 50000))
            {
                return new Tuple<byte[], byte[]>(derive.GetBytes(KeyLength), derive.GetBytes(AuthKeyLength));
            }
        }

        public static string Encrypt(string input)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(input)));
        }

        /* FORMAT
         * ----------------------------------------
         * |     HMAC     |   IV   |  CIPHERTEXT  |
         * ----------------------------------------
         *     32 bytes    16 bytes
         */
        public static byte[] Encrypt(byte[] input)
        {
            if (_defaultKey == null || _defaultKey.Length == 0) throw new ArgumentException("Key can not be empty.");
            if (_defaultAuthKey == null || _defaultAuthKey.Length == 0) throw new ArgumentException("Auth key can not be empty.");
            if (input == null || input.Length == 0) throw new ArgumentException("Input can not be empty.");

            using (var ms = new MemoryStream())
            {
                ms.Position = HmacSha256Length; // reserve first 32 bytes for HMAC
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.KeySize = 128;
                    aesProvider.BlockSize = 128;
                    aesProvider.Mode = CipherMode.CBC;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    aesProvider.Key = _defaultKey;
                    aesProvider.GenerateIV();

                    using (var cs = new CryptoStream(ms, aesProvider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        ms.Write(aesProvider.IV, 0, aesProvider.IV.Length); // write next 16 bytes the IV, followed by ciphertext
                        cs.Write(input, 0, input.Length);
                        cs.FlushFinalBlock();

                        using (var hmac = new HMACSHA256(_defaultAuthKey))
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

        public static string Encrypt(string input, string password)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(input), password));
        }

        public static byte[] Encrypt(byte[] input, string password)
        {
            if (string.IsNullOrEmpty(password)) throw new Exception("Password can not be empty.");

            var keys = DeriveKeys(password);
            byte[] key = keys.Item1;
            byte[] authKey = keys.Item2;

            using (var ms = new MemoryStream())
            {
                ms.Position = HmacSha256Length; // reserve first 32 bytes for HMAC
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.KeySize = 128;
                    aesProvider.BlockSize = 128;
                    aesProvider.Mode = CipherMode.CBC;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    aesProvider.Key = key;
                    aesProvider.GenerateIV();

                    using (var cs = new CryptoStream(ms, aesProvider.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        ms.Write(aesProvider.IV, 0, aesProvider.IV.Length); // write next 16 bytes the IV, followed by ciphertext
                        cs.Write(input, 0, input.Length);
                        cs.FlushFinalBlock();

                        using (var hmac = new HMACSHA256(authKey))
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

        public static string Decrypt(string input)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(input)));
        }

        public static byte[] Decrypt(byte[] input)
        {
            if (_defaultKey == null || _defaultKey.Length == 0) throw new ArgumentException("Key can not be empty.");
            if (_defaultAuthKey == null || _defaultAuthKey.Length == 0) throw new ArgumentException("Auth key can not be empty.");
            if (input == null || input.Length == 0) throw new ArgumentException("Input can not be empty.");

            using (var ms = new MemoryStream(input))
            {
                using (var aesProvider = new AesCryptoServiceProvider())
                {
                    aesProvider.KeySize = 128;
                    aesProvider.BlockSize = 128;
                    aesProvider.Mode = CipherMode.CBC;
                    aesProvider.Padding = PaddingMode.PKCS7;
                    aesProvider.Key = _defaultKey;

                    // read first 32 bytes for HMAC
                    using (var hmac = new HMACSHA256(_defaultAuthKey))
                    {
                        var hash = hmac.ComputeHash(ms.ToArray(), HmacSha256Length, ms.ToArray().Length - HmacSha256Length);
                        byte[] receivedHash = new byte[HmacSha256Length];
                        ms.Read(receivedHash, 0, receivedHash.Length);

                        if (!AreEqual(hash, receivedHash))
                            throw new CryptographicException("Invalid message authentication code (MAC).");
                    }

                    byte[] iv = new byte[IvLength];
                    ms.Read(iv, 0, IvLength); // read next 16 bytes for IV, followed by ciphertext
                    aesProvider.IV = iv;

                    using (var cs = new CryptoStream(ms, aesProvider.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] data = new byte[ms.Length - IvLength + 1];
                        cs.Read(data, 0, data.Length);
                        return data;
                    }
                }
            }
        }

        /// <summary>
        /// Compares two byte arrays for equality.
        /// </summary>
        /// <param name="a1">Byte array to compare</param>
        /// <param name="a2">Byte array to compare</param>
        /// <returns>True if equal, else false</returns>
        /// <remarks>
        /// Assumes that the byte arrays have the same length.
        /// This method is safe against timing attacks.
        /// </remarks>
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool AreEqual(byte[] a1, byte[] a2)
        {
            bool result = true;
            for (int i = 0; i < a1.Length; ++i)
            {
                if (a1[i] != a2[i])
                    result = false;
            }
            return result;
        }
    }
}
