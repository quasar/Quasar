using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
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

        [Test]
        public void EncryptAesArray()
        {
            var key = Fixture.Create<byte[]>();
            var value = Fixture.Create<byte[]>();

            var encrypt = AES.Encrypt(value, key);

            Assert.That(encrypt, Is.Not.Null);
            Assert.That(encrypt, Is.Not.Empty);
            Assert.That(encrypt, Is.Not.EquivalentTo(value));
            Assert.That(encrypt, Is.Not.EquivalentTo(key));
        }

        [Test]
        public void DecryptAesArray()
        {
            var key = Fixture.Create<byte[]>();
            var value = Fixture.Create<byte[]>();

            var encrypt = AES.Encrypt(value, key);

            var decrypt = AES.Decrypt(encrypt, key).Where(x => x > 0).ToArray();

            Assert.That(decrypt, Is.Not.Null);
            Assert.That(decrypt, Is.Not.Empty);
            Assert.That(decrypt, Is.EqualTo(value));
            Assert.That(decrypt, Is.Not.EquivalentTo(encrypt));
        }
    }
}
