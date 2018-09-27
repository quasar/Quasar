using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Helper;

namespace xClient.Tests.Core.Helper
{
    [TestClass]
    public class FileHelperTests
    {
        [TestMethod, TestCategory("Helper")]
        public void RandomFilenameTest()
        {
            int length = 100;
            var name = FileHelper.GetRandomFilename(length);
            Assert.IsNotNull(name);
            Assert.IsTrue(name.Length == length, "Filename has wrong length!");
        }

        [TestMethod, TestCategory("Helper")]
        public void ValidateExecutableTest()
        {
            var bytes = new byte[] {77, 90};

            var result = FileHelper.IsValidExecuteableFile(bytes);

            Assert.IsTrue(result, "Validating a .exe file failed!");
        }

        [TestMethod, TestCategory("Helper")]
        public void ValidateInvalidFileTest()
        {
            var bytes = new byte[] {22, 93};

            var result = FileHelper.IsValidExecuteableFile(bytes);

            Assert.IsFalse(result, "Validating an invalid file worked!");
        }
    }
}