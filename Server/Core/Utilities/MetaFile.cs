using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using xServer.Core.Packets.ServerPackets;

namespace xServer.Core.Utilities
{
    [Flags]
    public enum TransferType : byte
    {
        Upload = 1,
        Download = 2,
        File = 4,
        Folder = 8
    }

    public class MetaFile
    {
        public int CurrentBlock { get; set; }
        public int TransferId { get; set; }
        public decimal Progress { get; set; }
        public byte[] PrevHash { get; set; }
        public byte[] CurHash { get; set; }
        public string RemotePath { get; set; }
        public string LocalPath { get; set; }
        public TransferType Type { get; set; }
        public string[] FolderItems { get; set; }
        public ItemOption[] FolderItemOptions { get; set; }

        public MetaFile(int currentBlock, int transferId, decimal progress, byte[] prevHash, byte[] curHash, string remotePath, string localPath, TransferType type)
        {
            CurrentBlock = currentBlock;
            TransferId = transferId;
            Progress = progress;
            PrevHash = prevHash;
            CurHash = curHash;
            RemotePath = remotePath;
            Type = type;
            LocalPath = localPath;
        }

        public MetaFile(byte[] metadata)
        {
            using (var ms = new MemoryStream(metadata))
            using (var br = new BinaryReader(ms))
            {
                Type = (TransferType)br.ReadByte();
                CurrentBlock = br.ReadInt32();
                TransferId = br.ReadInt32();
                Progress = (decimal) br.ReadDouble();
                PrevHash = br.ReadBytes(16);
                CurHash = br.ReadBytes(16);
                RemotePath = br.ReadString();
                LocalPath = br.ReadString();
                if (br.ReadBoolean())
                {
                    var count = br.ReadInt32();
                    FolderItems = new string[count];
                    for (int i = 0; i < count; i++)
                        FolderItems[i] = br.ReadString();

                    count = br.ReadInt32();
                    FolderItemOptions = new ItemOption[count];
                    for (int i = 0; i < count; i++)
                        FolderItemOptions[i] = (ItemOption)br.ReadByte();
                }
            }
            //int idx = 0;

            //Type = (TransferType)metadata[idx++];
            //CurrentBlock = BitConverter.ToInt32(metadata, idx);
            //idx += sizeof(int);
            //TransferId = BitConverter.ToInt32(metadata, idx);
            //idx += sizeof(int);
            //Progress = (decimal)BitConverter.ToDouble(metadata, idx);
            //idx += sizeof(double);
            //PrevHash = new byte[16];
            //CurHash = new byte[16];
            //Buffer.BlockCopy(metadata, idx, PrevHash, 0, 16);
            //idx += 16;
            //Buffer.BlockCopy(metadata, idx, CurHash, 0, 16);
            //idx += 16;
            //var strBuff = new byte[metadata.Length - idx];
            //Buffer.BlockCopy(metadata, idx, strBuff, 0, strBuff.Length);
            //RemotePath = Encoding.UTF8.GetString(strBuff);
        }

        public void Save(string path)
        {
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            using (var bw = new BinaryWriter(fs))
            {
                bw.Write((byte) Type);
                bw.Write(CurrentBlock);
                bw.Write(TransferId);
                bw.Write((double) Progress);
                bw.Write(PrevHash);
                bw.Write(CurHash);
                bw.Write(RemotePath);
                bw.Write(LocalPath);
                if (FolderItems != null && FolderItems.Length > 0)
                {
                    bw.Write(true);
                    bw.Write(FolderItems.Length);
                    foreach (var fi in FolderItems)
                        bw.Write(fi);

                    bw.Write(FolderItemOptions.Length);
                    foreach (var fio in FolderItemOptions)
                        bw.Write((byte) fio);
                }
                else
                    bw.Write(false);
            }
            //    var data = new List<byte>();
            //data.Add((byte)Type);
            //data.AddRange(BitConverter.GetBytes(CurrentBlock));
            //data.AddRange(BitConverter.GetBytes(TransferId));
            //data.AddRange(BitConverter.GetBytes((double)Progress));
            //data.AddRange(PrevHash);
            //data.AddRange(CurHash);
            //data.AddRange(Encoding.UTF8.GetBytes(RemotePath));

            //File.WriteAllBytes(path, data.ToArray());
        }
    }
}