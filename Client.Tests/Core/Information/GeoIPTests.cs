using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xClient.Core.Information;


namespace xClient.Tests.Core.Information
{
    [TestClass]
    public class GeoIPTests
    {
        [TestMethod]
        public void EncryptAndDecryptStringTest()
        {
            var ipInformation = new GeoIP();
            Assert.IsNotNull(ipInformation.City);
            Assert.IsNotNull(ipInformation.Country);
            Assert.IsNotNull(ipInformation.CountryCode);
            Assert.IsNotNull(ipInformation.Region);
            Assert.IsNotNull(ipInformation.WanIp);
        }
    }
}