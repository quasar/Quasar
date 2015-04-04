using System;
using System.IO;

namespace xServer.Core.Helper
{
    public class FileSplit
    {
        private const int MAX_PACKET_SIZE = (1024*512);
        private int _maxBlocks;

        public FileSplit(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
        public string LastError { get; private set; }

        public int MaxBlocks
        {
            get
            {
                if (_maxBlocks > 0 || _maxBlocks == -1)
                    return _maxBlocks;
                try
                {
                    var fInfo = new FileInfo(Path);

                    if (!fInfo.Exists)
                        throw new FileNotFoundException();

                    _maxBlocks = (int) Math.Ceiling(fInfo.Length/(double) MAX_PACKET_SIZE);
                }
                catch (UnauthorizedAccessException)
                {
                    _maxBlocks = -1;
                    LastError = "Access denied";
                    return _maxBlocks;
                }
                catch (IOException)
                {
                    _maxBlocks = -1;
                    LastError = "File not found";
                    return _maxBlocks;
                }
                return _maxBlocks;
            }
        }

        private int GetSize(long length)
        {
            return (length < MAX_PACKET_SIZE) ? (int) length : MAX_PACKET_SIZE;
        }

        public bool ReadBlock(int blockNumber, out byte[] readBytes)
        {
            try
            {
                if (blockNumber > MaxBlocks) throw new ArgumentOutOfRangeException();

                using (var fStream = File.OpenRead(Path))
                {
                    if (blockNumber == 0)
                    {
                        fStream.Seek(0, SeekOrigin.Begin);
                        readBytes = new byte[GetSize(fStream.Length - fStream.Position)];
                        fStream.Read(readBytes, 0, readBytes.Length);
                    }
                    else
                    {
                        fStream.Seek(blockNumber*MAX_PACKET_SIZE + 1, SeekOrigin.Begin);
                        readBytes = new byte[GetSize(fStream.Length - fStream.Position)];
                        fStream.Read(readBytes, 0, readBytes.Length);
                    }
                }

                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                readBytes = new byte[0];
                LastError = "BlockNumber bigger than MaxBlocks";
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                readBytes = new byte[0];
                LastError = "Access denied";
                return false;
            }
            catch (IOException)
            {
                readBytes = new byte[0];
                LastError = "File not found";
                return false;
            }
        }

        public bool AppendBlock(byte[] block, int blockNumber)
        {
            try
            {
                if (!File.Exists(Path) && blockNumber > 0)
                    throw new FileNotFoundException(); // previous file got deleted somehow, error

                if (blockNumber == 0)
                {
                    using (var fStream = File.Open(Path, FileMode.Create, FileAccess.Write))
                    {
                        fStream.Seek(0, SeekOrigin.Begin);
                        fStream.Write(block, 0, block.Length);
                    }

                    return true;
                }

                using (var fStream = File.Open(Path, FileMode.Append, FileAccess.Write))
                {
                    fStream.Seek(blockNumber*MAX_PACKET_SIZE + 1, SeekOrigin.Begin);
                    fStream.Write(block, 0, block.Length);
                }

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                LastError = "Access denied";
                return false;
            }
            catch (IOException)
            {
                LastError = "File not found";
                return false;
            }
        }
    }
}