using System;
using ProtoBuf;
using Quasar.Common.Enums;

namespace Quasar.Common.Models
{
    [ProtoContract]
    public class FileSystemEntry
    {
        [ProtoMember(1)]
        public FileType EntryType { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public long Size { get; set; }

        [ProtoMember(4)]
        public DateTime LastAccessTimeUtc { get; set; }

        [ProtoMember(5)]
        public ContentType? ContentType { get; set; }
    }
}
