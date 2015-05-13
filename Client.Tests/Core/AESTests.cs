using System;
using System.Collections.Generic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.NUnit2;
using Ploeh.AutoFixture.NUnit2.Addins.Builders;
using xClient.Core.Encryption;

namespace Client.Tests.Core
{
    [TestFixture]
    public class AesTests
    {
        public Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
        }

        [Test]
        public void EncryptAesString()
        {
            var key = Fixture.Create<string>();
            var value = Fixture.Create<string>();

            var encrypt = AES.Encrypt(value, key);

            Assert.That(encrypt, Is.Not.Null);
            Assert.That(encrypt, Is.Not.Empty);
            Assert.That(encrypt, Is.Not.Contain(value));
            Assert.That(encrypt, Is.Not.Contain(key));
        }

        [Test]
        public void DecryptAesString()
        {
            var key = Fixture.Create<string>();
            var value = Fixture.Create<string>();

            var encrypt = AES.Encrypt(value, key);

            var decrypt = AES.Decrypt(encrypt, key);

            Assert.That(decrypt, Is.Not.Null);
            Assert.That(decrypt, Is.Not.Empty);
            Assert.That(decrypt, Is.EqualTo(value));
            Assert.That(decrypt, Is.Not.EqualTo(encrypt));
        }
    }
}
