using System;
using System.Collections.Generic;
using System.Linq;

namespace Quasar.Common.Messages
{
    public class PacketRegistry
    {
        public static IEnumerable<Type> GetPacketTypes(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface);
        }
    }
}
