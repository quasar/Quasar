using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Compression;
using xClient.Core.Helper;

namespace xClient.Tests.Core.Compression
{
    [TestClass]
    public class JpgCompressionTests
    {
        [TestMethod]
        public void EncryptAndDecryptStringTest()
        {
            var quality = Int64.MaxValue;
            var jpg = new JpgCompression(quality);
            var bitmap = new Bitmap(200, 200);

            var result = jpg.Compress(bitmap);

            Assert.IsNotNull(result);
            CollectionAssert.AllItemsAreNotNull(result);
        }
    }
}