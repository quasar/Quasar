// AForge Direct Show Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2009-2012
// contacts@aforgenet.com
//

using System;
using System.Runtime.InteropServices;

namespace AForge.Video.DirectShow.Internals
{
    /// <summary>
    /// The IAMCrossbar interface routes signals from an analog or digital source to a video capture filter.
    /// </summary>
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    Guid( "C6E13380-30AC-11D0-A18C-00A0C9118956" ),
    InterfaceType( ComInterfaceType.InterfaceIsIUnknown )]
    internal interface IAMCrossbar
    {
        /// <summary>
        /// Retrieves the number of input and output pins on the crossbar filter.
        /// </summary>
        /// 
        /// <param name="outputPinCount">Variable that receives the number of output pins.</param>
        /// <param name="inputPinCount">Variable that receives the number of input pins.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_PinCounts( [Out] out int outputPinCount, [Out] out int inputPinCount );
   
        /// <summary>
        /// Queries whether a specified input pin can be routed to a specified output pin.
        /// </summary>
        /// 
        /// <param name="outputPinIndex">Specifies the index of the output pin.</param>
        /// <param name="inputPinIndex">Specifies the index of input pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int CanRoute( [In] int outputPinIndex, [In] int inputPinIndex );

        /// <summary>
        /// Routes an input pin to an output pin.
        /// </summary>
        /// 
        /// <param name="outputPinIndex">Specifies the index of the output pin.</param>
        /// <param name="inputPinIndex">Specifies the index of the input pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int Route( [In] int outputPinIndex, [In] int inputPinIndex );

        /// <summary>
        /// Retrieves the input pin that is currently routed to the specified output pin.
        /// </summary>
        /// 
        /// <param name="outputPinIndex">Specifies the index of the output pin.</param>
        /// <param name="inputPinIndex">Variable that receives the index of the input pin, or -1 if no input pin is routed to this output pin.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_IsRoutedTo( [In] int outputPinIndex, [Out] out int inputPinIndex );

        /// <summary>
        /// Retrieves information about a specified pin.
        /// </summary>
        /// 
        /// <param name="isInputPin">Specifies the direction of the pin. Use one of the following values.</param>
        /// <param name="pinIndex">Specifies the index of the pin.</param>
        /// <param name="pinIndexRelated">Variable that receives the index of the related pin, or –1 if no pin is related to this pin.</param>
        /// <param name="physicalType">Variable that receives a member of the PhysicalConnectorType enumeration, indicating the pin's physical type.</param>
        /// 
        /// <returns>Return's <b>HRESULT</b> error code.</returns>
        /// 
        [PreserveSig]
        int get_CrossbarPinInfo(
            [In, MarshalAs( UnmanagedType.Bool )] bool isInputPin,
            [In] int pinIndex,
            [Out] out int pinIndexRelated,
            [Out] out PhysicalConnectorType physicalType );
    }
}
