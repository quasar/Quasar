// AForge Direct Show Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@gmail.com
//
// Written by Jeremy Noring 
// kidjan@gmail.com

namespace AForge.Video.DirectShow.Internals
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Provides the CLSID of an object that can be stored persistently in the system. Allows the object to specify which object 
    /// handler to use in the client process, as it is used in the default implementation of marshaling.
    /// </summary>
    [ComImport,
    Guid("0000010c-0000-0000-C000-000000000046"),
    InterfaceType(ComInterfaceType.InterfaceIsDual)]
    internal interface IPersist
    {
        /// <summary>
        /// Retrieves the class identifier (CLSID) of the object.
        /// </summary>
        /// <param name="pClassID"></param>
        /// <returns></returns>
        [PreserveSig]
        int GetClassID([Out] out Guid pClassID);
    }
}
