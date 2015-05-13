using Microsoft.VisualStudio.TestTools.UnitTesting;
using xServer.Core.Encryption;
using xServer.Core.Helper;

namespace xServer.Tests.Core.Encryption
{
    [TestClass]
    public class AESTests
    {
        [TestMethod]
        public void EncryptAndDecryptTest()
        {
            var input = Helper.GetRandomName(100);
            var password = Helper.GetRandomName(50);
            var encrypted = AES.Encrypt(input, password);

            Assert.IsNotNull(encrypted);
            Assert.AreNotEqual(encrypted, input);

            var decrypted = AES.Decrypt(encrypted, password);

            Assert.IsTrue(input == decrypted);
        }
    }
}