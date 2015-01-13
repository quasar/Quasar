using System;
using System.Diagnostics;

namespace xClient.Core.Compression
{
    /// <summary>
    /// Class for compressing a byte array into an LZ4 byte array.
    /// </summary>
    public unsafe class LZ4Compressor32
    {
        //**************************************
        // Tuning parameters
        //**************************************
        // COMPRESSIONLEVEL :
        // Increasing this value improves compression ratio
        // Lowering this value reduces memory usage
        // Reduced memory usage typically improves speed, due to cache effect (ex : L1 32KB for Intel, L1 64KB for AMD)
        // Memory usage formula : N->2^(N+2) Bytes (examples : 12 -> 16KB ; 17 -> 512KB)
        const int COMPRESSIONLEVEL = 12;

        // NOTCOMPRESSIBLE_CONFIRMATION :
        // Decreasing this value will make the algorithm skip faster data segments considered "incompressible"
        // This may decrease compression ratio dramatically, but will be faster on incompressible data
        // Increasing this value will make the algorithm search more before declaring a segment "incompressible"
        // This could improve compression a bit, but will be slower on incompressible data
        // The default value (6) is recommended
        // 2 is the minimum value.
        const int NOTCOMPRESSIBLE_CONFIRMATION = 6;

        //**************************************
        // Constants
        //**************************************
        const int HASH_LOG = COMPRESSIONLEVEL;
        const int MAXD_LOG = 16;
        const int MAX_DISTANCE = ((1 << MAXD_LOG) - 1);
        const int MINMATCH = 4;
        const int MFLIMIT = (LZ4Util.COPYLENGTH + MINMATCH);
        const int MINLENGTH = (MFLIMIT + 1);
        const uint LZ4_64KLIMIT = ((1U << 16) + (MFLIMIT - 1));
        const int HASHLOG64K = (HASH_LOG + 1);
        const int HASHTABLESIZE = (1 << HASH_LOG);
        const int HASH_MASK = (HASHTABLESIZE - 1);
        const int LASTLITERALS = 5;
        const int SKIPSTRENGTH = (NOTCOMPRESSIBLE_CONFIRMATION > 2 ? NOTCOMPRESSIBLE_CONFIRMATION : 2);
        const int SIZE_OF_LONG_TIMES_TWO_SHIFT = 4;
        const int STEPSIZE = 4;
        static byte[] DeBruijnBytePos = new byte[32] { 0, 0, 3, 0, 3, 1, 3, 0, 3, 2, 2, 1, 3, 2, 0, 1, 3, 3, 1, 2, 2, 2, 2, 0, 3, 1, 2, 0, 1, 0, 1, 1 };
        //**************************************
        // Macros
        //**************************************
        byte[] m_HashTable;

        public LZ4Compressor32()
        {
            m_HashTable = new byte[HASHTABLESIZE * IntPtr.Size];
            if (m_HashTable.Length % 16 != 0)
                throw new Exception("Hash table size must be divisible by 16");
        }


        public byte[] Compress(byte[] source)
        {
            int maxCompressedSize = CalculateMaxCompressedLength(source.Length);
            byte[] dst = new byte[maxCompressedSize];
            int length = Compress(source, dst);
            byte[] dest = new byte[length];
            Buffer.BlockCopy(dst, 0, dest, 0, length);
            return dest;
        }

        /// <summary>
        /// Calculate the max compressed byte[] size given the size of the uncompressed byte[]
        /// </summary>
        /// <param name="uncompressedLength">Length of the uncompressed data</param>
        /// <returns>The maximum required size in bytes of the compressed data</returns>
        public int CalculateMaxCompressedLength(int uncompressedLength)
        {
            return uncompressedLength + (uncompressedLength / 255) + 16;
        }

        /// <summary>
        /// Compress source into dest returning compressed length
        /// </summary>
        /// <param name="source">uncompressed data</param>
        /// <param name="dest">array into which source will be compressed</param>
        /// <returns>compressed length</returns>
        public int Compress(byte[] source, byte[] dest)
        {
            fixed (byte* s = source)
            fixed (byte* d = dest)
            {
                if (source.Length < (int)LZ4_64KLIMIT)
                    return Compress64K(s, d, source.Length, dest.Length);
                return Compress(s, d, source.Length, dest.Length);
            }
        }

