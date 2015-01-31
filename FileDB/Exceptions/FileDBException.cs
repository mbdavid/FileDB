using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Numeria.IO
{
    public class FileDBException : ApplicationException
    {
        public FileDBException(string message)
            : base(message)
        {
        }

        public FileDBException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }
    }
}
