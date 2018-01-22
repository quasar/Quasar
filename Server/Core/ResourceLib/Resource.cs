using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.IO;

namespace xServer.Core.ResourceLib
{
    /// <summary>
    /// A version resource.
    /// </summary>
    public abstract class Resource
    {
        /// <summary>
        /// Resource type.
        /// </summary>
        protected ResourceId _type;
        /// <summary>
        /// Resource name.
        /// </summary>
        protected ResourceId _name;
        /// <summary>
        /// Resource language.
        /// </summary>
        protected UInt16 _language;
        /// <summary>
        /// Loaded binary nodule.
        /// </summary>
        protected IntPtr _hModule = IntPtr.Zero;
        /// <summary>
        /// Pointer to the resource.
        /// </summary>
        protected IntPtr _hResource = IntPtr.Zero;
        /// <summary>
        /// Resource size.
        /// </summary>
        protected int _size = 0;

        /// <summary>
        /// Version resource size in bytes.
        /// </summary>
        public int Size
        {
            get
            {
                return _size;
            }
        }

        /// <summary>
        /// Language ID.
        /// </summary>
        public UInt16 Language
        {
            get
            {
                return _language;
            }
            set
            {
                _language = value;
            }
        }

        /// <summary>
        /// Resource type.
        /// </summary>
        public ResourceId Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// String representation of the resource type.
        /// </summary>
        public string TypeName
        {
            get
            {
                return _type.TypeName;
            }
        }

        /// <summary>
        /// Resource name.
        /// </summary>
        public ResourceId Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        /// <summary>
        /// A new resource.
        /// </summary>
        internal Resource()
        {

        }

        /// <summary>
        /// A structured resource embedded in an executable module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource handle.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="language">Language ID.</param>
        /// <param name="size">Resource size.</param>
        internal Resource(IntPtr hModule, IntPtr hResource, ResourceId type, ResourceId name, UInt16 language, int size)
        {
            _hModule = hModule;
            _type = type;
            _name = name;
            _language = language;
            _hResource = hResource;
            _size = size;

            LockAndReadResource(hModule, hResource);
        }

        /// <summary>
        /// Lock and read the resource.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="hResource">Resource handle.</param>
        internal void LockAndReadResource(IntPtr hModule, IntPtr hResource)
        {
            if (hResource == IntPtr.Zero)
                return;

            IntPtr lpRes = Kernel32.LockResource(hResource);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            Read(hModule, lpRes);
        }

        /// <summary>
        /// Load a resource from an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">An executable (.exe or .dll) file.</param>
        public virtual void LoadFrom(string filename)
        {
            LoadFrom(filename, _type, _name, _language);
        }

        /// <summary>
        /// Load a resource from an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">An executable (.exe or .dll) file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="lang">Resource language.</param>
        internal void LoadFrom(string filename, ResourceId type, ResourceId name, UInt16 lang)
        {
            IntPtr hModule = IntPtr.Zero;

            try
            {
                hModule = Kernel32.LoadLibraryEx(filename, IntPtr.Zero,
                    Kernel32.DONT_RESOLVE_DLL_REFERENCES | Kernel32.LOAD_LIBRARY_AS_DATAFILE);

                LoadFrom(hModule, type, name, lang);
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                    Kernel32.FreeLibrary(hModule);
            }
        }

