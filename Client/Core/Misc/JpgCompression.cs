using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace xClient.Core.Misc
{
    public class JpgCompression
    {
        private EncoderParameter parameter;
        private ImageCodecInfo encoderInfo;
        private EncoderParameters encoderParams;

        public JpgCompression(int Quality)
        {
            this.parameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)Quality);
            this.encoderInfo = GetEncoderInfo("image/jpeg");
            this.encoderParams = new EncoderParameters(2);
            this.encoderParams.Param[0] = parameter;
            this.encoderParams.Param[1] = new EncoderParameter(System.Drawing.Imaging.Encoder.Compression, (long)EncoderValue.CompressionRle);
        }

        public byte[] Compress(Bitmap bmp)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bmp.Save(stream, encoderInfo, encoderParams);
                return stream.ToArray();
            }
        }
        public void Compress(Bitmap bmp, ref Stream TargetStream)
        {
            bmp.Save(TargetStream, encoderInfo, encoderParams);
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
