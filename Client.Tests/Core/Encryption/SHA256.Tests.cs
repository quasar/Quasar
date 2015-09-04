using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Cryptography;
using xClient.Core.Helper;

namespace xClient.Tests.Core.Encryption
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
