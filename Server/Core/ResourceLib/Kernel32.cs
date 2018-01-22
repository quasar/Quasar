using System;
using System.Runtime.InteropServices;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// Kernel32.dll interop functions.
    /// </summary>
    public abstract class Kernel32
    {
        /// <summary>
        /// A resource header.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct RESOURCE_HEADER
        {
            /// <summary>
            /// Header length.
            /// </summary>
            public UInt16 wLength;
            /// <summary>
            /// Data length.
            /// </summary>
            public UInt16 wValueLength;
            /// <summary>
            /// Resource type.
            /// </summary>
            public UInt16 wType;

            /// <summary>
            /// A new resource header of a given length.
            /// </summary>
            /// <param name="valueLength"></param>
            public RESOURCE_HEADER(UInt16 valueLength)
            {
                wLength = 0;
                wValueLength = valueLength;
                wType = 0;
            }
        }

        /// <summary>
        /// Resource header type.
        /// </summary>
        public enum RESOURCE_HEADER_TYPE
        {
            /// <summary>
            /// Binary data.
            /// </summary>
            BinaryData = 0,
            /// <summary>
            /// String data.
            /// </summary>
            StringData = 1
        }

        /// <summary>
        /// Language and code page combinations.
        /// The low-order word of each DWORD must contain a Microsoft language identifier, 
        /// and the high-order word must contain the IBM code page number. 
        /// Either high-order or low-order word can be zero, indicating that the file is language 
        /// or code page independent.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct VAR_HEADER
        {
            /// <summary>
            /// Microsoft language identifier.
            /// </summary>
            public UInt16 wLanguageIDMS;
            /// <summary>
            /// IBM code page number.
            /// </summary>
            public UInt16 wCodePageIBM;
        }

        /// <summary>
        /// This structure contains version information about a file. 
        /// This information is language- and code page–independent.
        /// http://msdn.microsoft.com/en-us/library/ms647001.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct VS_FIXEDFILEINFO
        {
            /// <summary>
            /// Contains the value 0xFEEF04BD. This is used with the szKey member of the VS_VERSIONINFO structure when searching a file for the VS_FIXEDFILEINFO structure. 
            /// </summary>
            public UInt32 dwSignature;
            /// <summary>
            /// Specifies the binary version number of this structure. The high-order word of this member contains the major version number, and the low-order word contains the minor version number.
            /// </summary>
            public UInt32 dwStrucVersion;
            /// <summary>
            /// Specifies the most significant 32 bits of the file's binary version number. This member is used with dwFileVersionLS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 dwFileVersionMS;
            /// <summary>
            /// Specifies the least significant 32 bits of the file's binary version number. This member is used with dwFileVersionMS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 dwFileVersionLS;
            /// <summary>
            /// Specifies the most significant 32 bits of the binary version number of the product with which this file was distributed. This member is used with dwProductVersionLS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 dwProductVersionMS;
            /// <summary>
            /// Specifies the least significant 32 bits of the binary version number of the product with which this file was distributed. This member is used with dwProductVersionMS to form a 64-bit value used for numeric comparisons.
            /// </summary>
            public UInt32 dwProductVersionLS;
            /// <summary>
            /// Contains a bitmask that specifies the valid bits in dwFileFlags. A bit is valid only if it was defined when the file was created. 
            /// </summary>
            public UInt32 dwFileFlagsMask;
            /// <summary>
            /// Contains a bitmask that specifies the Boolean attributes of the file.
            /// </summary>
            public UInt32 dwFileFlags;
            /// <summary>
            /// Specifies the operating system for which this file was designed.
            /// </summary>
            public UInt32 dwFileOS;
            /// <summary>
            /// Specifies the general type of file. 
            /// </summary>
            public UInt32 dwFileType;
            /// <summary>
            /// Specifies the function of the file.
            /// </summary>
            public UInt32 dwFileSubtype;
            /// <summary>
            /// Specifies the most significant 32 bits of the file's 64-bit binary creation date and time stamp.
            /// </summary>
            public UInt32 dwFileDateMS;
            /// <summary>
            /// Specifies the least significant 32 bits of the file's 64-bit binary creation date and time stamp.
            /// </summary>
            public UInt32 dwFileDateLS;

            /// <summary>
            /// Creates a default Windows VS_FIXEDFILEINFO structure.
            /// </summary>
            /// <returns>A default Windows VS_FIXEDFILEINFO.</returns>
            public static VS_FIXEDFILEINFO GetWindowsDefault()
            {
                VS_FIXEDFILEINFO fixedFileInfo = new VS_FIXEDFILEINFO();
                fixedFileInfo.dwSignature = Winver.VS_FFI_SIGNATURE;
                fixedFileInfo.dwStrucVersion = Winver.VS_FFI_STRUCVERSION;
                fixedFileInfo.dwFileFlagsMask = Winver.VS_FFI_FILEFLAGSMASK;
                fixedFileInfo.dwFileOS = (uint) Winver.FileOs.VOS__WINDOWS32;
                fixedFileInfo.dwFileSubtype = (uint) Winver.FileSubType.VFT2_UNKNOWN;
                fixedFileInfo.dwFileType = (uint) Winver.FileType.VFT_DLL;
                return fixedFileInfo;
            }
        }

        /// <summary>
        /// A hardware-independent icon directory resource header.
        /// http://msdn.microsoft.com/en-us/library/ms997538.aspx
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GRPICONDIR
        {
            /// <summary>
            /// Reserved, must be zero.
            /// </summary>
            public UInt16 wReserved;
            /// <summary>
            /// Resource type, 1 for icons.
            /// </summary>
            public UInt16 wType;
            /// <summary>
            /// Number of images.
            /// </summary>
            public UInt16 wImageCount;
        }

        /// <summary>
        /// Hardware-independent icon directory entry.
        /// See http://msdn.microsoft.com/en-us/library/ms997538.aspx for more information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct GRPICONDIRENTRY
        {
            /// <summary>
            /// Width of the image. Starting with Windows 95 a value of 0 represents width of 256.
            /// </summary>
            public Byte bWidth;
            /// <summary>
            /// Height of the image. Starting with Windows 95 a value of 0 represents height of 256.
            /// </summary>
            public Byte bHeight;
            /// <summary>
            /// Number of colors in the image. 
            /// bColors = 1 &lt;&lt; (wBitsPerPixel * wPlanes)
            /// If wBitsPerPixel* wPlanes is greater orequal to 8, then bColors = 0.
            /// </summary>
            public Byte bColors;
            /// <summary>
            /// Reserved.
            /// </summary>
            public Byte bReserved;
            /// <summary>
            /// Number of bitmap planes.
            /// 1: monochrome bitmap
            /// </summary>
            public UInt16 wPlanes;
            /// <summary>
            /// Bits per pixel.
            /// 1: monochrome bitmap
            /// </summary>
            public UInt16 wBitsPerPixel;
            /// <summary>
            /// Image size in bytes.
            /// </summary>
            public UInt32 dwImageSize;
            /// <summary>
            /// Icon ID.
            /// </summary>
            public UInt16 nID;
        }

        /// <summary>
        /// Hardware-independent icon directory entry in an .ico file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct FILEGRPICONDIRENTRY
        {
            /// <summary>
            /// Icon width.
            /// </summary>
            public Byte bWidth;
            /// <summary>
            /// Icon height.
            /// </summary>
            public Byte bHeight;
            /// <summary>
            /// Colors; 0 means 256 or more.
            /// </summary>
            public Byte bColors;
            /// <summary>
            /// Reserved.
            /// </summary>
            public Byte bReserved;
            /// <summary>
            /// Number of bitmap planes for icons.
            /// Horizontal hotspot for cursors.
            /// </summary>
            public UInt16 wPlanes;
            /// <summary>
            /// Bits per pixel for icons.
            /// Vertical hostpot for cursors.
            /// </summary>
            public UInt16 wBitsPerPixel;
            /// <summary>
            /// Image size in bytes.
            /// </summary>
            public UInt32 dwImageSize;
            /// <summary>
            /// Offset of bitmap data from the beginning of the file.
            /// </summary>
            public UInt32 dwFileOffset;
        }

        /// <summary>
        /// Hardware-independent icon structure in an .ico file.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct FILEGRPICONDIR
        {
            /// <summary>
            /// Reserved, must be zero.
            /// </summary>
            public UInt16 wReserved;
            /// <summary>
            /// Resource Type (1 for icons).
            /// </summary>
            public UInt16 wType;
            /// <summary>
            /// Number of images.
            /// </summary>
            public UInt16 wCount;
        }

        /// <summary>
        /// If this value is used, the system maps the file into the calling process's virtual address space as if it were a data file.
        /// </summary>
        internal const uint LOAD_LIBRARY_AS_DATAFILE = 0x00000002;
        /// <summary>
        /// If this value is used, and the executable module is a DLL, the system does not call DllMain for process and thread initialization and termination.
        /// </summary>
        internal const uint DONT_RESOLVE_DLL_REFERENCES = 0x00000001;
        /// <summary>
        /// If this value is used and lpFileName specifies an absolute path, the system uses the alternate file search strategy.
        /// </summary>
        internal const uint LOAD_WITH_ALTERED_SEARCH_PATH = 0x00000008;
        /// <summary>
        /// If this value is used, the system does not perform automatic trust comparisons on the DLL or its dependents when they are loaded.
        /// </summary>
        internal const uint LOAD_IGNORE_CODE_AUTHZ_LEVEL = 0x00000010;

        /// <summary>
        /// Loads the specified module into the address space of the calling process. 
        /// The specified module may cause other modules to be loaded.
        /// </summary>
        /// <param name="lpFileName">The name of the module.</param>
        /// <param name="hFile">This parameter is reserved for future use.</param>
        /// <param name="dwFlags">The action to be taken when loading the module.</param>
        /// <returns></returns>
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryExW")]
        internal static extern IntPtr LoadLibraryEx(string lpFileName, IntPtr hFile, uint dwFlags);

        /// <summary>
        /// Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
        /// </summary>
        /// <param name="hModule">A handle to the loaded library module.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// Predefined resource types.
        /// </summary>
        public enum ResourceTypes
        {
            /// <summary>
            /// Hardware-dependent cursor resource.
            /// </summary>
            RT_CURSOR = 1,
            /// <summary>
            /// Bitmap resource.
            /// </summary>
            RT_BITMAP = 2,
            /// <summary>
            /// Hardware-dependent icon resource.
            /// </summary>
            RT_ICON = 3,
            /// <summary>
            /// Menu resource.
            /// </summary>
            RT_MENU = 4,
            /// <summary>
            /// Dialog box.
            /// </summary>
            RT_DIALOG = 5,
            /// <summary>
            /// String-table entry.
            /// </summary>
            RT_STRING = 6,
            /// <summary>
            /// Font directory resource.
            /// </summary>
            RT_FONTDIR = 7,
            /// <summary>
            /// Font resource.
            /// </summary>
            RT_FONT = 8,
            /// <summary>
            /// Accelerator table.
            /// </summary>
            RT_ACCELERATOR = 9,
            /// <summary>
            /// Application-defined resource (raw data).
            /// </summary>
            RT_RCDATA = 10,
            /// <summary>
            /// Message-table entry.
            /// </summary>
            RT_MESSAGETABLE = 11,
            /// <summary>
            /// Hardware-independent cursor resource.
            /// </summary>
            RT_GROUP_CURSOR = 12,
            /// <summary>
            /// Hardware-independent icon resource.
            /// </summary>
            RT_GROUP_ICON = 14,
            /// <summary>
            /// Version resource.
            /// </summary>
            RT_VERSION = 16,
            /// <summary>
            /// Allows a resource editing tool to associate a string with an .rc file.
            /// </summary>
            RT_DLGINCLUDE = 17,
            /// <summary>
            /// Plug and Play resource.
            /// </summary>
            RT_PLUGPLAY = 19,
            /// <summary>
            /// VXD.
            /// </summary>
            RT_VXD = 20,
            /// <summary>
            /// Animated cursor.
            /// </summary>
            RT_ANICURSOR = 21,
            /// <summary>
            /// Animated icon.
            /// </summary>
            RT_ANIICON = 22,
            /// <summary>
            /// HTML.
            /// </summary>
            RT_HTML = 23,
            /// <summary>
            /// Microsoft Windows XP: Side-by-Side Assembly XML Manifest.
            /// </summary>
            RT_MANIFEST = 24,
        }

        /// <summary>
        /// Enumerates resource types within a binary module.
        /// </summary>
        /// <param name="hModule">Handle to a module to search.</param>
        /// <param name="lpEnumFunc">Pointer to the callback function to be called for each enumerated resource type.</param>
        /// <param name="lParam">Specifies an application-defined value passed to the callback function.</param>
        /// <returns>Returns TRUE if successful; otherwise, FALSE.</returns>
        [DllImport("kernel32.dll", EntryPoint = "EnumResourceTypesW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EnumResourceTypes(IntPtr hModule, EnumResourceTypesDelegate lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// An application-defined callback function used with the EnumResourceTypes and EnumResourceTypesEx functions.
        /// </summary>
        /// <param name="hModule">The handle to the module whose executable file contains the resources for which the types are to be enumerated.</param>
        /// <param name="lpszType">Pointer to a null-terminated string specifying the type name of the resource for which the type is being enumerated.</param>
        /// <param name="lParam">Specifies the application-defined parameter passed to the EnumResourceTypes or EnumResourceTypesEx function. </param>
        /// <returns>Returns TRUE if successful; otherwise, FALSE.</returns>
        internal delegate bool EnumResourceTypesDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lParam);

        /// <summary>
        /// Enumerates resources of a specified type within a binary module. 
        /// </summary>
        /// <param name="hModule">Handle to a module to search.</param>
        /// <param name="lpszType">Pointer to a null-terminated string specifying the type of the resource for which the name is being enumerated.</param>
        /// <param name="lpEnumFunc">Pointer to the callback function to be called for each enumerated resource name or ID.</param>
        /// <param name="lParam">Specifies an application-defined value passed to the callback function.</param>
        /// <returns>Returns TRUE if the function succeeds or FALSE if the function does not find a resource of the type specified, or if the function fails for another reason.</returns>
        [DllImport("kernel32.dll", EntryPoint = "EnumResourceNamesW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EnumResourceNames(IntPtr hModule, IntPtr lpszType, EnumResourceNamesDelegate lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// An application-defined callback function used with the EnumResourceNames and EnumResourceNamesEx functions.
        /// </summary>
        /// <param name="hModule">The handle to the module whose executable file contains the resources that are being enumerated.</param>
        /// <param name="lpszType">Pointer to a null-terminated string specifying the type of resource that is being enumerated.</param>
        /// <param name="lpszName">Specifies the name of a resource of the type being enumerated.</param>
        /// <param name="lParam">Specifies the application-defined parameter passed to the EnumResourceNames or EnumResourceNamesEx function.</param>
        /// <returns>Returns TRUE if the function succeeds or FALSE if the function does not find a resource of the type specified, or if the function fails for another reason.</returns>
        internal delegate bool EnumResourceNamesDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, IntPtr lParam);

        /// <summary>
        /// Enumerates language-specific resources, of the specified type and name, associated with a binary module.
        /// </summary>
        /// <param name="hModule">The handle to a module to search.</param>
        /// <param name="lpszType">Pointer to a null-terminated string specifying the type of resource for which the language is being enumerated.</param>
        /// <param name="lpszName">Pointer to a null-terminated string specifying the name of the resource for which the language is being enumerated.</param>
        /// <param name="lpEnumFunc">Pointer to the callback function to be called for each enumerated resource language.</param>
        /// <param name="lParam">Specifies an application-defined value passed to the callback function.</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.</returns>
        [DllImport("kernel32.dll", EntryPoint = "EnumResourceLanguagesW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool EnumResourceLanguages(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, EnumResourceLanguagesDelegate lpEnumFunc, IntPtr lParam);

        /// <summary>
        /// An application-defined callback function used with the EnumResourceLanguages and EnumResourceLanguagesEx functions.
        /// </summary>
        /// <param name="hModule">The handle to the module whose executable file contains the resources for which the languages are being enumerated.</param>
        /// <param name="lpszType">Pointer to a null-terminated string specifying the type name of the resource for which the language is being enumerated.</param>
        /// <param name="lpszName">Pointer to a null-terminated string specifying the name of the resource for which the language is being enumerated.</param>
        /// <param name="wIDLanguage">Specifies the language identifier for the resource for which the language is being enumerated.</param>
        /// <param name="lParam">Specifies the application-defined parameter passed to the EnumResourceLanguages or EnumResourceLanguagesEx function.</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.</returns>
        internal delegate bool EnumResourceLanguagesDelegate(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, UInt16 wIDLanguage, IntPtr lParam);

        /// <summary>
        /// Determines the location of the resource with the specified type, name, and language in the specified module.
        /// </summary>
        /// <param name="hModule">Handle to the module whose executable file contains the resource.</param>
        /// <param name="lpszType">Pointer to a null-terminated string specifying the type name of the resource.</param>
        /// <param name="lpszName">Pointer to a null-terminated string specifying the name of the resource.</param>
        /// <param name="wLanguage">Specifies the language of the resource.</param>
        /// <returns>If the function succeeds, the return value is a handle to the specified resource's information block.</returns>
        [DllImport("kernel32.dll", EntryPoint = "FindResourceExW", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindResourceEx(IntPtr hModule, IntPtr lpszType, IntPtr lpszName, UInt16 wLanguage);

        /// <summary>
        /// Locks the specified resource in memory.
        /// </summary>
        /// <param name="hResData">Handle to the resource to be locked.</param>
        /// <returns>If the loaded resource is locked, the return value is a pointer to the first byte of the resource; otherwise, it is NULL.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LockResource(IntPtr hResData);

        /// <summary>
        /// Loads the specified resource into global memory.
        /// </summary>
        /// <param name="hModule">Handle to the module whose executable file contains the resource.</param>
        /// <param name="hResData">Handle to the resource to be loaded.</param>
        /// <returns>If the function succeeds, the return value is a handle to the data associated with the resource.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResData);

        /// <summary>
        /// Returns the size, in bytes, of the specified resource. 
        /// </summary>
        /// <param name="hInstance">Handle to the module whose executable file contains the resource.</param>
        /// <param name="hResInfo">Handle to the resource. This handle must be created by using the FindResource or FindResourceEx function.</param>
        /// <returns>If the function succeeds, the return value is the number of bytes in the resource.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern int SizeofResource(IntPtr hInstance, IntPtr hResInfo);

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hHandle">A valid handle to an open object.</param>
        /// <returns>If the function succeeds, the return value is nonzero.</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hHandle);

        /// <summary>
        /// Returns a handle to either a language-neutral portable executable file (LN file) or a 
        /// language-specific resource file (.mui file) that can be used by the UpdateResource function 
        /// to add, delete, or replace resources in a binary module.
        /// </summary>
        /// <param name="pFileName">Pointer to a null-terminated string that specifies the binary file in which to update resources.</param>
        /// <param name="bDeleteExistingResources">Specifies whether to delete the pFileName parameter's existing resources.</param>
        /// <returns>If the function succeeds, the return value is a handle that can be used by the UpdateResource and EndUpdateResource functions.</returns>
        [DllImport("kernel32.dll", EntryPoint = "BeginUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern IntPtr BeginUpdateResource(string pFileName, bool bDeleteExistingResources);

        /// <summary>
        /// Adds, deletes, or replaces a resource in a portable executable (PE) file. 
        /// There are some restrictions on resource updates in files that contain Resource Configuration (RC Config) data: 
        /// language-neutral (LN) files and language-specific resource (.mui) files.
        /// </summary>
        /// <param name="hUpdate">A module handle returned by the BeginUpdateResource function, referencing the file to be updated.</param>
        /// <param name="lpType">Pointer to a null-terminated string specifying the resource type to be updated.</param>
        /// <param name="lpName">Pointer to a null-terminated string specifying the name of the resource to be updated.</param>
        /// <param name="wLanguage">Specifies the language identifier of the resource to be updated.</param>
        /// <param name="lpData">Pointer to the resource data to be inserted into the file indicated by hUpdate.</param>
        /// <param name="cbData">Specifies the size, in bytes, of the resource data at lpData.</param>
        /// <returns>Returns TRUE if successful or FALSE otherwise.</returns>
        [DllImport("kernel32.dll", EntryPoint = "UpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool UpdateResource(IntPtr hUpdate, IntPtr lpType, IntPtr lpName, UInt16 wLanguage, byte[] lpData, UInt32 cbData);

        /// <summary>
        /// Commits or discards changes made prior to a call to UpdateResource.
        /// </summary>
        /// <param name="hUpdate">A module handle returned by the BeginUpdateResource function, and used by UpdateResource, referencing the file to be updated.</param>
        /// <param name="fDiscard">Specifies whether to write the resource updates to the file. If this parameter is TRUE, no changes are made. If it is FALSE, the changes are made: the resource updates will take effect.</param>
        /// <returns>Returns TRUE if the function succeeds; FALSE otherwise.</returns>
        [DllImport("kernel32.dll", EntryPoint = "EndUpdateResourceW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool EndUpdateResource(IntPtr hUpdate, bool fDiscard);

        /// <summary>
        /// Neutral primary language ID.
        /// </summary>
        public const UInt16 LANG_NEUTRAL = 0;
        /// <summary>
        /// US-English primary language ID.
        /// </summary>
        public const UInt16 LANG_ENGLISH = 9;

        /// <summary>
        /// Neutral sublanguage ID.
        /// </summary>
        public const UInt16 SUBLANG_NEUTRAL = 0;
        /// <summary>
        /// US-English sublanguage ID.
        /// </summary>
        public const UInt16 SUBLANG_ENGLISH_US = 1;

        /// <summary>
        /// CREATEPROCESS_MANIFEST_RESOURCE_ID is used primarily for EXEs. If an executable has a resource of type RT_MANIFEST, 
        /// ID CREATEPROCESS_MANIFEST_RESOURCE_ID, Windows will create a process default activation context for the process. 
        /// The process default activation context will be used by all components running in the process. 
        /// CREATEPROCESS_MANIFEST_RESOURCE_ID can also used by DLLs. When Windows probe for dependencies, if the dll has 
        /// a resource of type RT_MANIFEST, ID CREATEPROCESS_MANIFEST_RESOURCE_ID, Windows will use that manifest as the 
        /// dependency. 
        /// </summary>
        public const UInt16 CREATEPROCESS_MANIFEST_RESOURCE_ID  = 1;
        /// <summary>
        /// ISOLATIONAWARE_MANIFEST_RESOURCE_ID is used primarily for DLLs. It should be used if the dll wants private 
        /// dependencies other than the process default. For example, if an dll depends on comctl32.dll version 6.0.0.0. 
        /// It should have a resource of type RT_MANIFEST, ID ISOLATIONAWARE_MANIFEST_RESOURCE_ID to depend on comctl32.dll 
        /// version 6.0.0.0, so that even if the process executable wants comctl32.dll version 5.1, the dll itself will still 
        /// use the right version of comctl32.dll. 
        /// </summary>
        public const UInt16 ISOLATIONAWARE_MANIFEST_RESOURCE_ID = 2;
        /// <summary>
        /// When ISOLATION_AWARE_ENABLED is defined, Windows re-defines certain APIs. For example LoadLibraryExW 
        /// is redefined to IsolationAwareLoadLibraryExW. 
        /// </summary>
        public const UInt16 ISOLATIONAWARE_NOSTATICIMPORT_MANIFEST_RESOURCE_ID = 3;
        /// <summary>
        /// Resource manifest type.
        /// </summary>
        public enum ManifestType
        {
            /// <summary>
            /// CREATEPROCESS_MANIFEST_RESOURCE_ID
            /// </summary>
            CreateProcess = CREATEPROCESS_MANIFEST_RESOURCE_ID,
            /// <summary>
            /// ISOLATIONAWARE_MANIFEST_RESOURCE_ID
            /// </summary>
            IsolationAware = ISOLATIONAWARE_MANIFEST_RESOURCE_ID,
            /// <summary>
            /// ISOLATIONAWARE_NOSTATICIMPORT_MANIFEST_RESOURCE_ID
            /// </summary>
            IsolationAwareNonstaticImport = ISOLATIONAWARE_NOSTATICIMPORT_MANIFEST_RESOURCE_ID
        }
    }
}
