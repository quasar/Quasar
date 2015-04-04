﻿using System;

namespace xServer.Core.Compression
{
    // QuickLZ data compression library
    // Copyright (C) 2006-2011 Lasse Mikkel Reinhold
    // lar@quicklz.com
    //
    // QuickLZ can be used for free under the GPL 1, 2 or 3 license (where anything
    // released into public must be open source) or under a commercial license if such
    // has been acquired (see http://www.quicklz.com/order.html). The commercial license
    // does not cover derived or ported versions created by third parties under GPL.
    //
    // Only a subset of the C library has been ported, namely level 1 and 3 not in
    // streaming mode.
    //
    // Version: 1.5.0 final

    public class SafeQuickLZ
    {
        public const int QLZ_VERSION_MAJOR = 1;
        public const int QLZ_VERSION_MINOR = 5;
        public const int QLZ_VERSION_REVISION = 0;
        // Streaming mode not supported
        public const int QLZ_STREAMING_BUFFER = 0;
        // Bounds checking not supported  Use try...catch instead
        public const int QLZ_MEMORY_SAFE = 0;
        // Decrease QLZ_POINTERS_3 to increase level 3 compression speed. Do not edit any other values!
        private const int HASH_VALUES = 4096;
        private const int MINOFFSET = 2;
        private const int UNCONDITIONAL_MATCHLEN = 6;
        private const int UNCOMPRESSED_END = 4;
        private const int CWORD_LEN = 4;
        private const int DEFAULT_HEADERLEN = 9;
        private const int QLZ_POINTERS_1 = 1;
        private const int QLZ_POINTERS_3 = 16;

        private int HeaderLen(byte[] source, int offset)
        {
            return ((source[offset] & 2) == 2) ? 9 : 3;
        }

        public int SizeDecompressed(byte[] source, int offset)
        {
            if (HeaderLen(source, offset) == 9)
                return source[offset + 5] | (source[offset + 6] << 8) | (source[offset + 7] << 16) |
                       (source[offset + 8] << 24);
            return source[offset + 2];
        }

        public int SizeCompressed(byte[] source, int offset)
        {
            if (HeaderLen(source, offset) == 9)
                return source[offset + 1] | (source[offset + 2] << 8) | (source[offset + 3] << 16) |
                       (source[offset + 4] << 24);
            return source[offset + 1];
        }

        private void WriteHeader(byte[] dst, int level, bool compressible, int size_compressed, int size_decompressed)
        {
            dst[0] = (byte) (2 | (compressible ? 1 : 0));
            dst[0] |= (byte) (level << 2);
            dst[0] |= (1 << 6);
            dst[0] |= (0 << 4);
            FastWrite(dst, 1, size_decompressed, 4);
            FastWrite(dst, 5, size_compressed, 4);
        }

