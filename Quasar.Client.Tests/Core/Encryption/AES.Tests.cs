using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Cryptography;
using xClient.Core.Helper;

namespace xClient.Tests.Core.Encryption
{
    [TestClass]
    public class AESTests
    {
        [TestMethod, TestCategory("Encryption")]
        public void EncryptAndDecryptStringTest()
        {
            var input = FileHelper.GetRandomFilename(100);
            var password = FileHelper.GetRandomFilename(50);

            AES.SetDefaultKey(password);

            var encrypted = AES.Encrypt(input);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(encrypted, input);

            var decrypted = AES.Decrypt(encrypted);

            Assert.AreEqual(input, decrypted);
        }

        [TestMethod, TestCategory("Encryption")]
        public void EncryptAndDecryptByteArrayTest()
        {
            var input = FileHelper.GetRandomFilename(100);
            var inputByte = Encoding.UTF8.GetBytes(input);
            var password = FileHelper.GetRandomFilename(50);

            AES.SetDefaultKey(password);

            var encryptedByte = AES.Encrypt(inputByte);

            Assert.IsNotNull(encryptedByte);
            CollectionAssert.AllItemsAreNotNull(encryptedByte);
            CollectionAssert.AreNotEqual(encryptedByte, inputByte);

            var decryptedByte = AES.Decrypt(encryptedByte);

            CollectionAssert.AreEqual(inputByte, decryptedByte);
        }
    }
}