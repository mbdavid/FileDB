using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    public class EntryInfo
    {
        private Guid _id;
        private string _fileName;
        private uint _fileLength;
        private string _mimeType;

        public Guid ID { get { return _id; } }
        public string FileName { get { return _fileName; } }
        public uint FileLength { get { return _fileLength; } internal set { _fileLength = value; } }
        public string MimeType { get { return _mimeType; } }

        internal EntryInfo(string fileName)
        {
            _id = Guid.NewGuid();
            _fileName = Path.GetFileName(fileName);
            _mimeType = MimeTypeConverter.Convert(Path.GetExtension(_fileName));
            _fileLength = 0;
        }

        internal EntryInfo(IndexNode node)
        {
            _id = node.ID;
            _fileName = node.FileName + "." + node.FileExtension;
            _mimeType = MimeTypeConverter.Convert(node.FileExtension);
            _fileLength = node.FileLength;
        }
    }
}