        public byte[] Compress(byte[] source, int Offset, int Length, int level)
        {
            var src = Offset;
            var dst = DEFAULT_HEADERLEN + CWORD_LEN;
            var cword_val = 0x80000000;
            var cword_ptr = DEFAULT_HEADERLEN;
            var destination = new byte[Length + 400];
            int[,] hashtable;
            var cachetable = new int[HASH_VALUES];
            var hash_counter = new byte[HASH_VALUES];
            byte[] d2;
            var fetch = 0;
            var last_matchstart = (Length - UNCONDITIONAL_MATCHLEN - UNCOMPRESSED_END - 1);
            var lits = 0;

            if (level != 1 && level != 3)
                throw new ArgumentException("C# version only supports level 1 and 3");

            if (level == 1)
                hashtable = new int[HASH_VALUES, QLZ_POINTERS_1];
            else
                hashtable = new int[HASH_VALUES, QLZ_POINTERS_3];

            if (Length == 0)
                return new byte[0];

            if (src <= last_matchstart)
                fetch = source[src] | (source[src + 1] << 8) | (source[src + 2] << 16);

            while (src <= last_matchstart)
            {
                if ((cword_val & 1) == 1)
                {
                    if (src > Length >> 1 && dst > src - (src >> 5))
                    {
                        d2 = new byte[Length + DEFAULT_HEADERLEN];
                        WriteHeader(d2, level, false, Length, Length + DEFAULT_HEADERLEN);
                        Array.Copy(source, 0, d2, DEFAULT_HEADERLEN, Length);
                        return d2;
                    }

                    FastWrite(destination, cword_ptr, (int) ((cword_val >> 1) | 0x80000000), 4);
                    cword_ptr = dst;
                    dst += CWORD_LEN;
                    cword_val = 0x80000000;
                }

                if (level == 1)
                {
                    var hash = ((fetch >> 12) ^ fetch) & (HASH_VALUES - 1);
                    var o = hashtable[hash, 0];
                    var cache = cachetable[hash] ^ fetch;
                    cachetable[hash] = fetch;
                    hashtable[hash, 0] = src;

                    if (cache == 0 && hash_counter[hash] != 0 &&
                        (src - o > MINOFFSET ||
                         (src == o + 1 && lits >= 3 && src > 3 && source[src] == source[src - 3] &&
                          source[src] == source[src - 2] && source[src] == source[src - 1] &&
                          source[src] == source[src + 1] && source[src] == source[src + 2])))
                    {
                        cword_val = ((cword_val >> 1) | 0x80000000);
                        if (source[o + 3] != source[src + 3])
                        {
                            var f = 3 - 2 | (hash << 4);
                            destination[dst + 0] = (byte) (f >> 0*8);
                            destination[dst + 1] = (byte) (f >> 1*8);
                            src += 3;
                            dst += 2;
                        }
                        else
                        {
                            var old_src = src;
                            var remaining = ((Length - UNCOMPRESSED_END - src + 1 - 1) > 255
                                ? 255
                                : (Length - UNCOMPRESSED_END - src + 1 - 1));

                            src += 4;
                            if (source[o + src - old_src] == source[src])
                            {
                                src++;
                                if (source[o + src - old_src] == source[src])
                                {
                                    src++;
                                    while (source[o + (src - old_src)] == source[src] && (src - old_src) < remaining)
                                        src++;
                                }
                            }

                            var matchlen = src - old_src;

                            hash <<= 4;
                            if (matchlen < 18)
                            {
                                var f = (hash | (matchlen - 2));
                                destination[dst + 0] = (byte) (f >> 0*8);
                                destination[dst + 1] = (byte) (f >> 1*8);
                                dst += 2;
                            }
                            else
                            {
                                FastWrite(destination, dst, hash | (matchlen << 16), 3);
                                dst += 3;
                            }
                        }
                        fetch = source[src] | (source[src + 1] << 8) | (source[src + 2] << 16);
                        lits = 0;
                    }
                    else
                    {
                        lits++;
                        hash_counter[hash] = 1;
                        destination[dst] = source[src];
                        cword_val = (cword_val >> 1);
                        src++;
                        dst++;
                        fetch = ((fetch >> 8) & 0xffff) | (source[src + 2] << 16);
                    }
                }
                else
                {
                    fetch = source[src] | (source[src + 1] << 8) | (source[src + 2] << 16);

                    int o, offset2;
                    int matchlen, k, m, best_k = 0;
                    byte c;
                    var remaining = ((Length - UNCOMPRESSED_END - src + 1 - 1) > 255
                        ? 255
                        : (Length - UNCOMPRESSED_END - src + 1 - 1));
                    var hash = ((fetch >> 12) ^ fetch) & (HASH_VALUES - 1);

                    c = hash_counter[hash];
                    matchlen = 0;
                    offset2 = 0;
                    for (k = 0; k < QLZ_POINTERS_3 && c > k; k++)
                    {
                        o = hashtable[hash, k];
                        if ((byte) fetch == source[o] && (byte) (fetch >> 8) == source[o + 1] &&
                            (byte) (fetch >> 16) == source[o + 2] && o < src - MINOFFSET)
                        {
                            m = 3;
                            while (source[o + m] == source[src + m] && m < remaining)
                                m++;
                            if ((m > matchlen) || (m == matchlen && o > offset2))
                            {
                                offset2 = o;
                                matchlen = m;
                                best_k = k;
                            }
                        }
                    }
                    o = offset2;
                    hashtable[hash, c & (QLZ_POINTERS_3 - 1)] = src;
                    c++;
                    hash_counter[hash] = c;

                    if (matchlen >= 3 && src - o < 131071)
                    {
                        var offset = src - o;

                        for (var u = 1; u < matchlen; u++)
                        {
                            fetch = source[src + u] | (source[src + u + 1] << 8) | (source[src + u + 2] << 16);
                            hash = ((fetch >> 12) ^ fetch) & (HASH_VALUES - 1);
                            c = hash_counter[hash]++;
                            hashtable[hash, c & (QLZ_POINTERS_3 - 1)] = src + u;
                        }

                        src += matchlen;
                        cword_val = ((cword_val >> 1) | 0x80000000);

                        if (matchlen == 3 && offset <= 63)
                        {
                            FastWrite(destination, dst, offset << 2, 1);
                            dst++;
                        }
                        else if (matchlen == 3 && offset <= 16383)
                        {
                            FastWrite(destination, dst, (offset << 2) | 1, 2);
                            dst += 2;
                        }
                        else if (matchlen <= 18 && offset <= 1023)
                        {
                            FastWrite(destination, dst, ((matchlen - 3) << 2) | (offset << 6) | 2, 2);
                            dst += 2;
                        }
                        else if (matchlen <= 33)
                        {
                            FastWrite(destination, dst, ((matchlen - 2) << 2) | (offset << 7) | 3, 3);
                            dst += 3;
                        }
                        else
                        {
                            FastWrite(destination, dst, ((matchlen - 3) << 7) | (offset << 15) | 3, 4);
                            dst += 4;
                        }
                        lits = 0;
                    }
                    else
                    {
                        destination[dst] = source[src];
                        cword_val = (cword_val >> 1);
                        src++;
                        dst++;
                    }
                }
            }
            while (src <= Length - 1)
            {
                if ((cword_val & 1) == 1)
                {
                    FastWrite(destination, cword_ptr, (int) ((cword_val >> 1) | 0x80000000), 4);
                    cword_ptr = dst;
                    dst += CWORD_LEN;
                    cword_val = 0x80000000;
                }

                destination[dst] = source[src];
                src++;
                dst++;
                cword_val = (cword_val >> 1);
            }
            while ((cword_val & 1) != 1)
            {
                cword_val = (cword_val >> 1);
            }

            FastWrite(destination, cword_ptr, (int) ((cword_val >> 1) | 0x80000000), CWORD_LEN);
            WriteHeader(destination, level, true, Length, dst);
            d2 = new byte[dst];
            Array.Copy(destination, d2, dst);
            return d2;
        }

