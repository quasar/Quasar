using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Encryption;
using xClient.Core.Helper;

namespace xClientTests.Core.Encryption
{
    [TestClass]
    public class SHA256Tests
    {
        [TestMethod]
        public void ComputeHashTest()
        {
            var input = Helper.GetRandomName(100);
            var result = SHA256.ComputeHash(input);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(result, input);
        }
    }
}
