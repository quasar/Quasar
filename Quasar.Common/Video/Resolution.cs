using System;
using ProtoBuf;

namespace Quasar.Common.Video
{
    [ProtoContract]
    public class Resolution : IEquatable<Resolution>
    {
        [ProtoMember(1)]
        public int Width { get; set; }

        [ProtoMember(2)]
        public int Height { get; set; }

        public bool Equals(Resolution other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Width == other.Width && Height == other.Height;
        }

        public static bool operator ==(Resolution r1, Resolution r2)
        {
            if (ReferenceEquals(r1, null))
                return ReferenceEquals(r2, null);

            return r1.Equals(r2);
        }

        public static bool operator !=(Resolution r1, Resolution r2)
        {
            return !(r1 == r2);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Resolution);
        }

        public override int GetHashCode()
        {
            return Width ^ Height;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}
