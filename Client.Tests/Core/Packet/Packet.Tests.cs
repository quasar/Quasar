using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using xClient.Core.Packets;

namespace xClient.Tests.Core.Packet
{
    [TestClass]
    public class PacketTest
    {
        [TestMethod]
        public void AreAllPacketsRegistered()
        {
            var asm = Assembly.Load("Client");
            var expectedPacketTypes = asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPacket)) && t.GetCustomAttributes(typeof(SerializableAttribute), false).Any()).ToArray();
            var registeredPackets = PacketRegistery.GetPacketTypes();
            CollectionAssert.AreEquivalent(expectedPacketTypes, registeredPackets);
        }
    }
}
