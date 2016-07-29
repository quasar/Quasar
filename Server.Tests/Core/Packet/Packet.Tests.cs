using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Linq;
using xServer.Core.Packets;

namespace xServer.Tests.Core.Packet
{
    [TestClass]
    public class PacketTests
    {
        [TestMethod]
        public void AreAllPacketsRegistered()
        {
            var asm = Assembly.Load("Quasar");
            var expectedPacketTypes = asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IPacket)) && t.GetCustomAttributes(typeof(SerializableAttribute), false).Any()).ToArray();
            var registeredPackets = PacketRegistery.GetPacketTypes();
            CollectionAssert.AreEquivalent(expectedPacketTypes, registeredPackets);
        }
    }
}
