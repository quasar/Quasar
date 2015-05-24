using System;
using System.IO;

namespace xServer.Core.Helper
{
    public class FileSplit
    {
        private int _maxBlocks;

        private const int MAX_PACKET_SIZE = Client.MAX_PACKET_SIZE - Client.HEADER_SIZE - (1024 * 2);
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

                    this._maxBlocks = (int)Math.Ceiling(fInfo.Length / (double)MAX_PACKET_SIZE);
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
            return (length < MAX_PACKET_SIZE) ? (int) length : MAX_PACKET_SIZE;
        }

        public bool ReadBlock(int blockNumber, out byte[] readBytes)
        {
            try
            {
                if (blockNumber > this.MaxBlocks)
                    throw new ArgumentOutOfRangeException();

                using (FileStream fStream = File.OpenRead(this.Path))
                {
                    if (blockNumber == 0)
                    {
                        fStream.Seek(0, SeekOrigin.Begin);
                        readBytes = new byte[this.GetSize(fStream.Length - fStream.Position)];
                        fStream.Read(readBytes, 0, readBytes.Length);
                    }
                    else
                    {
                        fStream.Seek(blockNumber*MAX_PACKET_SIZE, SeekOrigin.Begin);
                        readBytes = new byte[this.GetSize(fStream.Length - fStream.Position)];
                        fStream.Read(readBytes, 0, readBytes.Length);
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
                    fStream.Seek(blockNumber*MAX_PACKET_SIZE, SeekOrigin.Begin);
                    fStream.Write(block, 0, block.Length);
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