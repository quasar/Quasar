using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quasar.Common.IO.Compression;
using System;

namespace Quasar.Common.Tests.Compression
{
    [TestClass]
    public class SafeQuickLZTests
    {
        /*
         * Purpose: To validate a small amount of data after compression/decompression
         *          using SafeQuickLZ with level 1 compression.
         */
        [TestMethod, TestCategory("Compression")]
        public void SmallDataCompressionTestLevel1()
        {
            byte[] smallData = new byte[100];

            // Fill the small data array with random data.
            new Random().NextBytes(smallData);

            // Store the compressed data.
            byte[] smallDataCompressed = SafeQuickLZ.Compress(smallData, 1);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(smallData, smallDataCompressed);

            // Store the decompressed data.
            byte[] smallDataDecompressed = SafeQuickLZ.Decompress(smallDataCompressed);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(smallDataCompressed, smallDataDecompressed);
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(smallData, smallDataDecompressed);
        }

        /*
         * Purpose: To validate a small amount of data after compression/decompression
         *          using SafeQuickLZ with level 3 compression.
         */
        [TestMethod, TestCategory("Compression")]
        public void SmallDataCompressionTestLevel3()
        {
            byte[] smallData = new byte[100];

            // Fill the small data array with random data.
            new Random().NextBytes(smallData);

            // Store the compressed data.
            byte[] smallDataCompressed = SafeQuickLZ.Compress(smallData, 3);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(smallData, smallDataCompressed);

            // Store the decompressed data.
            byte[] smallDataDecompressed = SafeQuickLZ.Decompress(smallDataCompressed);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(smallDataCompressed, smallDataDecompressed);
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(smallData, smallDataDecompressed);
        }

        /*
         * Purpose: To validate a large amount of data after compression/decompression
         *          using SafeQuickLZ with level 1 compression.
         */
        [TestMethod, TestCategory("Compression")]
        public void BigDataCompressionTestLevel1()
        {
            byte[] bigData = new byte[100000];

            // Fill the big data array with random data.
            new Random().NextBytes(bigData);

            // Store the compressed data.
            byte[] bigDataCompressed = SafeQuickLZ.Compress(bigData, 1);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(bigData, bigDataCompressed);

            // Store the decompressed data.
            byte[] bigDataDecompressed = SafeQuickLZ.Decompress(bigDataCompressed);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(bigDataCompressed, bigDataDecompressed);
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(bigData, bigDataDecompressed);
        }

        /*
         * Purpose: To validate a large amount of data after compression/decompression
         *          using SafeQuickLZ with level 3 compression.
         */
        [TestMethod, TestCategory("Compression")]
        public void BigDataCompressionTestLevel3()
        {
            byte[] bigData = new byte[100000];

            // Fill the big data array with random data.
            new Random().NextBytes(bigData);

            // Store the compressed data.
            byte[] bigDataCompressed = SafeQuickLZ.Compress(bigData, 3);

            // The original should not equal the compressed data.
            Assert.AreNotEqual(bigData, bigDataCompressed);

            // Store the decompressed data.
            byte[] bigDataDecompressed = SafeQuickLZ.Decompress(bigDataCompressed);

            // The compressed data should not equal the decompressed data.
            Assert.AreNotEqual(bigDataCompressed, bigDataDecompressed);
            // The original data must equal the decompressed data; must be able to make a round-trip.
            CollectionAssert.AreEqual(bigData, bigDataDecompressed);
        }
    }
}
