using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xServer.Core.Encryption;
using xServer.Core.Helper;

namespace xServer.Tests.Core.Encryption
{
    [TestClass]
    public class AESTests
    {
        [TestMethod, TestCategory("Encryption")]
        public void EncryptAndDecryptStringTest()
        {
            var input = FileHelper.GetRandomFilename(100);
            var password = FileHelper.GetRandomFilename(50);
            var encrypted = AES.Encrypt(input, password);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(encrypted, input);

            var decrypted = AES.Decrypt(encrypted, password);

            Assert.AreEqual(input, decrypted);
        }

        [TestMethod, TestCategory("Encryption")]
        public void EncryptAndDecryptByteArrayTest()
        {
            var input = FileHelper.GetRandomFilename(100);
            var inputByte = Encoding.UTF8.GetBytes(input);

            var passwordByte = Encoding.UTF8.GetBytes(FileHelper.GetRandomFilename(50));
            var encryptedByte = AES.Encrypt(inputByte, passwordByte);

            Assert.IsNotNull(encryptedByte);
            CollectionAssert.AllItemsAreNotNull(encryptedByte);
            CollectionAssert.AreNotEqual(encryptedByte, inputByte);

            var decryptedByte = AES.Decrypt(encryptedByte, passwordByte);

            CollectionAssert.AreEqual(inputByte, decryptedByte);
        }
    }
}