using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xServer.Core.Compression;

namespace xServer.Tests.Core.Compression
{
    [TestClass]
    public class SafeQuickLZTests
    {
        // Tests using pseudo-randomly generated data.
        #region Random Data

        /*
         * Purpose: To validate a small amount of data after compression/decompression
         *          using SafeQuickLZ with level 1 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void SmallDataTestLevel1()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] SmallData = new byte[100];

            // Fill the small data array with random data.
            new Random().NextBytes(SmallData);

            // Store the compressed data.
            byte[] SmallDataCompressed = safeQuickLZtest.Compress(SmallData, 0, SmallData.Length, 1);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(SmallData, SmallDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] SmallDataDecompressed = safeQuickLZtest.Decompress(SmallDataCompressed, 0, SmallDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(SmallDataCompressed, SmallDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(SmallData, SmallDataDecompressed, "Original data does not match the decompressed data!");
        }

        /*
         * Purpose: To validate a small amount of data after compression/decompression
         *          using SafeQuickLZ with level 3 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void SmallDataTestLevel3()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] SmallData = new byte[100];

            // Fill the small data array with random data.
            new Random().NextBytes(SmallData);

            // Store the compressed data.
            byte[] SmallDataCompressed = safeQuickLZtest.Compress(SmallData, 0, SmallData.Length, 3);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(SmallData, SmallDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] SmallDataDecompressed = safeQuickLZtest.Decompress(SmallDataCompressed, 0, SmallDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(SmallDataCompressed, SmallDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(SmallData, SmallDataDecompressed, "Original data does not match the decompressed data!");
        }

        /*
         * Purpose: To validate a large amount of data after compression/decompression
         *          using SafeQuickLZ with level 1 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void BigDataTestLevel1()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] BigData = new byte[100000];

            // Fill the big data array with random data.
            new Random().NextBytes(BigData);

            // Store the compressed data.
            byte[] BigDataCompressed = safeQuickLZtest.Compress(BigData, 0, BigData.Length, 1);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(BigData, BigDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] BigDataDecompressed = safeQuickLZtest.Decompress(BigDataCompressed, 0, BigDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(BigDataCompressed, BigDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(BigData, BigDataDecompressed, "Original data does not match the decompressed data!");
        }

        /*
         * Purpose: To validate a large amount of data after compression/decompression
         *          using SafeQuickLZ with level 3 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void BigDataTestLevel3()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] BigData = new byte[100000];

            // Fill the big data array with random data.
            new Random().NextBytes(BigData);

            // Store the compressed data.
            byte[] BigDataCompressed = safeQuickLZtest.Compress(BigData, 0, BigData.Length, 3);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(BigData, BigDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] BigDataDecompressed = safeQuickLZtest.Decompress(BigDataCompressed, 0, BigDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(BigDataCompressed, BigDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(BigData, BigDataDecompressed, "Original data does not match the decompressed data!");
        }

        #endregion
    }
}