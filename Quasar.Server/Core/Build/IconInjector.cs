using System;
using System.Runtime.InteropServices;
using System.Security;

namespace xServer.Core.Build
{
    public static class IconInjector
    {
        // Basically, you can change icons with the UpdateResource api call.
        // When you make the call you say "I'm updating an icon", and you send the icon data.
        // The main problem is that ICO files store the icons in one set of structures, and exe/dll files store them in
        // another set of structures. So you have to translate between the two -- you can't just load the ICO file as
        // bytes and send them with the UpdateResource api call.

        [SuppressUnmanagedCodeSecurity()]
        private class NativeMethods
        {
            [DllImport("kernel32")]
            public static extern IntPtr BeginUpdateResource(string fileName,
                [MarshalAs(UnmanagedType.Bool)] bool deleteExistingResources);

            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UpdateResource(IntPtr hUpdate, IntPtr type, IntPtr name, short language,
                [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)] byte[] data, int dataSize);

            [DllImport("kernel32")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EndUpdateResource(IntPtr hUpdate, [MarshalAs(UnmanagedType.Bool)] bool discard);
        }

        // The first structure in an ICO file lets us know how many images are in the file.
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONDIR
        {
            // Reserved, must be 0
            public ushort Reserved;
            // Resource type, 1 for icons.
            public ushort Type;
            // How many images.
            public ushort Count;
            // The native structure has an array of ICONDIRENTRYs as a final field.
        }

        // Each ICONDIRENTRY describes one icon stored in the ico file. The offset says where the icon image data
        // starts in the file. The other fields give the information required to turn that image data into a valid
        // bitmap.
        [StructLayout(LayoutKind.Sequential)]
        private struct ICONDIRENTRY
        {
            /// <summary>
            /// The width, in pixels, of the image.
            /// </summary>
            public byte Width;
            /// <summary>
            /// The height, in pixels, of the image.
            /// </summary>
            public byte Height;
            /// <summary>
            /// The number of colors in the image; (0 if >= 8bpp)
            /// </summary>
            public byte ColorCount;
            /// <summary>
            /// Reserved (must be 0).
            /// </summary>
            public byte Reserved;
            /// <summary>
            /// Color planes.
            /// </summary>
            public ushort Planes;
            /// <summary>
            /// Bits per pixel.
            /// </summary>
            public ushort BitCount;
            /// <summary>
            /// The length, in bytes, of the pixel data.
            /// </summary>
            public int BytesInRes;
            /// <summary>
            /// The offset in the file where the pixel data starts.
            /// </summary>
            public int ImageOffset;
        }

        // Each image is stored in the file as an ICONIMAGE structure:
        //typdef struct
        //{
        //   BITMAPINFOHEADER   icHeader;      // DIB header
        //   RGBQUAD         icColors[1];   // Color table
        //   BYTE            icXOR[1];      // DIB bits for XOR mask
        //   BYTE            icAND[1];      // DIB bits for AND mask
        //} ICONIMAGE, *LPICONIMAGE;


        [StructLayout(LayoutKind.Sequential)]
        private struct BITMAPINFOHEADER
        {
            public uint Size;
            public int Width;
            public int Height;
            public ushort Planes;
            public ushort BitCount;
            public uint Compression;
            public uint SizeImage;
            public int XPelsPerMeter;
            public int YPelsPerMeter;
            public uint ClrUsed;
            public uint ClrImportant;
        }

        // The icon in an exe/dll file is stored in a very similar structure:
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private struct GRPICONDIRENTRY
        {
            public byte Width;
            public byte Height;
            public byte ColorCount;
            public byte Reserved;
            public ushort Planes;
            public ushort BitCount;
            public int BytesInRes;
            public ushort ID;
        }

        public static void InjectIcon(string exeFileName, string iconFileName)
        {
            InjectIcon(exeFileName, iconFileName, 1, 1);
        }

        public static void InjectIcon(string exeFileName, string iconFileName, uint iconGroupID, uint iconBaseID)
        {
            const uint RT_ICON = 3u;
            const uint RT_GROUP_ICON = 14u;
            IconFile iconFile = IconFile.FromFile(iconFileName);
            var hUpdate = NativeMethods.BeginUpdateResource(exeFileName, false);
            var data = iconFile.CreateIconGroupData(iconBaseID);
            NativeMethods.UpdateResource(hUpdate, new IntPtr(RT_GROUP_ICON), new IntPtr(iconGroupID), 0, data,
                data.Length);
            for (int i = 0; i <= iconFile.ImageCount - 1; i++)
            {
                var image = iconFile.ImageData(i);
                NativeMethods.UpdateResource(hUpdate, new IntPtr(RT_ICON), new IntPtr(iconBaseID + i), 0, image,
                    image.Length);
            }
            NativeMethods.EndUpdateResource(hUpdate, false);
        }

