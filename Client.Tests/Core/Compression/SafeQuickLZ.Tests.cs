using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Compression;

namespace xClient.Tests.Core.Compression
{
    [TestClass]
    public class SafeQuickLZTests
    {
        /*
         * Purpose: To validate a small amount of data after compression/decompression
         *          using SafeQuickLZ with level 1 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void SmallDataCompressionTestLevel1()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] smallData = new byte[100];

            // Fill the small data array with random data.
            new Random().NextBytes(smallData);

            // Store the compressed data.
            byte[] smallDataCompressed = safeQuickLZtest.Compress(smallData, 0, smallData.Length, 1);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(smallData, smallDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] smallDataDecompressed = safeQuickLZtest.Decompress(smallDataCompressed, 0, smallDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(smallDataCompressed, smallDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(smallData, smallDataDecompressed, "Original data does not match the decompressed data!");
        }

        /*
         * Purpose: To validate a small amount of data after compression/decompression
         *          using SafeQuickLZ with level 3 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void SmallDataCompressionTestLevel3()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] smallData = new byte[100];

            // Fill the small data array with random data.
            new Random().NextBytes(smallData);

            // Store the compressed data.
            byte[] smallDataCompressed = safeQuickLZtest.Compress(smallData, 0, smallData.Length, 3);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(smallData, smallDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] smallDataDecompressed = safeQuickLZtest.Decompress(smallDataCompressed, 0, smallDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(smallDataCompressed, smallDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(smallData, smallDataDecompressed, "Original data does not match the decompressed data!");
        }

        /*
         * Purpose: To validate a large amount of data after compression/decompression
         *          using SafeQuickLZ with level 1 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void BigDataCompressionTestLevel1()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] bigData = new byte[100000];

            // Fill the big data array with random data.
            new Random().NextBytes(bigData);

            // Store the compressed data.
            byte[] bigDataCompressed = safeQuickLZtest.Compress(bigData, 0, bigData.Length, 1);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(bigData, bigDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] bigDataDecompressed = safeQuickLZtest.Decompress(bigDataCompressed, 0, bigDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(bigDataCompressed, bigDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(bigData, bigDataDecompressed, "Original data does not match the decompressed data!");
        }

        /*
         * Purpose: To validate a large amount of data after compression/decompression
         *          using SafeQuickLZ with level 3 compression.
         */
        [TestMethod]
        [TestCategory("Compression")]
        public void BigDataCompressionTestLevel3()
        {
            SafeQuickLZ safeQuickLZtest = new SafeQuickLZ();
            byte[] bigData = new byte[100000];

            // Fill the big data array with random data.
            new Random().NextBytes(bigData);

            // Store the compressed data.
            byte[] bigDataCompressed = safeQuickLZtest.Compress(bigData, 0, bigData.Length, 3);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(bigData, bigDataCompressed, "Original data is equal to the compressed data!");

            // Store the decompressed data.
            byte[] bigDataDecompressed = safeQuickLZtest.Decompress(bigDataCompressed, 0, bigDataCompressed.Length);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(bigDataCompressed, bigDataDecompressed, "Compressed data is equal to the decompressed data!");
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(bigData, bigDataDecompressed, "Original data does not match the decompressed data!");
        }
    }
}