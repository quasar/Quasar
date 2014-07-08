using System;
using System.Collections.Generic;
using System.Text;

namespace LZ4
{
    /// <summary>
    /// Constants and methods shared by LZ4Compressor and LZ4Decompressor
    /// </summary>
    internal class LZ4Util
    {
        //**************************************
        // Constants
        //**************************************
        public const int COPYLENGTH = 8;
        public const int ML_BITS = 4;
        public const uint ML_MASK = ((1U << ML_BITS) - 1);
        public const int RUN_BITS = (8 - ML_BITS);
        public const uint RUN_MASK = ((1U << RUN_BITS) - 1);

        public static unsafe void CopyMemory(byte* dst, byte* src, long length)
        {
            while (length >= 16)
            {
                *(ulong*)dst = *(ulong*)src; dst += 8; src += 8;
                *(ulong*)dst = *(ulong*)src; dst += 8; src += 8;
                length -= 16;
            }

            if (length >= 8)
            {
                *(ulong*)dst = *(ulong*)src; dst += 8; src += 8;
                length -= 8;
            }

            if (length >= 4)
            {
                *(uint*)dst = *(uint*)src; dst += 4; src += 4;
                length -= 4;
            }

            if (length >= 2)
            {
                *(ushort*)dst = *(ushort*)src; dst += 2; src += 2;
                length -= 2;
            }

            if (length != 0)
                *dst = *src;
        }
    }
}
