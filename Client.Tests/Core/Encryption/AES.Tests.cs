using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Encryption;
using xClient.Core.Helper;

namespace xClient.Tests.Core.Encryption
{
    [TestClass]
    public class AESTests
    {
        [TestMethod]
        public void EncryptAndDecryptStringTest()
        {
            var input = Helper.GetRandomName(100);
            var password = Helper.GetRandomName(50);
            var encrypted = AES.Encrypt(input, password);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(encrypted, input);

            var decrypted = AES.Decrypt(encrypted, password);

            Assert.IsTrue(input == decrypted);
        }

        [TestMethod]
        public void EncryptAndDecryptByteArrayTest()
        {
            var input = Helper.GetRandomName(100);
            var inputByte = GetBytes(input);

            var password = Helper.GetRandomName(50);
            var passwordByte = GetBytes(password);
            var encrypted = AES.Encrypt(inputByte, passwordByte);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(encrypted, input);

            var decrypted = AES.Decrypt(encrypted, passwordByte);

            //The decryption method is adding on 9 blank bytes.
            var realDecrypted = decrypted.Take(200).ToArray();

            CollectionAssert.AreEqual(inputByte,realDecrypted);
        }


        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

    }
}