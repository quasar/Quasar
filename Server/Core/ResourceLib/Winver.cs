using System;
using System.Collections.Generic;
using System.Text;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// winver.h, version management functions, types and definitions
    /// </summary>
    public abstract class Winver
    {
        /// <summary>
        /// VS_VERSION signature.
        /// </summary>
        public const UInt32 VS_FFI_SIGNATURE = 0xFEEF04BD;
        /// <summary>
        /// VS_VERSION structure version.
        /// </summary>
        public const UInt32 VS_FFI_STRUCVERSION = 0x00010000;
        /// <summary>
        /// VS_VERSION file flags mask.
        /// </summary>
        public const UInt32 VS_FFI_FILEFLAGSMASK = 0x0000003F;

        /// <summary>
        /// VS_VERSION file flags.
        /// </summary>
        public enum FileFlags : uint
        {
            /// <summary>
            /// The file contains debugging information.
            /// </summary>
            VS_FF_DEBUG = 0x00000001,
            /// <summary>
            /// The file is a prerelease development version, not a final commercial release.
            /// </summary>
            VS_FF_PRERELEASE = 0x00000002,
            /// <summary>
            /// PThe file has been modified somehow and is not identical to the original file
            /// that shipped with the product. 
            /// </summary>
            VS_FF_PATCHED = 0x00000004,
            /// <summary>
            /// The file was not built using standard release procedures. There should be data 
            /// in the file's "PrivateBuild" version information string. 
            /// </summary>
            VS_FF_PRIVATEBUILD = 0x00000008,
            /// <summary>
            /// The version information in this structure was not found inside the file, 
            /// but instead was created when needed based on the best information available. 
            /// Therefore, this structure's information may differ slightly from what the "real"
            /// values are.
            /// </summary>
            VS_FF_INFOINFERRED = 0x00000010,
            /// <summary>
            /// The file was built using standard release procedures, but is somehow different 
            /// from the normal file having the same version number. There should be data in the 
            /// file's "SpecialBuild" version information string.
            /// </summary>
            VS_FF_SPECIALBUILD = 0x00000020,
        }

        /// <summary>
        /// VS_VERSION file OSs.
        /// </summary>
        public enum FileOs : uint
        {
            /// <summary>
            /// The operating system under which the file was designed to run could not be determined.
            /// </summary>
            VOS_UNKNOWN = 0x00000000,
            /// <summary>
            /// The file was designed to run under MS-DOS. 
            /// </summary>
            VOS_DOS = 0x00010000,
            /// <summary>
            /// The file was designed to run under a 16-bit version of OS/2. 
            /// </summary>
            VOS_OS216 = 0x00020000,
            /// <summary>
            /// The file was designed to run under a 32-bit version of OS/2.
            /// </summary>
            VOS_OS232 = 0x00030000,
            /// <summary>
            /// The file was designed to run under Windows NT/2000.
            /// </summary>
            VOS_NT = 0x00040000,
            /// <summary>
            /// 
            /// </summary>
            VOS_WINCE = 0x00050000,
            /// <summary>
            /// The file was designed to run under the 16-bit Windows API. 
            /// </summary>
            VOS__WINDOWS16 = 0x00000001,
            /// <summary>
            /// The file was designed to be run under a 16-bit version of Presentation Manager. 
            /// </summary>
            VOS__PM16 = 0x00000002,
            /// <summary>
            /// The file was designed to be run under a 32-bit version of Presentation Manager.
            /// </summary>
            VOS__PM32 = 0x00000003,
            /// <summary>
            /// The file was designed to run under the 32-bit Windows API. 
            /// </summary>
            VOS__WINDOWS32 = 0x00000004,
            /// <summary>
            /// 
            /// </summary>
            VOS_DOS_WINDOWS16 = 0x00010001,
            /// <summary>
            /// 
            /// </summary>
            VOS_DOS_WINDOWS32 = 0x00010004,
            /// <summary>
            /// 
            /// </summary>
            VOS_OS216_PM16 = 0x00020002,
            /// <summary>
            /// 
            /// </summary>
            VOS_OS232_PM32 = 0x00030003,
            /// <summary>
            /// 
            /// </summary>
            VOS_NT_WINDOWS32 = 0x00040004
        }

        /// <summary>
        /// VS_VERSION file types.
        /// </summary>
        public enum FileType : uint
        {
            /// <summary>
            /// The type of file could not be determined.
            /// </summary>
            VFT_UNKNOWN = 0x00000000,
            /// <summary>
            /// The file is an application.
            /// </summary>
            VFT_APP = 0x00000001,
            /// <summary>
            /// The file is a Dynamic Link Library (DLL). 
            /// </summary>
            VFT_DLL = 0x00000002,
            /// <summary>
            /// The file is a device driver. dwFileSubtype contains more information. 
            /// </summary>
            VFT_DRV = 0x00000003,
            /// <summary>
            /// The file is a font. dwFileSubtype contains more information. 
            /// </summary>
            VFT_FONT = 0x00000004,
            /// <summary>
            /// The file is a virtual device.
            /// </summary>
            VFT_VXD = 0x00000005,
            /// <summary>
            /// The file is a static link library.
            /// </summary>
            VFT_STATIC_LIB = 0x00000007
        }

        /// <summary>
        /// File sub-type.
        /// </summary>
        public enum FileSubType : uint
        {
            /// <summary>
            /// The type of driver could not be determined. 
            /// </summary>
            VFT2_UNKNOWN = 0x00000000,
            /// <summary>
            /// The file is a printer driver.
            /// </summary>
            VFT2_DRV_PRINTER = 0x00000001,
            /// <summary>
            /// The file is a keyboard driver. 
            /// </summary>
            VFT2_DRV_KEYBOARD = 0x00000002,
            /// <summary>
            /// The file is a language driver. 
            /// </summary>
            VFT2_DRV_LANGUAGE = 0x00000003,
            /// <summary>
            /// The file is a display driver. 
            /// </summary>
            VFT2_DRV_DISPLAY = 0x00000004,
            /// <summary>
            /// The file is a mouse driver. 
            /// </summary>
            VFT2_DRV_MOUSE = 0x00000005,
            /// <summary>
            /// The file is a network driver. 
            /// </summary>
            VFT2_DRV_NETWORK = 0x00000006,
            /// <summary>
            /// The file is a system driver. 
            /// </summary>
            VFT2_DRV_SYSTEM = 0x00000007,
            /// <summary>
            /// The file is an installable driver. 
            /// </summary>
            VFT2_DRV_INSTALLABLE = 0x00000008,
            /// <summary>
            /// The file is a sound driver. 
            /// </summary>
            VFT2_DRV_SOUND = 0x00000009,
            /// <summary>
            /// The file is a communications driver. 
            /// </summary>
            VFT2_DRV_COMM = 0x0000000A,
            /// <summary>
            /// The file is an input method driver.
            /// </summary>
            VFT2_DRV_INPUTMETHOD = 0x0000000B,
            /// <summary>
            /// The file is a versioned printer driver.
            /// </summary>
            VFT2_DRV_VERSIONED_PRINTER = 0x0000000C,
            /// <summary>
            /// The file is a raster font.
            /// </summary>
            VFT2_FONT_RASTER = 0x00000001,
            /// <summary>
            /// The file is a vector font. 
            /// </summary>
            VFT2_FONT_VECTOR = 0x00000002,
            /// <summary>
            /// The file is a TrueType font. 
            /// </summary>
            VFT2_FONT_TRUETYPE = 0x00000003,
        }
    }
}
