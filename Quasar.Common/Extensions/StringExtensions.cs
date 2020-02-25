using Quasar.Common.Enums;

namespace Quasar.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts the file extension string to its <see cref="ContentType"/> representation.
        /// </summary>
        /// <param name="fileExtension">The file extension string.</param>
        /// <returns>The <see cref="ContentType"/> representation of the file extension string.</returns>
        public static ContentType ToContentType(this string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                default:
                    return ContentType.Blob;
                case ".exe":
                    return ContentType.Application;
                case ".txt":
                case ".log":
                case ".conf":
                case ".cfg":
                case ".asc":
                    return ContentType.Text;
                case ".rar":
                case ".zip":
                case ".zipx":
                case ".tar":
                case ".tgz":
                case ".gz":
                case ".s7z":
                case ".7z":
                case ".bz2":
                case ".cab":
                case ".zz":
                case ".apk":
                    return ContentType.Archive;
                case ".doc":
                case ".docx":
                case ".odt":
                    return ContentType.Word;
                case ".pdf":
                    return ContentType.Pdf;
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".gif":
                case ".ico":
                    return ContentType.Image;
                case ".mp4":
                case ".mov":
                case ".avi":
                case ".wmv":
                case ".mkv":
                case ".m4v":
                case ".flv":
                    return ContentType.Video;
                case ".mp3":
                case ".wav":
                case ".pls":
                case ".m3u":
                case ".m4a":
                    return ContentType.Audio;
            }
        }
    }
}
