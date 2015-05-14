using System.Linq;
using System.Text;
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
            var inputByte = Encoding.UTF8.GetBytes(input);

            var password = Helper.GetRandomName(50);
            var passwordByte = Encoding.UTF8.GetBytes(password);
            var encrypted = AES.Encrypt(inputByte, passwordByte);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(encrypted, input);

            var decrypted = AES.Decrypt(encrypted, passwordByte);

            //The decryption method is adding on blank bytes.
            var realDecrypted = decrypted.Take(100).ToArray();

            CollectionAssert.AreEqual(inputByte,realDecrypted);
        }
    }
}