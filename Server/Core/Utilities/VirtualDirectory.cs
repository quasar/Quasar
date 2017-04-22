using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using xServer.Core.Helper;

namespace xServer.Core.Utilities
{
    public class VirtualFile 
    {
        public long Size { get; set; }
        public string Name { get; set; }

        public static VirtualFile Create(string path)
        {
            var fi = new FileInfo(path);
            return new VirtualFile(fi.Length, fi.Name, File.ReadAllBytes(path));
        }

        public byte[] Data { get; set; }
        public VirtualFile(long size, string name, byte[] data)
        {
            Size = size;
            Name = name;
            Data = data;
        }
    }
    public class VirtualDirectory
    {
        public string Name { get; set; }

        public List<VirtualDirectory> SubDirectories { get; set; }
        public List<VirtualFile> Files { get; set; }
        public List<string> Items { get; set; }
        public bool Verified { get; private set; }
        public long Size { get { return Serialize().Length; } }

        private readonly byte[] _data;
        private readonly string _rootPath;
        private const int Magic = 0x0BBE101;

        public VirtualDirectory()
        {
            SubDirectories = new List<VirtualDirectory>();
            Files = new List<VirtualFile>();
        }

        public VirtualDirectory(string path, string[] items = null)
        {
            SubDirectories = new List<VirtualDirectory>();
            Files = new List<VirtualFile>();
            Items = new List<string>(items == null ? new string[] { } : items);
            _rootPath = path;
            Name = new DirectoryInfo(path).Name;
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

                return ms.ToArray();
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
                SubDirectories.Add(Create(dir));
        }

        private void LoadFiles()
        {
            foreach (var file in Directory.GetFiles(_rootPath))
                Files.Add(VirtualFile.Create(file));
        }
    }
}
