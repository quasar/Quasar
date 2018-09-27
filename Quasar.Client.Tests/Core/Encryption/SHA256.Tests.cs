using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasar.Client.Core.Cryptography;
using Quasar.Client.Core.Helper;

namespace Quasar.Client.Tests.Core.Encryption
{
    [TestClass]
    public class SHA256Tests
    {
        [TestMethod, TestCategory("Encryption")]
        public void ComputeHashTest()
        {
            var input = FileHelper.GetRandomFilename(100);
            var result = SHA256.ComputeHash(input);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result, input);
        }
    }
}
