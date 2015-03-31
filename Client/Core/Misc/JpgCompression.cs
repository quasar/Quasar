using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace xClient.Core.Misc
{
    public class JpgCompression
    {
        private EncoderParameter _parameter;
        private ImageCodecInfo _encoderInfo;
        private EncoderParameters _encoderParams;

        public JpgCompression(int quality)
        {
            this._parameter = new EncoderParameter(Encoder.Quality, (long)quality);
            this._encoderInfo = GetEncoderInfo("image/jpeg");
            this._encoderParams = new EncoderParameters(2);
            this._encoderParams.Param[0] = _parameter;
            this._encoderParams.Param[1] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionRle);
        }

        public byte[] Compress(Bitmap bmp)
        {
            using (MemoryStream stream = new MemoryStream())
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
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            int num2 = imageEncoders.Length - 1;
            for (int i = 0; i <= num2; i++)
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
