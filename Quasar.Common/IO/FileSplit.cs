using Quasar.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Quasar.Common.IO
{
    public class FileSplit : IEnumerable<FileChunk>, IDisposable
    {
        /// <summary>
        /// The maximum size per file chunk.
        /// </summary>
        public readonly int MaxChunkSize = 65535;

        /// <summary>
        /// The file path of the opened file.
        /// </summary>
        public string FilePath => _fileStream.Name;

        /// <summary>
        /// The file size of the opened file.
        /// </summary>
        public long FileSize => _fileStream.Length;

        /// <summary>
        /// The file stream of the opened file.
        /// </summary>
        private readonly FileStream _fileStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSplit"/> class using the given file path and access mode.
        /// </summary>
        /// <param name="filePath">The path to the file to open.</param>
        /// <param name="fileAccess">The file access mode for opening the file. Allowed are <see cref="FileAccess.Read"/> and <see cref="FileAccess.Write"/>.</param>
        public FileSplit(string filePath, FileAccess fileAccess)
        {
            switch (fileAccess)
            {
                case FileAccess.Read:
                    _fileStream = File.OpenRead(filePath);
                    break;
                case FileAccess.Write:
                    _fileStream = File.OpenWrite(filePath);
                    break;
                default:
                    throw new ArgumentException($"{nameof(fileAccess)} must be either Read or Write.");
            }
        }

        /// <summary>
        /// Writes a chunk to the file. In other words.
        /// </summary>
        /// <param name="chunk"></param>
        public void WriteChunk(FileChunk chunk)
        {
            _fileStream.Seek(chunk.Offset, SeekOrigin.Begin);
            _fileStream.Write(chunk.Data, 0, chunk.Data.Length);
        }

        /// <summary>
        /// Reads a chunk of the file.
        /// </summary>
        /// <param name="offset">Offset of the file, must be a multiple of <see cref="MaxChunkSize"/> for proper reconstruction.</param>
        /// <returns>The read file chunk at the given offset.</returns>
        /// <remarks>
        /// The returned file chunk can be smaller than <see cref="MaxChunkSize"/> iff the
        /// remaining file size from the offset is smaller than <see cref="MaxChunkSize"/>,
        /// then the remaining file size is used.
        /// </remarks>
        public FileChunk ReadChunk(long offset)
        {
            _fileStream.Seek(offset, SeekOrigin.Begin);

            long chunkSize = _fileStream.Length - _fileStream.Position < MaxChunkSize
                ? _fileStream.Length - _fileStream.Position
                : MaxChunkSize;

            var chunkData = new byte[chunkSize];
            _fileStream.Read(chunkData, 0, chunkData.Length);

            return new FileChunk
            {
                Data = chunkData,
                Offset = _fileStream.Position - chunkData.Length
            };
        }

        /// <summary>
        /// Returns an enumerator that iterates through the file chunks.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the file chunks.</returns>
        public IEnumerator<FileChunk> GetEnumerator()
        {
            for (long currentChunk = 0; currentChunk <= _fileStream.Length / MaxChunkSize; currentChunk++)
            {
                yield return ReadChunk(currentChunk * MaxChunkSize);
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _fileStream.Dispose();
            }
        }

        /// <summary>
        /// Disposes all managed and unmanaged resources associated with this class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
