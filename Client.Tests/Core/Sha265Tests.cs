using NUnit.Framework;
using Ploeh.AutoFixture;
using xClient.Core.Encryption;

namespace Client.Tests.Core
{
    [TestFixture]
    public class Sha256Test
    {
        public Fixture Fixture { get; set; }

        [SetUp]
        public void Setup()
        {
            Fixture = new Fixture();
        }

        [Test]
        public void ComputerHashSuccess()
        {
            var input = Fixture.Create<string>();
            var result = SHA256.ComputeHash(input);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.EqualTo(input));
        }
    }
}
