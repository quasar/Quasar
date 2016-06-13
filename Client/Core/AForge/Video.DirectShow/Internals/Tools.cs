// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007
// andrew.kirillov@gmail.com
//

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Some miscellaneous functions.
    /// </summary>
    /// 
    internal static class Tools
    {
        /// <summary>
        /// Get filter's pin.
        /// </summary>
        /// 
        /// <param name="filter">Filter to get pin of.</param>
        /// <param name="dir">Pin's direction.</param>
        /// <param name="num">Pin's number.</param>
        /// 
        /// <returns>Returns filter's pin.</returns>
        /// 
        public static IPin GetPin( IBaseFilter filter, PinDirection dir, int num )
        {
            IPin[] pin = new IPin[1];
            IEnumPins pinsEnum = null;

            // enum filter pins
            if ( filter.EnumPins( out pinsEnum ) == 0 )
            {
                PinDirection pinDir;
                int n;

                try
                {
                    // get next pin
                    while ( pinsEnum.Next( 1, pin, out n ) == 0 )
                    {
                        // query pin`s direction
                        pin[0].QueryDirection( out pinDir );

                        if ( pinDir == dir )
                        {
                            if ( num == 0 )
                                return pin[0];
                            num--;
                        }

                        Marshal.ReleaseComObject( pin[0] );
                        pin[0] = null;
                    }
                }
                finally
                {
                    Marshal.ReleaseComObject( pinsEnum );
                }
            }
            return null;
        }

        /// <summary>
        /// Get filter's input pin.
        /// </summary>
        /// 
        /// <param name="filter">Filter to get pin of.</param>
        /// <param name="num">Pin's number.</param>
        /// 
        /// <returns>Returns filter's pin.</returns>
        /// 
        public static IPin GetInPin( IBaseFilter filter, int num )
        {
            return GetPin( filter, PinDirection.Input, num );
        }

        /// <summary>
        /// Get filter's output pin.
        /// </summary>
        /// 
        /// <param name="filter">Filter to get pin of.</param>
        /// <param name="num">Pin's number.</param>
        /// 
        /// <returns>Returns filter's pin.</returns>
        /// 
        public static IPin GetOutPin( IBaseFilter filter, int num )
        {
            return GetPin( filter, PinDirection.Output, num );
        }
    }
}