        private class IconFile
        {
            private ICONDIR iconDir = new ICONDIR();
            private ICONDIRENTRY[] iconEntry;

            private byte[][] iconImage;

            public int ImageCount
            {
                get { return iconDir.Count; }
            }

            public byte[] ImageData(int index)
            {
                return iconImage[index];
            }

            public static IconFile FromFile(string filename)
            {
                IconFile instance = new IconFile();
                // Read all the bytes from the file.
                byte[] fileBytes = System.IO.File.ReadAllBytes(filename);
                // First struct is an ICONDIR
                // Pin the bytes from the file in memory so that we can read them.
                // If we didn't pin them then they could move around (e.g. when the
                // garbage collector compacts the heap)
                GCHandle pinnedBytes = GCHandle.Alloc(fileBytes, GCHandleType.Pinned);
                // Read the ICONDIR
                instance.iconDir = (ICONDIR) Marshal.PtrToStructure(pinnedBytes.AddrOfPinnedObject(), typeof (ICONDIR));
                // which tells us how many images are in the ico file. For each image, there's a ICONDIRENTRY, and associated pixel data.
                instance.iconEntry = new ICONDIRENTRY[instance.iconDir.Count];
                instance.iconImage = new byte[instance.iconDir.Count][];
                // The first ICONDIRENTRY will be immediately after the ICONDIR, so the offset to it is the size of ICONDIR
                int offset = Marshal.SizeOf(instance.iconDir);
                // After reading an ICONDIRENTRY we step forward by the size of an ICONDIRENTRY            
                var iconDirEntryType = typeof (ICONDIRENTRY);
                var size = Marshal.SizeOf(iconDirEntryType);
                for (int i = 0; i <= instance.iconDir.Count - 1; i++)
                {
                    // Grab the structure.
                    var entry =
                        (ICONDIRENTRY)
                            Marshal.PtrToStructure(new IntPtr(pinnedBytes.AddrOfPinnedObject().ToInt64() + offset),
                                iconDirEntryType);
                    instance.iconEntry[i] = entry;
                    // Grab the associated pixel data.
                    instance.iconImage[i] = new byte[entry.BytesInRes];
                    Buffer.BlockCopy(fileBytes, entry.ImageOffset, instance.iconImage[i], 0, entry.BytesInRes);
                    offset += size;
                }
                pinnedBytes.Free();
                return instance;
            }

            public byte[] CreateIconGroupData(uint iconBaseID)
            {
                // This will store the memory version of the icon.
                int sizeOfIconGroupData = Marshal.SizeOf(typeof (ICONDIR)) +
                                          Marshal.SizeOf(typeof (GRPICONDIRENTRY))*ImageCount;
                byte[] data = new byte[sizeOfIconGroupData];
                var pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);
                Marshal.StructureToPtr(iconDir, pinnedData.AddrOfPinnedObject(), false);
                var offset = Marshal.SizeOf(iconDir);
                for (int i = 0; i <= ImageCount - 1; i++)
                {
                    GRPICONDIRENTRY grpEntry = new GRPICONDIRENTRY();
                    BITMAPINFOHEADER bitmapheader = new BITMAPINFOHEADER();
                    var pinnedBitmapInfoHeader = GCHandle.Alloc(bitmapheader, GCHandleType.Pinned);
                    Marshal.Copy(ImageData(i), 0, pinnedBitmapInfoHeader.AddrOfPinnedObject(),
                        Marshal.SizeOf(typeof (BITMAPINFOHEADER)));
                    pinnedBitmapInfoHeader.Free();
                    grpEntry.Width = iconEntry[i].Width;
                    grpEntry.Height = iconEntry[i].Height;
                    grpEntry.ColorCount = iconEntry[i].ColorCount;
                    grpEntry.Reserved = iconEntry[i].Reserved;
                    grpEntry.Planes = bitmapheader.Planes;
                    grpEntry.BitCount = bitmapheader.BitCount;
                    grpEntry.BytesInRes = iconEntry[i].BytesInRes;
                    grpEntry.ID = Convert.ToUInt16(iconBaseID + i);
                    Marshal.StructureToPtr(grpEntry, new IntPtr(pinnedData.AddrOfPinnedObject().ToInt64() + offset),
                        false);
                    offset += Marshal.SizeOf(typeof (GRPICONDIRENTRY));
                }
                pinnedData.Free();
                return data;
            }
        }
    }
}