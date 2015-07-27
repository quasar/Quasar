using System;
using System.IO;

namespace xClient.Core.Utilities
{
    public class FileSplit
    {
        private int _maxBlocks;
        private readonly object _fileStreamLock = new object();
        private const int MAX_BLOCK_SIZE = 65535;
        public string Path { get; private set; }
        public string LastError { get; private set; }

        public int MaxBlocks
        {
            get
            {
                if (this._maxBlocks > 0 || this._maxBlocks == -1)
                    return this._maxBlocks;
                try
                {
                    FileInfo fInfo = new FileInfo(this.Path);

                    if (!fInfo.Exists)
                        throw new FileNotFoundException();

                    this._maxBlocks = (int)Math.Ceiling(fInfo.Length / (double)MAX_BLOCK_SIZE);
                }
                catch (UnauthorizedAccessException)
                {
                    this._maxBlocks = -1;
                    this.LastError = "Access denied";
                }
                catch (IOException ex)
                {
                    this._maxBlocks = -1;

                    if (ex is FileNotFoundException)
                        this.LastError = "File not found";
                    if (ex is PathTooLongException)
                        this.LastError = "Path is too long";
                }

                return this._maxBlocks;
            }
        }

        public FileSplit(string path)
        {
            this.Path = path;
        }

        private int GetSize(long length)
        {
            return (length < MAX_BLOCK_SIZE) ? (int)length : MAX_BLOCK_SIZE;
        }

        public bool ReadBlock(int blockNumber, out byte[] readBytes)
        {
            try
            {
                if (blockNumber > this.MaxBlocks)
                    throw new ArgumentOutOfRangeException();

                lock (_fileStreamLock)
                {
                    using (FileStream fStream = File.OpenRead(this.Path))
                    {
                        if (blockNumber == 0)
                        {
                            fStream.Seek(0, SeekOrigin.Begin);
                            var length = fStream.Length - fStream.Position;
                            if (length < 0)
                                throw new IOException("negative length");
                            readBytes = new byte[this.GetSize(length)];
                            fStream.Read(readBytes, 0, readBytes.Length);
                        }
                        else
                        {
                            fStream.Seek(blockNumber * MAX_BLOCK_SIZE, SeekOrigin.Begin);
                            var length = fStream.Length - fStream.Position;
                            if (length < 0)
                                throw new IOException("negative length");
                            readBytes = new byte[this.GetSize(length)];
                            fStream.Read(readBytes, 0, readBytes.Length);
                        }
                    }
                }

                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                readBytes = new byte[0];
                this.LastError = "BlockNumber bigger than MaxBlocks";
            }
            catch (UnauthorizedAccessException)
            {
                readBytes = new byte[0];
                this.LastError = "Access denied";
            }
            catch (IOException ex)
            {
                readBytes = new byte[0];

                if (ex is FileNotFoundException)
                    this.LastError = "File not found";
                else if (ex is DirectoryNotFoundException)
                    this.LastError = "Directory not found";
                else if (ex is PathTooLongException)
                    this.LastError = "Path is too long";
                else
                    this.LastError = "Unable to read from File Stream";
            }

            return false;
        }

        public bool AppendBlock(byte[] block, int blockNumber)
        {
            try
            {
                if (!File.Exists(this.Path) && blockNumber > 0)
                    throw new FileNotFoundException(); // previous file got deleted somehow, error

                lock (_fileStreamLock)
                {
                    if (blockNumber == 0)
                    {
                        using (FileStream fStream = File.Open(this.Path, FileMode.Create, FileAccess.Write))
                        {
                            fStream.Seek(0, SeekOrigin.Begin);
                            fStream.Write(block, 0, block.Length);
                        }

                        return true;
                    }

                    using (FileStream fStream = File.Open(this.Path, FileMode.Append, FileAccess.Write))
                    {
                        fStream.Seek(blockNumber * MAX_BLOCK_SIZE, SeekOrigin.Begin);
                        fStream.Write(block, 0, block.Length);
                    }
                }

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                this.LastError = "Access denied";
            }
            catch (IOException ex)
            {
                if (ex is FileNotFoundException)
                    this.LastError = "File not found";
                else if (ex is DirectoryNotFoundException)
                    this.LastError = "Directory not found";
                else if (ex is PathTooLongException)
                    this.LastError = "Path is too long";
                else
                    this.LastError = "Unable to write to File Stream";
            }

            return false;
        }
    }
}