        /// <summary>
        /// Compress source into dest returning compressed length
        /// </summary>
        /// <param name="source">uncompressed data</param>
        /// <param name="srcOffset">offset in source array where reading will start</param>
        /// <param name="count">count of bytes in source array to compress</param>
        /// <param name="dest">array into which source will be compressed</param>
        /// <param name="dstOffset">start index in dest array where writing will start</param>
        /// <returns>compressed length</returns>
        public int Compress(byte[] source, int srcOffset, int count, byte[] dest, int dstOffset)
        {
            fixed (byte* s = &source[srcOffset])
            fixed (byte* d = &dest[dstOffset])
            {
                if (source.Length < (int)LZ4_64KLIMIT)
                    return Compress64K(s, d, count, dest.Length - dstOffset);
                return Compress(s, d, count, dest.Length - dstOffset);
            }
        }

        int Compress(byte* source, byte* dest, int isize, int maxOutputSize)
        {
            fixed (byte* hashTablePtr = m_HashTable)
            fixed (byte* deBruijnBytePos = DeBruijnBytePos)
            {
                Clear(hashTablePtr, sizeof(byte*) * HASHTABLESIZE);
                byte** hashTable = (byte**)hashTablePtr;

                byte* ip = (byte*)source;
                int basePtr = 0; ;

                byte* anchor = ip;
                byte* iend = ip + isize;
                byte* mflimit = iend - MFLIMIT;
                byte* matchlimit = (iend - LASTLITERALS);
                byte* oend = dest + maxOutputSize;


                byte* op = (byte*)dest;

                int len, length;
                const int skipStrength = SKIPSTRENGTH;
                uint forwardH;


                // Init
                if (isize < MINLENGTH) goto _last_literals;

                // First Byte
                hashTable[(((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - HASH_LOG))] = ip - basePtr;
                ip++; forwardH = (((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - HASH_LOG));

                // Main Loop
                for (; ; )
                {
                    uint findMatchAttempts = (1U << skipStrength) + 3;
                    byte* forwardIp = ip;
                    byte* r;
                    byte* token;

                    // Find a match
                    do
                    {
                        uint h = forwardH;
                        uint step = findMatchAttempts++ >> skipStrength;
                        ip = forwardIp;
                        forwardIp = ip + step;

                        if (forwardIp > mflimit) { goto _last_literals; }

                        // LZ4_HASH_VALUE
                        forwardH = (((*(uint*)forwardIp) * 2654435761U) >> ((MINMATCH * 8) - HASH_LOG));
                        r = hashTable[h] + basePtr;
                        hashTable[h] = ip - basePtr;

                    } while ((r < ip - MAX_DISTANCE) || (*(uint*)r != *(uint*)ip));

                    // Catch up
                    while ((ip > anchor) && (r > (byte*)source) && (ip[-1] == r[-1])) { ip--; r--; }

                    // Encode Literal Length
                    length = (int)(ip - anchor);
                    token = op++;
                    if (length >= (int)LZ4Util.RUN_MASK) { *token = (byte)(LZ4Util.RUN_MASK << LZ4Util.ML_BITS); len = (int)(length - LZ4Util.RUN_MASK); for (; len > 254; len -= 255) *op++ = 255; *op++ = (byte)len; }
                    else *token = (byte)(length << LZ4Util.ML_BITS);

                    //Copy Literals
                    { byte* e = (op) + length; do { *(uint*)op = *(uint*)anchor; op += 4; anchor += 4; ; *(uint*)op = *(uint*)anchor; op += 4; anchor += 4; ; } while (op < e);; op = e; };

                _next_match:
                    // Encode Offset
                    *(ushort*)op = (ushort)(ip - r); op += 2;

                    // Start Counting
                    ip += MINMATCH; r += MINMATCH; // MinMatch verified
                    anchor = ip;
                    //					while (*(uint *)r == *(uint *)ip)
                    //					{
                    //						ip+=4; r+=4;
                    //						if (ip>matchlimit-4) { r -= ip - (matchlimit-3); ip = matchlimit-3; break; }
                    //					}
                    //					if (*(ushort *)r == *(ushort *)ip) { ip+=2; r+=2; }
                    //					if (*r == *ip) ip++;

                    while (ip < matchlimit - (STEPSIZE - 1))
                    {
                        int diff = (int)(*(int*)(r) ^ *(int*)(ip));
                        if (diff == 0) { ip += STEPSIZE; r += STEPSIZE; continue; }
                        ip += DeBruijnBytePos[((uint)((diff & -diff) * 0x077CB531U)) >> 27]; ;
                        goto _endCount;
                    }

                    if ((ip < (matchlimit - 1)) && (*(ushort*)(r) == *(ushort*)(ip))) { ip += 2; r += 2; }
                    if ((ip < matchlimit) && (*r == *ip)) ip++;
                _endCount:

                    len = (int)(ip - anchor);
                    if (op + (1 + LASTLITERALS) + (len >> 8) >= oend) return 0; // Check output limit
                    // Encode MatchLength
                    if (len >= (int)LZ4Util.ML_MASK) { *token += (byte)LZ4Util.ML_MASK; len -= (byte)LZ4Util.ML_MASK; for (; len > 509; len -= 510) { *op++ = 255; *op++ = 255; } if (len > 254) { len -= 255; *op++ = 255; } *op++ = (byte)len; }
                    else *token += (byte)len;

                    // Test end of chunk
                    if (ip > mflimit) { anchor = ip; break; }

                    // Fill table
                    hashTable[(((*(uint*)ip - 2) * 2654435761U) >> ((MINMATCH * 8) - HASH_LOG))] = ip - 2 - basePtr;

                    // Test next position
                    r = basePtr + hashTable[(((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - HASH_LOG))];
                    hashTable[(((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - HASH_LOG))] = ip - basePtr;
                    if ((r > ip - (MAX_DISTANCE + 1)) && (*(uint*)r == *(uint*)ip)) { token = op++; *token = 0; goto _next_match; }

                    // Prepare next loop
                    anchor = ip++;
                    forwardH = (((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - HASH_LOG));
                }

            _last_literals:
                // Encode Last Literals
                {
                    int lastRun = (int)(iend - anchor);
                    if (((byte*)op - dest) + lastRun + 1 + ((lastRun - 15) / 255) >= maxOutputSize) return 0;
                    if (lastRun >= (int)LZ4Util.RUN_MASK) { *op++ = (byte)(LZ4Util.RUN_MASK << LZ4Util.ML_BITS); lastRun -= (byte)LZ4Util.RUN_MASK; for (; lastRun > 254; lastRun -= 255) *op++ = 255; *op++ = (byte)lastRun; }
                    else *op++ = (byte)(lastRun << LZ4Util.ML_BITS);
                    LZ4Util.CopyMemory(op, anchor, iend - anchor);
                    op += iend - anchor;
                }

                // End
                return (int)(((byte*)op) - dest);
            }
        }

        // Note : this function is valid only if isize < LZ4_64KLIMIT
        int Compress64K(byte* source, byte* dest, int isize, int maxOutputSize)
        {
            fixed (byte* hashTablePtr = m_HashTable)
            fixed (byte* deBruijnBytePos = DeBruijnBytePos)
            {
                Clear(hashTablePtr, sizeof(ushort) * HASHTABLESIZE * 2);
                ushort* hashTable = (ushort*)hashTablePtr;

                byte* ip = (byte*)source;
                byte* anchor = ip;
                byte* basep = ip;
                byte* iend = ip + isize;
                byte* mflimit = iend - MFLIMIT;
                byte* matchlimit = (iend - LASTLITERALS);
                byte* op = (byte*)dest;
                byte* oend = dest + maxOutputSize;

                int len, length;
                const int skipStrength = SKIPSTRENGTH;
                uint forwardH;

                // Init
                if (isize < MINLENGTH) goto _last_literals;

                // First Byte
                ip++; forwardH = (((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - (HASH_LOG + 1)));

                // Main Loop
                for (; ; )
                {
                    int findMatchAttempts = (int)(1U << skipStrength) + 3;
                    byte* forwardIp = ip;
                    byte* r;
                    byte* token;

                    // Find a match
                    do
                    {
                        uint h = forwardH;
                        int step = findMatchAttempts++ >> skipStrength;
                        ip = forwardIp;
                        forwardIp = ip + step;

                        if (forwardIp > mflimit) { goto _last_literals; }

                        forwardH = (((*(uint*)forwardIp) * 2654435761U) >> ((MINMATCH * 8) - (HASH_LOG + 1)));
                        r = basep + hashTable[h];
                        hashTable[h] = (ushort)(ip - basep);

                    } while (*(uint*)r != *(uint*)ip);

                    // Catch up
                    while ((ip > anchor) && (r > (byte*)source) && (ip[-1] == r[-1])) { ip--; r--; }

                    // Encode Literal Length
                    length = (int)(ip - anchor);
                    token = op++;
                    if (op + length + (2 + 1 + LASTLITERALS) + (length >> 8) >= oend) return 0; // Check output limit
                    if (length >= (int)LZ4Util.RUN_MASK) { *token = (byte)(LZ4Util.RUN_MASK << LZ4Util.ML_BITS); len = (int)(length - LZ4Util.RUN_MASK); for (; len > 254; len -= 255) *op++ = 255; *op++ = (byte)len; }
                    else *token = (byte)(length << LZ4Util.ML_BITS);

                    // Copy Literals
                    { byte* e = (op) + length; do { *(uint*)op = *(uint*)anchor; op += 4; anchor += 4; ; *(uint*)op = *(uint*)anchor; op += 4; anchor += 4; ; } while (op < e);; op = e; };


                _next_match:
                    // Encode Offset
                    *(ushort*)op = (ushort)(ip - r); op += 2;

                    // Start Counting
                    ip += MINMATCH; r += MINMATCH; // MinMatch verified
                    anchor = ip;
                    //					while (ip<matchlimit-3)
                    //					{
                    //						if (*(uint *)r == *(uint *)ip) { ip+=4; r+=4; continue; }
                    //						if (*(ushort *)r == *(ushort *)ip) { ip+=2; r+=2; }
                    //						if (*r == *ip) ip++;

                    while (ip < matchlimit - (STEPSIZE - 1))
                    {
                        int diff = (int)(*(int*)(r) ^ *(int*)(ip));
                        if (diff == 0) { ip += STEPSIZE; r += STEPSIZE; continue; }
                        ip += DeBruijnBytePos[((uint)((diff & -diff) * 0x077CB531U)) >> 27]; ;
                        goto _endCount;
                    }

                    if ((ip < (matchlimit - 1)) && (*(ushort*)r == *(ushort*)ip)) { ip += 2; r += 2; }
                    if ((ip < matchlimit) && (*r == *ip)) ip++;
                _endCount:
                    len = (int)(ip - anchor);

                    //Encode MatchLength
                    if (len >= (int)LZ4Util.ML_MASK) { *token = (byte)(*token + LZ4Util.ML_MASK); len = (int)(len - LZ4Util.ML_MASK); for (; len > 509; len -= 510) { *op++ = 255; *op++ = 255; } if (len > 254) { len -= 255; *op++ = 255; } *op++ = (byte)len; }
                    else *token = (byte)(*token + len);

                    // Test end of chunk
                    if (ip > mflimit) { anchor = ip; break; }

                    // Fill table
                    hashTable[(((*(uint*)ip - 2) * 2654435761U) >> ((MINMATCH * 8) - (HASH_LOG + 1)))] = (ushort)(ip - 2 - basep);

                    // Test next position
                    r = basep + hashTable[(((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - (HASH_LOG + 1)))];
                    hashTable[(((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - (HASH_LOG + 1)))] = (ushort)(ip - basep);
                    if (*(uint*)r == *(uint*)ip) { token = op++; *token = 0; goto _next_match; }

                    // Prepare next loop
                    anchor = ip++;
                    forwardH = (((*(uint*)ip) * 2654435761U) >> ((MINMATCH * 8) - (HASH_LOG + 1)));
                }

            _last_literals:
                {
                    int lastRun = (int)(iend - anchor);
                    if (((byte*)op - dest) + lastRun + 1 + ((lastRun) >> 8) >= maxOutputSize) return 0;
                    if (lastRun >= (int)LZ4Util.RUN_MASK) { *op++ = (byte)(LZ4Util.RUN_MASK << LZ4Util.ML_BITS); lastRun -= (byte)LZ4Util.RUN_MASK; for (; lastRun > 254; lastRun -= 255) *op++ = 255; *op++ = (byte)lastRun; }
                    else *op++ = (byte)(lastRun << LZ4Util.ML_BITS);
                    LZ4Util.CopyMemory(op, anchor, iend - anchor);
                    op += iend - anchor;
                }


                return (int)(((byte*)op) - dest);
            }
        }

        /// <summary>
        /// TODO: test if this is faster or slower than Array.Clear.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="count"></param>
        static void Clear(byte* ptr, int count)
        {
            long* p = (long*)ptr;
            int longCount = count >> SIZE_OF_LONG_TIMES_TWO_SHIFT; // count / sizeof(long) * 2;
            while (longCount-- != 0)
            {
                *p++ = 0L;
                *p++ = 0L;
            }


            Debug.Assert(count % 16 == 0, "HashTable size must be divisible by 16");

            //for (int i = longCount << 4 ; i < count; i++)
            //    ptr[i] = 0;

        }
    }
}
