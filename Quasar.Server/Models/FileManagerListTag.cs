using Quasar.Common.Enums;

namespace Quasar.Server.Models
{
    public class FileManagerListTag
    {
        public FileType Type { get; set; }

        public long FileSize { get; set; }

        public FileManagerListTag(FileType type, long fileSize)
        {
            this.Type = type;
            this.FileSize = fileSize;
        }
    }
}
