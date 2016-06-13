// AForge Video Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Video
{
    using System;

    /// <summary>
    /// Some internal utilities for handling arrays.
    /// </summary>
    /// 
    internal static class ByteArrayUtils
    {
        /// <summary>
        /// Check if the array contains needle at specified position.
        /// </summary>
        /// 
        /// <param name="array">Source array to check for needle.</param>
        /// <param name="needle">Needle we are searching for.</param>
        /// <param name="startIndex">Start index in source array.</param>
        /// 
        /// <returns>Returns <b>true</b> if the source array contains the needle at
        /// the specified index. Otherwise it returns <b>false</b>.</returns>
        /// 
        public static bool Compare( byte[] array, byte[] needle, int startIndex )
        {
            int needleLen = needle.Length;
            // compare
            for ( int i = 0, p = startIndex; i < needleLen; i++, p++ )
            {
                if ( array[p] != needle[i] )
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Find subarray in the source array.
        /// </summary>
        /// 
        /// <param name="array">Source array to search for needle.</param>
        /// <param name="needle">Needle we are searching for.</param>
        /// <param name="startIndex">Start index in source array.</param>
        /// <param name="sourceLength">Number of bytes in source array, where the needle is searched for.</param>
        /// 
        /// <returns>Returns starting position of the needle if it was found or <b>-1</b> otherwise.</returns>
        /// 
        public static int Find( byte[] array, byte[] needle, int startIndex, int sourceLength )
        {
            int needleLen = needle.Length;
            int index;

            while ( sourceLength >= needleLen )
            {
                // find needle's starting element
                index = Array.IndexOf( array, needle[0], startIndex, sourceLength - needleLen + 1 );

                // if we did not find even the first element of the needls, then the search is failed
                if ( index == -1 )
                    return -1;

                int i, p;
                // check for needle
                for ( i = 0, p = index; i < needleLen; i++, p++ )
                {
                    if ( array[p] != needle[i] )
                    {
                        break;
                    }
                }

                if ( i == needleLen )
                {
                    // needle was found
                    return index;
                }

                // continue to search for needle
                sourceLength -= ( index - startIndex + 1 );
                startIndex = index + 1;
            }
            return -1;
        }
    }
}
