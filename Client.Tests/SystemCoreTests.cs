using System;
using System.Collections.Generic;
using NUnit.Framework;
using xClient.Core;

namespace Client.Tests
{
    [TestFixture]
    public class SystemCoreTests
    {

        [Test]
        public void GetAccountTypeIsNotNull()
        {
            var validAccounts = new List<string>()
            {
                "Guest",
                "User",
                "Admin"
            };
            var account = SystemCore.GetAccountType();
            
            Assert.That(account, Is.Not.Null);
            Assert.That(validAccounts, Has.Member(account));
            Assert.That(account, Is.Not.EqualTo("Unknown"));
        }

        [Test]
        public void GetIdIsNotNull()
        {
            var id = SystemCore.GetId();

            Assert.That(id, Is.Not.Null);
            Assert.That(id, Is.Not.Empty);
        }

        [Test]
        public void GetCpuIsNotNull()
        {
            var cpu = SystemCore.GetCpu();
            Assert.That(cpu, Is.Not.Null);
            Assert.That(cpu, Is.Not.EqualTo("Unknown"));
        }

        [Test]
        public void GetRamIsNotNull()
        {
            var ram = SystemCore.GetRam();
            Assert.That(ram, Is.Not.Null);
            Assert.That(ram, Is.GreaterThan(0));
        }

        [Test]
        public void GetGpuIsNotNull()
        {
            var gpu = SystemCore.GetGpu();
            Assert.That(gpu, Is.Not.Null);
            Assert.That(gpu, Is.Not.EqualTo("Unknown"));
        }

        [Test]
        public void GetAntivirusIsNotNull()
        {
            var virus = SystemCore.GetAntivirus();
            Assert.That(virus, Is.Not.Null);
            Assert.That(virus, Is.Not.EqualTo("Unknown"));
        }

        [Test]
        public void GetFirewallIsNotNull()
        {
            var firewall = SystemCore.GetFirewall();
            Assert.That(firewall, Is.Not.Null);
            Assert.That(firewall, Is.Not.EqualTo("Unknown"));
        }

        [Test]
        public void GetUptimeIsNotNull()
        {
            var upTime = SystemCore.GetUptime();
            Assert.That(upTime, Is.Not.Null);
            Assert.That(upTime, Is.Not.EqualTo("0d : 0h : 00m : 00s"));
            Assert.That(upTime, Contains.Substring("d"));
            Assert.That(upTime, Contains.Substring("h"));
            Assert.That(upTime, Contains.Substring("s"));
            Assert.That(upTime, Contains.Substring("m"));
        }

        [Test]
        public void GetLanIpIsNotNull()
        {
            var ip = SystemCore.GetLanIp();
            Assert.That(ip, Is.Not.Null);
            Assert.That(ip, Is.Not.EqualTo("-"));
            Assert.That(ip, Is.Not.EqualTo(UInt32.MaxValue));
        }

        [Test]
        public void GetMacAddressIsNotNull()
        {
            var mac = SystemCore.GetMacAddress();
            Assert.That(mac, Is.Not.Null);
            Assert.That(mac, Is.Not.EqualTo("-"));
            Assert.That(mac, Is.Not.EqualTo(UInt32.MaxValue));
        }

    }
}