        private void FastWrite(byte[] a, int i, int value, int numbytes)
        {
            for (var j = 0; j < numbytes; j++)
                a[i + j] = (byte) (value >> (j*8));
        }

        public byte[] Decompress(byte[] source, int Offset, int Length)
        {
            int level;
            var size = SizeDecompressed(source, Offset);
            var src = HeaderLen(source, Offset) + Offset;
            var dst = 0;
            uint cword_val = 1;
            var destination = new byte[size];
            var hashtable = new int[4096];
            var hash_counter = new byte[4096];
            var last_matchstart = size - UNCONDITIONAL_MATCHLEN - UNCOMPRESSED_END - 1;
            var last_hashed = -1;
            uint fetch = 0;

            level = (source[Offset] >> 2) & 0x3;

            if (level != 1 && level != 3)
                throw new ArgumentException("C# version only supports level 1 and 3");

            if ((source[Offset] & 1) != 1)
            {
                var d2 = new byte[size];
                Array.Copy(source, HeaderLen(source, Offset), d2, Offset, size);
                return d2;
            }

            for (;;)
            {
                if (cword_val == 1)
                {
                    cword_val =
                        (uint)
                            (source[src] | (source[src + 1] << 8) | (source[src + 2] << 16) | (source[src + 3] << 24));
                    src += 4;
                    if (dst <= last_matchstart)
                    {
                        if (level == 1)
                            fetch = (uint) (source[src] | (source[src + 1] << 8) | (source[src + 2] << 16));
                        else
                            fetch =
                                (uint)
                                    (source[src] | (source[src + 1] << 8) | (source[src + 2] << 16) |
                                     (source[src + 3] << 24));
                    }
                }

                int hash;
                if ((cword_val & 1) == 1)
                {
                    uint matchlen;
                    uint offset2;

                    cword_val = cword_val >> 1;

                    if (level == 1)
                    {
                        hash = ((int) fetch >> 4) & 0xfff;
                        offset2 = (uint) hashtable[hash];

                        if ((fetch & 0xf) != 0)
                        {
                            matchlen = (fetch & 0xf) + 2;
                            src += 2;
                        }
                        else
                        {
                            matchlen = source[src + 2];
                            src += 3;
                        }
                    }
                    else
                    {
                        uint offset;
                        if ((fetch & 3) == 0)
                        {
                            offset = (fetch & 0xff) >> 2;
                            matchlen = 3;
                            src++;
                        }
                        else if ((fetch & 2) == 0)
                        {
                            offset = (fetch & 0xffff) >> 2;
                            matchlen = 3;
                            src += 2;
                        }
                        else if ((fetch & 1) == 0)
                        {
                            offset = (fetch & 0xffff) >> 6;
                            matchlen = ((fetch >> 2) & 15) + 3;
                            src += 2;
                        }
                        else if ((fetch & 127) != 3)
                        {
                            offset = (fetch >> 7) & 0x1ffff;
                            matchlen = ((fetch >> 2) & 0x1f) + 2;
                            src += 3;
                        }
                        else
                        {
                            offset = (fetch >> 15);
                            matchlen = ((fetch >> 7) & 255) + 3;
                            src += 4;
                        }
                        offset2 = (uint) (dst - offset);
                    }

                    destination[dst + 0] = destination[offset2 + 0];
                    destination[dst + 1] = destination[offset2 + 1];
                    destination[dst + 2] = destination[offset2 + 2];

                    for (var i = 3; i < matchlen; i += 1)
                    {
                        destination[dst + i] = destination[offset2 + i];
                    }

                    dst += (int) matchlen;

                    if (level == 1)
                    {
                        fetch =
                            (uint)
                                (destination[last_hashed + 1] | (destination[last_hashed + 2] << 8) |
                                 (destination[last_hashed + 3] << 16));
                        while (last_hashed < dst - matchlen)
                        {
                            last_hashed++;
                            hash = (int)(((fetch >> 12) ^ fetch) & ((uint)HASH_VALUES - 1));
                            hashtable[hash] = last_hashed;
                            hash_counter[hash] = 1;
                            fetch = (fetch >> 8 & 0xffff | (uint)destination[last_hashed + 3] << 16);
                        }
                        fetch = (uint) (source[src] | (source[src + 1] << 8) | (source[src + 2] << 16));
                    }
                    else
                    {
                        fetch =
                            (uint)
                                (source[src] | (source[src + 1] << 8) | (source[src + 2] << 16) |
                                 (source[src + 3] << 24));
                    }
                    last_hashed = dst - 1;
                }
                else
                {
                    if (dst <= last_matchstart)
                    {
                        destination[dst] = source[src];
                        dst += 1;
                        src += 1;
                        cword_val = cword_val >> 1;

                        if (level == 1)
                        {
                            while (last_hashed < dst - 3)
                            {
                                last_hashed++;
                                var fetch2 = destination[last_hashed] | (destination[last_hashed + 1] << 8) |
                                             (destination[last_hashed + 2] << 16);
                                hash = ((fetch2 >> 12) ^ fetch2) & (HASH_VALUES - 1);
                                hashtable[hash] = last_hashed;
                                hash_counter[hash] = 1;
                            }
                            fetch =(fetch >> 8 & 0xffff | (uint)source[src + 2] << 16);
                        }
                        else
                        {
                            fetch = (fetch >> 8 & 0xffff | (uint)source[src + 2] << 16 | (uint)source[src + 3] << 24);
                        }
                    }
                    else
                    {
                        while (dst <= size - 1)
                        {
                            if (cword_val == 1)
                            {
                                src += CWORD_LEN;
                                cword_val = 0x80000000;
                            }

                            destination[dst] = source[src];
                            dst++;
                            src++;
                            cword_val = cword_val >> 1;
                        }
                        return destination;
                    }
                }
            }
        }
    }
}