        /// <summary>
        /// Load a resource from an executable (.exe or .dll) module.
        /// </summary>
        /// <param name="hModule">An executable (.exe or .dll) module.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="lang">Resource language.</param>
        internal void LoadFrom(IntPtr hModule, ResourceId type, ResourceId name, UInt16 lang)
        {
            if (IntPtr.Zero == hModule)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr hRes = Kernel32.FindResourceEx(hModule, type.Id, name.Id, lang);
            if (IntPtr.Zero == hRes)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr hGlobal = Kernel32.LoadResource(hModule, hRes);
            if (IntPtr.Zero == hGlobal)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            IntPtr lpRes = Kernel32.LockResource(hGlobal);

            if (lpRes == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _size = Kernel32.SizeofResource(hModule, hRes);
            if (_size <= 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            _type = type;
            _name = name;
            _language = lang;

            Read(hModule, lpRes);
        }

        /// <summary>
        /// Read a resource from a previously loaded module.
        /// </summary>
        /// <param name="hModule">Module handle.</param>
        /// <param name="lpRes">Pointer to the beginning of the resource.</param>
        /// <returns>Pointer to the end of the resource.</returns>
        internal abstract IntPtr Read(IntPtr hModule, IntPtr lpRes);

        /// <summary>
        /// Write the resource to a memory stream.
        /// </summary>
        /// <param name="w">Binary stream.</param>
        internal abstract void Write(BinaryWriter w);

        /// <summary>
        /// Return resource data.
        /// </summary>
        /// <returns>Resource data.</returns>
        public byte[] WriteAndGetBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter w = new BinaryWriter(ms, Encoding.Default);
            Write(w);
            w.Close();
            return ms.ToArray();
        }

        /// <summary>
        /// Save a resource.
        /// </summary>
        /// <param name="filename">Name of an executable file (.exe or .dll).</param>
        public virtual void SaveTo(string filename)
        {
            SaveTo(filename, _type, _name, _language);
        }

        /// <summary>
        /// Save a resource to an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="langid">Language id.</param>
        internal void SaveTo(string filename, ResourceId type, ResourceId name, UInt16 langid)
        {
            byte[] data = WriteAndGetBytes();
            SaveTo(filename, type, name, langid, data);
        }

        /// <summary>
        /// Delete a resource from an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        public virtual void DeleteFrom(string filename)
        {
            Delete(filename, _type, _name, _language);
        }

        /// <summary>
        /// Delete a resource from an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="lang">Resource language.</param>
        internal static void Delete(string filename, ResourceId type, ResourceId name, UInt16 lang)
        {
            SaveTo(filename, type, name, lang, null);
        }

        /// <summary>
        /// Save a resource to an executable (.exe or .dll) file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        /// <param name="name">Resource name.</param>
        /// <param name="type">Resource type.</param>
        /// <param name="lang">Resource language.</param>
        /// <param name="data">Resource data.</param>
        internal static void SaveTo(string filename, ResourceId type, ResourceId name, UInt16 lang, byte[] data)
        {
            IntPtr h = Kernel32.BeginUpdateResource(filename, false);

            if (h == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                if (data != null && data.Length == 0)
                {
                    data = null;
                }
                if (!Kernel32.UpdateResource(h, type.Id, name.Id,
                    lang, data, (data == null ? 0 : (uint)data.Length)))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            catch
            {
                Kernel32.EndUpdateResource(h, true);
                throw;
            }

            if (!Kernel32.EndUpdateResource(h, false))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        /// <summary>
        /// Save a batch of resources to a given file.
        /// </summary>
        /// <param name="filename">Path to an executable file.</param>
        /// <param name="resources">The resources to write.</param>
        public static void Save(string filename, IEnumerable<Resource> resources)
        {
            IntPtr h = Kernel32.BeginUpdateResource(filename, false);

            if (h == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            try
            {
                foreach (var resource in resources)
                {
                    var imageResource = resource as IconImageResource;
                    if (imageResource != null)
                    {
                        var bytes = imageResource.Image == null ? null : imageResource.Image.Data;
                        if (!Kernel32.UpdateResource(h, imageResource.Type.Id, new IntPtr(imageResource.Id),
                            imageResource.Language, bytes, (bytes == null ? 0 : (uint)bytes.Length)))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                    else
                    {
                        var bytes = resource.WriteAndGetBytes();
                        if (bytes != null && bytes.Length == 0)
                        {
                            bytes = null;
                        }
                        if (!Kernel32.UpdateResource(h, resource.Type.Id, resource.Name.Id,
                            resource.Language, bytes, (bytes == null ? 0 : (uint)bytes.Length)))
                        {
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                }
            }
            catch
            {
                Kernel32.EndUpdateResource(h, true);
                throw;
            }

            if (!Kernel32.EndUpdateResource(h, false))
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
