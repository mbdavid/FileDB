using System;
using System.IO;

namespace Numeria.IO
{
    public sealed class FileDBStream : Stream
    {
        private Engine _engine = null;
        private readonly long _streamLength = 0;

        private long _streamPosition = 0;
        private DataPage _currentPage = null;
        private int _positionInPage = 0;
        private EntryInfo _info = null;

        internal FileDBStream(Engine engine, Guid id)
        {
            _engine = engine;

            var indexNode = _engine.Search(id);
            if (indexNode != null)
            {
                _streamLength = indexNode.FileLength;
                _currentPage = PageFactory.GetDataPage(indexNode.DataPageID, engine.Reader, false);
                _info = new EntryInfo(indexNode);
            }
        }

        /// <summary>
        /// Get file information
        /// </summary>
        public EntryInfo FileInfo
        {
            get
            {
                return _info;
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Length
        {
            get { return _streamLength; }
        }

        public override long Position
        {
            get
            {
                return _streamPosition;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesLeft = count;

            while (_currentPage != null && bytesLeft > 0)
            {
                int bytesToCopy = Math.Min(bytesLeft, _currentPage.DataBlockLength - _positionInPage);
                Buffer.BlockCopy(_currentPage.DataBlock, _positionInPage, buffer, offset, bytesToCopy);

                _positionInPage += bytesToCopy;
                bytesLeft -= bytesToCopy;
                offset += bytesToCopy;
                _streamPosition += bytesToCopy;

                if (_positionInPage >= _currentPage.DataBlockLength)
                {
                    _positionInPage = 0;

                    if (_currentPage.NextPageID == uint.MaxValue)
                        _currentPage = null;
                    else
                        _currentPage = PageFactory.GetDataPage(_currentPage.NextPageID, _engine.Reader, false);
                }
            }

            return count - bytesLeft;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}